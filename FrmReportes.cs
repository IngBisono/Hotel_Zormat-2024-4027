using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Excepciones;
using HotelZormat.Negocio.Reportes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmReportes : Form
    {
        private readonly ReporteService reporteService;

        // Se usa sólo para el denominador del indicador de ocupación: el
        // reporte devuelve las habitaciones ocupadas, no el total del hotel.
        private readonly HabitacionService habitacionService;

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
            TemaVisual.EstilizarTabControl(tabControl);

            // El encabezado se acopla arriba; las pestañas, acopladas a Fill,
            // deben quedar al frente para repartirse el espacio restante.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Reportes",
                TemaVisual.Glifos.Reportes);
            Controls.Add(encabezado);
            tabControl.BringToFront();
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
