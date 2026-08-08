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
    public partial class FrmCheckInOut : Form
    {
        private readonly ReservaService _reservaService;
        private readonly EstadiaService _estadiaService;
        private readonly HabitacionService _habitacionService;
        private readonly HuespedService _huespedService;
        private Usuario _usuarioActual;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmCheckInOut()
        {
            InitializeComponent();
            _reservaService = new ReservaService();
            _estadiaService = new EstadiaService();
            _habitacionService = new HabitacionService();
            _huespedService = new HuespedService();
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
            ConfigurarColumnasCheckIn();
            ConfigurarColumnasCheckOut();
            ConfigurarDistribucion();

            Load += FrmCheckInOut_Load;
            btnCheckIn.Click += btnCheckIn_Click;
            btnCheckOut.Click += btnCheckOut_Click;
            dgvReservasConfirmadas.CellClick +=
                dgvReservasConfirmadas_CellClick;
            dgvEstadiasActivas.CellClick +=
                dgvEstadiasActivas_CellClick;
            dgvReservasConfirmadas.DataBindingComplete +=
                dgvReservasConfirmadas_DataBindingComplete;
            dgvEstadiasActivas.DataBindingComplete +=
                dgvEstadiasActivas_DataBindingComplete;
        }

        // Reemplaza las columnas autogeneradas del grid de check-in por
        // columnas explícitas, en el orden pedido, con encabezados en
        // español. "Nombre" no es una propiedad de Reserva: se deja sin
        // DataPropertyName y se llena a mano en
        // dgvReservasConfirmadas_DataBindingComplete, una vez que el
        // binding real ya ocurrió.
        private void ConfigurarColumnasCheckIn()
        {
            dgvReservasConfirmadas.AutoGenerateColumns = false;
            dgvReservasConfirmadas.Columns.Clear();

            CultureInfo formatoFechas = new CultureInfo("es-DO");

            dgvReservasConfirmadas.Columns.Add(CrearColumnaTexto(
                "NumeroDocumentoHuesped", "ID", "colId", 13));
            dgvReservasConfirmadas.Columns.Add(CrearColumnaNoEnlazada(
                "colNombre", "Nombre", 22));
            dgvReservasConfirmadas.Columns.Add(CrearColumnaTexto(
                "NumeroHabitacion", "Habitacion", "colHabitacion", 9));
            dgvReservasConfirmadas.Columns.Add(CrearColumnaFecha(
                "FechaCheckIn", "Fecha Ingreso", "colFechaIngreso", 15,
                formatoFechas));
            dgvReservasConfirmadas.Columns.Add(CrearColumnaFecha(
                "FechaCheckOut", "Fecha Salida", "colFechaSalida", 15,
                formatoFechas));
            dgvReservasConfirmadas.Columns.Add(CrearColumnaTexto(
                "TotalNoches", "Noches", "colNoches", 8));

            DataGridViewTextBoxColumn columnaMonto =
                new DataGridViewTextBoxColumn();
            columnaMonto.DataPropertyName = "MontoEstimado";
            columnaMonto.HeaderText = "Monto";
            columnaMonto.Name = "colMonto";
            columnaMonto.FillWeight = 21;
            columnaMonto.DefaultCellStyle.Format = "'RD$'#,##0.00";
            columnaMonto.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvReservasConfirmadas.Columns.Add(columnaMonto);
        }

        // Reemplaza las columnas autogeneradas del grid de check-out.
        // Estadia sólo aporta la fecha real (que además siempre sale null
        // mientras la estadía sigue "Activa" — ver comentario en
        // dgvEstadiasActivas_DataBindingComplete); ID, Nombre, Habitacion y
        // Fecha Salida se resuelven vía la Reserva relacionada
        // (Estadia.IdReserva) y se llenan a mano en el mismo handler.
        private void ConfigurarColumnasCheckOut()
        {
            dgvEstadiasActivas.AutoGenerateColumns = false;
            dgvEstadiasActivas.Columns.Clear();

            dgvEstadiasActivas.Columns.Add(CrearColumnaNoEnlazada(
                "colId", "ID", 18));
            dgvEstadiasActivas.Columns.Add(CrearColumnaNoEnlazada(
                "colNombre", "Nombre", 34));
            dgvEstadiasActivas.Columns.Add(CrearColumnaNoEnlazada(
                "colHabitacion", "Habitacion", 16));
            dgvEstadiasActivas.Columns.Add(CrearColumnaNoEnlazada(
                "colFechaSalida", "Fecha Salida", 22));
        }

        // Crea una columna de texto simple enlazada a una propiedad del
        // modelo.
        private static DataGridViewTextBoxColumn CrearColumnaTexto(
            string propiedad,
            string encabezado,
            string nombreColumna,
            int pesoRelativo)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;
            columna.HeaderText = encabezado;
            columna.Name = nombreColumna;
            columna.FillWeight = pesoRelativo;
            return columna;
        }

        // Crea una columna de fecha con el formato "07-ago-26" usado en el
        // resto de la aplicación.
        private static DataGridViewTextBoxColumn CrearColumnaFecha(
            string propiedad,
            string encabezado,
            string nombreColumna,
            int pesoRelativo,
            CultureInfo cultura)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;
            columna.HeaderText = encabezado;
            columna.Name = nombreColumna;
            columna.FillWeight = pesoRelativo;
            columna.DefaultCellStyle.Format = "dd-MMM-yy";
            columna.DefaultCellStyle.FormatProvider = cultura;
            return columna;
        }

        // Crea una columna sin DataPropertyName: su valor se llena a mano
        // en un handler DataBindingComplete, después de que el binding
        // real ya ocurrió (Reserva/Estadia no traen nombre de huésped ni,
        // en el caso de Estadia, número de habitación).
        private static DataGridViewTextBoxColumn CrearColumnaNoEnlazada(
            string nombreColumna,
            string encabezado,
            int pesoRelativo)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.HeaderText = encabezado;
            columna.Name = nombreColumna;
            columna.FillWeight = pesoRelativo;
            return columna;
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
                "Llegadas de hoy",
                "Reservas pendientes de registrar entrada; al hacer " +
                    "check-in se confirman y se emite la factura",
                TemaVisual.Glifos.CheckIn,
                dgvReservasConfirmadas,
                btnCheckIn,
                null);

            Panel seccionCheckOut = CrearSeccion(
                "Salidas de hoy",
                "Estadías activas; la factura ya se emitió al hacer check-in",
                TemaVisual.Glifos.CheckOut,
                dgvEstadiasActivas,
                btnCheckOut,
                lblEstadoHabitacionActual);

            tableLayoutPanel1.Controls.Add(seccionCheckIn, 0, 0);
            tableLayoutPanel1.Controls.Add(seccionCheckOut, 0, 1);
            flpCheckInOut.Visible = false;
        }

        // Crea una seccion con titulo, subtitulo, tabla y boton de accion.
        private static Panel CrearSeccion(
            string titulo,
            string subtitulo,
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

            Label apoyo = new Label();
            apoyo.Text = subtitulo;
            apoyo.Dock = DockStyle.Top;
            apoyo.Height = 22;
            apoyo.Font = TemaVisual.Fuentes.Pequena;
            apoyo.ForeColor = TemaVisual.Colores.TextoSuave;
            apoyo.BackColor = TemaVisual.Colores.Blanco;
            apoyo.TextAlign = ContentAlignment.MiddleLeft;
            apoyo.Padding = new Padding(2, 0, 0, 0);

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
                etiquetaEstado.Anchor = AnchorStyles.Left;
                etiquetaEstado.Size = new Size(280, 30);
                etiquetaEstado.TextAlign = ContentAlignment.MiddleCenter;
                etiquetaEstado.Margin = new Padding(0, 14, 0, 6);
                etiquetaEstado.Font = TemaVisual.Fuentes.CuerpoNegrita;
                etiquetaEstado.BackColor = TemaVisual.Colores.Blanco;
                etiquetaEstado.ForeColor = TemaVisual.Colores.TextoSuave;
                TemaVisual.AplicarEsquinasRedondeadas(etiquetaEstado, 14);
                barraAcciones.Controls.Add(etiquetaEstado, 1, 0);
            }

            tabla.Dock = DockStyle.Fill;

            // El orden de inserción define el reparto: el subtítulo se agrega
            // antes que el encabezado para quedar justo debajo de él, y la
            // tabla pasa al frente para que su Fill se resuelva al final.
            seccion.Controls.Add(apoyo);
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

        // Registra la entrada de la reserva seleccionada y factura de
        // inmediato (el huesped paga al hacer check-in, no al check-out).
        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            btnCheckIn.Enabled = false;
            Factura factura = null;

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

                if (reserva.Estado == "Cancelada")
                {
                    MostrarAdvertencia(
                        "No se puede hacer check-in a una reserva cancelada.");
                    return;
                }

                if (reserva.FechaCheckIn.Date > DateTime.Today)
                {
                    MostrarAdvertencia(
                        "No se puede registrar un check-in antes de la fecha de entrada.");
                    return;
                }

                factura = _estadiaService.RegistrarCheckIn(
                    reserva.IdReserva,
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
                    "No se pudo registrar el check-in en la base de datos.");
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

            using (FrmFactura formularioFactura = new FrmFactura(factura))
            {
                formularioFactura.ShowDialog(this);
            }
        }

        // Registra la salida y libera la habitacion. Ya no emite factura:
        // el pago se hizo al hacer check-in.
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            btnCheckOut.Enabled = false;
            Estadia estadiaCerrada = null;

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

                estadiaCerrada = _estadiaService.RegistrarCheckOut(
                    estadia.IdEstadia,
                    _usuarioActual);

                MessageBox.Show(
                    "El check-out fue registrado correctamente.",
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
                    "No se pudo registrar el check-out en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                if (estadiaCerrada == null)
                {
                    ActualizarEstadoBotones();
                }
            }

            if (estadiaCerrada == null)
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

        // Llena la columna "Nombre" del grid de check-in después de cada
        // binding (Reserva no trae el nombre del huésped, sólo su
        // documento).
        private void dgvReservasConfirmadas_DataBindingComplete(
            object sender,
            DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvReservasConfirmadas.Rows)
            {
                Reserva reserva = fila.DataBoundItem as Reserva;

                if (reserva == null)
                {
                    continue;
                }

                fila.Cells["colNombre"].Value =
                    ObtenerNombreCompletoHuesped(reserva.NumeroDocumentoHuesped);
            }
        }

        // Llena ID, Nombre, Habitacion y Fecha Salida del grid de
        // check-out, resolviendo la Reserva relacionada a cada Estadia por
        // su IdReserva. "Fecha Salida" muestra Reserva.FechaCheckOut (la
        // fecha planeada), no Estadia.FechaSalidaReal: esa última siempre
        // sale null mientras la estadía sigue "Activa" (RegistrarCheckOut
        // exige que sea null para poder ejecutarse), así que bindearla
        // directo dejaría la columna vacía en todas las filas de este grid.
        private void dgvEstadiasActivas_DataBindingComplete(
            object sender,
            DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvEstadiasActivas.Rows)
            {
                Estadia estadia = fila.DataBoundItem as Estadia;

                if (estadia == null)
                {
                    continue;
                }

                Reserva reserva = null;

                try
                {
                    reserva = _reservaService.Buscar(estadia.IdReserva);
                }
                catch (SqlException)
                {
                }

                if (reserva == null)
                {
                    fila.Cells["colId"].Value = "—";
                    fila.Cells["colNombre"].Value = "—";
                    fila.Cells["colHabitacion"].Value = "—";
                    fila.Cells["colFechaSalida"].Value = "—";
                    continue;
                }

                fila.Cells["colId"].Value = reserva.NumeroDocumentoHuesped;
                fila.Cells["colNombre"].Value =
                    ObtenerNombreCompletoHuesped(reserva.NumeroDocumentoHuesped);
                fila.Cells["colHabitacion"].Value = reserva.NumeroHabitacion;
                fila.Cells["colFechaSalida"].Value =
                    reserva.FechaCheckOut.ToString(
                        "dd-MMM-yy", new CultureInfo("es-DO"));
            }
        }

        // Resuelve "Nombre Apellido" a partir del documento del huésped,
        // con respaldo si el huésped no existe o la consulta falla — mismo
        // patrón defensivo que CargarReservaSeleccionada en
        // FrmReservas.cs.
        private string ObtenerNombreCompletoHuesped(string numeroDocumento)
        {
            if (string.IsNullOrEmpty(numeroDocumento))
            {
                return "—";
            }

            try
            {
                Huesped huesped =
                    _huespedService.BuscarPorDocumento(numeroDocumento);
                return huesped != null
                    ? huesped.Nombre + " " + huesped.Apellido
                    : numeroDocumento;
            }
            catch (SqlException)
            {
                return numeroDocumento;
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
                // El check-in es la confirmacion definitiva de la reserva
                // (ver EstadiaService.RegistrarCheckIn): una reserva
                // "Pendiente" o ya "Confirmada" puede llegar hasta aqui,
                // sin necesitar un paso manual aparte en Reservaciones.
                // Solo se descarta una reserva "Cancelada".
                if (reserva.Estado == "Cancelada")
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
                // El check-in confirma la reserva (ver
                // EstadiaService.RegistrarCheckIn): solo se bloquea una
                // reserva ya "Cancelada".
                if (reserva.Estado != "Cancelada" &&
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
                    lblEstadoHabitacionActual.BackColor =
                        TemaVisual.Colores.Blanco;
                    lblEstadoHabitacionActual.ForeColor =
                        TemaVisual.Colores.TextoSuave;
                    return;
                }

                lblEstadoHabitacionActual.Text =
                    "Habitacion " + habitacion.Numero +
                    " - Estado: " + habitacion.Estado;

                // El distintivo toma el color semántico del estado real.
                lblEstadoHabitacionActual.BackColor =
                    TemaVisual.Colores.PorEstadoSuave(habitacion.Estado);
                lblEstadoHabitacionActual.ForeColor =
                    TemaVisual.Colores.PorEstado(habitacion.Estado);
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
