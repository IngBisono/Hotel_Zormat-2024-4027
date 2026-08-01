using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmHuespedes : Form
    {
        private readonly HuespedService _huespedService;
        private Usuario _usuarioActual;
        private string _documentoSeleccionado;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmHuespedes()
        {
            InitializeComponent();
            _huespedService = new HuespedService();
            ConfigurarFormulario();
        }

        // Este constructor recibe al usuario que inicio sesion.
        public FrmHuespedes(Usuario usuarioActual) : this()
        {
            _usuarioActual = usuarioActual;

            if (_usuarioActual != null)
            {
                Text = "Huespedes - " + _usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            Text = "Gestion de huespedes";
            BackColor = Color.FromArgb(245, 248, 251);

            cboTipoDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoDocumento.Items.Clear();
            cboTipoDocumento.Items.Add("Cedula");
            cboTipoDocumento.Items.Add("Pasaporte");
            cboTipoDocumento.SelectedIndex = 0;

            btnBuscar.Text = "Buscar";
            btnGuardar.Text = "Guardar";
            btnNuevo.Text = "Nuevo";
            btnEliminar.Text = "Eliminar";

            AplicarColorPrincipal(btnBuscar);
            AplicarColorPrincipal(btnGuardar);
            AplicarColorSecundario(btnNuevo);
            btnEliminar.BackColor = Color.FromArgb(214, 72, 63);
            btnEliminar.ForeColor = Color.White;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Enabled = false;

            ConfigurarGrid(dgvHuespedes);
            ConfigurarGrid(dgvHistorialEstadias);

            Load += FrmHuespedes_Load;
            btnBuscar.Click += btnBuscar_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnNuevo.Click += btnNuevo_Click;
            btnEliminar.Click += btnEliminar_Click;
            dgvHuespedes.CellClick += dgvHuespedes_CellClick;
            txtBuscar.KeyDown += txtBuscar_KeyDown;
        }

        // Carga los huespedes cuando se abre la ventana.
        private void FrmHuespedes_Load(object sender, EventArgs e)
        {
            try
            {
                CargarHuespedes();
                PrepararNuevoHuesped();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Busca por documento, nombre o apellido.
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string criterio = txtBuscar.Text.Trim();

                if (criterio == "")
                {
                    CargarHuespedes();
                }
                else
                {
                    dgvHuespedes.DataSource = null;
                    dgvHuespedes.DataSource = _huespedService.Buscar(criterio);
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Permite buscar al presionar Enter.
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscar.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        // Guarda un huesped nuevo o actualiza el seleccionado.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Huesped huesped = CrearHuespedDesdeFormulario();

                if (_documentoSeleccionado == null)
                {
                    _huespedService.Crear(huesped);
                }
                else
                {
                    _huespedService.Actualizar(huesped);
                }

                MessageBox.Show(
                    "Los datos del huesped fueron guardados.",
                    "Hotel Zormat",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarHuespedes();
                PrepararNuevoHuesped();
            }
            catch (FormatException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se pudieron guardar los datos en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Limpia los campos para registrar otro huesped.
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararNuevoHuesped();
        }

        // Elimina el huesped seleccionado despues de pedir confirmacion.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_documentoSeleccionado == null)
            {
                MostrarAdvertencia("Seleccione un huesped para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "Desea eliminar el huesped seleccionado?",
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _huespedService.Eliminar(_documentoSeleccionado);
                CargarHuespedes();
                PrepararNuevoHuesped();

                MessageBox.Show(
                    "El huesped fue eliminado.",
                    "Hotel Zormat",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (SqlException)
            {
                MostrarAdvertencia(
                    "No se puede eliminar porque el huesped tiene reservas registradas.");
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Muestra en los campos el huesped seleccionado.
        private void dgvHuespedes_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            Huesped huesped =
                dgvHuespedes.Rows[e.RowIndex].DataBoundItem as Huesped;

            if (huesped == null)
            {
                return;
            }

            _documentoSeleccionado = huesped.NumeroDocumento;
            cboTipoDocumento.Text = huesped.TipoDocumento;
            txtNumeroDocumento.Text = huesped.NumeroDocumento;
            txtNumeroDocumento.ReadOnly = true;
            txtNombre.Text = huesped.Nombre;
            txtApellido.Text = huesped.Apellido;
            txtTelefono.Text = huesped.Telefono;
            txtEmail.Text = huesped.Email;
            btnEliminar.Enabled = true;

            CargarHistorial(huesped.NumeroDocumento);
        }

        // Convierte los campos en un objeto Huesped.
        private Huesped CrearHuespedDesdeFormulario()
        {
            string tipoDocumento = cboTipoDocumento.Text;
            string numeroDocumento = txtNumeroDocumento.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();

            if (tipoDocumento == "Cedula")
            {
                numeroDocumento = numeroDocumento.Replace("-", "");
                numeroDocumento = numeroDocumento.Replace(" ", "");
            }

            if (TextoEsNombreValido(nombre) == false)
            {
                throw new FormatException(
                    "El nombre solo puede contener letras, espacios, guion o apostrofe.");
            }

            if (TextoEsNombreValido(apellido) == false)
            {
                throw new FormatException(
                    "El apellido solo puede contener letras, espacios, guion o apostrofe.");
            }

            if (tipoDocumento == "Pasaporte")
            {
                ValidarPasaporte(numeroDocumento);
            }

            ValidarTelefono(txtTelefono.Text.Trim());

            Huesped huesped = new Huesped();
            huesped.TipoDocumento = tipoDocumento;
            huesped.NumeroDocumento = numeroDocumento;
            huesped.Nombre = nombre;
            huesped.Apellido = apellido;
            huesped.Telefono = txtTelefono.Text.Trim();
            huesped.Email = txtEmail.Text.Trim();

            return huesped;
        }

        // Carga todos los huespedes en la tabla.
        private void CargarHuespedes()
        {
            List<Huesped> huespedes = _huespedService.ObtenerTodos();
            dgvHuespedes.DataSource = null;
            dgvHuespedes.DataSource = huespedes;
        }

        // Carga el historial del huesped seleccionado.
        private void CargarHistorial(string numeroDocumento)
        {
            try
            {
                dgvHistorialEstadias.DataSource = null;
                dgvHistorialEstadias.DataSource =
                    _huespedService.ObtenerHistorialEstadias(numeroDocumento);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Deja el formulario listo para un registro nuevo.
        private void PrepararNuevoHuesped()
        {
            _documentoSeleccionado = null;
            cboTipoDocumento.SelectedIndex = 0;
            txtNumeroDocumento.ReadOnly = false;
            txtNumeroDocumento.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            dgvHistorialEstadias.DataSource = null;
            btnEliminar.Enabled = false;
            txtNumeroDocumento.Focus();
        }

        // Comprueba los caracteres permitidos en nombres y apellidos.
        private static bool TextoEsNombreValido(string texto)
        {
            if (texto == "")
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                bool permitido = char.IsLetter(caracter);

                if (caracter == ' ' || caracter == '-' || caracter == '\'')
                {
                    permitido = true;
                }

                if (permitido == false)
                {
                    return false;
                }
            }

            return true;
        }

        // Comprueba que el pasaporte tenga caracteres sencillos.
        private static void ValidarPasaporte(string pasaporte)
        {
            if (pasaporte == "")
            {
                throw new FormatException("El pasaporte es obligatorio.");
            }

            foreach (char caracter in pasaporte)
            {
                bool permitido = char.IsLetterOrDigit(caracter);

                if (caracter == '-')
                {
                    permitido = true;
                }

                if (permitido == false)
                {
                    throw new FormatException(
                        "El pasaporte solo puede contener letras, numeros y guion.");
                }
            }
        }

        // Comprueba los caracteres del telefono cuando fue escrito.
        private static void ValidarTelefono(string telefono)
        {
            if (telefono == "")
            {
                return;
            }

            foreach (char caracter in telefono)
            {
                bool permitido = char.IsDigit(caracter);

                if (caracter == '+' || caracter == '-' || caracter == ' ' ||
                    caracter == '(' || caracter == ')')
                {
                    permitido = true;
                }

                if (permitido == false)
                {
                    throw new FormatException(
                        "El telefono contiene caracteres no permitidos.");
                }
            }
        }

        // Configura una tabla para mostrar datos sin permitir cambios directos.
        private static void ConfigurarGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
        }

        // Aplica el color principal a un boton.
        private static void AplicarColorPrincipal(Button boton)
        {
            boton.BackColor = Color.FromArgb(2, 88, 151);
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
        }

        // Aplica el color secundario a un boton.
        private static void AplicarColorSecundario(Button boton)
        {
            boton.BackColor = Color.FromArgb(245, 182, 100);
            boton.ForeColor = Color.FromArgb(46, 66, 86);
            boton.FlatStyle = FlatStyle.Flat;
        }

        // Muestra un mensaje que el usuario puede corregir.
        private static void MostrarAdvertencia(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Revise los datos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // Muestra un error no esperado.
        private static void MostrarError(Exception error)
        {
            string mensaje = error.Message;

            if (error is SqlException)
            {
                mensaje = "No fue posible comunicarse con la base de datos.";
            }

            MessageBox.Show(
                mensaje,
                "Hotel Zormat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
