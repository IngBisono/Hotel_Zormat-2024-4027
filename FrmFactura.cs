using HotelZormat.Modelo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmFactura : Form
    {
        private Factura facturaActual;

        public FrmFactura()
        {
            InitializeComponent();
            ConfigurarPantalla();
        }

        public FrmFactura(Factura factura) : this()
        {
            facturaActual = factura;
            MostrarFactura();
        }

        private void ConfigurarPantalla()
        {
            Text = "Factura";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 248, 251);

            flpFactura.BackColor = Color.White;
            flpFactura.Padding = new Padding(24);

            lblNumeroNCF.Font = new Font("Georgia", 14, FontStyle.Bold);
            lblTotal.Font = new Font("Consolas", 16, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(2, 88, 151);

            btnCerrar.Text = "Cerrar";
            btnCerrar.BackColor = Color.FromArgb(245, 182, 100);
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Click += btnCerrar_Click;

            MostrarFactura();
        }

        private void MostrarFactura()
        {
            if (facturaActual == null)
            {
                lblNumeroNCF.Text = "NCF: pendiente";
                lblSubtotal.Text = "Subtotal: RD$ 0.00";
                lblITBIS.Text = "ITBIS (18%): RD$ 0.00";
                lblPropina.Text = "Propina (10%): RD$ 0.00";
                lblTotal.Text = "TOTAL: RD$ 0.00";
                return;
            }

            lblNumeroNCF.Text = "NCF: " + facturaActual.NumeroNCF;
            lblSubtotal.Text = "Subtotal: RD$ " + facturaActual.Subtotal.ToString("N2");
            lblITBIS.Text = "ITBIS (18%): RD$ " + facturaActual.ITBIS.ToString("N2");
            lblPropina.Text = "Propina (10%): RD$ " + facturaActual.Propina.ToString("N2");
            lblTotal.Text = "TOTAL: RD$ " + facturaActual.Total.ToString("N2");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
