// Cedula: 402-3047435-1
using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    // Diálogo reutilizable para elegir un huésped ya registrado. Se usa
    // desde FrmNuevaReserva y desde el panel de edición de FrmReservas.
    //
    // Mismo patrón de "resultado de diálogo" que FrmLogin.UsuarioAutenticado:
    // una propiedad publica de solo lectura desde afuera, asignada justo
    // antes de fijar DialogResult.OK (lo que cierra el formulario solo,
    // sin necesidad de llamar Close()).
    public partial class FrmBuscarHuesped : Form
    {
        private readonly HuespedService _huespedService;

        public Huesped HuespedSeleccionado { get; private set; }

        public FrmBuscarHuesped()
        {
            InitializeComponent();
            _huespedService = new HuespedService();
            ConfigurarFormulario();
        }

        // Prepara los textos, colores y eventos del formulario.
        private void ConfigurarFormulario()
        {
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Buscar Huésped");

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(640, 560);

            TemaVisual.EstilizarBoton(
                btnBuscar,
                TemaVisual.Colores.AzulPrimario,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.AzulHover);
            TemaVisual.PonerGlifo(btnBuscar, TemaVisual.Glifos.Buscar, "Buscar");

            TemaVisual.EstilizarBoton(
                btnSeleccionar,
                TemaVisual.Colores.TurquesaProfundo,
                TemaVisual.Colores.Blanco,
                TemaVisual.Colores.TurquesaPalmera);
            TemaVisual.PonerGlifo(
                btnSeleccionar,
                TemaVisual.Glifos.Confirmar,
                "Seleccionar");

            TemaVisual.EstilizarGrid(dgvHuespedes);
            dgvHuespedes.MultiSelect = false;
            dgvHuespedes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ConfigurarColumnasHuespedes();

            ConfigurarDistribucion();

            AcceptButton = btnBuscar;

            Load += FrmBuscarHuesped_Load;
            btnBuscar.Click += btnBuscar_Click;
            btnSeleccionar.Click += btnSeleccionar_Click;
            txtBuscar.KeyDown += txtBuscar_KeyDown;
            dgvHuespedes.CellDoubleClick += dgvHuespedes_CellDoubleClick;
        }

        // Reemplaza las columnas autogeneradas por columnas explícitas, con
        // encabezados más claros para el usuario final ("ID" en vez de
        // "NumeroDocumento", "Tipo de ID" en vez de "TipoDocumento"). Se
        // llama una única vez desde ConfigurarFormulario(), antes de la
        // primera asignación de DataSource: con AutoGenerateColumns = false,
        // cada EjecutarBusqueda() posterior sólo revincula filas, sin
        // recrear ni perder los encabezados ya puestos.
        private void ConfigurarColumnasHuespedes()
        {
            dgvHuespedes.AutoGenerateColumns = false;
            dgvHuespedes.Columns.Clear();

            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "NumeroDocumento", "ID", 20));
            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "TipoDocumento", "Tipo de ID", 12));
            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "Nombre", "Nombre", 14));
            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "Apellido", "Apellido", 14));
            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "Telefono", "Teléfono", 14));
            dgvHuespedes.Columns.Add(CrearColumnaTexto(
                "Email", "Email", 18));
        }

        // Crea una columna de texto simple para dgvHuespedes.
        private static DataGridViewTextBoxColumn CrearColumnaTexto(
            string propiedad,
            string encabezado,
            int pesoRelativo)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;
            columna.HeaderText = encabezado;
            columna.Name = "col" + propiedad;
            columna.FillWeight = pesoRelativo;
            return columna;
        }

        // Organiza la barra de busqueda, la tabla y el boton de seleccion.
        private void ConfigurarDistribucion()
        {
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Buscar Huésped",
                TemaVisual.Glifos.Huesped);
            encabezado.Dock = DockStyle.Top;

            FlowLayoutPanel barraBusqueda = new FlowLayoutPanel();
            barraBusqueda.Dock = DockStyle.Top;
            barraBusqueda.Height = 62;
            barraBusqueda.FlowDirection = FlowDirection.LeftToRight;
            barraBusqueda.WrapContents = false;
            barraBusqueda.Padding = new Padding(14, 13, 14, 10);
            barraBusqueda.BackColor = TemaVisual.Colores.FondoSecundario;

            Panel marcoBuscar = TemaVisual.EnvolverCampo(
                txtBuscar,
                TemaVisual.Colores.FondoSecundario);
            marcoBuscar.Size = new Size(360, 36);
            marcoBuscar.Margin = new Padding(0, 0, 10, 0);
            TemaVisual.AnteponerGlifo(
                marcoBuscar,
                txtBuscar,
                TemaVisual.Glifos.Buscar);

            btnBuscar.Size = new Size(110, 36);
            btnBuscar.Margin = new Padding(0);

            barraBusqueda.Controls.Add(marcoBuscar);
            barraBusqueda.Controls.Add(btnBuscar);

            Panel piePanel = new Panel();
            piePanel.Dock = DockStyle.Bottom;
            piePanel.Height = 56;
            piePanel.BackColor = TemaVisual.Colores.FondoClaro;
            piePanel.Padding = new Padding(14, 10, 14, 10);

            btnSeleccionar.AutoSize = false;
            btnSeleccionar.Size = new Size(160, 36);
            btnSeleccionar.Dock = DockStyle.Right;
            piePanel.Controls.Add(btnSeleccionar);

            dgvHuespedes.Dock = DockStyle.Fill;
            dgvHuespedes.Margin = new Padding(0);

            // El orden importa: el acoplamiento se resuelve del último
            // control al primero, así que la tabla (Fill) se agrega antes
            // que las barras Top/Bottom para que éstas no le quiten espacio
            // de más.
            Controls.Add(dgvHuespedes);
            Controls.Add(piePanel);
            Controls.Add(barraBusqueda);
            Controls.Add(encabezado);

            flpBuscarHuesped.Visible = false;
        }

        // Carga todos los huespedes al abrir la ventana.
        private void FrmBuscarHuesped_Load(object sender, EventArgs e)
        {
            EjecutarBusqueda("");
        }

        // Busca por el criterio escrito.
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            EjecutarBusqueda(txtBuscar.Text.Trim());
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

        // Consulta HuespedService.Buscar; con criterio vacio ya devuelve
        // todos los huespedes (HuespedRepository.Buscar lo resuelve así).
        private void EjecutarBusqueda(string criterio)
        {
            try
            {
                dgvHuespedes.DataSource = null;
                dgvHuespedes.DataSource = _huespedService.Buscar(criterio);
                dgvHuespedes.ClearSelection();
                dgvHuespedes.CurrentCell = null;
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
        }

        // Confirma la seleccion y cierra el dialogo.
        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            ConfirmarSeleccion();
        }

        // Doble clic sobre una fila selecciona ese huesped directamente.
        private void dgvHuespedes_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            ConfirmarSeleccion();
        }

        private void ConfirmarSeleccion()
        {
            if (dgvHuespedes.CurrentRow == null)
            {
                MostrarAdvertencia("Seleccione un huesped de la lista.");
                return;
            }

            Huesped huesped =
                dgvHuespedes.CurrentRow.DataBoundItem as Huesped;

            if (huesped == null)
            {
                MostrarAdvertencia("Seleccione un huesped de la lista.");
                return;
            }

            HuespedSeleccionado = huesped;
            DialogResult = DialogResult.OK;
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
