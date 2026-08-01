using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmCheckInOut : Form
    {
        private readonly ReservaService _reservaService;
        private readonly EstadiaService _estadiaService;
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmCheckInOut()
        {
            InitializeComponent();
            _reservaService = new ReservaService();
            _estadiaService = new EstadiaService();
            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        // Este constructor recibe al usuario que inicio sesion.
        public FrmCheckInOut(Usuario usuarioActual) : this()
        {
            _usuarioActual = usuarioActual;

            if (_usuarioActual != null)
            {
                Text = "Check-in y check-out - " +
                       _usuarioActual.NombreCompleto;
            }
        }

        // Prepara textos, colores, tablas y eventos.
        private void ConfigurarFormulario()
        {
            Text = "Check-in y check-out";
            BackColor = Color.FromArgb(245, 248, 251);

            btnCheckIn.Text = "Registrar check-in";
            btnCheckIn.BackColor = Color.FromArgb(10, 167, 200);
            btnCheckIn.ForeColor = Color.White;
            btnCheckIn.FlatStyle = FlatStyle.Flat;

            btnCheckOut.Text = "Registrar check-out";
            btnCheckOut.BackColor = Color.FromArgb(19, 57, 88);
            btnCheckOut.ForeColor = Color.White;
            btnCheckOut.FlatStyle = FlatStyle.Flat;

            lblEstadoHabitacionActual.Text =
                "Seleccione una reserva o una estadia.";
            lblEstadoHabitacionActual.ForeColor = Color.FromArgb(46, 66, 86);

            ConfigurarGrid(dgvReservasConfirmadas);
            ConfigurarGrid(dgvEstadiasActivas);

            Load += FrmCheckInOut_Load;
            btnCheckIn.Click += btnCheckIn_Click;
            btnCheckOut.Click += btnCheckOut_Click;
            dgvReservasConfirmadas.CellClick +=
                dgvReservasConfirmadas_CellClick;
            dgvEstadiasActivas.CellClick +=
                dgvEstadiasActivas_CellClick;
        }

        // Carga las reservas y estadias al abrir la ventana.
        private void FrmCheckInOut_Load(object sender, EventArgs e)
        {
            try
            {
                CargarDatos();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Registra la entrada de la reserva seleccionada.
        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            Reserva reserva = ObtenerReservaSeleccionada();

            if (reserva == null)
            {
                MostrarAdvertencia(
                    "Seleccione una reserva confirmada para hacer check-in.");
                return;
            }

            if (_usuarioActual == null)
            {
                MostrarAdvertencia(
                    "Debe iniciar sesion antes de registrar un check-in.");
                return;
            }

            if (reserva.FechaCheckIn.Date != DateTime.Today)
            {
                MostrarAdvertencia(
                    "El check-in solo puede registrarse en la fecha de entrada.");
                return;
            }

            try
            {
                _estadiaService.RegistrarCheckIn(
                    reserva.IdReserva,
                    _usuarioActual);

                MessageBox.Show(
                    "El check-in fue registrado correctamente.",
                    "Hotel Zormat",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarDatos();
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo registrar el check-in en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Registra la salida y abre el recibo generado.
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Estadia estadia = ObtenerEstadiaSeleccionada();

            if (estadia == null)
            {
                MostrarAdvertencia(
                    "Seleccione una estadia activa para hacer check-out.");
                return;
            }

            if (_usuarioActual == null)
            {
                MostrarAdvertencia(
                    "Debe iniciar sesion antes de registrar un check-out.");
                return;
            }

            try
            {
                Factura factura = _estadiaService.RegistrarCheckOut(
                    estadia.IdEstadia,
                    _usuarioActual);

                CargarDatos();

                FrmFactura formularioFactura = new FrmFactura(factura);
                formularioFactura.ShowDialog(this);
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudo registrar el check-out en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Muestra el estado de la habitacion de una reserva.
        private void dgvReservasConfirmadas_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            Reserva reserva =
                dgvReservasConfirmadas.Rows[e.RowIndex].DataBoundItem
                as Reserva;

            if (reserva == null)
            {
                return;
            }

            MostrarEstadoHabitacion(reserva.NumeroHabitacion);
        }

        // Muestra el estado de la habitacion de una estadia.
        private void dgvEstadiasActivas_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            Estadia estadia =
                dgvEstadiasActivas.Rows[e.RowIndex].DataBoundItem
                as Estadia;

            if (estadia == null)
            {
                return;
            }

            try
            {
                Reserva reserva = _reservaService.Buscar(estadia.IdReserva);

                if (reserva != null)
                {
                    MostrarEstadoHabitacion(reserva.NumeroHabitacion);
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Carga ambas tablas y limpia el texto de estado.
        private void CargarDatos()
        {
            CargarReservasConfirmadas();
            CargarEstadiasActivas();
            lblEstadoHabitacionActual.Text =
                "Seleccione una reserva o una estadia.";
        }

        // Carga reservas confirmadas que aun no tienen estadia.
        private void CargarReservasConfirmadas()
        {
            List<Reserva> reservas = _reservaService.ObtenerTodas();
            List<Estadia> estadias = _estadiaService.ObtenerTodas();
            List<Reserva> disponibles = new List<Reserva>();

            foreach (Reserva reserva in reservas)
            {
                if (reserva.Estado != "Confirmada")
                {
                    continue;
                }

                if (reserva.FechaCheckIn.Date != DateTime.Today)
                {
                    continue;
                }

                bool tieneEstadia = false;

                foreach (Estadia estadia in estadias)
                {
                    if (estadia.IdReserva == reserva.IdReserva)
                    {
                        tieneEstadia = true;
                        break;
                    }
                }

                if (tieneEstadia == false)
                {
                    disponibles.Add(reserva);
                }
            }

            dgvReservasConfirmadas.DataSource = null;
            dgvReservasConfirmadas.DataSource = disponibles;
        }

        // Carga las estadias que todavia estan abiertas.
        private void CargarEstadiasActivas()
        {
            dgvEstadiasActivas.DataSource = null;
            dgvEstadiasActivas.DataSource = _estadiaService.ObtenerActivas();
        }

        // Devuelve la reserva marcada en la primera tabla.
        private Reserva ObtenerReservaSeleccionada()
        {
            if (dgvReservasConfirmadas.CurrentRow == null)
            {
                return null;
            }

            return dgvReservasConfirmadas.CurrentRow.DataBoundItem as Reserva;
        }

        // Devuelve la estadia marcada en la segunda tabla.
        private Estadia ObtenerEstadiaSeleccionada()
        {
            if (dgvEstadiasActivas.CurrentRow == null)
            {
                return null;
            }

            return dgvEstadiasActivas.CurrentRow.DataBoundItem as Estadia;
        }

        // Consulta y muestra el estado actual de una habitacion.
        private void MostrarEstadoHabitacion(int numeroHabitacion)
        {
            try
            {
                Habitacion habitacion =
                    _habitacionService.Buscar(numeroHabitacion);

                if (habitacion == null)
                {
                    lblEstadoHabitacionActual.Text =
                        "La habitacion no fue encontrada.";
                    return;
                }

                lblEstadoHabitacionActual.Text =
                    "Habitacion " + habitacion.Numero +
                    " - Estado: " + habitacion.Estado;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Configura una tabla para consulta y seleccion completa.
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
                "Revise la operacion",
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
