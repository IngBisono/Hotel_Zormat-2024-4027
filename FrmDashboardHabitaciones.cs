using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmDashboardHabitaciones : Form
    {
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;
        private bool _cargandoFiltros;

        public FrmDashboardHabitaciones()
        {
            InitializeComponent();

            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        public FrmDashboardHabitaciones(Usuario usuarioActual)
            : this()
        {
            _usuarioActual = usuarioActual;
        }

        private void ConfigurarFormulario()
        {
            Text = "Hotel Zormat - Estado de habitaciones";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F5F8FB");

            pnlTablero.BackColor = ColorTranslator.FromHtml("#F5F8FB");
            pnlTablero.AutoScroll = true;
            pnlTablero.ContextMenuStrip = menuPrincipal;
            ContextMenuStrip = menuPrincipal;

            cboFiltroPiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            cboFiltroEstado.Items.Clear();
            cboFiltroEstado.Items.Add("Todos");
            cboFiltroEstado.Items.Add("Disponible");
            cboFiltroEstado.Items.Add("Ocupada");
            cboFiltroEstado.Items.Add("Reservada");
            cboFiltroEstado.Items.Add("Limpieza");
            cboFiltroEstado.SelectedIndex = 0;

            btnActualizar.Text = "Actualizar";
            btnActualizar.BackColor = ColorTranslator.FromHtml("#025897");
            btnActualizar.ForeColor = Color.White;
            btnActualizar.FlatStyle = FlatStyle.Flat;
            btnActualizar.UseVisualStyleBackColor = false;

            lblLeyenda.Text =
                "Disponible: verde | Ocupada: rojo | " +
                "Reservada: naranja | Limpieza: azul" +
                Environment.NewLine +
                "Haga clic derecho para abrir el menú principal.";
            lblLeyenda.ForeColor = ColorTranslator.FromHtml("#2E4256");

            Load += FrmDashboardHabitaciones_Load;
            btnActualizar.Click += btnActualizar_Click;
            cboFiltroPiso.SelectedIndexChanged += filtro_SelectedIndexChanged;
            cboFiltroEstado.SelectedIndexChanged += filtro_SelectedIndexChanged;
        }

        private void FrmDashboardHabitaciones_Load(
            object sender,
            EventArgs e)
        {
            ConfigurarMenu();
            ActualizarDashboard();
        }

        private void ConfigurarMenu()
        {
            menuPrincipal.Items.Clear();

            string usuario = "Usuario";
            if (_usuarioActual != null)
            {
                usuario = _usuarioActual.NombreCompleto +
                    " - " + _usuarioActual.Rol;
            }

            ToolStripMenuItem encabezado = new ToolStripMenuItem(usuario);
            encabezado.Enabled = false;
            menuPrincipal.Items.Add(encabezado);
            menuPrincipal.Items.Add(new ToolStripSeparator());

            menuPrincipal.Items.Add(
                "Habitaciones",
                null,
                AbrirHabitaciones);
            menuPrincipal.Items.Add(
                "Huéspedes",
                null,
                AbrirHuespedes);
            menuPrincipal.Items.Add(
                "Reservas",
                null,
                AbrirReservas);
            menuPrincipal.Items.Add(
                "Check-in / Check-out",
                null,
                AbrirCheckInOut);
            menuPrincipal.Items.Add(
                "Reportes",
                null,
                AbrirReportes);

            if (AutorizacionService.EsAdministrador(_usuarioActual))
            {
                menuPrincipal.Items.Add(
                    "Bitácora",
                    null,
                    AbrirBitacora);
            }

            menuPrincipal.Items.Add(new ToolStripSeparator());
            menuPrincipal.Items.Add(
                "Cerrar sesión",
                null,
                CerrarSesion);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ActualizarDashboard();
        }

        private void filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoFiltros)
            {
                return;
            }

            MostrarHabitacionesFiltradas();
        }

        private void ActualizarDashboard()
        {
            try
            {
                List<Habitacion> habitaciones =
                    _habitacionService.ObtenerTodas();

                CargarPisos(habitaciones);
                MostrarHabitacionesFiltradas();
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "No se pudo cargar el estado de las habitaciones. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarMensaje(
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

        private void MostrarHabitacionesFiltradas()
        {
            try
            {
                int? piso = ObtenerPisoSeleccionado();
                string estado = ObtenerEstadoSeleccionado();
                List<Habitacion> habitaciones =
                    _habitacionService.Filtrar(piso, estado);

                QuitarTarjetasAnteriores();

                if (habitaciones.Count == 0)
                {
                    Label mensaje = new Label();
                    mensaje.AutoSize = true;
                    mensaje.Text =
                        "No hay habitaciones para los filtros seleccionados.";
                    mensaje.ForeColor = ColorTranslator.FromHtml("#63798E");
                    mensaje.Tag = "TarjetaHabitacion";
                    pnlTablero.Controls.Add(mensaje);
                    return;
                }

                foreach (Habitacion habitacion in habitaciones)
                {
                    Button tarjeta = CrearTarjeta(habitacion);
                    pnlTablero.Controls.Add(tarjeta);
                }
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "No se pudo consultar las habitaciones. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "Ocurrió un error al aplicar los filtros.");
            }
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

        private void QuitarTarjetasAnteriores()
        {
            for (int indice = pnlTablero.Controls.Count - 1;
                indice >= 0;
                indice--)
            {
                Control control = pnlTablero.Controls[indice];

                if (control.Tag != null)
                {
                    if (control.Tag.ToString() == "TarjetaHabitacion")
                    {
                        pnlTablero.Controls.RemoveAt(indice);
                        control.Dispose();
                    }
                }
            }
        }

        private Button CrearTarjeta(Habitacion habitacion)
        {
            Button tarjeta = new Button();
            tarjeta.AutoSize = true;
            tarjeta.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tarjeta.Padding = new Padding(12);
            tarjeta.FlatStyle = FlatStyle.Flat;
            tarjeta.UseVisualStyleBackColor = false;
            tarjeta.ForeColor = Color.White;
            tarjeta.BackColor = ObtenerColorEstado(habitacion.Estado);
            tarjeta.Tag = "TarjetaHabitacion";
            tarjeta.Text =
                "Habitación " + habitacion.Numero +
                Environment.NewLine +
                "Piso: " + habitacion.Piso +
                " | Tipo: " + habitacion.Tipo +
                " | Estado: " + habitacion.Estado +
                " | Tarifa: " + habitacion.TarifaBase.ToString("C2");

            return tarjeta;
        }

        private Color ObtenerColorEstado(string estado)
        {
            if (estado == "Disponible")
            {
                return ColorTranslator.FromHtml("#2E9E52");
            }

            if (estado == "Ocupada")
            {
                return ColorTranslator.FromHtml("#D6483F");
            }

            if (estado == "Reservada")
            {
                return ColorTranslator.FromHtml("#E08A2E");
            }

            return ColorTranslator.FromHtml("#3D6FD8");
        }

        private void AbrirHabitaciones(object sender, EventArgs e)
        {
            using (FrmHabitaciones formulario =
                new FrmHabitaciones(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }

            ActualizarDashboard();
        }

        private void AbrirHuespedes(object sender, EventArgs e)
        {
            using (FrmHuespedes formulario =
                new FrmHuespedes(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }
        }

        private void AbrirReservas(object sender, EventArgs e)
        {
            using (FrmReservas formulario =
                new FrmReservas(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }

            ActualizarDashboard();
        }

        private void AbrirCheckInOut(object sender, EventArgs e)
        {
            using (FrmCheckInOut formulario =
                new FrmCheckInOut(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }

            ActualizarDashboard();
        }

        private void AbrirReportes(object sender, EventArgs e)
        {
            using (FrmReportes formulario =
                new FrmReportes(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }
        }

        private void AbrirBitacora(object sender, EventArgs e)
        {
            using (FrmBitacora formulario =
                new FrmBitacora(_usuarioActual))
            {
                formulario.ShowDialog(this);
            }
        }

        private void CerrarSesion(object sender, EventArgs e)
        {
            Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Zormat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
