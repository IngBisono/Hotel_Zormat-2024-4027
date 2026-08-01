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
                Text = "Reservas - " + _usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            Text = "Gestion de reservas";
            BackColor = Color.FromArgb(245, 248, 251);

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

            lblNochesCalculadas.Text = "Noches: 1";
            lblMontoCalculado.Text = "Monto estimado: RD$0.00";
            lblNochesCalculadas.BackColor = Color.FromArgb(253, 224, 146);
            lblMontoCalculado.BackColor = Color.FromArgb(253, 224, 146);
            lblNochesCalculadas.ForeColor = Color.FromArgb(46, 66, 86);
            lblMontoCalculado.ForeColor = Color.FromArgb(46, 66, 86);

            btnGuardar.Text = "Guardar reserva";
            btnGuardar.BackColor = Color.FromArgb(2, 88, 151);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;

            ConfigurarGrid(dgvReservasProximas);

            Load += FrmReservas_Load;
            btnGuardar.Click += btnGuardar_Click;
            dtpCheckIn.ValueChanged += DatosReserva_Changed;
            dtpCheckOut.ValueChanged += DatosReserva_Changed;
            cboHabitacion.SelectedIndexChanged += DatosReserva_Changed;
            cboTemporada.SelectedIndexChanged += DatosReserva_Changed;
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
            try
            {
                Reserva reserva = CrearReservaDesdeFormulario();
                _reservaService.Guardar(reserva);

                MessageBox.Show(
                    "La reserva fue guardada correctamente.",
                    "Hotel Zormat",
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
                lblNochesCalculadas.Text = "Noches: 0";
                lblMontoCalculado.Text = "Monto estimado: RD$0.00";
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
            catch (Exception)
            {
                lblNochesCalculadas.Text = "Noches: 0";
                lblMontoCalculado.Text = "Monto estimado: RD$0.00";
            }
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
        }

        // Configura la tabla de reservas para consulta.
        private static void ConfigurarGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
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
            string mensaje = error.Message;

            if (error is SqlException)
            {
                mensaje = "No fue posible comunicarse con la base de datos.";
            }

            MessageBox.Show(
                mensaje,
                "Hotel Zormat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
