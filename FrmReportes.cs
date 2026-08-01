using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Reportes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmReportes : Form
    {
        private readonly ReporteService reporteService;
        private Usuario usuarioActual;

        public FrmReportes()
        {
            InitializeComponent();
            reporteService = new ReporteService();
            ConfigurarPantalla();
        }

        public FrmReportes(Usuario usuario) : this()
        {
            usuarioActual = usuario;
        }

        private void ConfigurarPantalla()
        {
            Text = "Reportes";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 251);

            flpReportes.Padding = new Padding(16);
            tabPage1.Text = "Ocupación del día";
            tabPage2.Text = "Ingresos";

            dtpFechaInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFechaFin.Value = DateTime.Today;

            btnGenerarReporte.Text = "Generar reporte";
            btnGenerarReporte.BackColor = Color.FromArgb(2, 88, 151);
            btnGenerarReporte.ForeColor = Color.White;
            btnGenerarReporte.FlatStyle = FlatStyle.Flat;

            dgvOcupacionDelDia.ReadOnly = true;
            dgvOcupacionDelDia.AllowUserToAddRows = false;
            dgvOcupacionDelDia.AllowUserToDeleteRows = false;
            dgvOcupacionDelDia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvIngresos.ReadOnly = true;
            dgvIngresos.AllowUserToAddRows = false;
            dgvIngresos.AllowUserToDeleteRows = false;
            dgvIngresos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            lblTotalIngresos.Text = "Total de ingresos: RD$ 0.00";
            lblTotalIngresos.Font = new Font("Consolas", 12, FontStyle.Bold);
            lblTotalIngresos.ForeColor = Color.FromArgb(2, 88, 151);

            Load += FrmReportes_Load;
            btnGenerarReporte.Click += btnGenerarReporte_Click;
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            try
            {
                if (usuarioActual != null)
                {
                    AutorizacionService.ExigirUsuarioActivo(usuarioActual);
                }

                dgvOcupacionDelDia.DataSource = reporteService.ObtenerOcupacionDelDia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                ReporteIngresos reporte = reporteService.ObtenerIngresosPorRango(
                    dtpFechaInicio.Value,
                    dtpFechaFin.Value);

                dgvIngresos.DataSource = reporte.Facturas;
                lblTotalIngresos.Text = "Total de ingresos: RD$ " + reporte.TotalIngresos.ToString("N2");

                if (reporte.Facturas.Count == 0)
                {
                    MessageBox.Show("No hay facturas en el rango elegido.", "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
