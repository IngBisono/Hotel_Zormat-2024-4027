// Cedula: 402-3047435-1
using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Excepciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmHabitaciones : Form
    {
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;
        private int? _numeroSeleccionado;
        private bool _cargandoFiltros;
        private bool _limpiandoFormulario;

        // Cifras de la fila de indicadores. Se construyen por código para no
        // modificar FrmHabitaciones.Designer.cs.
        private Label _lblKpiTotal;
        private Label _lblKpiDisponibles;
        private Label _lblKpiOcupadas;
        private Label _lblKpiLimpieza;

        // Filtro por tipo. No existe en el diseñador ni en el servicio
        // (Filtrar sólo acepta piso y estado), así que se resuelve en el
        // cliente sobre la lista que ya se descarga.
        private ComboBox _cboFiltroTipo;

        public FrmHabitaciones()
        {
            InitializeComponent();

            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        public FrmHabitaciones(Usuario usuarioActual)
            : this()
        {
            _usuarioActual = usuarioActual;
        }

        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Habitaciones");
            StartPosition = FormStartPosition.CenterParent;
            flpHabitaciones.AutoScroll = true;

            ConfigurarDistribucion();

            TemaVisual.EstilizarGrid(dgvHabitaciones);

            txtNumero.Mask = "000";
            txtNumero.PromptChar = ' ';

            numPiso.Minimum = 1;
            numPiso.Maximum = 99;

            numCapacidad.Minimum = 1;
            numCapacidad.Maximum = 20;

            txtTarifaBase.Minimum = 1;
            txtTarifaBase.Maximum = 1000000;
            txtTarifaBase.DecimalPlaces = 2;
            txtTarifaBase.ThousandsSeparator = true;

            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            TemaVisual.EstilizarCombo(cboTipo);
            TemaVisual.EstilizarCombo(cboEstado);
            TemaVisual.EstilizarCombo(cboFiltroPiso);
            TemaVisual.EstilizarCombo(cboFiltroEstado);
            cboTipo.Items.Clear();

            string[] tiposHabitacion = { "Sencilla", "Doble", "Suite" };

            foreach (string tipo in tiposHabitacion)
            {
                cboTipo.Items.Add(tipo);
            }

            cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstado.Items.Clear();

            cboFiltroPiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.Items.Clear();
            cboFiltroEstado.Items.Add("Todos");

            string[] estadosHabitacion =
            {
                "Disponible",
                "Ocupada",
                "Reservada",
                "Limpieza"
            };

            foreach (string estado in estadosHabitacion)
            {
                cboEstado.Items.Add(estado);
                cboFiltroEstado.Items.Add(estado);
            }

            cboFiltroEstado.SelectedIndex = 0;

            ConfigurarBoton(
                btnNuevo,
                "Nuevo",
                TemaVisual.Glifos.Nuevo,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            ConfigurarBoton(
                btnGuardar,
                "Guardar",
                TemaVisual.Glifos.Guardar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            ConfigurarBoton(
                btnEliminar,
                "Eliminar",
                TemaVisual.Glifos.Eliminar,
                TemaVisual.Colores.Rojo,
                TemaVisual.Colores.Blanco,
                Color.FromArgb(180, 56, 48));
            ConfigurarBoton(
                btnCancelar,
                "Cancelar",
                TemaVisual.Glifos.Cancelar,
                TemaVisual.Colores.FondoSecundario,
                TemaVisual.Colores.Texto,
                TemaVisual.Colores.BordeSutil);
            btnEliminar.Enabled = false;

            Load += FrmHabitaciones_Load;
            dgvHabitaciones.SelectionChanged +=
                dgvHabitaciones_SelectionChanged;
            txtNumero.KeyPress += txtNumero_KeyPress;
            txtNumero.MouseUp += txtNumero_MouseUp;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnCancelar.Click += btnCancelar_Click;
            cboFiltroPiso.SelectedIndexChanged +=
                filtros_SelectedIndexChanged;
            cboFiltroEstado.SelectedIndexChanged +=
                filtros_SelectedIndexChanged;
        }

        // Organiza la lista, los filtros y el formulario de edicion.
        private void ConfigurarDistribucion()
        {
            // +20% de ancho / +5% de alto sobre el tamaño original
            // (1100x680), para que la fila de botones del formulario de
            // edición nunca se envuelva a una segunda línea y quede cortada.
            Size = new Size(1320, 714);
            MinimumSize = new Size(940, 660);

            splitContainer1.Parent = this;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Orientation = Orientation.Vertical;
            splitContainer1.SplitterDistance = 620;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.Panel1MinSize = 400;
            splitContainer1.Panel2MinSize = 340;
            splitContainer1.Panel1.Padding = new Padding(16, 12, 8, 16);
            splitContainer1.Panel2.Padding = new Padding(8, 12, 16, 16);
            splitContainer1.BackColor = TemaVisual.Colores.FondoClaro;
            splitContainer1.Panel1.BackColor = TemaVisual.Colores.FondoClaro;
            splitContainer1.Panel2.BackColor = TemaVisual.Colores.FondoClaro;
            // El acoplamiento se resuelve del último control al primero, así
            // que el control acoplado a Fill debe quedar al frente para
            // repartirse el espacio que deja libre el encabezado superior.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Gestión de Habitaciones",
                TemaVisual.Glifos.Habitacion);

            // El orden importa: el acoplamiento se resuelve del índice más
            // alto al más bajo, así que el encabezado se agrega después de la
            // fila de indicadores para quedar por encima de ella.
            Controls.Add(ConstruirFilaIndicadores());
            Controls.Add(encabezado);
            splitContainer1.BringToFront();

            flpHabitaciones.Controls.Clear();
            flpHabitaciones.Parent = splitContainer1.Panel1;
            flpHabitaciones.Dock = DockStyle.Top;
            flpHabitaciones.Height = 58;
            flpHabitaciones.AutoScroll = false;
            flpHabitaciones.FlowDirection = FlowDirection.LeftToRight;
            flpHabitaciones.WrapContents = false;
            flpHabitaciones.Padding = new Padding(14, 14, 14, 10);
            flpHabitaciones.BackColor = TemaVisual.Colores.FondoSecundario;

            Label lblFiltroPiso = CrearEtiqueta("Piso:");
            Label lblFiltroEstado = CrearEtiqueta("Estado:");
            Label lblFiltroTipo = CrearEtiqueta("Tipo:");

            cboFiltroPiso.Width = 110;
            cboFiltroEstado.Width = 130;
            cboFiltroPiso.Margin = new Padding(4, 2, 16, 0);
            cboFiltroEstado.Margin = new Padding(4, 2, 16, 0);

            _cboFiltroTipo = new ComboBox();
            _cboFiltroTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboFiltroTipo.Width = 130;
            _cboFiltroTipo.Margin = new Padding(4, 2, 0, 0);
            _cboFiltroTipo.Items.Add("Todos");
            _cboFiltroTipo.Items.Add("Sencilla");
            _cboFiltroTipo.Items.Add("Doble");
            _cboFiltroTipo.Items.Add("Suite");
            _cboFiltroTipo.SelectedIndex = 0;
            TemaVisual.EstilizarCombo(_cboFiltroTipo);

            // Reutiliza el manejador ya existente, que además está protegido
            // contra recargas mientras se repueblan los combos.
            _cboFiltroTipo.SelectedIndexChanged += filtros_SelectedIndexChanged;

            flpHabitaciones.Controls.Add(lblFiltroPiso);
            flpHabitaciones.Controls.Add(cboFiltroPiso);
            flpHabitaciones.Controls.Add(lblFiltroEstado);
            flpHabitaciones.Controls.Add(cboFiltroEstado);
            flpHabitaciones.Controls.Add(lblFiltroTipo);
            flpHabitaciones.Controls.Add(_cboFiltroTipo);

            dgvHabitaciones.Parent = splitContainer1.Panel1;
            dgvHabitaciones.Dock = DockStyle.Fill;
            dgvHabitaciones.Margin = new Padding(0);

            // La tabla debe quedar al frente para que el acoplamiento a Fill se
            // resuelva en último lugar; si no, ocuparía todo el panel y la
            // barra de filtros se dibujaría encima tapando sus encabezados.
            flpHabitaciones.SendToBack();
            dgvHabitaciones.BringToFront();

            tableLayoutPanel1.Parent = splitContainer1.Panel2;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(20, 16, 20, 16);
            tableLayoutPanel1.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 118F));
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40F));

            // Las filas crecen respecto al diseño anterior porque cada campo
            // va ahora dentro de un marco redondeado con su propio relleno.
            for (int fila = 0; fila < 6; fila++)
            {
                tableLayoutPanel1.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 54F));
            }

            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            Label tituloFicha = TemaVisual.CrearTituloSeccion(
                "Detalle de la habitación",
                TemaVisual.Glifos.Habitacion);
            tituloFicha.Dock = DockStyle.Fill;
            tituloFicha.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.Controls.Add(tituloFicha, 0, 0);
            tableLayoutPanel1.SetColumnSpan(tituloFicha, 2);

            AgregarCampo("Número", txtNumero, 1);
            AgregarCampo("Piso", numPiso, 2);
            AgregarCampo("Tipo", cboTipo, 3);
            AgregarCampo("Estado", cboEstado, 4);
            AgregarCampo("Tarifa base", txtTarifaBase, 5);
            AgregarCampo("Capacidad", numCapacidad, 6);

            FlowLayoutPanel barraBotones = new FlowLayoutPanel();
            barraBotones.Dock = DockStyle.Fill;
            barraBotones.FlowDirection = FlowDirection.LeftToRight;
            barraBotones.WrapContents = true;
            barraBotones.BackColor = TemaVisual.Colores.Blanco;
            barraBotones.Padding = new Padding(0, 16, 0, 0);

            btnNuevo.Size = new Size(112, 36);
            btnGuardar.Size = new Size(112, 36);
            btnEliminar.Size = new Size(112, 36);
            btnCancelar.Size = new Size(112, 36);
            btnNuevo.Margin = new Padding(0, 3, 8, 3);
            btnGuardar.Margin = new Padding(0, 3, 8, 3);
            btnEliminar.Margin = new Padding(0, 3, 8, 3);
            btnCancelar.Margin = new Padding(0, 3, 0, 3);

            barraBotones.Controls.Add(btnNuevo);
            barraBotones.Controls.Add(btnGuardar);
            barraBotones.Controls.Add(btnEliminar);
            barraBotones.Controls.Add(btnCancelar);

            tableLayoutPanel1.Controls.Add(barraBotones, 0, 7);
            tableLayoutPanel1.SetColumnSpan(barraBotones, 2);
        }

        // Fila de indicadores con el resumen del inventario. Los conteos se
        // calculan en el cliente sobre la lista que ya se descarga para el
        // grid, así que no añade ninguna consulta extra a la base de datos.
        private Panel ConstruirFilaIndicadores()
        {
            FlowLayoutPanel fila = new FlowLayoutPanel();
            fila.Dock = DockStyle.Top;
            fila.Height = 96;
            fila.FlowDirection = FlowDirection.LeftToRight;
            fila.WrapContents = false;
            fila.AutoScroll = false;
            fila.Padding = new Padding(16, 12, 16, 8);
            fila.BackColor = TemaVisual.Colores.FondoClaro;

            fila.Controls.Add(TemaVisual.CrearTarjetaIndicador(
                "Total de habitaciones",
                TemaVisual.Glifos.Habitacion,
                TemaVisual.Colores.AzulPrimario,
                out _lblKpiTotal));

            fila.Controls.Add(TemaVisual.CrearTarjetaIndicador(
                "Disponibles",
                TemaVisual.Glifos.Confirmar,
                TemaVisual.Colores.Verde,
                out _lblKpiDisponibles));

            fila.Controls.Add(TemaVisual.CrearTarjetaIndicador(
                "Ocupadas",
                TemaVisual.Glifos.Huesped,
                TemaVisual.Colores.Rojo,
                out _lblKpiOcupadas));

            fila.Controls.Add(TemaVisual.CrearTarjetaIndicador(
                "En limpieza",
                TemaVisual.Glifos.Actualizar,
                TemaVisual.Colores.AzulGris,
                out _lblKpiLimpieza));

            return fila;
        }

        // Refresca las cifras de los indicadores con el inventario completo.
        private void ActualizarIndicadores(List<Habitacion> habitaciones)
        {
            if (_lblKpiTotal == null || habitaciones == null)
            {
                return;
            }

            int disponibles = 0;
            int ocupadas = 0;
            int limpieza = 0;

            foreach (Habitacion habitacion in habitaciones)
            {
                if (string.Equals(
                        habitacion.Estado,
                        "Disponible",
                        StringComparison.OrdinalIgnoreCase))
                {
                    disponibles = disponibles + 1;
                }
                else if (string.Equals(
                        habitacion.Estado,
                        "Ocupada",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ocupadas = ocupadas + 1;
                }
                else if (string.Equals(
                        habitacion.Estado,
                        "Limpieza",
                        StringComparison.OrdinalIgnoreCase))
                {
                    limpieza = limpieza + 1;
                }
            }

            _lblKpiTotal.Text = habitaciones.Count.ToString();
            _lblKpiDisponibles.Text = disponibles.ToString();
            _lblKpiOcupadas.Text = ocupadas.ToString();
            _lblKpiLimpieza.Text = limpieza.ToString();
        }

        // Agrega una etiqueta y su control en una fila del formulario.
        //
        // Las cajas de texto y los numéricos se envuelven en un marco con
        // esquinas redondeadas, porque WinForms no permite curvar el borde de
        // esos controles directamente. Las listas desplegables conservan su
        // borde nativo, que no es personalizable sin repintar todo el control.
        private void AgregarCampo(string texto, Control control, int fila)
        {
            Label etiqueta = CrearEtiqueta(texto);
            tableLayoutPanel1.Controls.Add(etiqueta, 0, fila);

            ComboBox lista = control as ComboBox;
            if (lista != null)
            {
                lista.Dock = DockStyle.Fill;
                lista.Margin = new Padding(3, 12, 3, 12);
                tableLayoutPanel1.Controls.Add(lista, 1, fila);
                return;
            }

            Panel marco = TemaVisual.EnvolverCampo(
                control,
                TemaVisual.Colores.Blanco);
            marco.Dock = DockStyle.Fill;
            marco.Margin = new Padding(3, 7, 3, 7);
            tableLayoutPanel1.Controls.Add(marco, 1, fila);
        }

        // Crea una etiqueta sencilla para los filtros y campos.
        private static Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Anchor = AnchorStyles.Left;
            etiqueta.Margin = new Padding(3, 8, 3, 3);
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.Texto;
            return etiqueta;
        }

        private void ConfigurarBoton(
            Button boton,
            string texto,
            string glifo,
            Color colorFondo,
            Color colorTexto,
            Color colorHover)
        {
            TemaVisual.EstilizarBoton(boton, colorFondo, colorTexto, colorHover);
            TemaVisual.PonerGlifo(boton, glifo, texto);
        }

        private void FrmHabitaciones_Load(object sender, EventArgs e)
        {
            LimpiarFormulario();
            ActualizarEstadoBotonEliminar();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                List<Habitacion> habitaciones =
                    _habitacionService.ObtenerTodas();

                CargarPisos(habitaciones);
                ActualizarIndicadores(habitaciones);
                CargarHabitacionesFiltradas();
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudieron cargar las habitaciones. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al cargar las habitaciones.");
            }
        }

        private void CargarPisos(List<Habitacion> habitaciones)
        {
            _cargandoFiltros = true;
            cboFiltroPiso.Items.Clear();
            cboFiltroPiso.Items.Add("Todos");

            foreach (Habitacion habitacion in habitaciones)
            {
                string piso = habitacion.Piso.ToString();
                bool yaExiste = false;

                foreach (object item in cboFiltroPiso.Items)
                {
                    if (item.ToString() == piso)
                    {
                        yaExiste = true;
                    }
                }

                if (yaExiste == false)
                {
                    cboFiltroPiso.Items.Add(piso);
                }
            }

            cboFiltroPiso.SelectedIndex = 0;
            _cargandoFiltros = false;
        }

        private void CargarHabitacionesFiltradas()
        {
            LimpiarFormulario();

            try
            {
                int? piso = ObtenerPisoSeleccionado();
                string estado = ObtenerEstadoSeleccionado();

                List<Habitacion> habitaciones =
                    _habitacionService.Filtrar(piso, estado);

                dgvHabitaciones.DataSource = null;
                dgvHabitaciones.DataSource =
                    AplicarFiltroTipo(habitaciones);

                LimpiarFormulario();
                ColorearFilas();
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo aplicar el filtro de habitaciones.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al aplicar los filtros.");
            }
        }

        // Deja sólo las habitaciones del tipo elegido. El servicio no ofrece
        // este filtro, así que se recorta la lista ya devuelta.
        private List<Habitacion> AplicarFiltroTipo(List<Habitacion> origen)
        {
            if (origen == null || _cboFiltroTipo == null)
            {
                return origen;
            }

            string tipo = _cboFiltroTipo.SelectedItem as string;

            if (string.IsNullOrEmpty(tipo) || tipo == "Todos")
            {
                return origen;
            }

            List<Habitacion> filtradas = new List<Habitacion>();

            foreach (Habitacion habitacion in origen)
            {
                if (string.Equals(
                        habitacion.Tipo,
                        tipo,
                        StringComparison.OrdinalIgnoreCase))
                {
                    filtradas.Add(habitacion);
                }
            }

            return filtradas;
        }

        private int? ObtenerPisoSeleccionado()
        {
            if (cboFiltroPiso.SelectedItem == null)
            {
                return null;
            }

            string valor = cboFiltroPiso.SelectedItem.ToString();
            if (valor == "Todos")
            {
                return null;
            }

            int piso;
            if (int.TryParse(valor, out piso))
            {
                return piso;
            }

            return null;
        }

        private string ObtenerEstadoSeleccionado()
        {
            if (cboFiltroEstado.SelectedItem == null)
            {
                return null;
            }

            string estado = cboFiltroEstado.SelectedItem.ToString();
            if (estado == "Todos")
            {
                return null;
            }

            return estado;
        }

        private void ColorearFilas()
        {
            foreach (DataGridViewRow fila in dgvHabitaciones.Rows)
            {
                Habitacion habitacion = fila.DataBoundItem as Habitacion;
                if (habitacion != null)
                {
                    fila.DefaultCellStyle.BackColor =
                        ObtenerColorSuave(habitacion.Estado);
                }
            }
        }

        private Color ObtenerColorSuave(string estado)
        {
            return TemaVisual.Colores.PorEstadoSuave(estado);
        }

        private void dgvHabitaciones_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (_limpiandoFormulario)
            {
                return;
            }

            if (dgvHabitaciones.CurrentRow == null)
            {
                return;
            }

            Habitacion habitacion =
                dgvHabitaciones.CurrentRow.DataBoundItem as Habitacion;

            if (habitacion == null)
            {
                return;
            }

            _numeroSeleccionado = habitacion.Numero;
            txtNumero.Text = habitacion.Numero.ToString();
            txtNumero.Enabled = false;
            numPiso.Value = habitacion.Piso;
            cboTipo.SelectedItem = habitacion.Tipo;
            cboEstado.SelectedItem = habitacion.Estado;
            txtTarifaBase.Value = habitacion.TarifaBase;
            numCapacidad.Value = habitacion.Capacidad;
            ActualizarEstadoBotonEliminar();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtNumero.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _limpiandoFormulario = true;
            _numeroSeleccionado = null;
            txtNumero.Enabled = true;
            txtNumero.Clear();
            numPiso.Value = 1;
            cboTipo.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
            txtTarifaBase.Value = 1;
            numCapacidad.Value = 1;
            dgvHabitaciones.ClearSelection();
            dgvHabitaciones.CurrentCell = null;
            _limpiandoFormulario = false;
            ActualizarEstadoBotonEliminar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                btnGuardar.Enabled = false;
                Habitacion habitacion = CrearHabitacionDesdeFormulario();

                if (_numeroSeleccionado.HasValue)
                {
                    _habitacionService.Actualizar(habitacion);
                    MostrarInformacion("La habitación fue actualizada.");
                }
                else
                {
                    _habitacionService.Crear(habitacion);
                    MostrarInformacion("La habitación fue registrada.");
                }

                LimpiarFormulario();
                CargarDatos();
            }
            catch (HabitacionOcupadaException error)
            {
                MostrarError(error.Message);
            }
            catch (InvalidOperationException error)
            {
                MostrarError(error.Message);
            }
            catch (ArgumentException error)
            {
                MostrarError(error.Message);
            }
            catch (FormatException error)
            {
                MostrarError(error.Message);
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo guardar la habitación. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al guardar la habitación.");
            }
            finally
            {
                btnGuardar.Enabled = true;
            }
        }

        private Habitacion CrearHabitacionDesdeFormulario()
        {
            int numero;
            if (int.TryParse(txtNumero.Text.Trim(), out numero) == false)
            {
                throw new FormatException(
                    "Ingrese un número de habitación válido.");
            }

            if (cboTipo.SelectedItem == null)
            {
                throw new FormatException(
                    "Seleccione el tipo de habitación.");
            }

            if (cboEstado.SelectedItem == null)
            {
                throw new FormatException(
                    "Seleccione el estado de la habitación.");
            }

            string tipo = cboTipo.SelectedItem.ToString();

            Habitacion habitacion = new Habitacion();
            habitacion.Numero = numero;
            habitacion.Piso = Convert.ToInt32(numPiso.Value);
            habitacion.Tipo = tipo;
            habitacion.Capacidad = Convert.ToInt32(numCapacidad.Value);
            habitacion.TarifaBase = txtTarifaBase.Value;
            habitacion.Estado = cboEstado.SelectedItem.ToString();

            return habitacion;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_numeroSeleccionado.HasValue == false)
            {
                MostrarError("Seleccione una habitación para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar la habitación " +
                _numeroSeleccionado.Value + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            try
            {
                btnEliminar.Enabled = false;
                _habitacionService.Eliminar(
                    _numeroSeleccionado.Value,
                    _usuarioActual);

                MostrarInformacion("La habitación fue eliminada.");
                LimpiarFormulario();
                CargarDatos();
            }
            catch (PermisoDenegadoException error)
            {
                MostrarError(error.Message);
            }
            catch (InvalidOperationException error)
            {
                MostrarError(error.Message);
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo eliminar la habitación. " +
                    "Puede tener datos relacionados.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al eliminar la habitación.");
            }
            finally
            {
                ActualizarEstadoBotonEliminar();
            }
        }

        private void ActualizarEstadoBotonEliminar()
        {
            bool esAdministrador =
                AutorizacionService.EsAdministrador(_usuarioActual);

            btnEliminar.Enabled =
                esAdministrador && _numeroSeleccionado.HasValue;
        }

        private void filtros_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (_cargandoFiltros)
            {
                return;
            }

            CargarHabitacionesFiltradas();
        }

        private void txtNumero_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            if (char.IsDigit(e.KeyChar) == false)
            {
                e.Handled = true;
            }
        }

        // Corrige la posición del cursor al hacer clic en txtNumero.
        //
        // El campo se estira a todo el ancho del marco redondeado (ver
        // TemaVisual.EnvolverCampo), mucho más ancho que sus 3 dígitos. Al
        // hacer clic en la franja vacía a la derecha del texto ya escrito,
        // WinForms no encuentra un carácter bajo el punto y coloca el cursor
        // en la última posición de la máscara en vez de la primera posición
        // vacía. Se corrige sólo para esa franja: un clic dentro del texto
        // ya escrito se deja tal cual lo resolvió el control, para no
        // interferir con la edición normal a mitad del valor.
        private void txtNumero_MouseUp(
            object sender,
            MouseEventArgs e)
        {
            string escrito = txtNumero.Text.TrimEnd(' ');
            Size medida = TextRenderer.MeasureText(escrito, txtNumero.Font);

            if (e.X > medida.Width)
            {
                txtNumero.SelectionStart = escrito.Length;
                txtNumero.SelectionLength = 0;
            }
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Bisono",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void MostrarInformacion(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Bisono",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
