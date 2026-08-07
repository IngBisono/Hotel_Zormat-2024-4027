using Hotel_Zormat.Estilos;
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
                Text = "Hotel Bisono - Huéspedes - " +
                    _usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Huéspedes");

            ConfigurarDistribucion();

            cboTipoDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
            TemaVisual.EstilizarCombo(cboTipoDocumento);
            cboTipoDocumento.Items.Clear();

            string[] tiposDocumento = { "Cedula", "Pasaporte" };

            foreach (string tipoDocumento in tiposDocumento)
            {
                cboTipoDocumento.Items.Add(tipoDocumento);
            }

            cboTipoDocumento.SelectedIndex = 0;
            ConfigurarFormatoDocumento();

            AplicarColorPrincipal(btnBuscar, "Buscar", TemaVisual.Glifos.Buscar);
            AplicarColorPrincipal(btnGuardar, "Guardar", TemaVisual.Glifos.Guardar);
            AplicarColorSecundario(btnNuevo, "Nuevo", TemaVisual.Glifos.Nuevo);

            TemaVisual.EstilizarBoton(
                btnEliminar,
                TemaVisual.Colores.Rojo,
                TemaVisual.Colores.Blanco,
                Color.FromArgb(180, 56, 48));
            TemaVisual.PonerGlifo(
                btnEliminar,
                TemaVisual.Glifos.Eliminar,
                "Eliminar");
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
            cboTipoDocumento.SelectedIndexChanged +=
                cboTipoDocumento_SelectedIndexChanged;
        }

        // Organiza la lista y las pestanas de datos e historial.
        private void ConfigurarDistribucion()
        {
            Size = new Size(1140, 680);
            MinimumSize = new Size(960, 600);

            splitContainer1.Parent = this;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Orientation = Orientation.Vertical;
            splitContainer1.SplitterDistance = 540;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.Panel1MinSize = 380;
            splitContainer1.Panel2MinSize = 380;
            splitContainer1.Panel1.Padding = new Padding(16, 12, 8, 16);
            splitContainer1.Panel2.Padding = new Padding(8, 12, 16, 16);
            splitContainer1.BackColor = TemaVisual.Colores.FondoClaro;
            splitContainer1.Panel1.BackColor = TemaVisual.Colores.FondoClaro;
            splitContainer1.Panel2.BackColor = TemaVisual.Colores.FondoClaro;

            // El control acoplado a Fill debe quedar al frente: el
            // acoplamiento se resuelve del último control al primero.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Gestión de Huéspedes",
                TemaVisual.Glifos.Huesped);
            Controls.Add(encabezado);
            splitContainer1.BringToFront();

            flpHuespedes.Controls.Clear();
            flpHuespedes.Parent = splitContainer1.Panel1;
            flpHuespedes.Dock = DockStyle.Top;
            flpHuespedes.Height = 62;
            flpHuespedes.AutoScroll = false;
            flpHuespedes.FlowDirection = FlowDirection.LeftToRight;
            flpHuespedes.WrapContents = false;
            flpHuespedes.Padding = new Padding(14, 13, 14, 10);
            flpHuespedes.BackColor = TemaVisual.Colores.FondoSecundario;

            Label lblBuscar = CrearEtiqueta("Buscar:");
            lblBuscar.Margin = new Padding(3, 11, 3, 3);

            Panel marcoBuscar = TemaVisual.EnvolverCampo(
                txtBuscar,
                TemaVisual.Colores.FondoSecundario);
            marcoBuscar.Size = new Size(300, 36);
            marcoBuscar.Margin = new Padding(4, 2, 10, 0);

            btnBuscar.Size = new Size(110, 36);
            btnBuscar.Margin = new Padding(0, 2, 0, 0);

            flpHuespedes.Controls.Add(lblBuscar);
            flpHuespedes.Controls.Add(marcoBuscar);
            flpHuespedes.Controls.Add(btnBuscar);

            dgvHuespedes.Parent = splitContainer1.Panel1;
            dgvHuespedes.Dock = DockStyle.Fill;
            dgvHuespedes.Margin = new Padding(0);
            flpHuespedes.SendToBack();
            dgvHuespedes.BringToFront();

            tabControl1.Parent = splitContainer1.Panel2;
            tabControl1.Dock = DockStyle.Fill;
            tabPage1.Text = "Datos";
            tabPage2.Text = "Historial";
            tabPage1.Padding = new Padding(12);
            tabPage2.Padding = new Padding(12);
            tabPage1.AutoScroll = true;
            tabPage1.BackColor = TemaVisual.Colores.Blanco;
            tabPage2.BackColor = TemaVisual.Colores.Blanco;
            TemaVisual.EstilizarTabControl(tabControl1);

            TableLayoutPanel tablaDatos = new TableLayoutPanel();
            tablaDatos.Dock = DockStyle.Fill;
            tablaDatos.Padding = new Padding(12, 10, 12, 10);
            tablaDatos.BackColor = TemaVisual.Colores.Blanco;
            tablaDatos.ColumnCount = 2;
            tablaDatos.RowCount = 7;
            tablaDatos.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tablaDatos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150F));
            tablaDatos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            // Filas más altas que en el diseño anterior: cada campo va ahora
            // dentro de un marco redondeado con relleno propio.
            for (int fila = 0; fila < 6; fila++)
            {
                tablaDatos.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 54F));
            }

            tablaDatos.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            AgregarCampo(tablaDatos, "Tipo de documento",
                cboTipoDocumento, 0);
            AgregarCampo(tablaDatos, "N.º de documento",
                txtNumeroDocumento, 1);
            AgregarCampo(tablaDatos, "Nombre", txtNombre, 2);
            AgregarCampo(tablaDatos, "Apellido", txtApellido, 3);
            AgregarCampo(tablaDatos, "Teléfono", txtTelefono, 4);
            AgregarCampo(tablaDatos, "Correo", txtEmail, 5);

            FlowLayoutPanel barraBotones = new FlowLayoutPanel();
            barraBotones.Dock = DockStyle.Fill;
            barraBotones.FlowDirection = FlowDirection.LeftToRight;
            barraBotones.WrapContents = true;
            barraBotones.BackColor = TemaVisual.Colores.Blanco;
            barraBotones.Padding = new Padding(0, 16, 0, 0);

            btnNuevo.Size = new Size(112, 36);
            btnGuardar.Size = new Size(112, 36);
            btnEliminar.Size = new Size(112, 36);
            btnNuevo.Margin = new Padding(0, 3, 8, 3);
            btnGuardar.Margin = new Padding(0, 3, 8, 3);
            btnEliminar.Margin = new Padding(0, 3, 0, 3);

            barraBotones.Controls.Add(btnNuevo);
            barraBotones.Controls.Add(btnGuardar);
            barraBotones.Controls.Add(btnEliminar);

            tablaDatos.Controls.Add(barraBotones, 0, 6);
            tablaDatos.SetColumnSpan(barraBotones, 2);
            tabPage1.Controls.Add(tablaDatos);

            dgvHistorialEstadias.Parent = tabPage2;
            dgvHistorialEstadias.Dock = DockStyle.Fill;
            dgvHistorialEstadias.Margin = new Padding(0);
        }

        // Agrega una etiqueta y su control en una fila.
        //
        // Las cajas de texto se envuelven en un marco redondeado; las listas
        // desplegables conservan su borde nativo, que WinForms no permite
        // personalizar sin repintar el control entero.
        private static void AgregarCampo(
            TableLayoutPanel tabla,
            string texto,
            Control control,
            int fila)
        {
            Label etiqueta = CrearEtiqueta(texto);
            tabla.Controls.Add(etiqueta, 0, fila);

            ComboBox lista = control as ComboBox;
            if (lista != null)
            {
                lista.Dock = DockStyle.Fill;
                lista.Margin = new Padding(3, 12, 3, 12);
                tabla.Controls.Add(lista, 1, fila);
                return;
            }

            Panel marco = TemaVisual.EnvolverCampo(
                control,
                TemaVisual.Colores.Blanco);
            marco.Dock = DockStyle.Fill;
            marco.Margin = new Padding(3, 7, 3, 7);
            tabla.Controls.Add(marco, 1, fila);
        }

        // Crea una etiqueta sencilla para los campos.
        private static Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Anchor = AnchorStyles.Left;
            etiqueta.Margin = new Padding(3, 8, 3, 3);
            etiqueta.Font = TemaVisual.Fuentes.Etiqueta;
            etiqueta.ForeColor = TemaVisual.Colores.Texto;
            return etiqueta;
        }

        // Cambia el formato segun el tipo de documento elegido.
        private void cboTipoDocumento_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ConfigurarFormatoDocumento();
        }

        // La cedula acepta once digitos y el pasaporte acepta texto libre.
        private void ConfigurarFormatoDocumento()
        {
            txtNumeroDocumento.Clear();

            if (cboTipoDocumento.Text == "Cedula")
            {
                txtNumeroDocumento.Mask = "00000000000";
                return;
            }

            txtNumeroDocumento.Mask = "";
        }

        // Carga los huespedes cuando se abre la ventana.
        private void FrmHuespedes_Load(object sender, EventArgs e)
        {
            try
            {
                CargarHuespedes();
                PrepararNuevoHuesped();
            }
            catch (SqlException)
            {
                MostrarError(
                    "No fue posible cargar los huespedes desde la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        // Busca por documento, nombre o apellido.
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            btnBuscar.Enabled = false;

            try
            {
                string criterio = txtBuscar.Text.Trim();

                PrepararNuevoHuesped();

                if (criterio == "")
                {
                    CargarHuespedes();
                }
                else
                {
                    dgvHuespedes.DataSource = null;
                    dgvHuespedes.DataSource = _huespedService.Buscar(criterio);
                    dgvHuespedes.ClearSelection();
                    dgvHuespedes.CurrentCell = null;
                }
            }
            catch (SqlException)
            {
                MostrarError(
                    "No fue posible buscar los huespedes en la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
            finally
            {
                btnBuscar.Enabled = true;
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
            btnGuardar.Enabled = false;

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
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarHuespedes();
                PrepararNuevoHuesped();
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (FormatException ex)
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
                MostrarError(ex.Message);
            }
            finally
            {
                btnGuardar.Enabled = true;
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
            btnEliminar.Enabled = false;

            try
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

                _huespedService.Eliminar(_documentoSeleccionado);
                CargarHuespedes();
                PrepararNuevoHuesped();

                MessageBox.Show(
                    "El huesped fue eliminado.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia(ex.Message);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MostrarAdvertencia(
                        "No se puede eliminar porque el huesped tiene datos relacionados.");
                }
                else
                {
                    MostrarAdvertencia(
                        "No se pudo eliminar el huesped en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
            finally
            {
                btnEliminar.Enabled = _documentoSeleccionado != null;
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
                ValidarCedula(numeroDocumento);
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
            PrepararNuevoHuesped();

            List<Huesped> huespedes = _huespedService.ObtenerTodos();
            dgvHuespedes.DataSource = null;
            dgvHuespedes.DataSource = huespedes;
            dgvHuespedes.ClearSelection();
            dgvHuespedes.CurrentCell = null;
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
            catch (SqlException)
            {
                MostrarError(
                    "No fue posible cargar el historial desde la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
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
            dgvHuespedes.ClearSelection();
            dgvHuespedes.CurrentCell = null;
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

        // Comprueba que la cedula contenga exactamente once digitos.
        private static void ValidarCedula(string cedula)
        {
            if (cedula.Length != 11)
            {
                throw new FormatException(
                    "La cedula debe contener 11 digitos.");
            }

            foreach (char caracter in cedula)
            {
                if (char.IsDigit(caracter) == false)
                {
                    throw new FormatException(
                        "La cedula debe contener solamente numeros.");
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
            TemaVisual.EstilizarGrid(grid);
        }

        // Aplica el color principal a un boton.
        private static void AplicarColorPrincipal(
            Button boton,
            string texto,
            string glifo)
        {
            TemaVisual.EstilizarBoton(
                boton,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(boton, glifo, texto);
        }

        // Aplica el color secundario a un boton.
        private static void AplicarColorSecundario(
            Button boton,
            string texto,
            string glifo)
        {
            TemaVisual.EstilizarBoton(
                boton,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            TemaVisual.PonerGlifo(boton, glifo, texto);
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
        private static void MostrarError(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Bisono",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
