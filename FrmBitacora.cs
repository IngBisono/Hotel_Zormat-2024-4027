// Cedula: 402-3047435-1
using Hotel_Zormat.Estilos;
using HotelZormat.Modelo;
using HotelZormat.Negocio;
using HotelZormat.Negocio.Excepciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmBitacora : Form
    {
        private readonly BitacoraService bitacoraService;
        private Usuario usuarioActual;

        // Conteo de registros del filtro activo. Creado por código para no
        // modificar FrmBitacora.Designer.cs.
        private Label _lblConteoEventos;

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
            TemaVisual.PrepararFormulario(this, "Hotel Bisono - Bitácora");
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(960, 620);
            MinimumSize = new Size(860, 560);

            flpFiltrosBitacora.BackColor = TemaVisual.Colores.FondoSecundario;
            flpFiltrosBitacora.Padding = new Padding(16, 13, 16, 10);
            flpFiltrosBitacora.WrapContents = false;
            flpFiltrosBitacora.Height = 60;

            Label etiquetaAccion = new Label();
            etiquetaAccion.Text = "Acción:";
            etiquetaAccion.AutoSize = true;
            etiquetaAccion.Font = TemaVisual.Fuentes.Etiqueta;
            etiquetaAccion.ForeColor = TemaVisual.Colores.Texto;
            etiquetaAccion.Margin = new Padding(0, 8, 6, 0);

            Label etiquetaFecha = new Label();
            etiquetaFecha.Text = "Fecha:";
            etiquetaFecha.AutoSize = true;
            etiquetaFecha.Font = TemaVisual.Fuentes.Etiqueta;
            etiquetaFecha.ForeColor = TemaVisual.Colores.Texto;
            etiquetaFecha.Margin = new Padding(18, 8, 6, 0);

            // Distintivo que recuerda que la bitácora es sólo para
            // administradores, coherente con la validación del Load.
            Label distintivoAdmin = TemaVisual.CrearDistintivo(
                "Solo administradores",
                TemaVisual.Colores.SolAmarillo,
                TemaVisual.Colores.AzulProfundo);
            distintivoAdmin.Size = new Size(170, 28);
            distintivoAdmin.Margin = new Padding(26, 4, 0, 0);

            // Coloca los filtros dentro de la barra creada en el diseñador.
            flpFiltrosBitacora.Controls.Add(etiquetaAccion);
            flpFiltrosBitacora.Controls.Add(cboFiltroAccion);
            flpFiltrosBitacora.Controls.Add(etiquetaFecha);
            flpFiltrosBitacora.Controls.Add(dtpFiltroFecha);
            flpFiltrosBitacora.Controls.Add(distintivoAdmin);

            _lblConteoEventos = new Label();
            _lblConteoEventos.Text = "0 registros";
            _lblConteoEventos.Font = TemaVisual.Fuentes.CuerpoNegrita;
            _lblConteoEventos.ForeColor = TemaVisual.Colores.AzulMarino;
            _lblConteoEventos.AutoSize = false;
            _lblConteoEventos.Size = new Size(140, 28);
            _lblConteoEventos.Margin = new Padding(18, 4, 0, 0);
            _lblConteoEventos.TextAlign = ContentAlignment.MiddleLeft;
            flpFiltrosBitacora.Controls.Add(_lblConteoEventos);

            cboFiltroAccion.Width = 170;
            cboFiltroAccion.Margin = new Padding(0, 4, 12, 0);
            TemaVisual.EstilizarCombo(cboFiltroAccion);
            dtpFiltroFecha.Width = 220;
            dtpFiltroFecha.Margin = new Padding(0, 4, 0, 0);
            TemaVisual.EstilizarFecha(dtpFiltroFecha);

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

            TemaVisual.EstilizarGrid(dgvBitacora);
            dgvBitacora.Dock = DockStyle.Fill;

            // El encabezado se acopla arriba y la tabla al resto: la tabla, al
            // ir acoplada a Fill, debe quedar al frente para que su
            // acoplamiento se resuelva en último lugar y no tape la barra.
            Panel encabezado = TemaVisual.CrearEncabezado(
                "Bitácora / Auditoría",
                TemaVisual.Glifos.Bitacora);
            Controls.Add(encabezado);

            flpFiltrosBitacora.SendToBack();
            encabezado.SendToBack();
            dgvBitacora.BringToFront();

            cboFiltroAccion.SelectedIndexChanged += FiltroCambiado;
            dtpFiltroFecha.ValueChanged += FiltroCambiado;

            // Nota: el evento Load de este formulario ya está enlazado en
            // FrmBitacora.Designer.cs. No debe volver a suscribirse aquí o la
            // comprobación de administrador se ejecutaría dos veces.
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            if (usuarioActual == null)
            {
                MessageBox.Show(
                    "Debe iniciar sesión para consultar la bitácora.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
                return;
            }

            try
            {
                AutorizacionService.ExigirAdministrador(usuarioActual);
                CargarBitacora();
            }
            catch (PermisoDenegadoException)
            {
                MessageBox.Show(
                    "Solo un administrador puede consultar la bitácora.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "No se pudo verificar el acceso a la bitácora.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
            }
        }

        private void FiltroCambiado(object sender, EventArgs e)
        {
            if (usuarioActual == null)
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

                List<Bitacora> registros =
                    bitacoraService.Filtrar(usuarioActual, accion, fecha);

                dgvBitacora.DataSource = registros;

                if (_lblConteoEventos != null)
                {
                    int total = registros == null ? 0 : registros.Count;
                    _lblConteoEventos.Text = total == 1
                        ? "1 registro"
                        : total + " registros";
                }
            }
            catch (PermisoDenegadoException)
            {
                MessageBox.Show(
                    "Solo un administrador puede consultar la bitacora.",
                    "Bitacora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Seleccione filtros válidos para consultar la bitácora.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "No se pudo consultar la bitácora en la base de datos.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Ocurrió un error al cargar la bitácora.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
