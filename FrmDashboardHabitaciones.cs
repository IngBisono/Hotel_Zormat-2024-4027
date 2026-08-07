using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmDashboardHabitaciones : Form
    {
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;
        private bool _cargandoFiltros;

        // Barra lateral de navegación. No procede del diseñador: se construye
        // por código para no modificar FrmDashboardHabitaciones.Designer.cs.
        private Panel pnlSidebar;
        private FlowLayoutPanel flpNavegacion;
        private Button btnNavHabitaciones;
        private Button btnNavHuespedes;
        private Button btnNavReservas;
        private Button btnNavCheckInOut;
        private Button btnNavReportes;
        private Button btnNavBitacora;
        private Button btnNavCerrarSesion;

        public bool CerrarSesionSolicitado { get; private set; }

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
            TemaVisual.PrepararFormulario(
                this,
                "Hotel Bisono - Estado de habitaciones");

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 720);
            MinimumSize = new Size(1040, 640);

            ConfigurarDistribucion();

            pnlTablero.BackColor = TemaVisual.Colores.FondoClaro;
            pnlTablero.AutoScroll = false;
            pnlTablero.WrapContents = true;
            pnlTablero.ContextMenuStrip = menuPrincipal;
            pnlContenedorTablero.ContextMenuStrip = menuPrincipal;
            ContextMenuStrip = menuPrincipal;

            cboFiltroPiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            TemaVisual.EstilizarCombo(cboFiltroPiso);
            TemaVisual.EstilizarCombo(cboFiltroEstado);

            cboFiltroEstado.Items.Clear();
            cboFiltroEstado.Items.Add("Todos");
            cboFiltroEstado.Items.Add("Disponible");
            cboFiltroEstado.Items.Add("Ocupada");
            cboFiltroEstado.Items.Add("Reservada");
            cboFiltroEstado.Items.Add("Limpieza");
            cboFiltroEstado.SelectedIndex = 0;

            btnActualizar.Text = "Actualizar";
            btnActualizar.Size = new Size(132, 34);
            TemaVisual.EstilizarBoton(
                btnActualizar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnActualizar,
                TemaVisual.Glifos.Actualizar,
                "Actualizar");

            lblLeyenda.Text =
                "Disponible: verde   ·   Ocupada: rojo   ·   " +
                "Reservada: naranja   ·   Limpieza: azul" +
                Environment.NewLine +
                "Use el menú lateral o haga clic derecho sobre el tablero.";
            lblLeyenda.Font = TemaVisual.Fuentes.Pequena;
            lblLeyenda.ForeColor = TemaVisual.Colores.TextoSuave;

            Load += FrmDashboardHabitaciones_Load;
            btnActualizar.Click += btnActualizar_Click;
            cboFiltroPiso.SelectedIndexChanged += filtro_SelectedIndexChanged;
            cboFiltroEstado.SelectedIndexChanged += filtro_SelectedIndexChanged;
        }

        // Barra lateral de navegación.
        //
        // Se construye durante el Load y no en el constructor porque necesita
        // el usuario autenticado, que el constructor encadenado asigna después
        // de ejecutar ConfigurarFormulario.
        private void ConstruirBarraLateral()
        {
            if (pnlSidebar != null)
            {
                return;
            }

            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 238;
            pnlSidebar.BackColor = TemaVisual.Colores.AzulProfundo;

            // El relleno reserva los últimos píxeles del borde derecho para la
            // línea de acento, que los controles internos no llegan a tapar.
            pnlSidebar.Padding = new Padding(0, 0, 3, 0);
            pnlSidebar.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                Rectangle acento = new Rectangle(
                    lienzo.Width - 3,
                    0,
                    3,
                    lienzo.Height);
                TemaVisual.PintarAtardecer(e.Graphics, acento);
            };

            Panel marca = ConstruirMarcaLateral();

            flpNavegacion = new FlowLayoutPanel();
            flpNavegacion.Dock = DockStyle.Fill;
            flpNavegacion.FlowDirection = FlowDirection.TopDown;
            flpNavegacion.WrapContents = false;
            flpNavegacion.BackColor = TemaVisual.Colores.AzulProfundo;
            flpNavegacion.Padding = new Padding(10, 8, 0, 0);

            btnNavHabitaciones = TemaVisual.CrearBotonNavegacion(
                "Habitaciones",
                TemaVisual.Glifos.Habitacion);
            btnNavHuespedes = TemaVisual.CrearBotonNavegacion(
                "Huéspedes",
                TemaVisual.Glifos.Huesped);
            btnNavReservas = TemaVisual.CrearBotonNavegacion(
                "Reservas",
                TemaVisual.Glifos.Reserva);
            btnNavCheckInOut = TemaVisual.CrearBotonNavegacion(
                "Check-in / Check-out",
                TemaVisual.Glifos.CheckIn);
            btnNavReportes = TemaVisual.CrearBotonNavegacion(
                "Reportes",
                TemaVisual.Glifos.Reportes);

            flpNavegacion.Controls.Add(btnNavHabitaciones);
            flpNavegacion.Controls.Add(btnNavHuespedes);
            flpNavegacion.Controls.Add(btnNavReservas);
            flpNavegacion.Controls.Add(btnNavCheckInOut);
            flpNavegacion.Controls.Add(btnNavReportes);

            // La bitácora sólo existe para administradores, igual que en el
            // menú contextual.
            if (AutorizacionService.EsAdministrador(_usuarioActual))
            {
                btnNavBitacora = TemaVisual.CrearBotonNavegacion(
                    "Bitácora",
                    TemaVisual.Glifos.Bitacora);
                flpNavegacion.Controls.Add(btnNavBitacora);
                btnNavBitacora.Click += BtnNavBitacora_Click;
            }

            btnNavCerrarSesion = TemaVisual.CrearBotonNavegacion(
                "Cerrar sesión",
                TemaVisual.Glifos.CerrarSesion);
            btnNavCerrarSesion.Dock = DockStyle.Bottom;
            btnNavCerrarSesion.Height = 48;
            btnNavCerrarSesion.Padding = new Padding(26, 0, 0, 0);
            btnNavCerrarSesion.ForeColor = TemaVisual.Colores.SolDurazno;
            btnNavCerrarSesion.FlatAppearance.MouseOverBackColor =
                TemaVisual.Colores.Rojo;

            btnNavHabitaciones.Click += BtnNavHabitaciones_Click;
            btnNavHuespedes.Click += BtnNavHuespedes_Click;
            btnNavReservas.Click += BtnNavReservas_Click;
            btnNavCheckInOut.Click += BtnNavCheckInOut_Click;
            btnNavReportes.Click += BtnNavReportes_Click;
            btnNavCerrarSesion.Click += BtnNavCerrarSesion_Click;

            pnlSidebar.Controls.Add(flpNavegacion);
            pnlSidebar.Controls.Add(btnNavCerrarSesion);
            pnlSidebar.Controls.Add(marca);

            // El acoplamiento se resuelve del último control al primero, así
            // que el panel de relleno debe quedar al frente para repartirse el
            // espacio que dejan libre el encabezado y el pie.
            flpNavegacion.BringToFront();

            Controls.Add(pnlSidebar);
        }

        // Encabezado de la barra lateral: emblema, nombre del hotel y usuario.
        private Panel ConstruirMarcaLateral()
        {
            Panel marca = new Panel();
            marca.Dock = DockStyle.Top;
            marca.Height = 208;
            marca.BackColor = TemaVisual.Colores.AzulProfundo;

            PictureBox emblema = new PictureBox();
            emblema.Image = TemaVisual.ObtenerEmblema();
            emblema.SizeMode = PictureBoxSizeMode.Zoom;
            emblema.BackColor = Color.Transparent;

            Label nombre = new Label();
            nombre.Text = "HOTEL BISONO";
            nombre.Font = TemaVisual.Fuentes.TituloSeccion;
            nombre.ForeColor = TemaVisual.Colores.DoradoTexto;
            nombre.BackColor = Color.Transparent;
            nombre.AutoSize = false;
            nombre.TextAlign = ContentAlignment.MiddleCenter;

            string nombreUsuario = "Usuario";
            string rolUsuario = "Sin sesión";
            if (_usuarioActual != null)
            {
                nombreUsuario = _usuarioActual.NombreCompleto;
                rolUsuario = _usuarioActual.Rol;
            }

            Label etiquetaUsuario = new Label();
            etiquetaUsuario.Text = nombreUsuario;
            etiquetaUsuario.Font = TemaVisual.Fuentes.CuerpoNegrita;
            etiquetaUsuario.ForeColor = TemaVisual.Colores.Blanco;
            etiquetaUsuario.BackColor = Color.Transparent;
            etiquetaUsuario.AutoSize = false;
            etiquetaUsuario.TextAlign = ContentAlignment.MiddleCenter;

            Label etiquetaRol = new Label();
            etiquetaRol.Text = rolUsuario;
            etiquetaRol.Font = TemaVisual.Fuentes.Pequena;
            etiquetaRol.ForeColor = TemaVisual.Colores.TurquesaClaro;
            etiquetaRol.BackColor = Color.Transparent;
            etiquetaRol.AutoSize = false;
            etiquetaRol.TextAlign = ContentAlignment.MiddleCenter;

            marca.Controls.Add(emblema);
            marca.Controls.Add(nombre);
            marca.Controls.Add(etiquetaUsuario);
            marca.Controls.Add(etiquetaRol);

            emblema.Size = new Size(186, 84);
            emblema.Location = new Point(22, 16);
            nombre.Size = new Size(230, 26);
            nombre.Location = new Point(0, 104);
            etiquetaUsuario.Size = new Size(230, 22);
            etiquetaUsuario.Location = new Point(0, 148);
            etiquetaRol.Size = new Size(230, 18);
            etiquetaRol.Location = new Point(0, 170);

            // Línea divisoria tenue entre la identidad y los datos de sesión.
            marca.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                using (Pen lapiz = new Pen(TemaVisual.Colores.AzulPrimario))
                {
                    e.Graphics.DrawLine(lapiz, 26, 138, lienzo.Width - 30, 138);
                    e.Graphics.DrawLine(
                        lapiz,
                        26,
                        lienzo.Height - 6,
                        lienzo.Width - 30,
                        lienzo.Height - 6);
                }
            };

            return marca;
        }

        // Coloca el tablero dentro del panel con desplazamiento.
        private void ConfigurarDistribucion()
        {
            pnlTablero.Controls.Remove(pnlContenedorTablero);

            Label etiquetaPiso = new Label();
            etiquetaPiso.Text = "Piso:";
            etiquetaPiso.AutoSize = true;
            etiquetaPiso.Margin = new Padding(0, 8, 4, 0);

            Label etiquetaEstado = new Label();
            etiquetaEstado.Text = "Estado:";
            etiquetaEstado.AutoSize = true;
            etiquetaEstado.Margin = new Padding(14, 8, 4, 0);

            etiquetaPiso.Font = TemaVisual.Fuentes.Etiqueta;
            etiquetaPiso.ForeColor = TemaVisual.Colores.Texto;
            etiquetaEstado.Font = TemaVisual.Fuentes.Etiqueta;
            etiquetaEstado.ForeColor = TemaVisual.Colores.Texto;

            FlowLayoutPanel barraFiltros = new FlowLayoutPanel();
            barraFiltros.AutoSize = true;
            barraFiltros.Dock = DockStyle.Fill;
            barraFiltros.FlowDirection = FlowDirection.LeftToRight;
            barraFiltros.WrapContents = true;
            barraFiltros.Padding = new Padding(20, 12, 20, 12);
            barraFiltros.BackColor = TemaVisual.Colores.FondoSecundario;
            barraFiltros.Controls.Add(etiquetaPiso);
            barraFiltros.Controls.Add(cboFiltroPiso);
            barraFiltros.Controls.Add(etiquetaEstado);
            barraFiltros.Controls.Add(cboFiltroEstado);
            barraFiltros.Controls.Add(btnActualizar);
            barraFiltros.Controls.Add(lblLeyenda);

            cboFiltroPiso.Width = 120;
            cboFiltroEstado.Width = 140;
            cboFiltroPiso.Margin = new Padding(0, 4, 0, 0);
            cboFiltroEstado.Margin = new Padding(0, 4, 0, 0);
            btnActualizar.Margin = new Padding(20, 0, 24, 0);
            lblLeyenda.Margin = new Padding(0, 2, 0, 0);

            Panel encabezado = TemaVisual.CrearEncabezado(
                "Estado de Habitaciones",
                TemaVisual.Glifos.Habitacion);
            encabezado.Dock = DockStyle.Fill;

            TableLayoutPanel contenido = new TableLayoutPanel();
            contenido.ColumnCount = 1;
            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            contenido.RowCount = 3;
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            contenido.Dock = DockStyle.Fill;
            contenido.Controls.Add(encabezado, 0, 0);
            contenido.Controls.Add(barraFiltros, 0, 1);
            contenido.Controls.Add(pnlContenedorTablero, 0, 2);

            Controls.Add(contenido);
            contenido.BringToFront();

            pnlContenedorTablero.Dock = DockStyle.Fill;
            pnlContenedorTablero.AutoScroll = true;
            pnlContenedorTablero.BackColor = TemaVisual.Colores.FondoClaro;

            pnlContenedorTablero.Controls.Add(pnlTablero);
            pnlTablero.Dock = DockStyle.Top;
            pnlTablero.AutoSize = true;
            pnlTablero.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTablero.Padding = new Padding(14);
        }

        // ------------------------------------------------------------------
        // Eventos de la barra lateral
        //
        // Quedan deliberadamente vacíos: la navegación real sigue viviendo en
        // el menú contextual, que no se ha tocado. Cada nota indica el método
        // ya existente cuya lógica corresponde replicar aquí.
        // ------------------------------------------------------------------

        private void BtnNavHabitaciones_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirHabitaciones(sender, e) en este
            // mismo archivo: abre FrmHabitaciones con ShowDialog(this) y llama
            // a ActualizarDashboard() al cerrarse.
        }

        private void BtnNavHuespedes_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirHuespedes(sender, e) en este
            // mismo archivo: abre FrmHuespedes con ShowDialog(this).
        }

        private void BtnNavReservas_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirReservas(sender, e) en este
            // mismo archivo: abre FrmReservas con ShowDialog(this) y llama a
            // ActualizarDashboard() al cerrarse.
        }

        private void BtnNavCheckInOut_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirCheckInOut(sender, e) en este
            // mismo archivo: abre FrmCheckInOut con ShowDialog(this) y llama a
            // ActualizarDashboard() al cerrarse.
        }

        private void BtnNavReportes_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirReportes(sender, e) en este
            // mismo archivo: abre FrmReportes con ShowDialog(this).
        }

        private void BtnNavBitacora_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de AbrirBitacora(sender, e) en este
            // mismo archivo: abre FrmBitacora con ShowDialog(this). El botón
            // sólo se crea si el usuario es administrador.
        }

        private void BtnNavCerrarSesion_Click(object sender, EventArgs e)
        {
            // TODO: replicar la lógica de CerrarSesion(sender, e) en este
            // mismo archivo: marca CerrarSesionSolicitado y cierra el
            // formulario para que Program.cs vuelva al inicio de sesión.
        }

        private void FrmDashboardHabitaciones_Load(
            object sender,
            EventArgs e)
        {
            ConstruirBarraLateral();
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
            try
            {
                btnActualizar.Enabled = false;
                ActualizarDashboard();
            }
            finally
            {
                btnActualizar.Enabled = true;
            }
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
                    mensaje.Font = TemaVisual.Fuentes.Cuerpo;
                    mensaje.ForeColor = TemaVisual.Colores.TextoSuave;
                    mensaje.Margin = new Padding(8, 12, 8, 8);
                    mensaje.Tag = "TarjetaHabitacion";
                    pnlTablero.Controls.Add(mensaje);
                    return;
                }

                foreach (Habitacion habitacion in habitaciones)
                {
                    Panel tarjeta = CrearTarjeta(habitacion);
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

        // Tarjeta de habitación.
        //
        // Se abandona el bloque de color macizo por una ficha blanca con una
        // banda lateral del color del estado: se conserva el mismo código de
        // colores que ya conocía el personal, pero el texto gana legibilidad.
        private Panel CrearTarjeta(Habitacion habitacion)
        {
            Color acento = ObtenerColorEstado(habitacion.Estado);

            Panel tarjeta = new Panel();
            tarjeta.Size = new Size(242, 122);
            tarjeta.Margin = new Padding(9);
            tarjeta.Tag = "TarjetaHabitacion";
            tarjeta.ContextMenuStrip = menuPrincipal;

            TemaVisual.AplicarTarjeta(
                tarjeta,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.BordeSutil,
                TemaVisual.Colores.FondoClaro,
                12);

            // Banda de color del estado, recortada a la curva de la tarjeta.
            tarjeta.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                if (lienzo.Width <= 1 || lienzo.Height <= 1)
                {
                    return;
                }

                Rectangle area = new Rectangle(0, 0, lienzo.Width - 1, lienzo.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath ruta = TemaVisual.CrearRutaRedondeada(area, 12))
                using (SolidBrush brocha = new SolidBrush(acento))
                {
                    e.Graphics.SetClip(ruta);
                    e.Graphics.FillRectangle(brocha, 0, 0, 7, lienzo.Height);
                    e.Graphics.ResetClip();
                }
            };

            Label numero = new Label();
            numero.Text = "Habitación " + habitacion.Numero;
            numero.Font = TemaVisual.Fuentes.TituloSeccion;
            numero.ForeColor = TemaVisual.Colores.AzulMarino;
            numero.BackColor = TemaVisual.Colores.Blanco;
            numero.AutoSize = false;
            numero.TextAlign = ContentAlignment.MiddleLeft;

            Label estado = new Label();
            estado.Text = "• " + habitacion.Estado;
            estado.Font = TemaVisual.Fuentes.CuerpoNegrita;
            estado.ForeColor = acento;
            estado.BackColor = TemaVisual.Colores.Blanco;
            estado.AutoSize = false;
            estado.TextAlign = ContentAlignment.MiddleLeft;

            Label detalle = new Label();
            detalle.Text =
                habitacion.Tipo + "  ·  Piso " + habitacion.Piso +
                "  ·  " + habitacion.Capacidad + " pers.";
            detalle.Font = TemaVisual.Fuentes.Pequena;
            detalle.ForeColor = TemaVisual.Colores.TextoSuave;
            detalle.BackColor = TemaVisual.Colores.Blanco;
            detalle.AutoSize = false;
            detalle.TextAlign = ContentAlignment.MiddleLeft;

            Label tarifa = new Label();
            tarifa.Text = habitacion.TarifaBase.ToString("C2");
            tarifa.Font = TemaVisual.Fuentes.Monto;
            tarifa.ForeColor = TemaVisual.Colores.TurquesaProfundo;
            tarifa.BackColor = TemaVisual.Colores.Blanco;
            tarifa.AutoSize = false;
            tarifa.TextAlign = ContentAlignment.MiddleLeft;

            tarjeta.Controls.Add(numero);
            tarjeta.Controls.Add(estado);
            tarjeta.Controls.Add(detalle);
            tarjeta.Controls.Add(tarifa);

            numero.SetBounds(22, 12, 208, 26);
            estado.SetBounds(22, 40, 208, 20);
            detalle.SetBounds(22, 62, 208, 18);
            tarifa.SetBounds(22, 86, 208, 24);

            // El menú contextual debe seguir apareciendo aunque el clic caiga
            // sobre una etiqueta interna y no sobre el fondo de la tarjeta.
            foreach (Control hijo in tarjeta.Controls)
            {
                hijo.ContextMenuStrip = menuPrincipal;
            }

            return tarjeta;
        }

        private Color ObtenerColorEstado(string estado)
        {
            return TemaVisual.Colores.PorEstado(estado);
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
            CerrarSesionSolicitado = true;
            Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Bisono",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
