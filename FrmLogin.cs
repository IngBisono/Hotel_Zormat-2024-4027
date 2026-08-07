using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;

        // Controles decorativos propios de la nueva portada. No están en el
        // diseñador: se crean aquí para no alterar FrmLogin.Designer.cs.
        private PictureBox picEmblema;
        private Label lblTagline;
        private Panel pnlOlas;
        private Panel marcoUsuario;
        private Panel marcoContrasena;

        public Usuario UsuarioAutenticado { get; private set; }

        public FrmLogin()
        {
            InitializeComponent();

            _usuarioService = new UsuarioService();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Iniciar sesión");

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(540, 586);

            ConstruirPortada();
            ConstruirTarjeta();

            AcceptButton = btnIngresar;

            btnIngresar.Click += btnIngresar_Click;
            chkMostrarContrasena.CheckedChanged +=
                chkMostrarContrasena_CheckedChanged;
        }

        // Mitad superior: cielo azul del atardecer con el emblema del logo,
        // el nombre del hotel y una línea de olas que da paso a la tarjeta.
        private void ConstruirPortada()
        {
            SuspendLayout();

            // La franja decorativa del diseñador se reaprovecha como portada.
            pnlFranjaSuperior.Dock = DockStyle.None;
            pnlFranjaSuperior.Location = new Point(0, 0);
            pnlFranjaSuperior.Size = new Size(ClientSize.Width, 210);
            pnlFranjaSuperior.BackColor = TemaVisual.Colores.AzulProfundo;
            pnlFranjaSuperior.Paint += PintarPortada;

            picEmblema = new PictureBox();
            picEmblema.Image = TemaVisual.ObtenerEmblema();
            picEmblema.SizeMode = PictureBoxSizeMode.Zoom;
            picEmblema.BackColor = Color.Transparent;

            lblTagline = new Label();
            lblTagline.Text = "Tu escape junto al mar";
            lblTagline.Font = TemaVisual.Fuentes.Script;
            lblTagline.ForeColor = TemaVisual.Colores.SolDurazno;
            lblTagline.BackColor = Color.Transparent;
            lblTagline.AutoSize = false;
            lblTagline.TextAlign = ContentAlignment.MiddleCenter;

            lblTituloLogin.Text = "HOTEL BISONO";
            lblTituloLogin.Font = TemaVisual.Fuentes.TituloHero;
            lblTituloLogin.ForeColor = TemaVisual.Colores.DoradoTexto;
            lblTituloLogin.BackColor = Color.Transparent;
            lblTituloLogin.AutoSize = false;
            lblTituloLogin.TextAlign = ContentAlignment.MiddleCenter;

            // Los controles se reparientan primero y se colocan después: dentro
            // del TableLayoutPanel original, su motor de posicionamiento
            // descartaría cualquier Location asignada de antemano.
            pnlFranjaSuperior.Controls.Add(picEmblema);
            pnlFranjaSuperior.Controls.Add(lblTituloLogin);
            pnlFranjaSuperior.Controls.Add(lblTagline);

            lblTituloLogin.Dock = DockStyle.None;
            lblTagline.Dock = DockStyle.None;

            picEmblema.Size = new Size(228, 104);
            picEmblema.Location = new Point((ClientSize.Width - 228) / 2, 12);
            lblTituloLogin.Size = new Size(ClientSize.Width, 42);
            lblTituloLogin.Location = new Point(0, 122);
            lblTagline.Size = new Size(ClientSize.Width, 32);
            lblTagline.Location = new Point(0, 166);

            // Franja de olas: enlaza el cielo con el cuerpo claro del formulario.
            pnlOlas = new Panel();
            pnlOlas.Location = new Point(0, 210);
            pnlOlas.Size = new Size(ClientSize.Width, 38);
            pnlOlas.BackColor = TemaVisual.Colores.FondoClaro;
            pnlOlas.Paint += delegate (object remitente, PaintEventArgs e)
            {
                TemaVisual.PintarOlas(e.Graphics, ((Control)remitente).ClientRectangle);
            };

            Controls.Add(pnlOlas);

            ResumeLayout();
        }

        private void PintarPortada(object remitente, PaintEventArgs e)
        {
            Control lienzo = (Control)remitente;
            Rectangle area = lienzo.ClientRectangle;
            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            TemaVisual.PintarAzulProfundo(
                e.Graphics,
                area,
                LinearGradientMode.ForwardDiagonal);

            // Remate cálido en el borde inferior, como el sol del logotipo.
            Rectangle franja = new Rectangle(0, area.Height - 4, area.Width, 4);
            TemaVisual.PintarAtardecer(e.Graphics, franja);
        }

        // Mitad inferior: tarjeta blanca con el formulario de acceso.
        private void ConstruirTarjeta()
        {
            tableLayoutPanel1.SuspendLayout();

            tableLayoutPanel1.Dock = DockStyle.None;
            tableLayoutPanel1.Location = new Point(40, 266);
            tableLayoutPanel1.Size = new Size(460, 292);
            tableLayoutPanel1.BackColor = TemaVisual.Colores.Blanco;
            tableLayoutPanel1.Padding = new Padding(34, 22, 34, 18);

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 7;

            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 48F));

            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblSubtituloLogin.Text = "INICIAR SESIÓN";
            lblSubtituloLogin.Font = TemaVisual.Fuentes.TituloForm;
            lblSubtituloLogin.ForeColor = TemaVisual.Colores.AzulMarino;
            lblSubtituloLogin.BackColor = Color.Transparent;
            lblSubtituloLogin.AutoSize = false;
            lblSubtituloLogin.Dock = DockStyle.Fill;
            lblSubtituloLogin.TextAlign = ContentAlignment.MiddleLeft;

            PrepararEtiqueta(lblUsuarioPrompt, "Usuario");
            PrepararEtiqueta(lblContrasenaPrompt, "Contraseña");

            marcoUsuario = TemaVisual.EnvolverCampo(
                txtUsuario,
                TemaVisual.Colores.Blanco);
            marcoUsuario.Dock = DockStyle.Fill;
            marcoUsuario.Margin = new Padding(0, 0, 0, 4);

            txtContrasena.UseSystemPasswordChar = true;
            marcoContrasena = TemaVisual.EnvolverCampo(
                txtContrasena,
                TemaVisual.Colores.Blanco);
            marcoContrasena.Dock = DockStyle.Fill;
            marcoContrasena.Margin = new Padding(0, 0, 0, 4);

            PrepararBotonOjo();

            btnIngresar.Text = "Ingresar";
            btnIngresar.Dock = DockStyle.Fill;
            btnIngresar.Margin = new Padding(0, 10, 0, 0);
            btnIngresar.Font = TemaVisual.Fuentes.CuerpoNegrita;
            TemaVisual.EstilizarBoton(
                btnIngresar,
                TemaVisual.Colores.ArenaDorada,
                TemaVisual.Colores.AzulProfundo,
                TemaVisual.Colores.SolDurazno);

            PrepararMensajeError();

            tableLayoutPanel1.Controls.Add(lblSubtituloLogin, 0, 0);
            tableLayoutPanel1.SetColumnSpan(lblSubtituloLogin, 2);
            tableLayoutPanel1.Controls.Add(lblUsuarioPrompt, 0, 1);
            tableLayoutPanel1.SetColumnSpan(lblUsuarioPrompt, 2);
            tableLayoutPanel1.Controls.Add(marcoUsuario, 0, 2);
            tableLayoutPanel1.SetColumnSpan(marcoUsuario, 2);
            tableLayoutPanel1.Controls.Add(lblContrasenaPrompt, 0, 3);
            tableLayoutPanel1.SetColumnSpan(lblContrasenaPrompt, 2);
            tableLayoutPanel1.Controls.Add(marcoContrasena, 0, 4);
            tableLayoutPanel1.Controls.Add(chkMostrarContrasena, 1, 4);
            tableLayoutPanel1.Controls.Add(btnIngresar, 0, 5);
            tableLayoutPanel1.SetColumnSpan(btnIngresar, 2);
            tableLayoutPanel1.Controls.Add(lblMensajeError, 0, 6);
            tableLayoutPanel1.SetColumnSpan(lblMensajeError, 2);

            // El panel de flujo del diseñador ya no participa en la
            // composición; se oculta en lugar de tocar el archivo generado.
            flpAcceso.Visible = false;

            txtUsuario.TabIndex = 0;
            txtContrasena.TabIndex = 1;
            chkMostrarContrasena.TabIndex = 2;
            btnIngresar.TabIndex = 3;

            TemaVisual.AplicarEsquinasRedondeadas(tableLayoutPanel1, 14);

            tableLayoutPanel1.ResumeLayout();
            tableLayoutPanel1.BringToFront();
        }

        private void PrepararEtiqueta(Label etiqueta, string texto)
        {
            etiqueta.Text = texto;
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.TextoSuave;
            etiqueta.BackColor = Color.Transparent;
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.BottomLeft;
        }

        // La casilla de "mostrar contraseña" se presenta como un botón con
        // forma de ojo, junto al campo. Conserva su nombre y su evento.
        private void PrepararBotonOjo()
        {
            chkMostrarContrasena.Appearance = Appearance.Button;
            chkMostrarContrasena.Text = TemaVisual.Glifos.VerClave;
            chkMostrarContrasena.Font = TemaVisual.Fuentes.Icono(11F);
            chkMostrarContrasena.ForeColor = TemaVisual.Colores.AzulPrimario;
            chkMostrarContrasena.BackColor = TemaVisual.Colores.Blanco;
            chkMostrarContrasena.FlatStyle = FlatStyle.Flat;
            chkMostrarContrasena.FlatAppearance.BorderColor =
                TemaVisual.Colores.BordeSutil;
            chkMostrarContrasena.FlatAppearance.BorderSize = 1;
            chkMostrarContrasena.TextAlign = ContentAlignment.MiddleCenter;
            chkMostrarContrasena.AutoSize = false;
            chkMostrarContrasena.Dock = DockStyle.Fill;
            chkMostrarContrasena.Margin = new Padding(6, 0, 0, 4);
            chkMostrarContrasena.Cursor = Cursors.Hand;
        }

        private void PrepararMensajeError()
        {
            lblMensajeError.Text = "";
            lblMensajeError.Font = TemaVisual.Fuentes.Cuerpo;
            lblMensajeError.ForeColor = TemaVisual.Colores.Rojo;
            lblMensajeError.BackColor = TemaVisual.Colores.RojoSuave;
            lblMensajeError.AutoSize = false;
            lblMensajeError.Dock = DockStyle.Fill;
            lblMensajeError.Margin = new Padding(0, 8, 0, 0);
            lblMensajeError.TextAlign = ContentAlignment.MiddleLeft;
            lblMensajeError.Visible = false;
            TemaVisual.PonerGlifoEtiqueta(
                lblMensajeError,
                TemaVisual.Glifos.Alerta,
                TemaVisual.Colores.Rojo);
            TemaVisual.AplicarEsquinasRedondeadas(lblMensajeError, 8);
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                btnIngresar.Enabled = false;

                string nombreUsuario = txtUsuario.Text.Trim();
                string contrasena = txtContrasena.Text;

                if (nombreUsuario == "" || contrasena == "")
                {
                    MostrarError("Usuario o contraseña incorrectos");
                    return;
                }

                Usuario usuario = _usuarioService.Autenticar(
                    nombreUsuario,
                    contrasena);

                if (usuario == null)
                {
                    MostrarError("Usuario o contraseña incorrectos");
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                    return;
                }

                UsuarioAutenticado = usuario;
                lblMensajeError.Visible = false;
                DialogResult = DialogResult.OK;
            }
            catch (FormatException error)
            {
                MostrarError(error.Message);
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo conectar con la base de datos. " +
                    "Verifique la conexión e intente de nuevo.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al iniciar sesión. Intente de nuevo.");
            }
            finally
            {
                btnIngresar.Enabled = true;
            }
        }

        private void chkMostrarContrasena_CheckedChanged(
            object sender,
            EventArgs e)
        {
            txtContrasena.UseSystemPasswordChar =
                chkMostrarContrasena.Checked == false;

            chkMostrarContrasena.Text = chkMostrarContrasena.Checked
                ? TemaVisual.Glifos.OcultarClave
                : TemaVisual.Glifos.VerClave;
        }

        private void MostrarError(string mensaje)
        {
            lblMensajeError.Text = "  " + mensaje;
            lblMensajeError.Visible = true;
        }
    }
}
