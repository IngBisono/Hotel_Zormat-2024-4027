using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    // Diálogo de alta pura para reservas. No expone ningún control de
    // Estado: toda reserva creada aquí nace "Pendiente" — la confirmación
    // definitiva ocurre en el check-in (FrmCheckInOut), no aquí. Se abre
    // exclusivamente desde el botón "Nueva reserva" de FrmReservas.cs.
    public partial class FrmNuevaReserva : Form
    {
        private readonly ReservaService _reservaService;
        private readonly HabitacionService _habitacionService;

        // Huésped elegido en FrmBuscarHuesped. El campo de texto solo
        // muestra su nombre; este es el dato real que se envía al guardar.
        private Huesped _huespedSeleccionado;

        // Líneas adicionales del resumen. Se crean por código para no tocar
        // FrmNuevaReserva.Designer.cs.
        private Label _lblTarifaNoche;
        private Label _lblFactorTemporada;
        private Label _lblNotaImpuestos;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmNuevaReserva()
        {
            InitializeComponent();
            _reservaService = new ReservaService();
            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        // Este constructor recibe al usuario que inicio sesion.
        public FrmNuevaReserva(Usuario usuarioActual) : this()
        {
            if (usuarioActual != null)
            {
                Text = "Hotel Bisono - Nueva Reserva - " +
                    usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Nueva Reserva");

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            txtHuesped.ReadOnly = true;
            txtHuesped.BackColor = TemaVisual.Colores.FondoSecundario;

            // ReadOnly sólo bloquea la edición, no el foco: sin esto, el
            // formulario enfoca el campo al abrir (es el primer TabIndex) y
            // el cursor queda parpadeando como si se pudiera escribir ahí.
            // TabStop=false lo saca del ciclo de Tab, y GotFocus cubre el
            // caso de un clic directo con el mouse, redirigiendo al botón
            // "Buscar" que es la única forma real de completar este campo.
            txtHuesped.TabStop = false;
            txtHuesped.GotFocus += delegate { btnBuscarHuesped.Focus(); };

            TemaVisual.EstilizarBoton(
                btnBuscarHuesped,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            TemaVisual.PonerGlifo(
                btnBuscarHuesped,
                TemaVisual.Glifos.Buscar,
                "Buscar");

            ConfigurarCombo(cboHabitacion);
            ConfigurarCombo(cboTemporada);

            cboTemporada.Items.Clear();
            cboTemporada.Items.Add("Alta");
            cboTemporada.Items.Add("Media");
            cboTemporada.Items.Add("Baja");
            cboTemporada.SelectedIndex = 0;

            dtpCheckIn.Format = DateTimePickerFormat.Short;
            dtpCheckOut.Format = DateTimePickerFormat.Short;
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            TemaVisual.EstilizarFecha(dtpCheckIn);
            TemaVisual.EstilizarFecha(dtpCheckOut);

            lblNochesCalculadas.Text = "Noches: 1";
            lblMontoCalculado.Text = "RD$0.00";

            TemaVisual.EstilizarBoton(
                btnGuardar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnGuardar,
                TemaVisual.Glifos.Guardar,
                "Guardar reserva");

            ConfigurarDistribucion();

            AcceptButton = btnGuardar;

            Load += FrmNuevaReserva_Load;
            btnBuscarHuesped.Click += btnBuscarHuesped_Click;
            btnGuardar.Click += btnGuardar_Click;
            dtpCheckIn.ValueChanged += DatosReserva_Changed;
            dtpCheckOut.ValueChanged += DatosReserva_Changed;
            cboHabitacion.SelectedIndexChanged += DatosReserva_Changed;
            cboTemporada.SelectedIndexChanged += DatosReserva_Changed;
        }

        // Organiza los campos en dos columnas, con el resumen a la derecha.
        private void ConfigurarDistribucion()
        {
            Size = new Size(760, 620);

            TableLayoutPanel tabla = new TableLayoutPanel();
            tabla.Dock = DockStyle.Fill;
            tabla.Padding = new Padding(22, 18, 22, 18);
            tabla.BackColor = TemaVisual.Colores.FondoClaro;
            tabla.ColumnCount = 3;
            tabla.RowCount = 6;
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320F));

            for (int fila = 0; fila < 5; fila++)
            {
                tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // El campo de huésped ocupa un renglón propio con dos columnas:
            // el texto de solo lectura y, a su derecha, el botón de buscar.
            AgregarCampoHuesped(tabla, 0);
            AgregarCampo(tabla, "Habitación", cboHabitacion, 1);
            AgregarCampo(tabla, "Fecha de entrada", dtpCheckIn, 2);
            AgregarCampo(tabla, "Fecha de salida", dtpCheckOut, 3);
            AgregarCampo(tabla, "Temporada", cboTemporada, 4);

            panel1.Controls.Clear();
            panel1.Dock = DockStyle.Fill;
            panel1.Margin = new Padding(6, 10, 6, 10);
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
            tabla.Controls.Add(panel1, 2, 0);
            // RowSpan=6 (no 5): las primeras 5 filas son AutoSize y, juntas,
            // no llegan a la altura real del contenido del resumen (título +
            // 3 líneas + nota de impuestos ≈ 238px). Al incluir también la
            // fila 5 (Percent 100F, donde vive btnGuardar en la columna 1),
            // el panel1 puede crecer con el espacio restante del formulario
            // sin que el texto se recorte. No choca con btnGuardar porque
            // ese botón vive en la columna 1, no en la 2.
            tabla.SetRowSpan(panel1, 6);

            btnGuardar.AutoSize = false;
            btnGuardar.Size = new Size(190, 38);
            btnGuardar.Anchor = AnchorStyles.Left;
            btnGuardar.Margin = new Padding(6, 10, 6, 6);
            tabla.Controls.Add(btnGuardar, 1, 5);

            Panel encabezado = TemaVisual.CrearEncabezado(
                "Nueva Reserva",
                TemaVisual.Glifos.Reserva);
            Controls.Add(tabla);
            Controls.Add(encabezado);
            tabla.BringToFront();

            flpNuevaReserva.Visible = false;
        }

        // Agrega el campo de huésped: texto de solo lectura + botón buscar.
        private void AgregarCampoHuesped(TableLayoutPanel tabla, int fila)
        {
            Label etiqueta = CrearEtiqueta("Huésped");
            tabla.Controls.Add(etiqueta, 0, fila);

            Panel contenedor = new Panel();
            contenedor.Dock = DockStyle.Fill;
            contenedor.Margin = new Padding(6, 8, 6, 8);
            contenedor.Height = 36;

            Panel marco = TemaVisual.EnvolverCampo(
                txtHuesped,
                TemaVisual.Colores.Blanco);
            marco.Dock = DockStyle.Fill;
            marco.Margin = new Padding(0);

            btnBuscarHuesped.AutoSize = false;
            btnBuscarHuesped.Size = new Size(100, 30);
            btnBuscarHuesped.Dock = DockStyle.Right;
            btnBuscarHuesped.Margin = new Padding(0);

            contenedor.Controls.Add(marco);
            contenedor.Controls.Add(btnBuscarHuesped);
            marco.BringToFront();

            // Ocupa solo la columna 1, igual que AgregarCampo: la columna 2
            // es del panel1 de resumen (RowSpan=5 desde la fila 0) y un
            // SetColumnSpan aquí invadía esa celda, produciendo controles
            // superpuestos y el recorte visual del resumen.
            tabla.Controls.Add(contenedor, 1, fila);
        }

        // Arma la tarjeta de resumen. Misma fórmula y misma nota que en
        // FrmReservas.cs: ITBIS y propina se aplican al facturar, no aquí.
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
            titulo.Size = new Size(272, 26);
            titulo.Location = new Point(20, 16);
            titulo.TextAlign = ContentAlignment.MiddleLeft;

            lblNochesCalculadas.Dock = DockStyle.None;
            lblNochesCalculadas.AutoSize = false;
            lblNochesCalculadas.BackColor = Color.Transparent;
            lblNochesCalculadas.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblNochesCalculadas.Font = TemaVisual.Fuentes.Cuerpo;
            lblNochesCalculadas.Size = new Size(272, 22);
            lblNochesCalculadas.Location = new Point(20, 54);
            lblNochesCalculadas.TextAlign = ContentAlignment.MiddleLeft;

            _lblTarifaNoche = CrearLineaResumen(new Point(20, 78));
            _lblFactorTemporada = CrearLineaResumen(new Point(20, 102));

            lblMontoCalculado.Dock = DockStyle.None;
            lblMontoCalculado.AutoSize = false;
            lblMontoCalculado.BackColor = Color.Transparent;
            lblMontoCalculado.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblMontoCalculado.Font = TemaVisual.Fuentes.MontoTotal;
            lblMontoCalculado.Size = new Size(272, 40);
            lblMontoCalculado.Location = new Point(20, 138);
            lblMontoCalculado.TextAlign = ContentAlignment.MiddleLeft;

            _lblNotaImpuestos = new Label();
            _lblNotaImpuestos.Text =
                "El ITBIS (18%) y la propina (10%) se aplican al facturar, " +
                "no sobre este estimado.";
            _lblNotaImpuestos.Font = TemaVisual.Fuentes.Pequena;
            _lblNotaImpuestos.ForeColor = TemaVisual.Colores.AzulMarino;
            _lblNotaImpuestos.BackColor = Color.Transparent;
            _lblNotaImpuestos.AutoSize = false;
            _lblNotaImpuestos.Size = new Size(272, 52);
            _lblNotaImpuestos.Location = new Point(20, 186);
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
            linea.Size = new Size(272, 22);
            linea.Location = ubicacion;
            linea.TextAlign = ContentAlignment.MiddleLeft;
            return linea;
        }

        // Agrega una etiqueta y su campo en una fila de la tabla.
        private static void AgregarCampo(
            TableLayoutPanel tabla,
            string texto,
            Control campo,
            int fila)
        {
            Label etiqueta = CrearEtiqueta(texto);
            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(6, 8, 6, 8);

            tabla.Controls.Add(etiqueta, 0, fila);
            tabla.Controls.Add(campo, 1, fila);
        }

        private static Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Anchor = AnchorStyles.Left;
            etiqueta.Margin = new Padding(6, 12, 6, 6);
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.Texto;
            return etiqueta;
        }

        // Carga los datos necesarios al abrir la ventana.
        private void FrmNuevaReserva_Load(object sender, EventArgs e)
        {
            try
            {
                CargarHabitacionesDisponibles();
                CalcularResumen();
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudieron cargar las habitaciones disponibles.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Abre el buscador y toma el huesped que el usuario elija.
        private void btnBuscarHuesped_Click(object sender, EventArgs e)
        {
            using (FrmBuscarHuesped buscador = new FrmBuscarHuesped())
            {
                if (buscador.ShowDialog(this) == DialogResult.OK)
                {
                    _huespedSeleccionado = buscador.HuespedSeleccionado;
                    txtHuesped.Text =
                        _huespedSeleccionado.Nombre + " " +
                        _huespedSeleccionado.Apellido + " — " +
                        _huespedSeleccionado.NumeroDocumento;
                }
            }
        }

        // Vuelve a calcular noches y monto cuando cambia un dato.
        private void DatosReserva_Changed(object sender, EventArgs e)
        {
            CalcularResumen();
        }

        // Guarda la reserva. Siempre nace "Pendiente": el estado no se lee
        // de ningun control de este formulario.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;

            try
            {
                Reserva reserva = CrearReservaDesdeFormulario();
                _reservaService.Crear(reserva);

                MessageBox.Show(
                    "La reserva fue guardada correctamente.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
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
                    "No se pudo guardar la reserva en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                btnGuardar.Enabled = true;
            }
        }

        // Convierte los controles en un objeto Reserva.
        private Reserva CrearReservaDesdeFormulario()
        {
            Habitacion habitacion = cboHabitacion.SelectedItem as Habitacion;

            if (_huespedSeleccionado == null)
            {
                throw new FormatException("Seleccione un huesped.");
            }

            if (habitacion == null)
            {
                throw new FormatException("Seleccione una habitacion disponible.");
            }

            if (cboTemporada.SelectedIndex < 0)
            {
                throw new FormatException("Seleccione una temporada.");
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
            reserva.NumeroDocumentoHuesped = _huespedSeleccionado.NumeroDocumento;
            reserva.NumeroHabitacion = habitacion.Numero;
            reserva.FechaCheckIn = dtpCheckIn.Value.Date;
            reserva.FechaCheckOut = dtpCheckOut.Value.Date;
            reserva.Temporada = cboTemporada.Text;
            reserva.Estado = "Pendiente";
            reserva.TotalNoches = noches;
            reserva.MontoEstimado = monto;

            return reserva;
        }

        // Carga solamente las habitaciones disponibles.
        private void CargarHabitacionesDisponibles()
        {
            List<Habitacion> habitaciones = _habitacionService.ObtenerTodas();
            List<Habitacion> disponibles = new List<Habitacion>();

            foreach (Habitacion habitacion in habitaciones)
            {
                if (habitacion.Estado == "Disponible")
                {
                    disponibles.Add(habitacion);
                }
            }

            cboHabitacion.DataSource = null;
            cboHabitacion.DisplayMember = "Numero";
            cboHabitacion.ValueMember = "Numero";
            cboHabitacion.DataSource = disponibles;
        }

        // Calcula el resumen que se muestra antes de guardar.
        private void CalcularResumen()
        {
            Habitacion habitacion = cboHabitacion.SelectedItem as Habitacion;

            if (habitacion == null)
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
