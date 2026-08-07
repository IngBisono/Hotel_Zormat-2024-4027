using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmFactura : Form
    {
        private Factura facturaActual;

        // Membrete del recibo. No procede del diseñador.
        private PictureBox picLogoFactura;

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
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Factura");
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            Size = new Size(600, 660);
            MinimumSize = new Size(580, 620);

            ConfigurarDistribucion();

            lblNumeroNCF.Font = TemaVisual.Fuentes.TituloSeccion;
            lblNumeroNCF.BackColor = TemaVisual.Colores.AzulProfundo;
            lblNumeroNCF.ForeColor = TemaVisual.Colores.DoradoTexto;
            lblNumeroNCF.TextAlign = ContentAlignment.MiddleCenter;

            ConfigurarMonto(lblSubtotal, false);
            ConfigurarMonto(lblITBIS, false);
            ConfigurarMonto(lblPropina, false);
            ConfigurarMonto(lblTotal, true);

            TemaVisual.EstilizarBoton(
                btnCerrar,
                TemaVisual.Colores.ArenaDorada,
                TemaVisual.Colores.AzulProfundo,
                TemaVisual.Colores.SolDurazno);
            TemaVisual.PonerGlifo(
                btnCerrar,
                TemaVisual.Glifos.Cancelar,
                "Cerrar");
            btnCerrar.Click += btnCerrar_Click;

            MostrarFactura();
        }

        private void ConfigurarDistribucion()
        {
            Label conceptoSubtotal = CrearEtiquetaConcepto("Subtotal");
            Label conceptoITBIS = CrearEtiquetaConcepto("ITBIS (18%)");
            Label conceptoPropina = CrearEtiquetaConcepto("Propina (10%)");
            Label conceptoTotal = CrearEtiquetaConcepto("TOTAL");
            conceptoTotal.Font = new Font("Trebuchet MS", 11, FontStyle.Bold);

            // Las filas de conceptos se alternan en tono, como en un recibo
            // impreso, y el total se destaca con el cálido del atardecer.
            PintarFila(conceptoSubtotal, lblSubtotal, TemaVisual.Colores.Blanco);
            PintarFila(conceptoITBIS, lblITBIS, TemaVisual.Colores.FondoClaro);
            PintarFila(conceptoPropina, lblPropina, TemaVisual.Colores.Blanco);
            PintarFila(conceptoTotal, lblTotal, TemaVisual.Colores.SolAmarillo);

            Panel membrete = CrearMembrete();

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 55F));
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 45F));
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 52F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 44F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 44F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 44F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 58F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 70F));
            tableLayoutPanel1.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 18F));
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Padding = new Padding(0);

            lblNumeroNCF.AutoSize = false;
            lblNumeroNCF.Dock = DockStyle.Fill;
            lblNumeroNCF.Margin = new Padding(0);

            tableLayoutPanel1.Controls.Add(membrete, 0, 0);
            tableLayoutPanel1.SetColumnSpan(membrete, 2);
            tableLayoutPanel1.Controls.Add(lblNumeroNCF, 0, 1);
            tableLayoutPanel1.SetColumnSpan(lblNumeroNCF, 2);
            tableLayoutPanel1.Controls.Add(conceptoSubtotal, 0, 2);
            tableLayoutPanel1.Controls.Add(lblSubtotal, 1, 2);
            tableLayoutPanel1.Controls.Add(conceptoITBIS, 0, 3);
            tableLayoutPanel1.Controls.Add(lblITBIS, 1, 3);
            tableLayoutPanel1.Controls.Add(conceptoPropina, 0, 4);
            tableLayoutPanel1.Controls.Add(lblPropina, 1, 4);
            tableLayoutPanel1.Controls.Add(conceptoTotal, 0, 5);
            tableLayoutPanel1.Controls.Add(lblTotal, 1, 5);
            tableLayoutPanel1.Controls.Add(btnCerrar, 0, 6);
            tableLayoutPanel1.SetColumnSpan(btnCerrar, 2);

            btnCerrar.Anchor = AnchorStyles.None;
            btnCerrar.Size = new Size(150, 38);

            panel1.Controls.Clear();
            panel1.BackColor = TemaVisual.Colores.Blanco;
            panel1.BorderStyle = BorderStyle.None;
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Margin = new Padding(0);

            // Se recorta la región en lugar de pintar el fondo: así los
            // controles hijos, que cubren todo el panel, también quedan
            // recortados por la curva de las esquinas.
            TemaVisual.AplicarEsquinasRedondeadas(panel1, 14);

            TableLayoutPanel contenedorPrincipal = new TableLayoutPanel();
            contenedorPrincipal.BackColor = TemaVisual.Colores.FondoClaro;
            contenedorPrincipal.ColumnCount = 3;
            contenedorPrincipal.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));
            contenedorPrincipal.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 470F));
            contenedorPrincipal.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));
            contenedorPrincipal.Dock = DockStyle.Fill;
            contenedorPrincipal.Padding = new Padding(20);

            // El recibo se centra vertical y horizontalmente en una altura
            // fija; sin esto se estiraría y dejaría un gran vacío en blanco.
            contenedorPrincipal.RowCount = 3;
            contenedorPrincipal.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));
            contenedorPrincipal.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 480F));
            contenedorPrincipal.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));
            contenedorPrincipal.Controls.Add(panel1, 1, 1);

            flpFactura.Visible = false;
            Controls.Add(contenedorPrincipal);
            contenedorPrincipal.BringToFront();
        }

        // Membrete del recibo con el logotipo del hotel.
        private Panel CrearMembrete()
        {
            Panel membrete = new Panel();
            membrete.Dock = DockStyle.Fill;
            membrete.Margin = new Padding(0);
            membrete.BackColor = TemaVisual.Colores.Blanco;

            picLogoFactura = new PictureBox();
            picLogoFactura.Image = TemaVisual.ObtenerLogo();
            picLogoFactura.SizeMode = PictureBoxSizeMode.Zoom;
            picLogoFactura.BackColor = Color.Transparent;
            picLogoFactura.Dock = DockStyle.Fill;
            picLogoFactura.Margin = new Padding(0);

            membrete.Padding = new Padding(20, 14, 20, 8);
            membrete.Controls.Add(picLogoFactura);

            // Línea de olas bajo el logotipo, como en la portada de acceso.
            membrete.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                Rectangle area = new Rectangle(
                    24,
                    lienzo.Height - 12,
                    lienzo.Width - 48,
                    10);
                TemaVisual.PintarOlas(e.Graphics, area);
            };

            return membrete;
        }

        // Aplica el mismo fondo al concepto y a su importe.
        //
        // Se anulan los márgenes porque el margen por omisión de las etiquetas
        // dejaría una franja sin pintar entre ambas columnas y la banda de
        // color de la fila se vería partida por la mitad.
        private static void PintarFila(Label concepto, Label monto, Color fondo)
        {
            concepto.BackColor = fondo;
            concepto.Margin = new Padding(0);
            monto.BackColor = fondo;
            monto.Margin = new Padding(0);
        }

        private Label CrearEtiquetaConcepto(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.Font = new Font("Trebuchet MS", 10, FontStyle.Regular);
            etiqueta.ForeColor = TemaVisual.Colores.Texto;
            etiqueta.Padding = new Padding(22, 0, 0, 0);
            etiqueta.Text = texto;
            etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            return etiqueta;
        }

        private void ConfigurarMonto(Label etiqueta, bool esTotal)
        {
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.Padding = new Padding(0, 0, 22, 0);
            etiqueta.TextAlign = ContentAlignment.MiddleRight;

            if (esTotal)
            {
                etiqueta.Font = TemaVisual.Fuentes.MontoTotal;
                etiqueta.ForeColor = TemaVisual.Colores.AzulProfundo;
            }
            else
            {
                etiqueta.Font = TemaVisual.Fuentes.Monto;
                etiqueta.ForeColor = TemaVisual.Colores.Texto;
            }
        }

        private void MostrarFactura()
        {
            if (facturaActual == null)
            {
                lblNumeroNCF.Text = "NCF: pendiente";
                lblSubtotal.Text = "RD$ 0.00";
                lblITBIS.Text = "RD$ 0.00";
                lblPropina.Text = "RD$ 0.00";
                lblTotal.Text = "RD$ 0.00";
                return;
            }

            lblNumeroNCF.Text = "NCF: " + facturaActual.NumeroNCF;
            lblSubtotal.Text = "RD$ " +
                facturaActual.Subtotal.ToString("N2");
            lblITBIS.Text = "RD$ " +
                facturaActual.ITBIS.ToString("N2");
            lblPropina.Text = "RD$ " +
                facturaActual.Propina.ToString("N2");
            lblTotal.Text = "RD$ " +
                facturaActual.Total.ToString("N2");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
