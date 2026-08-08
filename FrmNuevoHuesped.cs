using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    // Diálogo minimalista para registrar un huésped nuevo: solo el
    // encabezado, los campos y el botón Guardar. Se abre exclusivamente
    // desde el botón "Nuevo" de FrmHuespedes.cs.
    //
    // No toca HuespedService ni Huesped: llama directamente a
    // _huespedService.Crear(huesped), que ya valida documento duplicado,
    // tipo de documento y datos obligatorios. Este formulario no agrega
    // ninguna regla de negocio nueva.
    public partial class FrmNuevoHuesped : Form
    {
        private readonly HuespedService _huespedService;

        // Este constructor permite abrir el formulario en el Disenador.
        public FrmNuevoHuesped()
        {
            InitializeComponent();
            _huespedService = new HuespedService();
            ConfigurarFormulario();
        }

        // Este constructor recibe al usuario que inicio sesion.
        public FrmNuevoHuesped(Usuario usuarioActual) : this()
        {
            if (usuarioActual != null)
            {
                Text = "Hotel Bisono - Nuevo Huésped - " +
                    usuarioActual.NombreCompleto;
            }
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Nuevo Huésped");

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(480, 520);

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

            TemaVisual.EstilizarBoton(
                btnGuardar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(
                btnGuardar,
                TemaVisual.Glifos.Guardar,
                "Guardar");

            ConfigurarDistribucion();

            AcceptButton = btnGuardar;

            cboTipoDocumento.SelectedIndexChanged +=
                cboTipoDocumento_SelectedIndexChanged;
            txtNumeroDocumento.MouseUp += txtNumeroDocumento_MouseUp;
            btnGuardar.Click += btnGuardar_Click;
        }

        // Organiza el encabezado y los campos en una sola columna de datos.
        private void ConfigurarDistribucion()
        {
            // El control acoplado a Fill debe quedar al frente: el
            // acoplamiento se resuelve del último control al primero.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Nuevo Huésped",
                TemaVisual.Glifos.Huesped);
            Controls.Add(encabezado);

            TableLayoutPanel tabla = new TableLayoutPanel();
            tabla.Dock = DockStyle.Fill;
            tabla.Padding = new Padding(20, 20, 20, 20);
            tabla.BackColor = TemaVisual.Colores.FondoClaro;
            tabla.ColumnCount = 2;
            tabla.RowCount = 7;
            tabla.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150F));
            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            for (int fila = 0; fila < 6; fila++)
            {
                tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            }

            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            AgregarCampo(tabla, "Tipo de documento", cboTipoDocumento, 0);
            AgregarCampo(tabla, "N.º de documento", txtNumeroDocumento, 1);
            AgregarCampo(tabla, "Nombre", txtNombre, 2);
            AgregarCampo(tabla, "Apellido", txtApellido, 3);
            AgregarCampo(tabla, "Teléfono", txtTelefono, 4);
            AgregarCampo(tabla, "Correo", txtEmail, 5);

            btnGuardar.AutoSize = false;
            btnGuardar.Size = new Size(140, 38);
            btnGuardar.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            btnGuardar.Margin = new Padding(3, 16, 3, 3);
            tabla.Controls.Add(btnGuardar, 1, 6);

            Controls.Add(tabla);
            tabla.BringToFront();

            // El panel de flujo del diseñador ya no participa en la
            // composición; se oculta en lugar de tocar el archivo generado.
            flpNuevoHuesped.Visible = false;
        }

        // Agrega una etiqueta y su control en una fila de la tabla.
        //
        // Mismo patrón que AgregarCampo en FrmHuespedes.cs: las cajas de
        // texto se envuelven en un marco redondeado; las listas
        // desplegables conservan su borde nativo.
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

        // Corrige la posición del cursor al hacer clic en txtNumeroDocumento.
        //
        // El campo se estira a todo el ancho del marco redondeado (ver
        // TemaVisual.EnvolverCampo), mucho más ancho que sus 11 dígitos como
        // máximo. Al hacer clic en la franja vacía a la derecha del texto ya
        // escrito, WinForms no encuentra un carácter bajo el punto y coloca
        // el cursor en la última posición de la máscara en vez de la primera
        // posición vacía. Se corrige sólo para esa franja: un clic dentro
        // del texto ya escrito se deja tal cual lo resolvió el control, para
        // no interferir con la edición normal a mitad del valor. Mismo
        // patrón que txtNumero_MouseUp en FrmHabitaciones.cs.
        private void txtNumeroDocumento_MouseUp(
            object sender,
            MouseEventArgs e)
        {
            string escrito = txtNumeroDocumento.Text.TrimEnd(' ');
            Size medida = TextRenderer.MeasureText(
                escrito,
                txtNumeroDocumento.Font);

            if (e.X > medida.Width)
            {
                txtNumeroDocumento.SelectionStart = escrito.Length;
                txtNumeroDocumento.SelectionLength = 0;
            }
        }

        // Registra el huesped nuevo. La validacion de documento duplicado
        // ya vive en HuespedService.Crear (ver HuespedService.cs) — no se
        // repite aqui.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;

            try
            {
                Huesped huesped = CrearHuespedDesdeFormulario();
                _huespedService.Crear(huesped);

                MessageBox.Show(
                    "El huesped fue registrado.",
                    "Hotel Bisono",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
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

        // Convierte los campos en un objeto Huesped.
        //
        // Mismas normalizaciones y reglas de formato que
        // CrearHuespedDesdeFormulario en FrmHuespedes.cs, para que crear un
        // huesped desde este dialogo exija la misma calidad de datos que
        // editarlo desde el formulario principal.
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
