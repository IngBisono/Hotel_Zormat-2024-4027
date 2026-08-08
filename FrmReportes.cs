// Cedula: 402-3047435-1
using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Excepciones;
using HotelZormat.Negocio.Reportes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmReportes : Form
    {
        private readonly ReporteService reporteService;

        // Se usa sólo para el denominador del indicador de ocupación: el
        // reporte devuelve las habitaciones ocupadas, no el total del hotel.
        private readonly HabitacionService habitacionService;

        // Servicios para los reportes nuevos; reutilizan métodos ya existentes.
        private readonly ReservaService reservaService;
        private readonly EstadiaService estadiaService;
        private readonly HuespedService huespedService;
        private readonly FacturaService facturaService;
        private readonly BitacoraService bitacoraService;
        private readonly UsuarioService usuarioService;

        private Usuario usuarioActual;

        // Indicador de ocupación de la pestaña del día. Creado por código
        // para no modificar FrmReportes.Designer.cs.
        private Label _lblOcupacion;
        private Label _lblDetalleOcupacion;

        public FrmReportes()
        {
            InitializeComponent();
            reporteService = new ReporteService();
            habitacionService = new HabitacionService();
            reservaService = new ReservaService();
            estadiaService = new EstadiaService();
            huespedService = new HuespedService();
            facturaService = new FacturaService();
            bitacoraService = new BitacoraService();
            usuarioService = new UsuarioService();
            ConfigurarPantalla();
        }

        public FrmReportes(Usuario usuario) : this()
        {
            usuarioActual = usuario;
        }

        private void ConfigurarPantalla()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Reportes");
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 660);
            MinimumSize = new Size(860, 580);

            tabPage1.Text = "Ocupación del día";
            tabPage2.Text = "Ingresos";

            dtpFechaInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFechaFin.Value = DateTime.Today;
            TemaVisual.EstilizarFecha(dtpFechaInicio);
            TemaVisual.EstilizarFecha(dtpFechaFin);

            TemaVisual.EstilizarBoton(
                btnGenerarReporte,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnGenerarReporte,
                TemaVisual.Glifos.Reportes,
                "Generar reporte");

            TemaVisual.EstilizarGrid(dgvOcupacionDelDia);
            TemaVisual.EstilizarGrid(dgvIngresos);

            lblTotalIngresos.Text = "Total de ingresos: RD$ 0.00";
            lblTotalIngresos.Font = TemaVisual.Fuentes.MontoTotal;
            lblTotalIngresos.ForeColor = TemaVisual.Colores.AzulProfundo;
            lblTotalIngresos.BackColor = TemaVisual.Colores.SolAmarillo;
            lblTotalIngresos.AutoSize = false;
            lblTotalIngresos.Size = new Size(340, 44);
            lblTotalIngresos.TextAlign = ContentAlignment.MiddleCenter;
            TemaVisual.AplicarEsquinasRedondeadas(lblTotalIngresos, 10);

            ConfigurarDistribucion();

            Load += FrmReportes_Load;
            btnGenerarReporte.Click += btnGenerarReporte_Click;
        }

        private void ConfigurarDistribucion()
        {
            Label lblFechaInicio = new Label();
            lblFechaInicio.AutoSize = true;
            lblFechaInicio.Text = "Desde:";
            lblFechaInicio.Font = TemaVisual.Fuentes.Etiqueta;
            lblFechaInicio.ForeColor = TemaVisual.Colores.Texto;
            lblFechaInicio.Margin = new Padding(0, 10, 6, 0);

            Label lblFechaFin = new Label();
            lblFechaFin.AutoSize = true;
            lblFechaFin.Text = "Hasta:";
            lblFechaFin.Font = TemaVisual.Fuentes.Etiqueta;
            lblFechaFin.ForeColor = TemaVisual.Colores.Texto;
            lblFechaFin.Margin = new Padding(16, 10, 6, 0);

            FlowLayoutPanel barraFiltros = new FlowLayoutPanel();
            barraFiltros.AutoSize = true;
            barraFiltros.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            barraFiltros.Dock = DockStyle.Fill;
            barraFiltros.FlowDirection = FlowDirection.LeftToRight;
            barraFiltros.BackColor = TemaVisual.Colores.FondoSecundario;
            barraFiltros.Padding = new Padding(14, 10, 14, 10);
            barraFiltros.WrapContents = true;

            dtpFechaInicio.Width = 180;
            dtpFechaFin.Width = 180;
            dtpFechaInicio.Margin = new Padding(0, 6, 0, 0);
            dtpFechaFin.Margin = new Padding(0, 6, 0, 0);
            btnGenerarReporte.AutoSize = false;
            btnGenerarReporte.Size = new Size(184, 36);
            btnGenerarReporte.Margin = new Padding(20, 2, 0, 2);

            barraFiltros.Controls.Add(lblFechaInicio);
            barraFiltros.Controls.Add(dtpFechaInicio);
            barraFiltros.Controls.Add(lblFechaFin);
            barraFiltros.Controls.Add(dtpFechaFin);
            barraFiltros.Controls.Add(btnGenerarReporte);

            TableLayoutPanel contenidoIngresos = new TableLayoutPanel();
            contenidoIngresos.ColumnCount = 1;
            contenidoIngresos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            contenidoIngresos.Dock = DockStyle.Fill;
            contenidoIngresos.BackColor = TemaVisual.Colores.Blanco;
            contenidoIngresos.Padding = new Padding(14, 12, 14, 12);
            contenidoIngresos.RowCount = 3;
            contenidoIngresos.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));
            contenidoIngresos.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            contenidoIngresos.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            dgvIngresos.Dock = DockStyle.Fill;
            dgvIngresos.Margin = new Padding(0, 8, 0, 8);
            lblTotalIngresos.Anchor = AnchorStyles.Right;
            lblTotalIngresos.Margin = new Padding(0, 4, 0, 0);

            contenidoIngresos.Controls.Add(barraFiltros, 0, 0);
            contenidoIngresos.Controls.Add(dgvIngresos, 0, 1);
            contenidoIngresos.Controls.Add(lblTotalIngresos, 0, 2);

            tabPage1.Padding = new Padding(14);
            tabPage2.Padding = new Padding(0);
            tabPage1.BackColor = TemaVisual.Colores.Blanco;
            tabPage2.BackColor = TemaVisual.Colores.Blanco;
            tabPage2.Controls.Add(contenidoIngresos);

            ConstruirIndicadorOcupacion();

            flpReportes.Visible = false;
            Controls.Add(tabControl);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Padding = new Point(0, 0);

            // TemaVisual.EstilizarTabControl se llama después, desde ConfigurarPestanasPorRol en Load.

            // El encabezado se acopla arriba; las pestañas quedan al frente para llenar el resto.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Reportes",
                TemaVisual.Glifos.Reportes);
            Controls.Add(encabezado);
            tabControl.BringToFront();
        }

        // ------------------------------------------------------------------
        // Reportes nuevos: infraestructura compartida
        // ------------------------------------------------------------------

        // Fila genérica para reportes de "conteo agrupado por etiqueta"
        // (distribución por piso, huéspedes por tipo de documento).
        private class FilaConteo
        {
            public string Etiqueta { get; set; }
            public int Cantidad { get; set; }
        }

        // Fila genérica para reportes de "conteo + monto agrupado"
        // (reservas por estado/temporada, ingresos por habitación/temporada).
        private class FilaConteoMonto
        {
            public string Etiqueta { get; set; }
            public int Cantidad { get; set; }
            public decimal Monto { get; set; }
        }

        // Fila genérica para reportes de ocupación/porcentaje por categoría.
        private class FilaConteoPorcentaje
        {
            public string Etiqueta { get; set; }
            public int Ocupadas { get; set; }
            public int Total { get; set; }
            public decimal Porcentaje { get; set; }
        }

        // Fila para "reservas vencidas sin check-in" y "próximas salidas".
        private class FilaReservaResumen
        {
            public string Documento { get; set; }
            public string Nombre { get; set; }
            public int Habitacion { get; set; }
            public DateTime Fecha { get; set; }
        }

        // Fila para "huéspedes frecuentes".
        private class FilaHuespedFrecuente
        {
            public string Documento { get; set; }
            public string Nombre { get; set; }
            public int CantidadReservas { get; set; }
        }

        // Fila para "actividad reciente".
        private class FilaActividad
        {
            public DateTime FechaHora { get; set; }
            public string Usuario { get; set; }
            public string Accion { get; set; }
            public string Detalle { get; set; }
        }

        // Arma las pestañas según el rol del usuario y carga sus datos iniciales.
        private void ConfigurarPestanasPorRol()
        {
            bool esAdministrador = AutorizacionService.EsAdministrador(usuarioActual);

            tabControl.TabPages.Add(ConstruirTabHabitaciones());
            tabControl.TabPages.Add(ConstruirTabCheckInSalidas());
            tabControl.TabPages.Add(ConstruirTabReservasHuespedes());

            if (esAdministrador)
            {
                tabControl.TabPages.Add(ConstruirTabFinanciero());
                tabControl.TabPages.Add(ConstruirTabEstrategia());
                tabControl.TabPages.Add(ConstruirTabActividad());
            }
            else
            {
                // Recepcionista no debe ni ver que "Ingresos" existe.
                tabControl.TabPages.Remove(tabPage2);
            }

            // Se llama una sola vez, ya con todas las pestañas agregadas.
            TemaVisual.EstilizarTabControl(tabControl);

            CargarDatosHabitaciones();
            CargarDatosCheckInSalidas();
            CargarDatosReservasHuespedes();

            if (esAdministrador)
            {
                CargarDatosEstrategia();
                CargarDatosActividad();
            }
        }

        // Contenedor estándar de una pestaña de reportes: apila secciones
        // verticalmente y permite scroll si no caben todas.
        private static FlowLayoutPanel CrearContenedorPestana()
        {
            FlowLayoutPanel contenedor = new FlowLayoutPanel();
            contenedor.Dock = DockStyle.Fill;
            contenedor.FlowDirection = FlowDirection.TopDown;
            contenedor.WrapContents = false;
            contenedor.AutoScroll = true;
            contenedor.BackColor = TemaVisual.Colores.Blanco;
            contenedor.Padding = new Padding(14, 12, 14, 12);
            return contenedor;
        }

        // Une un título de sección con una grilla, en un panel de ancho fijo.
        private static Panel CrearSeccionGrid(
            string titulo,
            string glifo,
            DataGridView grid,
            int altoGrid)
        {
            Panel seccion = new Panel();
            seccion.AutoSize = false;
            seccion.Size = new Size(900, 34 + altoGrid + 12);
            seccion.Margin = new Padding(0, 0, 0, 16);
            seccion.BackColor = TemaVisual.Colores.Blanco;

            Label rotulo = TemaVisual.CrearTituloSeccion(titulo, glifo);
            rotulo.Dock = DockStyle.Top;
            rotulo.BackColor = TemaVisual.Colores.Blanco;

            grid.Dock = DockStyle.Top;
            grid.Height = altoGrid;
            grid.Margin = new Padding(0);

            // El orden importa: el que va Dock.Top primero queda más
            // arriba, así que el título se agrega antes que la grilla.
            seccion.Controls.Add(grid);
            seccion.Controls.Add(rotulo);

            return seccion;
        }

        // Une un título de sección con una fila de tarjetas indicador (KPIs).
        private static Panel CrearSeccionTarjetas(
            string titulo,
            string glifo,
            params Panel[] tarjetas)
        {
            Panel seccion = new Panel();
            seccion.AutoSize = false;
            seccion.Size = new Size(900, 34 + 76 + 12);
            seccion.Margin = new Padding(0, 0, 0, 16);
            seccion.BackColor = TemaVisual.Colores.Blanco;

            Label rotulo = TemaVisual.CrearTituloSeccion(titulo, glifo);
            rotulo.Dock = DockStyle.Top;
            rotulo.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel fila = new FlowLayoutPanel();
            fila.Dock = DockStyle.Top;
            fila.Height = 76;
            fila.FlowDirection = FlowDirection.LeftToRight;
            fila.WrapContents = false;
            fila.BackColor = TemaVisual.Colores.Blanco;

            foreach (Panel tarjeta in tarjetas)
            {
                fila.Controls.Add(tarjeta);
            }

            seccion.Controls.Add(fila);
            seccion.Controls.Add(rotulo);

            return seccion;
        }

        // Crea una columna de texto simple enlazada a una propiedad.
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

        // Crea una columna de fecha con el formato "07-ago-26" usado en la app.
        private static DataGridViewTextBoxColumn CrearColumnaFecha(
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
            columna.DefaultCellStyle.Format = "dd-MMM-yy";
            columna.DefaultCellStyle.FormatProvider = new CultureInfo("es-DO");
            return columna;
        }

        // Crea una columna de monto con el formato "RD$3,500.00".
        private static DataGridViewTextBoxColumn CrearColumnaMonto(
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
            columna.DefaultCellStyle.Format = "'RD$'#,##0.00";
            columna.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            return columna;
        }

        // Crea una tarjeta indicador más ancha, para montos en pesos que no caben en la original.
        private static Panel CrearTarjetaMonto(string rotulo, Color acento, out Label valor)
        {
            Panel tarjeta = new Panel();
            tarjeta.Size = new Size(206, 76);
            tarjeta.Margin = new Padding(0, 0, 12, 0);
            TemaVisual.AplicarTarjeta(
                tarjeta,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.BordeSutil,
                TemaVisual.Colores.FondoClaro,
                12);

            Label icono = new Label();
            icono.Text = TemaVisual.Glifos.Factura;
            icono.Font = TemaVisual.Fuentes.Icono(14F);
            icono.ForeColor = acento;
            icono.BackColor = Color.Transparent;
            icono.AutoSize = false;
            icono.Size = new Size(24, 20);
            icono.Location = new Point(12, 10);
            icono.TextAlign = ContentAlignment.MiddleLeft;

            valor = new Label();
            valor.Text = "RD$0.00";
            valor.Font = new Font("Georgia", 14F, FontStyle.Bold);
            valor.ForeColor = TemaVisual.Colores.AzulMarino;
            valor.BackColor = Color.Transparent;
            valor.AutoSize = false;
            valor.Size = new Size(182, 26);
            valor.Location = new Point(12, 28);
            valor.TextAlign = ContentAlignment.MiddleLeft;

            Label texto = new Label();
            texto.Text = rotulo;
            texto.Font = TemaVisual.Fuentes.Pequena;
            texto.ForeColor = TemaVisual.Colores.TextoSuave;
            texto.BackColor = Color.Transparent;
            texto.AutoSize = false;
            texto.Size = new Size(182, 18);
            texto.Location = new Point(12, 54);
            texto.TextAlign = ContentAlignment.MiddleLeft;

            tarjeta.Controls.Add(icono);
            tarjeta.Controls.Add(valor);
            tarjeta.Controls.Add(texto);

            return tarjeta;
        }

        // Crea una grilla nueva ya estilizada, con columnas explícitas.
        private static DataGridView CrearGrid()
        {
            DataGridView grid = new DataGridView();
            TemaVisual.EstilizarGrid(grid);
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            return grid;
        }

        // Busca una fila por etiqueta en una lista de conteo agrupado.
        private static FilaConteo BuscarConteo(List<FilaConteo> lista, string etiqueta)
        {
            foreach (FilaConteo fila in lista)
            {
                if (fila.Etiqueta == etiqueta)
                {
                    return fila;
                }
            }

            return null;
        }

        private static FilaConteoMonto BuscarConteoMonto(
            List<FilaConteoMonto> lista, string etiqueta)
        {
            foreach (FilaConteoMonto fila in lista)
            {
                if (fila.Etiqueta == etiqueta)
                {
                    return fila;
                }
            }

            return null;
        }

        private static FilaConteoPorcentaje BuscarConteoPorcentaje(
            List<FilaConteoPorcentaje> lista, string etiqueta)
        {
            foreach (FilaConteoPorcentaje fila in lista)
            {
                if (fila.Etiqueta == etiqueta)
                {
                    return fila;
                }
            }

            return null;
        }

        // Cabecera de la pestaña de ocupación: el porcentaje del día y el
        // rótulo de la tabla, que lista únicamente habitaciones ocupadas
        // porque es lo que devuelve ObtenerOcupacionDelDia.
        private void ConstruirIndicadorOcupacion()
        {
            Panel barra = new Panel();
            barra.Dock = DockStyle.Top;
            barra.Height = 72;
            barra.BackColor = TemaVisual.Colores.Blanco;

            _lblOcupacion = new Label();
            _lblOcupacion.Text = "—";
            _lblOcupacion.Font = new Font("Georgia", 24F, FontStyle.Bold);
            _lblOcupacion.ForeColor = TemaVisual.Colores.AzulMarino;
            _lblOcupacion.AutoSize = false;
            _lblOcupacion.Size = new Size(140, 38);
            _lblOcupacion.Location = new Point(2, 4);
            _lblOcupacion.TextAlign = ContentAlignment.MiddleLeft;

            _lblDetalleOcupacion = new Label();
            _lblDetalleOcupacion.Text = "Ocupación de hoy";
            _lblDetalleOcupacion.Font = TemaVisual.Fuentes.Pequena;
            _lblDetalleOcupacion.ForeColor = TemaVisual.Colores.TextoSuave;
            _lblDetalleOcupacion.AutoSize = false;
            _lblDetalleOcupacion.Size = new Size(320, 20);
            _lblDetalleOcupacion.Location = new Point(4, 44);
            _lblDetalleOcupacion.TextAlign = ContentAlignment.MiddleLeft;

            Label rotuloTabla = TemaVisual.CrearTituloSeccion(
                "Habitaciones ocupadas ahora",
                TemaVisual.Glifos.Habitacion);
            rotuloTabla.Location = new Point(360, 22);
            rotuloTabla.Size = new Size(340, 32);
            rotuloTabla.BackColor = TemaVisual.Colores.Blanco;

            barra.Controls.Add(_lblOcupacion);
            barra.Controls.Add(_lblDetalleOcupacion);
            barra.Controls.Add(rotuloTabla);

            tabPage1.Controls.Add(barra);
            dgvOcupacionDelDia.BringToFront();
        }

        // Calcula el porcentaje con el total real de habitaciones del hotel.
        private void ActualizarIndicadorOcupacion(int ocupadas)
        {
            if (_lblOcupacion == null)
            {
                return;
            }

            try
            {
                List<Habitacion> habitaciones =
                    habitacionService.ObtenerTodas();
                int total = habitaciones == null ? 0 : habitaciones.Count;

                if (total <= 0)
                {
                    _lblOcupacion.Text = "—";
                    _lblDetalleOcupacion.Text = "Ocupación de hoy";
                    return;
                }

                int porcentaje = (int)Math.Round(
                    (decimal)ocupadas * 100m / total);

                _lblOcupacion.Text = porcentaje + "%";
                _lblDetalleOcupacion.Text =
                    "Ocupación de hoy · " + ocupadas + " de " +
                    total + " habitaciones";
            }
            catch (Exception)
            {
                // El indicador es accesorio: si el total no se puede
                // consultar, el reporte principal debe seguir mostrándose.
                _lblOcupacion.Text = "—";
                _lblDetalleOcupacion.Text = "Ocupación de hoy";
            }
        }

        // Mensaje de advertencia estándar para los reportes nuevos.
        private static void MostrarAdvertenciaReporte(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // Resuelve "Nombre Apellido" a partir del documento del huésped.
        private string ObtenerNombreHuesped(string numeroDocumento)
        {
            if (string.IsNullOrEmpty(numeroDocumento))
            {
                return "—";
            }

            try
            {
                Huesped huesped = huespedService.BuscarPorDocumento(numeroDocumento);
                return huesped != null
                    ? huesped.Nombre + " " + huesped.Apellido
                    : numeroDocumento;
            }
            catch (SqlException)
            {
                return numeroDocumento;
            }
        }

        // ------------------------------------------------------------------
        // Pestaña "Habitaciones" (ambos roles)
        // ------------------------------------------------------------------

        private DataGridView _dgvHabitacionesDisponibles;
        private DataGridView _dgvHabitacionesLimpieza;
        private DataGridView _dgvOcupacionPorTipo;
        private DataGridView _dgvDistribucionPorPiso;

        private TabPage ConstruirTabHabitaciones()
        {
            TabPage pagina = new TabPage("Habitaciones");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            _dgvHabitacionesDisponibles = CrearGrid();
            _dgvHabitacionesDisponibles.Columns.Add(CrearColumnaTexto(
                "Numero", "Número", "colNumero", 20));
            _dgvHabitacionesDisponibles.Columns.Add(CrearColumnaTexto(
                "Tipo", "Tipo", "colTipo", 25));
            _dgvHabitacionesDisponibles.Columns.Add(CrearColumnaTexto(
                "Piso", "Piso", "colPiso", 15));
            _dgvHabitacionesDisponibles.Columns.Add(CrearColumnaMonto(
                "TarifaBase", "Tarifa", "colTarifa", 40));

            _dgvHabitacionesLimpieza = CrearGrid();
            _dgvHabitacionesLimpieza.Columns.Add(CrearColumnaTexto(
                "Numero", "Número", "colNumero", 20));
            _dgvHabitacionesLimpieza.Columns.Add(CrearColumnaTexto(
                "Tipo", "Tipo", "colTipo", 25));
            _dgvHabitacionesLimpieza.Columns.Add(CrearColumnaTexto(
                "Piso", "Piso", "colPiso", 15));
            _dgvHabitacionesLimpieza.Columns.Add(CrearColumnaMonto(
                "TarifaBase", "Tarifa", "colTarifa", 40));

            _dgvOcupacionPorTipo = CrearGrid();
            _dgvOcupacionPorTipo.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Tipo", "colTipo", 30));
            _dgvOcupacionPorTipo.Columns.Add(CrearColumnaTexto(
                "Ocupadas", "Ocupadas", "colOcupadas", 20));
            _dgvOcupacionPorTipo.Columns.Add(CrearColumnaTexto(
                "Total", "Total", "colTotal", 20));
            _dgvOcupacionPorTipo.Columns.Add(CrearColumnaTexto(
                "Porcentaje", "% Ocupación", "colPorcentaje", 30));

            _dgvDistribucionPorPiso = CrearGrid();
            _dgvDistribucionPorPiso.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Piso", "colPiso", 40));
            _dgvDistribucionPorPiso.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Cantidad de habitaciones", "colCantidad", 60));

            contenedor.Controls.Add(CrearSeccionGrid(
                "Habitaciones disponibles ahora", TemaVisual.Glifos.Habitacion,
                _dgvHabitacionesDisponibles, 150));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Habitaciones en limpieza ahora", TemaVisual.Glifos.Habitacion,
                _dgvHabitacionesLimpieza, 150));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Ocupación por tipo de habitación", TemaVisual.Glifos.Habitacion,
                _dgvOcupacionPorTipo, 130));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Distribución de habitaciones por piso", TemaVisual.Glifos.Habitacion,
                _dgvDistribucionPorPiso, 130));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void CargarDatosHabitaciones()
        {
            try
            {
                List<Habitacion> todas = habitacionService.ObtenerTodas();

                _dgvHabitacionesDisponibles.DataSource =
                    FiltrarHabitacionesPorEstado(todas, "Disponible");
                _dgvHabitacionesLimpieza.DataSource =
                    FiltrarHabitacionesPorEstado(todas, "Limpieza");
                _dgvOcupacionPorTipo.DataSource = ObtenerOcupacionPorTipo(todas);
                _dgvDistribucionPorPiso.DataSource = ObtenerDistribucionPorPiso(todas);
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo cargar el reporte de habitaciones.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al cargar el reporte de habitaciones.");
            }
        }

        private static List<Habitacion> FiltrarHabitacionesPorEstado(
            List<Habitacion> todas, string estado)
        {
            List<Habitacion> filtradas = new List<Habitacion>();

            foreach (Habitacion habitacion in todas)
            {
                if (habitacion.Estado == estado)
                {
                    filtradas.Add(habitacion);
                }
            }

            return filtradas;
        }

        private static List<FilaConteoPorcentaje> ObtenerOcupacionPorTipo(
            List<Habitacion> todas)
        {
            List<FilaConteoPorcentaje> resultado = new List<FilaConteoPorcentaje>();

            foreach (Habitacion habitacion in todas)
            {
                FilaConteoPorcentaje fila =
                    BuscarConteoPorcentaje(resultado, habitacion.Tipo);

                if (fila == null)
                {
                    fila = new FilaConteoPorcentaje();
                    fila.Etiqueta = habitacion.Tipo;
                    resultado.Add(fila);
                }

                fila.Total = fila.Total + 1;

                if (habitacion.Estado == "Ocupada")
                {
                    fila.Ocupadas = fila.Ocupadas + 1;
                }
            }

            foreach (FilaConteoPorcentaje fila in resultado)
            {
                fila.Porcentaje = fila.Total > 0
                    ? Math.Round((decimal)fila.Ocupadas * 100m / fila.Total, 0)
                    : 0;
            }

            return resultado;
        }

        private static List<FilaConteo> ObtenerDistribucionPorPiso(
            List<Habitacion> todas)
        {
            List<FilaConteo> resultado = new List<FilaConteo>();

            foreach (Habitacion habitacion in todas)
            {
                string etiqueta = "Piso " + habitacion.Piso;
                FilaConteo fila = BuscarConteo(resultado, etiqueta);

                if (fila == null)
                {
                    fila = new FilaConteo();
                    fila.Etiqueta = etiqueta;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
            }

            return resultado;
        }

        // ------------------------------------------------------------------
        // Pestaña "Check-in / Salidas" (ambos roles)
        // ------------------------------------------------------------------

        private DataGridView _dgvNoShows;
        private DataGridView _dgvProximasSalidas;

        private TabPage ConstruirTabCheckInSalidas()
        {
            TabPage pagina = new TabPage("Check-in / Salidas");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            _dgvNoShows = CrearGrid();
            _dgvNoShows.Columns.Add(CrearColumnaTexto(
                "Documento", "ID", "colId", 18));
            _dgvNoShows.Columns.Add(CrearColumnaTexto(
                "Nombre", "Nombre", "colNombre", 32));
            _dgvNoShows.Columns.Add(CrearColumnaTexto(
                "Habitacion", "Habitación", "colHabitacion", 15));
            _dgvNoShows.Columns.Add(CrearColumnaFecha(
                "Fecha", "Check-in esperado", "colFecha", 35));

            _dgvProximasSalidas = CrearGrid();
            _dgvProximasSalidas.Columns.Add(CrearColumnaTexto(
                "Documento", "ID", "colId", 18));
            _dgvProximasSalidas.Columns.Add(CrearColumnaTexto(
                "Nombre", "Nombre", "colNombre", 32));
            _dgvProximasSalidas.Columns.Add(CrearColumnaTexto(
                "Habitacion", "Habitación", "colHabitacion", 15));
            _dgvProximasSalidas.Columns.Add(CrearColumnaFecha(
                "Fecha", "Fecha de salida", "colFecha", 35));

            contenedor.Controls.Add(CrearSeccionGrid(
                "Reservas vencidas sin check-in", TemaVisual.Glifos.Alerta,
                _dgvNoShows, 160));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Próximas salidas", TemaVisual.Glifos.CheckOut,
                _dgvProximasSalidas, 160));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void CargarDatosCheckInSalidas()
        {
            try
            {
                List<Reserva> reservas = reservaService.ObtenerTodas();
                List<Estadia> estadias = estadiaService.ObtenerTodas();

                _dgvNoShows.DataSource = ObtenerNoShows(reservas, estadias);

                List<Estadia> activas = estadiaService.ObtenerActivas();
                _dgvProximasSalidas.DataSource = ObtenerProximasSalidas(activas);
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo cargar el reporte de check-in/salidas.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al cargar el reporte de check-in/salidas.");
            }
        }

        // Reservas cuya fecha de entrada ya pasó y todavía no tienen estadía registrada.
        private List<FilaReservaResumen> ObtenerNoShows(
            List<Reserva> reservas, List<Estadia> estadias)
        {
            List<FilaReservaResumen> resultado = new List<FilaReservaResumen>();

            foreach (Reserva reserva in reservas)
            {
                if (reserva.Estado == "Cancelada")
                {
                    continue;
                }

                if (reserva.FechaCheckIn.Date >= DateTime.Today)
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

                if (tieneEstadia)
                {
                    continue;
                }

                FilaReservaResumen fila = new FilaReservaResumen();
                fila.Documento = reserva.NumeroDocumentoHuesped;
                fila.Nombre = ObtenerNombreHuesped(reserva.NumeroDocumentoHuesped);
                fila.Habitacion = reserva.NumeroHabitacion;
                fila.Fecha = reserva.FechaCheckIn;
                resultado.Add(fila);
            }

            return resultado;
        }

        // Resuelve la Reserva de cada Estadia activa para listar las próximas salidas.
        private List<FilaReservaResumen> ObtenerProximasSalidas(
            List<Estadia> activas)
        {
            List<FilaReservaResumen> resultado = new List<FilaReservaResumen>();

            foreach (Estadia estadia in activas)
            {
                Reserva reserva = null;

                try
                {
                    reserva = reservaService.Buscar(estadia.IdReserva);
                }
                catch (SqlException)
                {
                }

                if (reserva == null)
                {
                    continue;
                }

                FilaReservaResumen fila = new FilaReservaResumen();
                fila.Documento = reserva.NumeroDocumentoHuesped;
                fila.Nombre = ObtenerNombreHuesped(reserva.NumeroDocumentoHuesped);
                fila.Habitacion = reserva.NumeroHabitacion;
                fila.Fecha = reserva.FechaCheckOut;
                resultado.Add(fila);
            }

            resultado.Sort(delegate (FilaReservaResumen a, FilaReservaResumen b)
            {
                return a.Fecha.CompareTo(b.Fecha);
            });

            return resultado;
        }

        // ------------------------------------------------------------------
        // Pestaña "Reservas y Huéspedes" (ambos roles)
        // ------------------------------------------------------------------

        private DataGridView _dgvReservasPorEstado;
        private DataGridView _dgvHuespedesPorTipo;

        private TabPage ConstruirTabReservasHuespedes()
        {
            TabPage pagina = new TabPage("Reservas y Huéspedes");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            _dgvReservasPorEstado = CrearGrid();
            _dgvReservasPorEstado.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Estado", "colEstado", 30));
            _dgvReservasPorEstado.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Cantidad", "colCantidad", 30));
            _dgvReservasPorEstado.Columns.Add(CrearColumnaMonto(
                "Monto", "Monto estimado", "colMonto", 40));

            _dgvHuespedesPorTipo = CrearGrid();
            _dgvHuespedesPorTipo.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Tipo de documento", "colTipo", 50));
            _dgvHuespedesPorTipo.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Cantidad de huéspedes", "colCantidad", 50));

            contenedor.Controls.Add(CrearSeccionGrid(
                "Reservas por estado", TemaVisual.Glifos.Reserva,
                _dgvReservasPorEstado, 130));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Huéspedes por tipo de documento", TemaVisual.Glifos.Huesped,
                _dgvHuespedesPorTipo, 110));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void CargarDatosReservasHuespedes()
        {
            try
            {
                List<Reserva> reservas = reservaService.ObtenerTodas();
                _dgvReservasPorEstado.DataSource = ObtenerReservasPorEstado(reservas);

                List<Huesped> huespedes = huespedService.ObtenerTodos();
                _dgvHuespedesPorTipo.DataSource =
                    ObtenerHuespedesPorTipoDocumento(huespedes);
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo cargar el reporte de reservas y huéspedes.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al cargar el reporte de reservas y huéspedes.");
            }
        }

        private static List<FilaConteoMonto> ObtenerReservasPorEstado(
            List<Reserva> reservas)
        {
            List<FilaConteoMonto> resultado = new List<FilaConteoMonto>();

            foreach (Reserva reserva in reservas)
            {
                FilaConteoMonto fila = BuscarConteoMonto(resultado, reserva.Estado);

                if (fila == null)
                {
                    fila = new FilaConteoMonto();
                    fila.Etiqueta = reserva.Estado;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
                fila.Monto = fila.Monto + reserva.MontoEstimado;
            }

            return resultado;
        }

        private static List<FilaConteo> ObtenerHuespedesPorTipoDocumento(
            List<Huesped> huespedes)
        {
            List<FilaConteo> resultado = new List<FilaConteo>();

            foreach (Huesped huesped in huespedes)
            {
                FilaConteo fila = BuscarConteo(resultado, huesped.TipoDocumento);

                if (fila == null)
                {
                    fila = new FilaConteo();
                    fila.Etiqueta = huesped.TipoDocumento;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
            }

            return resultado;
        }

        // ------------------------------------------------------------------
        // Pestaña "Financiero" (solo Administrador)
        // ------------------------------------------------------------------

        private DateTimePicker _dtpFinancieroInicio;
        private DateTimePicker _dtpFinancieroFin;
        private Button _btnGenerarFinanciero;
        private Label _lblSubtotal;
        private Label _lblITBIS;
        private Label _lblPropina;
        private Label _lblFacturaPromedio;
        private Label _lblEstimadoVsFacturado;
        private DataGridView _dgvIngresosPorHabitacion;
        private DataGridView _dgvIngresosPorTemporada;

        private TabPage ConstruirTabFinanciero()
        {
            TabPage pagina = new TabPage("Financiero");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            Panel barraFiltros = new Panel();
            barraFiltros.AutoSize = false;
            barraFiltros.Size = new Size(900, 52);
            barraFiltros.BackColor = TemaVisual.Colores.FondoSecundario;
            barraFiltros.Margin = new Padding(0, 0, 0, 16);

            Label lblDesde = new Label();
            lblDesde.AutoSize = true;
            lblDesde.Text = "Desde:";
            lblDesde.Font = TemaVisual.Fuentes.Etiqueta;
            lblDesde.ForeColor = TemaVisual.Colores.Texto;
            lblDesde.Location = new Point(14, 18);

            _dtpFinancieroInicio = new DateTimePicker();
            _dtpFinancieroInicio.Value =
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _dtpFinancieroInicio.Location = new Point(64, 13);
            _dtpFinancieroInicio.Width = 150;
            TemaVisual.EstilizarFecha(_dtpFinancieroInicio);

            Label lblHasta = new Label();
            lblHasta.AutoSize = true;
            lblHasta.Text = "Hasta:";
            lblHasta.Font = TemaVisual.Fuentes.Etiqueta;
            lblHasta.ForeColor = TemaVisual.Colores.Texto;
            lblHasta.Location = new Point(228, 18);

            _dtpFinancieroFin = new DateTimePicker();
            _dtpFinancieroFin.Value = DateTime.Today;
            _dtpFinancieroFin.Location = new Point(278, 13);
            _dtpFinancieroFin.Width = 150;
            TemaVisual.EstilizarFecha(_dtpFinancieroFin);

            _btnGenerarFinanciero = new Button();
            _btnGenerarFinanciero.Location = new Point(450, 9);
            _btnGenerarFinanciero.Size = new Size(160, 34);
            TemaVisual.EstilizarBoton(
                _btnGenerarFinanciero,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                _btnGenerarFinanciero, TemaVisual.Glifos.Reportes, "Generar");
            _btnGenerarFinanciero.Click += btnGenerarFinanciero_Click;

            barraFiltros.Controls.Add(lblDesde);
            barraFiltros.Controls.Add(_dtpFinancieroInicio);
            barraFiltros.Controls.Add(lblHasta);
            barraFiltros.Controls.Add(_dtpFinancieroFin);
            barraFiltros.Controls.Add(_btnGenerarFinanciero);

            Panel tarjetaSubtotal = CrearTarjetaMonto(
                "Subtotal", TemaVisual.Colores.AzulPrimario, out _lblSubtotal);
            Panel tarjetaITBIS = CrearTarjetaMonto(
                "ITBIS", TemaVisual.Colores.TurquesaProfundo, out _lblITBIS);
            Panel tarjetaPropina = CrearTarjetaMonto(
                "Propina", TemaVisual.Colores.SolNaranja, out _lblPropina);
            Panel tarjetaPromedio = CrearTarjetaMonto(
                "Factura promedio", TemaVisual.Colores.AzulMarino, out _lblFacturaPromedio);
            Panel tarjetaEstimadoVsFacturado = CrearTarjetaMonto(
                "Estimado vs. facturado", TemaVisual.Colores.Naranja,
                out _lblEstimadoVsFacturado);

            _dgvIngresosPorHabitacion = CrearGrid();
            _dgvIngresosPorHabitacion.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Habitación", "colHabitacion", 25));
            _dgvIngresosPorHabitacion.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Facturas", "colCantidad", 25));
            _dgvIngresosPorHabitacion.Columns.Add(CrearColumnaMonto(
                "Monto", "Ingresos", "colMonto", 50));

            _dgvIngresosPorTemporada = CrearGrid();
            _dgvIngresosPorTemporada.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Temporada", "colTemporada", 30));
            _dgvIngresosPorTemporada.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Facturas", "colCantidad", 25));
            _dgvIngresosPorTemporada.Columns.Add(CrearColumnaMonto(
                "Monto", "Ingresos", "colMonto", 45));

            contenedor.Controls.Add(barraFiltros);
            contenedor.Controls.Add(CrearSeccionTarjetas(
                "Desglose de ingresos del rango", TemaVisual.Glifos.Factura,
                tarjetaSubtotal, tarjetaITBIS, tarjetaPropina, tarjetaPromedio));
            contenedor.Controls.Add(CrearSeccionTarjetas(
                "Coherencia estimado vs. facturado (histórico)",
                TemaVisual.Glifos.Alerta, tarjetaEstimadoVsFacturado));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Ingresos por habitación", TemaVisual.Glifos.Habitacion,
                _dgvIngresosPorHabitacion, 160));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Ingresos por temporada", TemaVisual.Glifos.Reserva,
                _dgvIngresosPorTemporada, 130));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void btnGenerarFinanciero_Click(object sender, EventArgs e)
        {
            CargarDatosFinanciero();
        }

        private void CargarDatosFinanciero()
        {
            _btnGenerarFinanciero.Enabled = false;

            try
            {
                if (_dtpFinancieroFin.Value.Date < _dtpFinancieroInicio.Value.Date)
                {
                    MostrarAdvertenciaReporte(
                        "La fecha final no puede ser anterior a la inicial.");
                    return;
                }

                ReporteIngresos reporte = reporteService.ObtenerIngresosPorRango(
                    _dtpFinancieroInicio.Value,
                    _dtpFinancieroFin.Value);

                decimal subtotal = 0;
                decimal itbis = 0;
                decimal propina = 0;

                foreach (Factura factura in reporte.Facturas)
                {
                    subtotal = subtotal + factura.Subtotal;
                    itbis = itbis + factura.ITBIS;
                    propina = propina + factura.Propina;
                }

                _lblSubtotal.Text = "RD$" + subtotal.ToString("N2");
                _lblITBIS.Text = "RD$" + itbis.ToString("N2");
                _lblPropina.Text = "RD$" + propina.ToString("N2");

                decimal promedio = reporte.Facturas.Count > 0
                    ? reporte.TotalIngresos / reporte.Facturas.Count
                    : 0;
                _lblFacturaPromedio.Text = "RD$" + promedio.ToString("N2");

                _dgvIngresosPorHabitacion.DataSource =
                    ObtenerIngresosPorHabitacion(reporte.Facturas);
                _dgvIngresosPorTemporada.DataSource =
                    ObtenerIngresosPorTemporada(reporte.Facturas);

                CargarEstimadoVsFacturado();
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo consultar los ingresos en la base de datos.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al generar el reporte financiero.");
            }
            finally
            {
                _btnGenerarFinanciero.Enabled = true;
            }
        }

        private List<FilaConteoMonto> ObtenerIngresosPorHabitacion(
            List<Factura> facturas)
        {
            List<FilaConteoMonto> resultado = new List<FilaConteoMonto>();

            foreach (Factura factura in facturas)
            {
                Reserva reserva = ObtenerReservaDeFactura(factura);

                if (reserva == null)
                {
                    continue;
                }

                string etiqueta = "Habitación " + reserva.NumeroHabitacion;
                FilaConteoMonto fila = BuscarConteoMonto(resultado, etiqueta);

                if (fila == null)
                {
                    fila = new FilaConteoMonto();
                    fila.Etiqueta = etiqueta;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
                fila.Monto = fila.Monto + factura.Total;
            }

            return resultado;
        }

        private List<FilaConteoMonto> ObtenerIngresosPorTemporada(
            List<Factura> facturas)
        {
            List<FilaConteoMonto> resultado = new List<FilaConteoMonto>();

            foreach (Factura factura in facturas)
            {
                Reserva reserva = ObtenerReservaDeFactura(factura);

                if (reserva == null)
                {
                    continue;
                }

                FilaConteoMonto fila =
                    BuscarConteoMonto(resultado, reserva.Temporada);

                if (fila == null)
                {
                    fila = new FilaConteoMonto();
                    fila.Etiqueta = reserva.Temporada;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
                fila.Monto = fila.Monto + factura.Total;
            }

            return resultado;
        }

        // Resuelve la Reserva de una Factura, cruzando por su Estadia.
        private Reserva ObtenerReservaDeFactura(Factura factura)
        {
            try
            {
                Estadia estadia = estadiaService.Buscar(factura.IdEstadia);

                if (estadia == null)
                {
                    return null;
                }

                return reservaService.Buscar(estadia.IdReserva);
            }
            catch (SqlException)
            {
                return null;
            }
        }

        // Compara el total estimado en las reservas contra el total realmente facturado.
        private void CargarEstimadoVsFacturado()
        {
            try
            {
                List<Reserva> reservas = reservaService.ObtenerTodas();
                List<Factura> facturas = facturaService.ObtenerTodas();

                decimal totalEstimado = 0;

                foreach (Reserva reserva in reservas)
                {
                    totalEstimado = totalEstimado + reserva.MontoEstimado;
                }

                decimal totalFacturado = 0;

                foreach (Factura factura in facturas)
                {
                    totalFacturado = totalFacturado + factura.Total;
                }

                decimal diferencia = totalFacturado - totalEstimado;
                _lblEstimadoVsFacturado.Text = "RD$" + diferencia.ToString("N2");
            }
            catch (SqlException)
            {
                _lblEstimadoVsFacturado.Text = "—";
            }
        }

        // ------------------------------------------------------------------
        // Pestaña "Estrategia" (solo Administrador)
        // ------------------------------------------------------------------

        private Label _lblTasaCancelacion;
        private Label _lblDuracionPromedio;
        private DataGridView _dgvReservasPorTemporada;
        private DataGridView _dgvHuespedesFrecuentes;

        private TabPage ConstruirTabEstrategia()
        {
            TabPage pagina = new TabPage("Estrategia");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            Panel tarjetaCancelacion = TemaVisual.CrearTarjetaIndicador(
                "Tasa de cancelación", TemaVisual.Glifos.Cancelar,
                TemaVisual.Colores.Rojo, out _lblTasaCancelacion);
            Panel tarjetaDuracion = TemaVisual.CrearTarjetaIndicador(
                "Noches promedio", TemaVisual.Glifos.Reserva,
                TemaVisual.Colores.AzulPrimario, out _lblDuracionPromedio);

            _dgvReservasPorTemporada = CrearGrid();
            _dgvReservasPorTemporada.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Temporada", "colTemporada", 30));
            _dgvReservasPorTemporada.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Reservas", "colCantidad", 25));
            _dgvReservasPorTemporada.Columns.Add(CrearColumnaMonto(
                "Monto", "Monto estimado", "colMonto", 45));

            _dgvHuespedesFrecuentes = CrearGrid();
            _dgvHuespedesFrecuentes.Columns.Add(CrearColumnaTexto(
                "Documento", "ID", "colId", 20));
            _dgvHuespedesFrecuentes.Columns.Add(CrearColumnaTexto(
                "Nombre", "Nombre", "colNombre", 45));
            _dgvHuespedesFrecuentes.Columns.Add(CrearColumnaTexto(
                "CantidadReservas", "Reservas", "colCantidad", 35));

            contenedor.Controls.Add(CrearSeccionTarjetas(
                "Indicadores generales", TemaVisual.Glifos.Reportes,
                tarjetaCancelacion, tarjetaDuracion));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Reservas por temporada", TemaVisual.Glifos.Reserva,
                _dgvReservasPorTemporada, 130));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Huéspedes frecuentes (top 10)", TemaVisual.Glifos.Huesped,
                _dgvHuespedesFrecuentes, 220));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void CargarDatosEstrategia()
        {
            try
            {
                List<Reserva> reservas = reservaService.ObtenerTodas();

                int total = reservas.Count;
                int canceladas = 0;
                int sumaNoches = 0;

                foreach (Reserva reserva in reservas)
                {
                    if (reserva.Estado == "Cancelada")
                    {
                        canceladas = canceladas + 1;
                    }

                    sumaNoches = sumaNoches + reserva.TotalNoches;
                }

                decimal tasa = total > 0
                    ? Math.Round((decimal)canceladas * 100m / total, 1)
                    : 0;
                _lblTasaCancelacion.Text = tasa.ToString("0.0") + "%";

                decimal promedioNoches = total > 0
                    ? Math.Round((decimal)sumaNoches / total, 1)
                    : 0;
                _lblDuracionPromedio.Text = promedioNoches.ToString("0.0");

                _dgvReservasPorTemporada.DataSource =
                    ObtenerReservasPorTemporada(reservas);

                List<Huesped> huespedes = huespedService.ObtenerTodos();
                _dgvHuespedesFrecuentes.DataSource =
                    ObtenerHuespedesFrecuentes(reservas, huespedes);
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo cargar el reporte de estrategia.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al cargar el reporte de estrategia.");
            }
        }

        private static List<FilaConteoMonto> ObtenerReservasPorTemporada(
            List<Reserva> reservas)
        {
            List<FilaConteoMonto> resultado = new List<FilaConteoMonto>();

            foreach (Reserva reserva in reservas)
            {
                FilaConteoMonto fila =
                    BuscarConteoMonto(resultado, reserva.Temporada);

                if (fila == null)
                {
                    fila = new FilaConteoMonto();
                    fila.Etiqueta = reserva.Temporada;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
                fila.Monto = fila.Monto + reserva.MontoEstimado;
            }

            return resultado;
        }

        private static List<FilaHuespedFrecuente> ObtenerHuespedesFrecuentes(
            List<Reserva> reservas, List<Huesped> huespedes)
        {
            List<FilaHuespedFrecuente> resultado = new List<FilaHuespedFrecuente>();

            foreach (Reserva reserva in reservas)
            {
                FilaHuespedFrecuente fila = null;

                foreach (FilaHuespedFrecuente candidata in resultado)
                {
                    if (candidata.Documento == reserva.NumeroDocumentoHuesped)
                    {
                        fila = candidata;
                        break;
                    }
                }

                if (fila == null)
                {
                    fila = new FilaHuespedFrecuente();
                    fila.Documento = reserva.NumeroDocumentoHuesped;
                    fila.Nombre = reserva.NumeroDocumentoHuesped;

                    foreach (Huesped huesped in huespedes)
                    {
                        if (huesped.NumeroDocumento == reserva.NumeroDocumentoHuesped)
                        {
                            fila.Nombre = huesped.Nombre + " " + huesped.Apellido;
                            break;
                        }
                    }

                    resultado.Add(fila);
                }

                fila.CantidadReservas = fila.CantidadReservas + 1;
            }

            resultado.Sort(delegate (FilaHuespedFrecuente a, FilaHuespedFrecuente b)
            {
                return b.CantidadReservas.CompareTo(a.CantidadReservas);
            });

            if (resultado.Count > 10)
            {
                resultado.RemoveRange(10, resultado.Count - 10);
            }

            return resultado;
        }

        // ------------------------------------------------------------------
        // Pestaña "Actividad" (solo Administrador — BitacoraService ya
        // exige Administrador internamente, ver AutorizacionService)
        // ------------------------------------------------------------------

        private DataGridView _dgvActividadReciente;
        private DataGridView _dgvActividadPorUsuario;
        private DataGridView _dgvCheckInOutPorDia;

        private TabPage ConstruirTabActividad()
        {
            TabPage pagina = new TabPage("Actividad");
            pagina.BackColor = TemaVisual.Colores.Blanco;

            FlowLayoutPanel contenedor = CrearContenedorPestana();

            _dgvActividadReciente = CrearGrid();
            _dgvActividadReciente.Columns.Add(CrearColumnaFecha(
                "FechaHora", "Fecha", "colFecha", 20));
            _dgvActividadReciente.Columns.Add(CrearColumnaTexto(
                "Usuario", "Usuario", "colUsuario", 25));
            _dgvActividadReciente.Columns.Add(CrearColumnaTexto(
                "Accion", "Acción", "colAccion", 20));
            _dgvActividadReciente.Columns.Add(CrearColumnaTexto(
                "Detalle", "Detalle", "colDetalle", 35));

            _dgvActividadPorUsuario = CrearGrid();
            _dgvActividadPorUsuario.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Usuario", "colUsuario", 50));
            _dgvActividadPorUsuario.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Acciones registradas", "colCantidad", 50));

            _dgvCheckInOutPorDia = CrearGrid();
            _dgvCheckInOutPorDia.Columns.Add(CrearColumnaTexto(
                "Etiqueta", "Fecha", "colFecha", 50));
            _dgvCheckInOutPorDia.Columns.Add(CrearColumnaTexto(
                "Cantidad", "Check-ins/check-outs", "colCantidad", 50));

            contenedor.Controls.Add(CrearSeccionGrid(
                "Actividad reciente", TemaVisual.Glifos.Bitacora,
                _dgvActividadReciente, 180));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Actividad por usuario", TemaVisual.Glifos.Administrador,
                _dgvActividadPorUsuario, 130));
            contenedor.Controls.Add(CrearSeccionGrid(
                "Check-ins / check-outs por día", TemaVisual.Glifos.CheckIn,
                _dgvCheckInOutPorDia, 150));

            pagina.Controls.Add(contenedor);
            return pagina;
        }

        private void CargarDatosActividad()
        {
            try
            {
                List<Bitacora> bitacora = bitacoraService.ObtenerTodos(usuarioActual);
                List<Usuario> usuarios = usuarioService.ObtenerTodos();

                _dgvActividadReciente.DataSource =
                    ObtenerActividadReciente(bitacora, usuarios);
                _dgvActividadPorUsuario.DataSource =
                    ObtenerActividadPorUsuario(bitacora, usuarios);
                _dgvCheckInOutPorDia.DataSource =
                    ObtenerCheckInOutPorDia(bitacora);
            }
            catch (PermisoDenegadoException)
            {
                // No debería pasar (esta pestaña es solo para Administrador); deja la pestaña vacía.
            }
            catch (SqlException)
            {
                MostrarAdvertenciaReporte(
                    "No se pudo cargar el reporte de actividad.");
            }
            catch (Exception)
            {
                MostrarAdvertenciaReporte(
                    "Ocurrió un error al cargar el reporte de actividad.");
            }
        }

        private static List<FilaActividad> ObtenerActividadReciente(
            List<Bitacora> bitacora, List<Usuario> usuarios)
        {
            List<FilaActividad> resultado = new List<FilaActividad>();

            foreach (Bitacora registro in bitacora)
            {
                FilaActividad fila = new FilaActividad();
                fila.FechaHora = registro.FechaHora;
                fila.Usuario = ObtenerNombreUsuario(usuarios, registro.IdUsuario);
                fila.Accion = registro.Accion;
                fila.Detalle = registro.Detalle;
                resultado.Add(fila);
            }

            resultado.Sort(delegate (FilaActividad a, FilaActividad b)
            {
                return b.FechaHora.CompareTo(a.FechaHora);
            });

            if (resultado.Count > 30)
            {
                resultado.RemoveRange(30, resultado.Count - 30);
            }

            return resultado;
        }

        private static List<FilaConteo> ObtenerActividadPorUsuario(
            List<Bitacora> bitacora, List<Usuario> usuarios)
        {
            List<FilaConteo> resultado = new List<FilaConteo>();

            foreach (Bitacora registro in bitacora)
            {
                string etiqueta = ObtenerNombreUsuario(usuarios, registro.IdUsuario);
                FilaConteo fila = BuscarConteo(resultado, etiqueta);

                if (fila == null)
                {
                    fila = new FilaConteo();
                    fila.Etiqueta = etiqueta;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
            }

            return resultado;
        }

        private static List<FilaConteo> ObtenerCheckInOutPorDia(
            List<Bitacora> bitacora)
        {
            List<FilaConteo> resultado = new List<FilaConteo>();
            CultureInfo cultura = new CultureInfo("es-DO");

            foreach (Bitacora registro in bitacora)
            {
                if (registro.Accion != "CheckIn" && registro.Accion != "CheckOut")
                {
                    continue;
                }

                string etiqueta = registro.FechaHora.ToString("dd-MMM-yy", cultura);
                FilaConteo fila = BuscarConteo(resultado, etiqueta);

                if (fila == null)
                {
                    fila = new FilaConteo();
                    fila.Etiqueta = etiqueta;
                    resultado.Add(fila);
                }

                fila.Cantidad = fila.Cantidad + 1;
            }

            return resultado;
        }

        private static string ObtenerNombreUsuario(
            List<Usuario> usuarios, int idUsuario)
        {
            foreach (Usuario usuario in usuarios)
            {
                if (usuario.IdUsuario == idUsuario)
                {
                    return usuario.NombreCompleto;
                }
            }

            return "Usuario #" + idUsuario;
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            if (UsuarioTieneAcceso() == false)
            {
                CerrarPorAccesoDenegado();
                return;
            }

            try
            {
                AutorizacionService.ExigirUsuarioActivo(usuarioActual);

                ConfigurarPestanasPorRol();

                List<OcupacionDia> ocupacion =
                    reporteService.ObtenerOcupacionDelDia();

                dgvOcupacionDelDia.DataSource = ocupacion;
                ActualizarIndicadorOcupacion(ocupacion.Count);

                if (ocupacion.Count == 0)
                {
                    MessageBox.Show(
                        "No hay habitaciones ocupadas en este momento.",
                        "Reportes",
                        MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
            }
            catch (PermisoDenegadoException)
            {
                CerrarPorAccesoDenegado();
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "No se pudo cargar la ocupación desde la base de datos.",
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Ocurrió un error al cargar el reporte de ocupación.",
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            if (UsuarioTieneAcceso() == false)
            {
                CerrarPorAccesoDenegado();
                return;
            }

            btnGenerarReporte.Enabled = false;

            try
            {
                AutorizacionService.ExigirUsuarioActivo(usuarioActual);

                if (dtpFechaFin.Value.Date < dtpFechaInicio.Value.Date)
                {
                    throw new FormatException(
                        "La fecha final no puede ser anterior a la fecha inicial.");
                }

                ReporteIngresos reporte = reporteService.ObtenerIngresosPorRango(
                    dtpFechaInicio.Value,
                    dtpFechaFin.Value);

                dgvIngresos.DataSource = reporte.Facturas;
                lblTotalIngresos.Text = "Total de ingresos: RD$ " + reporte.TotalIngresos.ToString("N2");

                if (reporte.Facturas.Count == 0)
                {
                    MessageBox.Show(
                        "No hay facturas entre las fechas seleccionadas.",
                        "Reportes",
                        MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
            }
            catch (PermisoDenegadoException)
            {
                CerrarPorAccesoDenegado();
            }
            catch (FormatException error)
            {
                MessageBox.Show(
                    error.Message,
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "No se pudo consultar los ingresos en la base de datos.",
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Ocurrió un error al generar el reporte de ingresos.",
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                btnGenerarReporte.Enabled = true;
            }
        }

        private bool UsuarioTieneAcceso()
        {
            if (usuarioActual == null)
            {
                return false;
            }

            if (usuarioActual.Activo == false)
            {
                return false;
            }

            if (usuarioActual.Rol == "Administrador")
            {
                return true;
            }

            if (usuarioActual.Rol == "Recepcionista")
            {
                return true;
            }

            return false;
        }

        private void CerrarPorAccesoDenegado()
        {
            tabControl.Enabled = false;
            btnGenerarReporte.Enabled = false;

            MessageBox.Show(
                "No tiene permiso para consultar los reportes.",
                "Reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            Close();
        }
    }
}
