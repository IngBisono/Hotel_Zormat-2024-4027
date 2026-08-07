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
                Text = "Hotel Bisono - Check-in y check-out - " +
                       _usuarioActual.NombreCompleto;
            }
        }

        // Prepara textos, colores, tablas y eventos.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(
                this,
                "Hotel Bisono - Check-in y check-out");

            TemaVisual.EstilizarBoton(
                btnCheckIn,
                TemaVisual.Colores.TurquesaPalmera,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaProfundo);
            TemaVisual.PonerGlifo(
                btnCheckIn,
                TemaVisual.Glifos.CheckIn,
                "Registrar check-in");
            btnCheckIn.Enabled = false;

            TemaVisual.EstilizarBoton(
                btnCheckOut,
                TemaVisual.Colores.ArenaDorada,
                TemaVisual.Colores.AzulProfundo,
                TemaVisual.Colores.SolDurazno);
            TemaVisual.PonerGlifo(
                btnCheckOut,
                TemaVisual.Glifos.CheckOut,
                "Registrar check-out");
            btnCheckOut.Enabled = false;

            lblEstadoHabitacionActual.Text =
                "Seleccione una reserva o una estadía.";
            lblEstadoHabitacionActual.Font = TemaVisual.Fuentes.Cuerpo;
            lblEstadoHabitacionActual.ForeColor = TemaVisual.Colores.TextoSuave;

            ConfigurarGrid(dgvReservasConfirmadas);
            ConfigurarGrid(dgvEstadiasActivas);
            ConfigurarDistribucion();

            Load += FrmCheckInOut_Load;
            btnCheckIn.Click += btnCheckIn_Click;
            btnCheckOut.Click += btnCheckOut_Click;
            dgvReservasConfirmadas.CellClick +=
                dgvReservasConfirmadas_CellClick;
            dgvEstadiasActivas.CellClick +=
                dgvEstadiasActivas_CellClick;
        }

        // Divide la pantalla en las secciones de entrada y salida.
        private void ConfigurarDistribucion()
        {
            Size = new Size(900, 720);
            MinimumSize = new Size(820, 620);

            Controls.Add(tableLayoutPanel1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(16, 14, 16, 16);
            tableLayoutPanel1.BackColor = TemaVisual.Colores.FondoClaro;
            tableLayoutPanel1.CellBorderStyle =
                TableLayoutPanelCellBorderStyle.None;

            // El control acoplado a Fill debe quedar al frente porque el
            // acoplamiento se resuelve del último control al primero.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Check-in / Check-out",
                TemaVisual.Glifos.CheckIn);
            Controls.Add(encabezado);
            tableLayoutPanel1.BringToFront();

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            Panel seccionCheckIn = CrearSeccion(
                "Check-in · Reservas confirmadas",
                TemaVisual.Glifos.CheckIn,
                dgvReservasConfirmadas,
                btnCheckIn,
                null);

            Panel seccionCheckOut = CrearSeccion(
                "Check-out · Estadías activas",
                TemaVisual.Glifos.CheckOut,
                dgvEstadiasActivas,
                btnCheckOut,
                lblEstadoHabitacionActual);

            tableLayoutPanel1.Controls.Add(seccionCheckIn, 0, 0);
            tableLayoutPanel1.Controls.Add(seccionCheckOut, 0, 1);
            flpCheckInOut.Visible = false;
        }

        // Crea una seccion con titulo, tabla y boton de accion.
        private static Panel CrearSeccion(
            string titulo,
            string glifo,
            DataGridView tabla,
            Button boton,
            Label etiquetaEstado)
        {
            Panel seccion = new Panel();
            seccion.Dock = DockStyle.Fill;
            seccion.Margin = new Padding(0, 4, 0, 8);
            seccion.Padding = new Padding(16, 12, 16, 12);
            seccion.BackColor = TemaVisual.Colores.Blanco;

            Label encabezado = TemaVisual.CrearTituloSeccion(titulo, glifo);
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 34;
            encabezado.BackColor = TemaVisual.Colores.Blanco;

            TableLayoutPanel barraAcciones = new TableLayoutPanel();
            barraAcciones.Dock = DockStyle.Bottom;
            barraAcciones.Height = 52;
            barraAcciones.BackColor = TemaVisual.Colores.Blanco;
            barraAcciones.ColumnCount = 2;
            barraAcciones.RowCount = 1;
            barraAcciones.ColumnStyles.Add(
                new ColumnStyle(SizeType.AutoSize));
            barraAcciones.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            boton.AutoSize = false;
            boton.Size = new Size(206, 38);
            boton.Anchor = AnchorStyles.Left;
            boton.Margin = new Padding(0, 10, 16, 6);
            barraAcciones.Controls.Add(boton, 0, 0);

            if (etiquetaEstado != null)
            {
                etiquetaEstado.AutoSize = false;
                etiquetaEstado.Dock = DockStyle.Fill;
                etiquetaEstado.TextAlign = ContentAlignment.MiddleLeft;
                etiquetaEstado.Margin = new Padding(0, 10, 0, 6);
                etiquetaEstado.BackColor = TemaVisual.Colores.Blanco;
                barraAcciones.Controls.Add(etiquetaEstado, 1, 0);
            }

            tabla.Dock = DockStyle.Fill;

            // La tabla queda al frente para que su acoplamiento a Fill se
            // resuelva en último lugar y no invada el encabezado ni la barra.
            seccion.Controls.Add(encabezado);
            seccion.Controls.Add(barraAcciones);
            seccion.Controls.Add(tabla);
            tabla.BringToFront();

            return seccion;
        }

        // Carga las reservas y estadias al abrir la ventana.
        private void FrmCheckInOut_Load(object sender, EventArgs e)
        {
            try
            {
                CargarDatos();
            }
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                    "No se pudieron cargar los datos desde la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Registra la entrada de la reserva seleccionada.
        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            btnCheckIn.Enabled = false;
            bool checkInRegistrado = false;

            try
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

                if (reserva.Estado != "Confirmada")
                {
                    MostrarAdvertencia(
                        "Solo una reserva confirmada permite hacer check-in.");
                    return;
                }

                if (reserva.FechaCheckIn.Date > DateTime.Today)
                {
                    MostrarAdvertencia(
                        "No se puede registrar un check-in antes de la fecha de entrada.");
                    return;
                }

                _estadiaService.RegistrarCheckIn(
                    reserva.IdReserva,
                    _usuarioActual);

                checkInRegistrado = true;

                MessageBox.Show(
                    "El check-in fue registrado correctamente.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (HabitacionOcupadaException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                    "No se pudo registrar el check-in en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                if (checkInRegistrado == false)
                {
                    ActualizarEstadoBotones();
                }
            }

            if (checkInRegistrado == false)
            {
                return;
            }

            try
            {
                CargarDatos();
            }
            catch (PermisoDenegadoException)
            {
                MostrarProblemaRecarga("check-in");
            }
            catch (FormatException)
            {
                MostrarProblemaRecarga("check-in");
            }
            catch (InvalidOperationException)
            {
                MostrarProblemaRecarga("check-in");
            }
            catch (SqlException)
            {
                MostrarProblemaRecarga("check-in");
            }
            catch (Exception)
            {
                MostrarProblemaRecarga("check-in");
            }
        }

        // Registra la salida y abre el recibo generado.
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            btnCheckOut.Enabled = false;
            Factura factura = null;

            try
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

                factura = _estadiaService.RegistrarCheckOut(
                    estadia.IdEstadia,
                    _usuarioActual);
            }
            catch (HabitacionOcupadaException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                    "No se pudo registrar el check-out en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                if (factura == null)
                {
                    ActualizarEstadoBotones();
                }
            }

            if (factura == null)
            {
                return;
            }

            try
            {
                CargarDatos();
            }
            catch (PermisoDenegadoException)
            {
                MostrarProblemaRecarga("check-out");
            }
            catch (FormatException)
            {
                MostrarProblemaRecarga("check-out");
            }
            catch (InvalidOperationException)
            {
                MostrarProblemaRecarga("check-out");
            }
            catch (SqlException)
            {
                MostrarProblemaRecarga("check-out");
            }
            catch (Exception)
            {
                MostrarProblemaRecarga("check-out");
            }

            using (FrmFactura formularioFactura = new FrmFactura(factura))
            {
                formularioFactura.ShowDialog(this);
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
            ActualizarEstadoBotones();
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

                ActualizarEstadoBotones();
            }
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                    "No se pudo consultar la reserva en la base de datos.");
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
            ActualizarEstadoBotones();
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

                if (reserva.FechaCheckIn.Date > DateTime.Today)
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

        // Habilita cada boton solamente cuando su seleccion es valida.
        private void ActualizarEstadoBotones()
        {
            Reserva reserva = ObtenerReservaSeleccionada();
            bool puedeHacerCheckIn = false;

            if (_usuarioActual != null && reserva != null)
            {
                if (reserva.Estado == "Confirmada" &&
                    reserva.FechaCheckIn.Date <= DateTime.Today)
                {
                    puedeHacerCheckIn = true;
                }
            }

            btnCheckIn.Enabled = puedeHacerCheckIn;

            Estadia estadia = ObtenerEstadiaSeleccionada();
            btnCheckOut.Enabled = _usuarioActual != null && estadia != null;
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
            catch (PermisoDenegadoException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                    "No se pudo consultar la habitacion en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Configura una tabla para consulta y seleccion completa.
        private static void ConfigurarGrid(DataGridView grid)
        {
            TemaVisual.EstilizarGrid(grid);
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

        // Aclara que la operacion termino aunque falle la recarga.
        private static void MostrarProblemaRecarga(string operacion)
        {
            MostrarAdvertencia(
                "El " + operacion +
                " se registro, pero no se pudieron actualizar las listas. " +
                "Cierre y abra este formulario para ver los datos actuales.");
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
