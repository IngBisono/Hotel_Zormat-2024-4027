using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;

        public FrmLogin()
        {
            InitializeComponent();

            _usuarioService = new UsuarioService();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            Text = "Hotel Zormat - Iniciar sesión";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = ColorTranslator.FromHtml("#F5F8FB");

            tableLayoutPanel1.BackColor = ColorTranslator.FromHtml("#F5F8FB");

            lblTituloLogin.Text = "HOTEL ZORMAT";
            lblTituloLogin.Font = new Font("Georgia", 22, FontStyle.Bold);
            lblTituloLogin.ForeColor = ColorTranslator.FromHtml("#025897");

            lblSubtituloLogin.Text = "INICIAR SESIÓN";
            lblSubtituloLogin.Font = new Font("Georgia", 16, FontStyle.Bold);
            lblSubtituloLogin.ForeColor = ColorTranslator.FromHtml("#133958");

            lblUsuarioPrompt.Text = "Usuario";
            lblContrasenaPrompt.Text = "Contraseña";
            chkMostrarContrasena.Text = "Mostrar contraseña";

            txtContrasena.UseSystemPasswordChar = true;

            btnIngresar.Text = "Ingresar";
            btnIngresar.BackColor = ColorTranslator.FromHtml("#F5B664");
            btnIngresar.ForeColor = ColorTranslator.FromHtml("#133958");
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.UseVisualStyleBackColor = false;

            lblMensajeError.Text = "";
            lblMensajeError.ForeColor = ColorTranslator.FromHtml("#D6483F");
            lblMensajeError.Visible = false;

            AcceptButton = btnIngresar;

            btnIngresar.Click += btnIngresar_Click;
            chkMostrarContrasena.CheckedChanged +=
                chkMostrarContrasena_CheckedChanged;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (nombreUsuario == "" || contrasena == "")
            {
                MostrarError("Debe ingresar el usuario y la contraseña.");
                return;
            }

            try
            {
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

                lblMensajeError.Visible = false;
                Hide();

                using (FrmDashboardHabitaciones dashboard =
                    new FrmDashboardHabitaciones(usuario))
                {
                    dashboard.ShowDialog();
                }

                Show();
                txtContrasena.Clear();
                txtUsuario.Focus();
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
        }

        private void chkMostrarContrasena_CheckedChanged(
            object sender,
            EventArgs e)
        {
            txtContrasena.UseSystemPasswordChar =
                chkMostrarContrasena.Checked == false;
        }

        private void MostrarError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
        }
    }
}
