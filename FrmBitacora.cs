using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmBitacora : Form
    {
        private readonly BitacoraService bitacoraService;
        private Usuario usuarioActual;

        public FrmBitacora()
        {
            InitializeComponent();
            bitacoraService = new BitacoraService();
            ConfigurarPantalla();
        }

        public FrmBitacora(Usuario usuario) : this()
        {
            usuarioActual = usuario;
        }

        private void ConfigurarPantalla()
        {
            Text = "Bitácora";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 251);

            cboFiltroAccion.Items.Clear();
            cboFiltroAccion.Items.Add("Todas");
            cboFiltroAccion.Items.Add("Login");
            cboFiltroAccion.Items.Add("CheckIn");
            cboFiltroAccion.Items.Add("CheckOut");
            cboFiltroAccion.Items.Add("Facturacion");
            cboFiltroAccion.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroAccion.SelectedIndex = 0;

            dtpFiltroFecha.ShowCheckBox = true;
            dtpFiltroFecha.Checked = false;

            dgvBitacora.ReadOnly = true;
            dgvBitacora.AllowUserToAddRows = false;
            dgvBitacora.AllowUserToDeleteRows = false;
            dgvBitacora.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBitacora.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            cboFiltroAccion.SelectedIndexChanged += FiltroCambiado;
            dtpFiltroFecha.ValueChanged += FiltroCambiado;
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            if (usuarioActual == null)
            {
                return;
            }

            try
            {
                AutorizacionService.ExigirAdministrador(usuarioActual);
                CargarBitacora();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void FiltroCambiado(object sender, EventArgs e)
        {
            if (usuarioActual == null || IsHandleCreated == false)
            {
                return;
            }

            CargarBitacora();
        }

        private void CargarBitacora()
        {
            try
            {
                string accion = null;

                if (cboFiltroAccion.SelectedIndex > 0)
                {
                    accion = cboFiltroAccion.Text;
                }

                DateTime? fecha = null;

                if (dtpFiltroFecha.Checked)
                {
                    fecha = dtpFiltroFecha.Value.Date;
                }

                dgvBitacora.DataSource = bitacoraService.Filtrar(usuarioActual, accion, fecha);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
