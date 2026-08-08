// Cedula: 402-3047435-1
using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Excepciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    // Pantalla de gestión de reservas: lista todas las reservas (con filtro
    // por estado), permite actualizar y eliminar la seleccionada, y abre
    // FrmNuevaReserva para dar de alta una reserva nueva. La creación ya no
    // vive aquí — este formulario nunca llama a ReservaService.Crear.
    public partial class FrmReservas : Form
    {
        private readonly ReservaService _reservaService;
        private readonly HuespedService _huespedService;
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;

        // Reserva actualmente cargada en el panel de edición. Null cuando
        // no hay ninguna fila seleccionada.
        private int? _idReservaSeleccionada;

        // Huésped de la reserva seleccionada (o el que el usuario acaba de
        // elegir en FrmBuscarHuesped mientras edita).
        private Huesped _huespedEditado;

        // Evita que limpiar la selección del grid dispare en cascada el
        // manejador de selección. Mismo patrón que FrmHabitaciones.cs.
        private bool _limpiandoFormulario;

        // Controles creados por código: no existen en el diseñador.
        private TextBox _txtHuesped;
        private Button _btnBuscarHuesped;
        private Button _btnNuevaReserva;
        private Button _btnEliminar;
        private ComboBox _cboFiltroEstado;
        private TextBox _txtBuscarHuesped;

        // Líneas adicionales del resumen. Se crean por código para no tocar
        // FrmReservas.Designer.cs.
        private Label _lblTarifaNoche;
        private Label _lblFactorTemporada;
        private Label _lblNotaImpuestos;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmReservas()
        {
            InitializeComponent();
            _reservaService = new ReservaService();
            _huespedService = new HuespedService();
            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        // Este constructor recibe al usuario que inicio sesion.
        public FrmReservas(Usuario usuarioActual) : this()
        {
            _usuarioActual = usuarioActual;

            if (_usuarioActual != null)
            {
                Text = "Hotel Bisono - Reservaciones - " +
                    _usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Reservaciones");

            // El huésped ya no se elige en un combo: se ve en un texto de
            // solo lectura y se busca con un diálogo aparte.
            cboHuesped.Visible = false;

            ConfigurarCombo(cboHabitacion);
            ConfigurarCombo(cboTemporada);
            ConfigurarCombo(cboEstadoReserva);

            cboTemporada.Items.Clear();
            cboTemporada.Items.Add("Alta");
            cboTemporada.Items.Add("Media");
            cboTemporada.Items.Add("Baja");

            cboEstadoReserva.Items.Clear();
            cboEstadoReserva.Items.Add("Pendiente");
            cboEstadoReserva.Items.Add("Confirmada");
            cboEstadoReserva.Items.Add("Cancelada");

            dtpCheckIn.Format = DateTimePickerFormat.Short;
            dtpCheckOut.Format = DateTimePickerFormat.Short;
            TemaVisual.EstilizarFecha(dtpCheckIn);
            TemaVisual.EstilizarFecha(dtpCheckOut);

            lblNochesCalculadas.Text = "Noches: 0";
            lblMontoCalculado.Text = "RD$0.00";

            // "Actualizar", no "Guardar": este botón ya sólo edita una
            // reserva existente, la creación se hace desde FrmNuevaReserva.
            TemaVisual.EstilizarBoton(
                btnGuardar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnGuardar,
                TemaVisual.Glifos.Actualizar,
                "Actualizar");

            _btnEliminar = new Button();
            TemaVisual.EstilizarBoton(
                _btnEliminar,
                TemaVisual.Colores.Rojo,
                TemaVisual.Colores.Blanco,
                Color.FromArgb(180, 56, 48));
            TemaVisual.PonerGlifo(
                _btnEliminar,
                TemaVisual.Glifos.Eliminar,
                "Eliminar");

            _btnNuevaReserva = new Button();
            TemaVisual.EstilizarBoton(
                _btnNuevaReserva,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            TemaVisual.PonerGlifo(
                _btnNuevaReserva,
                TemaVisual.Glifos.Nuevo,
                "Nueva reserva");

            _txtHuesped = new TextBox();
            _txtHuesped.ReadOnly = true;
            _txtHuesped.BackColor = TemaVisual.Colores.FondoSecundario;

            // Evita que el campo de solo lectura reciba foco por Tab o clic; redirige a "Buscar".
            _txtHuesped.TabStop = false;
            _txtHuesped.GotFocus += delegate { _btnBuscarHuesped.Focus(); };

            _btnBuscarHuesped = new Button();
            TemaVisual.EstilizarBoton(
                _btnBuscarHuesped,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            TemaVisual.PonerGlifo(
                _btnBuscarHuesped,
                TemaVisual.Glifos.Buscar,
                "Buscar");

            _cboFiltroEstado = new ComboBox();
            ConfigurarCombo(_cboFiltroEstado);
            _cboFiltroEstado.Items.Clear();
            _cboFiltroEstado.Items.Add("Todas");
            _cboFiltroEstado.Items.Add("Pendiente");
            _cboFiltroEstado.Items.Add("Confirmada");
            _cboFiltroEstado.Items.Add("Cancelada");
            _cboFiltroEstado.SelectedIndex = 0;

            // Caja de búsqueda por huésped (nombre o documento); dispara solo con Enter.
            _txtBuscarHuesped = new TextBox();

            TemaVisual.EstilizarGrid(dgvReservasProximas);
            dgvReservasProximas.MultiSelect = false;
            dgvReservasProximas.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            ConfigurarColumnasReservas();

            ConfigurarDistribucion();

            Load += FrmReservas_Load;
            btnGuardar.Click += btnActualizar_Click;
            _btnEliminar.Click += btnEliminar_Click;
            _btnNuevaReserva.Click += btnNuevaReserva_Click;
            _btnBuscarHuesped.Click += btnBuscarHuesped_Click;
            _cboFiltroEstado.SelectedIndexChanged += filtroEstado_SelectedIndexChanged;
            _txtBuscarHuesped.KeyDown += txtBuscarHuesped_KeyDown;
            dgvReservasProximas.SelectionChanged +=
                dgvReservasProximas_SelectionChanged;
            dtpCheckIn.ValueChanged += DatosReserva_Changed;
            dtpCheckOut.ValueChanged += DatosReserva_Changed;
            cboHabitacion.SelectedIndexChanged += DatosReserva_Changed;
            cboTemporada.SelectedIndexChanged += DatosReserva_Changed;
        }

        // Organiza la tabla a la izquierda y el panel de edición a la
        // derecha, con una barra superior para filtrar y crear reservas.
        private void ConfigurarDistribucion()
        {
            Size = new Size(1200, 760);
            MinimumSize = new Size(1040, 680);

            // Hay que emparentar el SplitContainer antes de fijar SplitterDistance/Panel1MinSize/Panel2MinSize.
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Vertical;
            split.SplitterWidth = 6;
            split.BackColor = TemaVisual.Colores.FondoClaro;

            Panel encabezado = TemaVisual.CrearEncabezado(
                "Reservaciones",
                "Cree, actualice o elimine reservas",
                TemaVisual.Glifos.Reserva);
            Controls.Add(split);
            Controls.Add(encabezado);
            split.BringToFront();

            split.Panel1MinSize = 420;
            split.Panel2MinSize = 380;
            // Da más ancho a la grilla para la caja de búsqueda y las 8 columnas.
            split.SplitterDistance = 720;
            split.Panel1.Padding = new Padding(16, 12, 8, 16);
            split.Panel2.Padding = new Padding(8, 12, 16, 16);
            split.Panel1.BackColor = TemaVisual.Colores.FondoClaro;
            split.Panel2.BackColor = TemaVisual.Colores.FondoClaro;

            // --- Panel izquierdo: barra superior + tabla ---
            FlowLayoutPanel barraSuperior = new FlowLayoutPanel();
            barraSuperior.Parent = split.Panel1;
            barraSuperior.Dock = DockStyle.Top;
            barraSuperior.Height = 58;
            barraSuperior.FlowDirection = FlowDirection.LeftToRight;
            barraSuperior.WrapContents = false;
            barraSuperior.Padding = new Padding(0, 10, 0, 10);
            barraSuperior.BackColor = TemaVisual.Colores.FondoClaro;

            _btnNuevaReserva.Size = new Size(160, 36);
            _btnNuevaReserva.Margin = new Padding(0, 0, 24, 0);

            Label lblFiltro = CrearEtiqueta("Estado:");
            lblFiltro.Margin = new Padding(0, 10, 4, 0);

            _cboFiltroEstado.Width = 140;
            _cboFiltroEstado.Margin = new Padding(0, 6, 0, 0);

            barraSuperior.Controls.Add(_btnNuevaReserva);
            barraSuperior.Controls.Add(lblFiltro);
            barraSuperior.Controls.Add(_cboFiltroEstado);

            // Caja de búsqueda por huésped, a la derecha del filtro de Estado.
            Label lblBuscarHuesped = CrearEtiqueta("Huésped:");
            lblBuscarHuesped.Margin = new Padding(16, 10, 4, 0);

            Panel marcoBuscarHuesped = TemaVisual.EnvolverCampo(
                _txtBuscarHuesped,
                TemaVisual.Colores.FondoClaro);
            marcoBuscarHuesped.Size = new Size(200, 36);
            marcoBuscarHuesped.Margin = new Padding(0, 6, 0, 0);
            TemaVisual.AnteponerGlifo(
                marcoBuscarHuesped,
                _txtBuscarHuesped,
                TemaVisual.Glifos.Buscar);

            barraSuperior.Controls.Add(lblBuscarHuesped);
            barraSuperior.Controls.Add(marcoBuscarHuesped);

            dgvReservasProximas.Parent = split.Panel1;
            dgvReservasProximas.Dock = DockStyle.Fill;
            dgvReservasProximas.Margin = new Padding(0);
            barraSuperior.SendToBack();
            dgvReservasProximas.BringToFront();

            // --- Panel derecho: ficha de edición ---
            tableLayoutPanel1.Parent = split.Panel2;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(20, 16, 20, 16);
            tableLayoutPanel1.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            for (int fila = 0; fila < 6; fila++)
            {
                tableLayoutPanel1.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 48F));
            }

            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Label tituloFicha = TemaVisual.CrearTituloSeccion(
                "Detalle de la reserva",
                TemaVisual.Glifos.Reserva);
            tituloFicha.Dock = DockStyle.Fill;
            tituloFicha.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.Controls.Add(tituloFicha, 0, 0);
            tableLayoutPanel1.SetColumnSpan(tituloFicha, 2);

            AgregarCampoHuesped(1);
            AgregarCampo("Habitación", cboHabitacion, 2);
            AgregarCampo("Fecha de entrada", dtpCheckIn, 3);
            AgregarCampo("Fecha de salida", dtpCheckOut, 4);
            AgregarCampo("Temporada", cboTemporada, 5);
            AgregarCampo("Estado", cboEstadoReserva, 6);

            panel1.Controls.Clear();
            panel1.Dock = DockStyle.Fill;
            panel1.Margin = new Padding(0, 6, 0, 6);
            panel1.Padding = new Padding(0);
            panel1.BorderStyle = BorderStyle.None;
            panel1.BackColor = TemaVisual.Colores.FondoClaro;
            panel1.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                if (lienzo.Width <= 1 || lienzo.Height <= 1)
                {
                    return;
                }

                TemaVisual.PintarAtardecer(e.Graphics, lienzo.ClientRectangle);
            };
            panel1.Resize += delegate { panel1.Invalidate(); };
            TemaVisual.AplicarEsquinasRedondeadas(panel1, 12);

            ConstruirResumen();
            tableLayoutPanel1.Controls.Add(panel1, 0, 7);
            tableLayoutPanel1.SetColumnSpan(panel1, 2);

            FlowLayoutPanel barraBotones = new FlowLayoutPanel();
            barraBotones.Dock = DockStyle.Fill;
            barraBotones.FlowDirection = FlowDirection.LeftToRight;
            barraBotones.WrapContents = true;
            barraBotones.BackColor = TemaVisual.Colores.Blanco;
            barraBotones.Padding = new Padding(0, 12, 0, 0);

            btnGuardar.AutoSize = false;
            btnGuardar.Size = new Size(130, 36);
            btnGuardar.Margin = new Padding(0, 3, 8, 3);
            _btnEliminar.AutoSize = false;
            _btnEliminar.Size = new Size(130, 36);
            _btnEliminar.Margin = new Padding(0, 3, 0, 3);

            barraBotones.Controls.Add(btnGuardar);
            barraBotones.Controls.Add(_btnEliminar);

            tableLayoutPanel1.Controls.Add(barraBotones, 0, 8);
            tableLayoutPanel1.SetColumnSpan(barraBotones, 2);

            flpReservas.Visible = false;
        }

        // Define las columnas explícitas de la grilla de reservas (reemplaza las autogeneradas).
        private void ConfigurarColumnasReservas()
        {
            dgvReservasProximas.AutoGenerateColumns = false;
            dgvReservasProximas.Columns.Clear();

            // Cultura fija para que los meses salgan abreviados en español ("ago").
            CultureInfo formatoFechas = new CultureInfo("es-DO");

            dgvReservasProximas.Columns.Add(CrearColumnaTexto(
                "NumeroDocumentoHuesped", "Documento", 15));
            dgvReservasProximas.Columns.Add(CrearColumnaTexto(
                "NumeroHabitacion", "Habitación", 8));
            dgvReservasProximas.Columns.Add(CrearColumnaFecha(
                "FechaCheckIn", "Check-in", 14, formatoFechas));
            dgvReservasProximas.Columns.Add(CrearColumnaFecha(
                "FechaCheckOut", "Check-out", 14, formatoFechas));
            dgvReservasProximas.Columns.Add(CrearColumnaTexto(
                "Temporada", "Temporada", 10));
            dgvReservasProximas.Columns.Add(CrearColumnaTexto(
                "Estado", "Estado", 11));
            dgvReservasProximas.Columns.Add(CrearColumnaTexto(
                "TotalNoches", "Noches", 7));

            DataGridViewTextBoxColumn columnaMonto =
                new DataGridViewTextBoxColumn();
            columnaMonto.DataPropertyName = "MontoEstimado";
            columnaMonto.HeaderText = "Monto";
            columnaMonto.Name = "colMontoEstimado";
            columnaMonto.FillWeight = 21;
            columnaMonto.DefaultCellStyle.Format = "'RD$'#,##0.00";
            columnaMonto.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvReservasProximas.Columns.Add(columnaMonto);
        }

        // Crea una columna de texto simple para dgvReservasProximas.
        private static DataGridViewTextBoxColumn CrearColumnaTexto(
            string propiedad,
            string encabezado,
            int pesoRelativo)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;
            columna.HeaderText = encabezado;
            columna.Name = "col" + propiedad;
            columna.FillWeight = pesoRelativo;
            return columna;
        }

        // Crea una columna de fecha con el formato "07-ago-26" usado en la app.
        private static DataGridViewTextBoxColumn CrearColumnaFecha(
            string propiedad,
            string encabezado,
            int pesoRelativo,
            CultureInfo cultura)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;
            columna.HeaderText = encabezado;
            columna.Name = "col" + propiedad;
            columna.FillWeight = pesoRelativo;
            columna.DefaultCellStyle.Format = "dd-MMM-yy";
            columna.DefaultCellStyle.FormatProvider = cultura;
            return columna;
        }

        // Agrega el campo de huésped: texto de solo lectura + botón buscar.
        private void AgregarCampoHuesped(int fila)
        {
            Label etiqueta = CrearEtiqueta("Huésped");
            tableLayoutPanel1.Controls.Add(etiqueta, 0, fila);

            Panel contenedor = new Panel();
            contenedor.Dock = DockStyle.Fill;
            contenedor.Margin = new Padding(3, 6, 3, 6);

            Panel marco = TemaVisual.EnvolverCampo(
                _txtHuesped,
                TemaVisual.Colores.Blanco);
            marco.Dock = DockStyle.Fill;
            marco.Margin = new Padding(0);

            _btnBuscarHuesped.AutoSize = false;
            _btnBuscarHuesped.Size = new Size(90, 30);
            _btnBuscarHuesped.Dock = DockStyle.Right;
            _btnBuscarHuesped.Margin = new Padding(0);

            contenedor.Controls.Add(marco);
            contenedor.Controls.Add(_btnBuscarHuesped);
            marco.BringToFront();

            tableLayoutPanel1.Controls.Add(contenedor, 1, fila);
        }

        // Arma la tarjeta de resumen de la reserva (noches, tarifa, temporada, monto).
        private void ConstruirResumen()
        {
            panel1.Controls.Add(lblNochesCalculadas);
            panel1.Controls.Add(lblMontoCalculado);

            Label titulo = new Label();
            titulo.Text = "Resumen de la reserva";
            titulo.Font = TemaVisual.Fuentes.TituloSeccion;
            titulo.ForeColor = TemaVisual.Colores.AzulProfundo;
            titulo.BackColor = Color.Transparent;
            titulo.AutoSize = false;
            titulo.Size = new Size(340, 24);
            titulo.Location = new Point(16, 10);
            titulo.TextAlign = ContentAlignment.MiddleLeft;

            lblNochesCalculadas.Dock = DockStyle.None;
            lblNochesCalculadas.AutoSize = false;
            lblNochesCalculadas.BackColor = Color.Transparent;
            lblNochesCalculadas.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblNochesCalculadas.Font = TemaVisual.Fuentes.Cuerpo;
            lblNochesCalculadas.Size = new Size(340, 20);
            lblNochesCalculadas.Location = new Point(16, 40);
            lblNochesCalculadas.TextAlign = ContentAlignment.MiddleLeft;

            _lblTarifaNoche = CrearLineaResumen(new Point(16, 60));
            _lblFactorTemporada = CrearLineaResumen(new Point(16, 80));

            lblMontoCalculado.Dock = DockStyle.None;
            lblMontoCalculado.AutoSize = false;
            lblMontoCalculado.BackColor = Color.Transparent;
            lblMontoCalculado.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblMontoCalculado.Font = TemaVisual.Fuentes.MontoTotal;
            lblMontoCalculado.Size = new Size(340, 36);
            lblMontoCalculado.Location = new Point(16, 108);
            lblMontoCalculado.TextAlign = ContentAlignment.MiddleLeft;

            _lblNotaImpuestos = new Label();
            _lblNotaImpuestos.Text =
                "El ITBIS (18%) y la propina (10%) se aplican al facturar, " +
                "no sobre este estimado.";
            _lblNotaImpuestos.Font = TemaVisual.Fuentes.Pequena;
            _lblNotaImpuestos.ForeColor = TemaVisual.Colores.AzulMarino;
            _lblNotaImpuestos.BackColor = Color.Transparent;
            _lblNotaImpuestos.AutoSize = false;
            _lblNotaImpuestos.Size = new Size(340, 44);
            _lblNotaImpuestos.Location = new Point(16, 148);
            _lblNotaImpuestos.TextAlign = ContentAlignment.TopLeft;

            panel1.Controls.Add(titulo);
            panel1.Controls.Add(_lblTarifaNoche);
            panel1.Controls.Add(_lblFactorTemporada);
            panel1.Controls.Add(_lblNotaImpuestos);
        }

        private static Label CrearLineaResumen(Point ubicacion)
        {
            Label linea = new Label();
            linea.Font = TemaVisual.Fuentes.Cuerpo;
            linea.ForeColor = TemaVisual.Colores.AzulProfundo;
            linea.BackColor = Color.Transparent;
            linea.AutoSize = false;
            linea.Size = new Size(340, 20);
            linea.Location = ubicacion;
            linea.TextAlign = ContentAlignment.MiddleLeft;
            return linea;
        }

        // Agrega una etiqueta y su campo en una fila de la ficha.
        private void AgregarCampo(string texto, Control campo, int fila)
        {
            Label etiqueta = CrearEtiqueta(texto);
            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 6, 3, 6);

            tableLayoutPanel1.Controls.Add(etiqueta, 0, fila);
            tableLayoutPanel1.Controls.Add(campo, 1, fila);
        }

        private static Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Anchor = AnchorStyles.Left;
            etiqueta.Margin = new Padding(3, 10, 3, 3);
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.Texto;
            return etiqueta;
        }

        // Carga las reservas al abrir la ventana y deja la ficha vacía.
        private void FrmReservas_Load(object sender, EventArgs e)
        {
            try
            {
                CargarReservas();
                LimpiarFormulario();
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudieron cargar las reservas desde la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Vuelve a consultar todas las reservas y aplica los filtros de estado y huésped.
        private void CargarReservas()
        {
            List<Reserva> todas = _reservaService.ObtenerTodas();
            List<Reserva> filtradasPorEstado = AplicarFiltroEstado(todas);
            List<Reserva> filtradasPorHuesped =
                AplicarFiltroHuesped(filtradasPorEstado);

            dgvReservasProximas.DataSource = null;
            dgvReservasProximas.DataSource = filtradasPorHuesped;
        }

        // Deja sólo las reservas del estado elegido. El servicio no ofrece
        // este filtro, así que se recorta la lista ya devuelta.
        private List<Reserva> AplicarFiltroEstado(List<Reserva> origen)
        {
            if (origen == null || _cboFiltroEstado == null)
            {
                return origen;
            }

            string estado = _cboFiltroEstado.SelectedItem as string;

            if (string.IsNullOrEmpty(estado) || estado == "Todas")
            {
                return origen;
            }

            List<Reserva> filtradas = new List<Reserva>();

            foreach (Reserva reserva in origen)
            {
                if (reserva.Estado == estado)
                {
                    filtradas.Add(reserva);
                }
            }

            return filtradas;
        }

        // Deja sólo las reservas cuyo huésped coincide con el criterio buscado (nombre o documento).
        private List<Reserva> AplicarFiltroHuesped(List<Reserva> origen)
        {
            if (origen == null || _txtBuscarHuesped == null)
            {
                return origen;
            }

            string criterio = _txtBuscarHuesped.Text.Trim();

            if (criterio == "")
            {
                return origen;
            }

            List<Huesped> coincidencias = _huespedService.Buscar(criterio);
            HashSet<string> documentos = new HashSet<string>();

            foreach (Huesped huesped in coincidencias)
            {
                documentos.Add(huesped.NumeroDocumento);
            }

            List<Reserva> filtradas = new List<Reserva>();

            foreach (Reserva reserva in origen)
            {
                if (documentos.Contains(reserva.NumeroDocumentoHuesped))
                {
                    filtradas.Add(reserva);
                }
            }

            return filtradas;
        }

        // Vuelve a cargar la tabla cuando cambia el filtro de estado.
        private void filtroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CargarReservas();
                LimpiarFormulario();
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo aplicar el filtro de reservas.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Aplica el filtro de huésped al presionar Enter en la caja de búsqueda.
        private void txtBuscarHuesped_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            try
            {
                CargarReservas();
                LimpiarFormulario();
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo buscar el huésped en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Carga en la ficha la reserva que se seleccionó en la tabla.
        private void dgvReservasProximas_SelectionChanged(
            object sender,
            EventArgs e)
        {
            // Difiere la lectura porque SelectionChanged puede disparar antes de que CurrentRow se actualice.
            BeginInvoke(new MethodInvoker(CargarReservaSeleccionada));
        }

        // Lee la reserva seleccionada y llena la ficha de edición.
        private void CargarReservaSeleccionada()
        {
            if (_limpiandoFormulario)
            {
                return;
            }

            if (dgvReservasProximas.CurrentRow == null)
            {
                return;
            }

            Reserva reserva =
                dgvReservasProximas.CurrentRow.DataBoundItem as Reserva;

            if (reserva == null)
            {
                return;
            }

            _idReservaSeleccionada = reserva.IdReserva;

            _huespedEditado =
                _huespedService.BuscarPorDocumento(
                    reserva.NumeroDocumentoHuesped);

            _txtHuesped.Text = _huespedEditado != null
                ? _huespedEditado.Nombre + " " + _huespedEditado.Apellido
                : reserva.NumeroDocumentoHuesped;

            CargarHabitacionesParaEdicion(reserva);
            cboHabitacion.SelectedValue = reserva.NumeroHabitacion;

            dtpCheckIn.Value = reserva.FechaCheckIn;
            dtpCheckOut.Value = reserva.FechaCheckOut;
            cboTemporada.SelectedItem = reserva.Temporada;
            cboEstadoReserva.SelectedItem = reserva.Estado;

            CalcularResumen();
            ActualizarEstadoControles();
        }

        // Carga las habitaciones disponibles más la que ya tiene asignada la reserva editada.
        private void CargarHabitacionesParaEdicion(Reserva reservaActual)
        {
            List<Habitacion> todas = _habitacionService.ObtenerTodas();
            List<Habitacion> opciones = new List<Habitacion>();
            bool incluidaActual = false;

            foreach (Habitacion habitacion in todas)
            {
                bool esLaActual = reservaActual != null &&
                    habitacion.Numero == reservaActual.NumeroHabitacion;

                if (habitacion.Estado == "Disponible" || esLaActual)
                {
                    opciones.Add(habitacion);
                }

                if (esLaActual)
                {
                    incluidaActual = true;
                }
            }

            if (incluidaActual == false && reservaActual != null)
            {
                Habitacion actual = _habitacionService.Buscar(
                    reservaActual.NumeroHabitacion);

                if (actual != null)
                {
                    opciones.Add(actual);
                }
            }

            cboHabitacion.DataSource = null;
            cboHabitacion.DisplayMember = "Numero";
            cboHabitacion.ValueMember = "Numero";
            cboHabitacion.DataSource = opciones;
        }

        // Abre el buscador y toma el huesped que el usuario elija.
        private void btnBuscarHuesped_Click(object sender, EventArgs e)
        {
            using (FrmBuscarHuesped buscador = new FrmBuscarHuesped())
            {
                if (buscador.ShowDialog(this) == DialogResult.OK)
                {
                    _huespedEditado = buscador.HuespedSeleccionado;
                    _txtHuesped.Text =
                        _huespedEditado.Nombre + " " + _huespedEditado.Apellido;
                }
            }
        }

        // Vuelve a calcular noches y monto cuando cambia un dato.
        private void DatosReserva_Changed(object sender, EventArgs e)
        {
            CalcularResumen();
        }

        // Abre el dialogo de alta y refresca la tabla al volver.
        private void btnNuevaReserva_Click(object sender, EventArgs e)
        {
            using (FrmNuevaReserva formulario =
                new FrmNuevaReserva(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }

            try
            {
                CargarReservas();
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo actualizar la lista de reservas.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Actualiza la reserva seleccionada con los datos de la ficha.
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_idReservaSeleccionada.HasValue == false)
            {
                MostrarAdvertencia("Seleccione una reserva para actualizar.");
                return;
            }

            btnGuardar.Enabled = false;

            try
            {
                Reserva reserva = CrearReservaActualizada();
                _reservaService.Actualizar(reserva);

                MessageBox.Show(
                    "La reserva fue actualizada correctamente.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarReservas();
                LimpiarFormulario();
            }
            catch (FormatException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo actualizar la reserva en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                ActualizarEstadoControles();
            }
        }

        // Elimina la reserva seleccionada despues de pedir confirmacion.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idReservaSeleccionada.HasValue == false)
            {
                MostrarAdvertencia("Seleccione una reserva para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar la reserva #" +
                _idReservaSeleccionada.Value + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            _btnEliminar.Enabled = false;

            try
            {
                _reservaService.Eliminar(
                    _idReservaSeleccionada.Value, _usuarioActual);

                MessageBox.Show(
                    "La reserva fue eliminada.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarReservas();
                LimpiarFormulario();
            }
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo eliminar la reserva. Puede tener datos relacionados.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                ActualizarEstadoControles();
            }
        }

        // Convierte los controles de la ficha en la reserva actualizada.
        private Reserva CrearReservaActualizada()
        {
            Habitacion habitacion = cboHabitacion.SelectedItem as Habitacion;

            if (_huespedEditado == null)
            {
                throw new FormatException("Seleccione un huesped.");
            }

            if (habitacion == null)
            {
                throw new FormatException("Seleccione una habitacion.");
            }

            if (cboTemporada.SelectedIndex < 0)
            {
                throw new FormatException("Seleccione una temporada.");
            }

            if (cboEstadoReserva.SelectedIndex < 0)
            {
                throw new FormatException("Seleccione el estado de la reserva.");
            }

            int noches = _reservaService.CalcularNoches(
                dtpCheckIn.Value,
                dtpCheckOut.Value);
            decimal monto = _reservaService.CalcularMonto(
                dtpCheckIn.Value,
                dtpCheckOut.Value,
                habitacion.TarifaBase,
                cboTemporada.Text);

            Reserva reserva = new Reserva();
            reserva.IdReserva = _idReservaSeleccionada.Value;
            reserva.NumeroDocumentoHuesped = _huespedEditado.NumeroDocumento;
            reserva.NumeroHabitacion = habitacion.Numero;
            reserva.FechaCheckIn = dtpCheckIn.Value.Date;
            reserva.FechaCheckOut = dtpCheckOut.Value.Date;
            reserva.Temporada = cboTemporada.Text;
            reserva.Estado = cboEstadoReserva.Text;
            reserva.TotalNoches = noches;
            reserva.MontoEstimado = monto;

            return reserva;
        }

        // Calcula el resumen que se muestra en la ficha.
        private void CalcularResumen()
        {
            Habitacion habitacion = cboHabitacion.SelectedItem as Habitacion;

            if (habitacion == null || cboTemporada.SelectedIndex < 0)
            {
                LimpiarResumen();
                return;
            }

            try
            {
                int noches = _reservaService.CalcularNoches(
                    dtpCheckIn.Value,
                    dtpCheckOut.Value);
                decimal monto = _reservaService.CalcularMonto(
                    dtpCheckIn.Value,
                    dtpCheckOut.Value,
                    habitacion.TarifaBase,
                    cboTemporada.Text);
                decimal factor = _reservaService.ObtenerFactorTemporada(
                    cboTemporada.Text);

                lblNochesCalculadas.Text = "Noches: " + noches;
                lblMontoCalculado.Text = "RD$" + monto.ToString("N2");

                if (_lblTarifaNoche != null)
                {
                    _lblTarifaNoche.Text =
                        "Tarifa por noche: RD$" +
                        habitacion.TarifaBase.ToString("N2");
                }

                if (_lblFactorTemporada != null)
                {
                    _lblFactorTemporada.Text =
                        "Temporada " + cboTemporada.Text +
                        " (factor " + factor.ToString("0.00") + ")";
                }
            }
            catch (FormatException)
            {
                LimpiarResumen();
            }
            catch (ArgumentException)
            {
                LimpiarResumen();
            }
            catch (InvalidOperationException)
            {
                LimpiarResumen();
            }
            catch (Exception)
            {
                LimpiarResumen();
            }
        }

        // Muestra valores vacios cuando el resumen no puede calcularse.
        private void LimpiarResumen()
        {
            lblNochesCalculadas.Text = "Noches: 0";
            lblMontoCalculado.Text = "RD$0.00";

            if (_lblTarifaNoche != null)
            {
                _lblTarifaNoche.Text = "Tarifa por noche: —";
            }

            if (_lblFactorTemporada != null)
            {
                _lblFactorTemporada.Text = "Temporada: —";
            }
        }

        // Deja la ficha vacía y deshabilitada: sin selección no hay nada
        // que actualizar ni eliminar (el alta se hace en otro formulario).
        private void LimpiarFormulario()
        {
            _limpiandoFormulario = true;

            _idReservaSeleccionada = null;
            _huespedEditado = null;
            _txtHuesped.Clear();
            cboHabitacion.DataSource = null;
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            cboTemporada.SelectedIndex = -1;
            cboEstadoReserva.SelectedIndex = -1;
            LimpiarResumen();

            dgvReservasProximas.ClearSelection();
            dgvReservasProximas.CurrentCell = null;

            _limpiandoFormulario = false;
            ActualizarEstadoControles();
        }

        // Habilita la ficha y los botones según haya selección; "Eliminar" exige Administrador.
        private void ActualizarEstadoControles()
        {
            bool haySeleccion = _idReservaSeleccionada.HasValue;

            tableLayoutPanel1.Enabled = haySeleccion;
            btnGuardar.Enabled = haySeleccion;

            if (_btnEliminar != null)
            {
                bool esAdministrador =
                    AutorizacionService.EsAdministrador(_usuarioActual);
                _btnEliminar.Enabled = haySeleccion && esAdministrador;
            }
        }

        // Configura una lista para aceptar solo opciones existentes.
        private static void ConfigurarCombo(ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            TemaVisual.EstilizarCombo(combo);
        }

        // Muestra un mensaje que el usuario puede corregir.
        private static void MostrarAdvertencia(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Revise los datos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // Muestra un error no esperado.
        private static void MostrarError(Exception error)
        {
            MessageBox.Show(
                error.Message,
                "Hotel Bisono",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
