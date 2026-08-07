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
    public partial class FrmReservas : Form
    {
        private readonly ReservaService _reservaService;
        private readonly HuespedService _huespedService;
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;

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
                Text = "Hotel Bisono - Reservas - " +
                    _usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Reservas");

            ConfigurarCombo(cboHuesped);
            ConfigurarCombo(cboHabitacion);
            ConfigurarCombo(cboTemporada);
            ConfigurarCombo(cboEstadoReserva);

            cboTemporada.Items.Clear();
            cboTemporada.Items.Add("Alta");
            cboTemporada.Items.Add("Media");
            cboTemporada.Items.Add("Baja");
            cboTemporada.SelectedIndex = 0;

            cboEstadoReserva.Items.Clear();
            cboEstadoReserva.Items.Add("Pendiente");
            cboEstadoReserva.Items.Add("Confirmada");
            cboEstadoReserva.Items.Add("Cancelada");
            cboEstadoReserva.SelectedIndex = 1;

            dtpCheckIn.Format = DateTimePickerFormat.Short;
            dtpCheckOut.Format = DateTimePickerFormat.Short;
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            TemaVisual.EstilizarFecha(dtpCheckIn);
            TemaVisual.EstilizarFecha(dtpCheckOut);

            lblNochesCalculadas.Text = "Noches: 1";
            lblMontoCalculado.Text = "Monto estimado: RD$0.00";
            lblNochesCalculadas.Font = TemaVisual.Fuentes.CuerpoNegrita;
            lblMontoCalculado.Font = TemaVisual.Fuentes.MontoTotal;
            lblNochesCalculadas.ForeColor = TemaVisual.Colores.AzulMarino;
            lblMontoCalculado.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblNochesCalculadas.BackColor = Color.Transparent;
            lblMontoCalculado.BackColor = Color.Transparent;

            TemaVisual.EstilizarBoton(
                btnGuardar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnGuardar,
                TemaVisual.Glifos.Guardar,
                "Guardar reserva");

            ConfigurarGrid(dgvReservasProximas);
            ConfigurarDistribucion();

            Load += FrmReservas_Load;
            btnGuardar.Click += btnGuardar_Click;
            dtpCheckIn.ValueChanged += DatosReserva_Changed;
            dtpCheckOut.ValueChanged += DatosReserva_Changed;
            cboHabitacion.SelectedIndexChanged += DatosReserva_Changed;
            cboTemporada.SelectedIndexChanged += DatosReserva_Changed;
        }

        // Organiza los campos en dos columnas y deja crecer la tabla.
        private void ConfigurarDistribucion()
        {
            Size = new Size(840, 720);
            MinimumSize = new Size(800, 640);

            Controls.Add(tableLayoutPanel1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(22, 18, 22, 18);
            tableLayoutPanel1.BackColor = TemaVisual.Colores.FondoClaro;

            // El encabezado se acopla arriba y la tabla al resto: el control
            // acoplado a Fill debe quedar al frente porque el acoplamiento se
            // resuelve del último control al primero.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Nueva Reserva",
                TemaVisual.Glifos.Reserva);
            Controls.Add(encabezado);
            tableLayoutPanel1.BringToFront();

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            for (int fila = 0; fila < 9; fila++)
            {
                tableLayoutPanel1.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));
            }

            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            AgregarCampo("Huésped", cboHuesped, 0);
            AgregarCampo("Habitación", cboHabitacion, 1);
            AgregarCampo("Fecha de entrada", dtpCheckIn, 2);
            AgregarCampo("Fecha de salida", dtpCheckOut, 3);
            AgregarCampo("Temporada", cboTemporada, 4);
            AgregarCampo("Estado", cboEstadoReserva, 5);

            // Tarjeta de resumen con el degradado cálido del atardecer.
            panel1.Controls.Clear();
            panel1.Dock = DockStyle.Fill;
            panel1.Height = 88;
            panel1.Margin = new Padding(6, 10, 6, 10);
            panel1.Padding = new Padding(18, 12, 18, 12);
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

            lblNochesCalculadas.AutoSize = false;
            lblNochesCalculadas.Dock = DockStyle.Top;
            lblNochesCalculadas.Height = 22;
            lblNochesCalculadas.TextAlign = ContentAlignment.MiddleLeft;

            lblMontoCalculado.AutoSize = false;
            lblMontoCalculado.Dock = DockStyle.Fill;
            lblMontoCalculado.TextAlign = ContentAlignment.MiddleLeft;

            panel1.Controls.Add(lblMontoCalculado);
            panel1.Controls.Add(lblNochesCalculadas);
            tableLayoutPanel1.Controls.Add(panel1, 0, 6);
            tableLayoutPanel1.SetColumnSpan(panel1, 2);

            btnGuardar.AutoSize = false;
            btnGuardar.Size = new Size(190, 38);
            btnGuardar.Anchor = AnchorStyles.Left;
            btnGuardar.Margin = new Padding(6, 6, 6, 10);
            tableLayoutPanel1.Controls.Add(btnGuardar, 1, 7);

            Label tituloTabla = TemaVisual.CrearTituloSeccion(
                "Reservas próximas",
                TemaVisual.Glifos.Reserva);
            tituloTabla.AutoSize = false;
            tituloTabla.Dock = DockStyle.Fill;
            tituloTabla.Margin = new Padding(6, 10, 6, 4);
            tableLayoutPanel1.Controls.Add(tituloTabla, 0, 8);
            tableLayoutPanel1.SetColumnSpan(tituloTabla, 2);

            dgvReservasProximas.Dock = DockStyle.Fill;
            dgvReservasProximas.Margin = new Padding(6);
            tableLayoutPanel1.Controls.Add(dgvReservasProximas, 0, 9);
            tableLayoutPanel1.SetColumnSpan(dgvReservasProximas, 2);

            flpReservas.Visible = false;
        }

        // Agrega una etiqueta y su campo en una fila de la tabla.
        private void AgregarCampo(
            string texto,
            Control campo,
            int fila)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Anchor = AnchorStyles.Left;
            etiqueta.Margin = new Padding(6, 12, 6, 6);
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.Texto;

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(6, 8, 6, 8);

            tableLayoutPanel1.Controls.Add(etiqueta, 0, fila);
            tableLayoutPanel1.Controls.Add(campo, 1, fila);
        }

        // Carga los datos necesarios al abrir la ventana.
        private void FrmReservas_Load(object sender, EventArgs e)
        {
            try
            {
                CargarHuespedes();
                CargarHabitacionesDisponibles();
                CargarReservasProximas();
                CalcularResumen();
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
                    "No se pudieron cargar las reservas desde la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Vuelve a calcular noches y monto cuando cambia un dato.
        private void DatosReserva_Changed(object sender, EventArgs e)
        {
            CalcularResumen();
        }

        // Guarda una reserva con los datos escritos.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;

            try
            {
                Reserva reserva = CrearReservaDesdeFormulario();
                _reservaService.Guardar(reserva);

                MessageBox.Show(
                    "La reserva fue guardada correctamente.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarHabitacionesDisponibles();
                CargarReservasProximas();
                PrepararNuevaReserva();
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
            Huesped huesped = cboHuesped.SelectedItem as Huesped;
            Habitacion habitacion = cboHabitacion.SelectedItem as Habitacion;

            if (huesped == null)
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
            reserva.NumeroDocumentoHuesped = huesped.NumeroDocumento;
            reserva.NumeroHabitacion = habitacion.Numero;
            reserva.FechaCheckIn = dtpCheckIn.Value.Date;
            reserva.FechaCheckOut = dtpCheckOut.Value.Date;
            reserva.Temporada = cboTemporada.Text;
            reserva.Estado = cboEstadoReserva.Text;
            reserva.TotalNoches = noches;
            reserva.MontoEstimado = monto;

            return reserva;
        }

        // Carga los huespedes en su lista.
        private void CargarHuespedes()
        {
            List<Huesped> huespedes = _huespedService.ObtenerTodos();
            cboHuesped.DataSource = null;
            cboHuesped.DisplayMember = "NumeroDocumento";
            cboHuesped.ValueMember = "NumeroDocumento";
            cboHuesped.DataSource = huespedes;
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

        // Carga las reservas de los proximos siete dias.
        private void CargarReservasProximas()
        {
            dgvReservasProximas.DataSource = null;
            dgvReservasProximas.DataSource =
                _reservaService.ObtenerProximasSieteDias();
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

                lblNochesCalculadas.Text = "Noches: " + noches;
                lblMontoCalculado.Text =
                    "Monto estimado: RD$" + monto.ToString("N2");
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
            lblMontoCalculado.Text = "Monto estimado: RD$0.00";
        }

        // Deja fechas y opciones listas para otra reserva.
        private void PrepararNuevaReserva()
        {
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            cboTemporada.SelectedIndex = 0;
            cboEstadoReserva.SelectedIndex = 1;

            if (cboHuesped.Items.Count > 0)
            {
                cboHuesped.SelectedIndex = 0;
            }

            if (cboHabitacion.Items.Count > 0)
            {
                cboHabitacion.SelectedIndex = 0;
            }

            CalcularResumen();
        }

        // Configura una lista para aceptar solo opciones existentes.
        private static void ConfigurarCombo(ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            TemaVisual.EstilizarCombo(combo);
        }

        // Configura la tabla de reservas para consulta.
        private static void ConfigurarGrid(DataGridView grid)
        {
            TemaVisual.EstilizarGrid(grid);
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
