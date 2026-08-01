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
    public partial class FrmHabitaciones : Form
    {
        private readonly HabitacionService _habitacionService;
        private Usuario _usuarioActual;
        private int? _numeroSeleccionado;
        private bool _cargandoFiltros;
        private bool _limpiandoFormulario;

        public FrmHabitaciones()
        {
            InitializeComponent();

            _habitacionService = new HabitacionService();
            ConfigurarFormulario();
        }

        public FrmHabitaciones(Usuario usuarioActual)
            : this()
        {
            _usuarioActual = usuarioActual;
        }

        private void ConfigurarFormulario()
        {
            Text = "Hotel Zormat - Habitaciones";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = ColorTranslator.FromHtml("#F5F8FB");
            flpHabitaciones.BackColor = ColorTranslator.FromHtml("#F5F8FB");
            flpHabitaciones.AutoScroll = true;

            dgvHabitaciones.ReadOnly = true;
            dgvHabitaciones.AllowUserToAddRows = false;
            dgvHabitaciones.AllowUserToDeleteRows = false;
            dgvHabitaciones.MultiSelect = false;
            dgvHabitaciones.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvHabitaciones.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            txtNumero.Mask = "000";
            txtNumero.PromptChar = ' ';

            numPiso.Minimum = 1;
            numPiso.Maximum = 99;

            numCapacidad.Minimum = 1;
            numCapacidad.Maximum = 20;

            txtTarifaBase.Minimum = 1;
            txtTarifaBase.Maximum = 1000000;
            txtTarifaBase.DecimalPlaces = 2;
            txtTarifaBase.ThousandsSeparator = true;

            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.Items.Clear();
            cboTipo.Items.Add("Sencilla");
            cboTipo.Items.Add("Doble");
            cboTipo.Items.Add("Suite");

            cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstado.Items.Clear();
            cboEstado.Items.Add("Disponible");
            cboEstado.Items.Add("Ocupada");
            cboEstado.Items.Add("Reservada");
            cboEstado.Items.Add("Limpieza");

            cboFiltroPiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.Items.Clear();
            cboFiltroEstado.Items.Add("Todos");
            cboFiltroEstado.Items.Add("Disponible");
            cboFiltroEstado.Items.Add("Ocupada");
            cboFiltroEstado.Items.Add("Reservada");
            cboFiltroEstado.Items.Add("Limpieza");
            cboFiltroEstado.SelectedIndex = 0;

            ConfigurarBoton(btnNuevo, "Nuevo", "#E9EFF5", "#133958");
            ConfigurarBoton(btnGuardar, "Guardar", "#025897", "#FFFFFF");
            ConfigurarBoton(btnEliminar, "Eliminar", "#D6483F", "#FFFFFF");
            ConfigurarBoton(btnCancelar, "Cancelar", "#63798E", "#FFFFFF");

            Load += FrmHabitaciones_Load;
            dgvHabitaciones.SelectionChanged +=
                dgvHabitaciones_SelectionChanged;
            txtNumero.KeyPress += txtNumero_KeyPress;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnCancelar.Click += btnCancelar_Click;
            cboFiltroPiso.SelectedIndexChanged +=
                filtros_SelectedIndexChanged;
            cboFiltroEstado.SelectedIndexChanged +=
                filtros_SelectedIndexChanged;
        }

        private void ConfigurarBoton(
            Button boton,
            string texto,
            string colorFondo,
            string colorTexto)
        {
            boton.Text = texto;
            boton.BackColor = ColorTranslator.FromHtml(colorFondo);
            boton.ForeColor = ColorTranslator.FromHtml(colorTexto);
            boton.FlatStyle = FlatStyle.Flat;
            boton.UseVisualStyleBackColor = false;
        }

        private void FrmHabitaciones_Load(object sender, EventArgs e)
        {
            btnEliminar.Enabled =
                AutorizacionService.EsAdministrador(_usuarioActual);

            LimpiarFormulario();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                List<Habitacion> habitaciones =
                    _habitacionService.ObtenerTodas();

                CargarPisos(habitaciones);
                CargarHabitacionesFiltradas();
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudieron cargar las habitaciones. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al cargar las habitaciones.");
            }
        }

        private void CargarPisos(List<Habitacion> habitaciones)
        {
            _cargandoFiltros = true;
            cboFiltroPiso.Items.Clear();
            cboFiltroPiso.Items.Add("Todos");

            foreach (Habitacion habitacion in habitaciones)
            {
                string piso = habitacion.Piso.ToString();
                bool yaExiste = false;

                foreach (object item in cboFiltroPiso.Items)
                {
                    if (item.ToString() == piso)
                    {
                        yaExiste = true;
                    }
                }

                if (yaExiste == false)
                {
                    cboFiltroPiso.Items.Add(piso);
                }
            }

            cboFiltroPiso.SelectedIndex = 0;
            _cargandoFiltros = false;
        }

        private void CargarHabitacionesFiltradas()
        {
            try
            {
                int? piso = ObtenerPisoSeleccionado();
                string estado = ObtenerEstadoSeleccionado();

                dgvHabitaciones.DataSource = null;
                dgvHabitaciones.DataSource =
                    _habitacionService.Filtrar(piso, estado);

                ColorearFilas();
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo aplicar el filtro de habitaciones.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al aplicar los filtros.");
            }
        }

        private int? ObtenerPisoSeleccionado()
        {
            if (cboFiltroPiso.SelectedItem == null)
            {
                return null;
            }

            string valor = cboFiltroPiso.SelectedItem.ToString();
            if (valor == "Todos")
            {
                return null;
            }

            int piso;
            if (int.TryParse(valor, out piso))
            {
                return piso;
            }

            return null;
        }

        private string ObtenerEstadoSeleccionado()
        {
            if (cboFiltroEstado.SelectedItem == null)
            {
                return null;
            }

            string estado = cboFiltroEstado.SelectedItem.ToString();
            if (estado == "Todos")
            {
                return null;
            }

            return estado;
        }

        private void ColorearFilas()
        {
            foreach (DataGridViewRow fila in dgvHabitaciones.Rows)
            {
                Habitacion habitacion = fila.DataBoundItem as Habitacion;
                if (habitacion != null)
                {
                    fila.DefaultCellStyle.BackColor =
                        ObtenerColorSuave(habitacion.Estado);
                }
            }
        }

        private Color ObtenerColorSuave(string estado)
        {
            if (estado == "Disponible")
            {
                return ColorTranslator.FromHtml("#DDF3E3");
            }

            if (estado == "Ocupada")
            {
                return ColorTranslator.FromHtml("#F7DEDC");
            }

            if (estado == "Reservada")
            {
                return ColorTranslator.FromHtml("#FCEBD8");
            }

            return ColorTranslator.FromHtml("#DFE7FA");
        }

        private void dgvHabitaciones_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (_limpiandoFormulario)
            {
                return;
            }

            if (dgvHabitaciones.CurrentRow == null)
            {
                return;
            }

            Habitacion habitacion =
                dgvHabitaciones.CurrentRow.DataBoundItem as Habitacion;

            if (habitacion == null)
            {
                return;
            }

            _numeroSeleccionado = habitacion.Numero;
            txtNumero.Text = habitacion.Numero.ToString();
            txtNumero.Enabled = false;
            numPiso.Value = habitacion.Piso;
            cboTipo.SelectedItem = habitacion.Tipo;
            cboEstado.SelectedItem = habitacion.Estado;
            txtTarifaBase.Value = habitacion.TarifaBase;
            numCapacidad.Value = habitacion.Capacidad;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtNumero.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _limpiandoFormulario = true;
            _numeroSeleccionado = null;
            txtNumero.Enabled = true;
            txtNumero.Clear();
            numPiso.Value = 1;
            cboTipo.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
            txtTarifaBase.Value = 1;
            numCapacidad.Value = 1;
            dgvHabitaciones.ClearSelection();
            dgvHabitaciones.CurrentCell = null;
            _limpiandoFormulario = false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Habitacion habitacion = CrearHabitacionDesdeFormulario();

                if (_numeroSeleccionado.HasValue)
                {
                    _habitacionService.Actualizar(habitacion);
                    MostrarInformacion("La habitación fue actualizada.");
                }
                else
                {
                    _habitacionService.Crear(habitacion);
                    MostrarInformacion("La habitación fue registrada.");
                }

                LimpiarFormulario();
                CargarDatos();
            }
            catch (HabitacionOcupadaException error)
            {
                MostrarError(error.Message);
            }
            catch (FormatException error)
            {
                MostrarError(error.Message);
            }
            catch (ArgumentException error)
            {
                MostrarError(error.Message);
            }
            catch (InvalidOperationException error)
            {
                MostrarError(error.Message);
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo guardar la habitación. " +
                    "Verifique la conexión con la base de datos.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al guardar la habitación.");
            }
        }

        private Habitacion CrearHabitacionDesdeFormulario()
        {
            int numero;
            if (int.TryParse(txtNumero.Text.Trim(), out numero) == false)
            {
                throw new FormatException(
                    "Ingrese un número de habitación válido.");
            }

            if (cboTipo.SelectedItem == null)
            {
                throw new FormatException(
                    "Seleccione el tipo de habitación.");
            }

            if (cboEstado.SelectedItem == null)
            {
                throw new FormatException(
                    "Seleccione el estado de la habitación.");
            }

            string tipo = cboTipo.SelectedItem.ToString();

            Habitacion habitacion = new Habitacion();
            habitacion.Numero = numero;
            habitacion.Piso = Convert.ToInt32(numPiso.Value);
            habitacion.Tipo = tipo;
            habitacion.Capacidad = Convert.ToInt32(numCapacidad.Value);
            habitacion.TarifaBase = txtTarifaBase.Value;
            habitacion.Estado = cboEstado.SelectedItem.ToString();

            return habitacion;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_numeroSeleccionado.HasValue == false)
            {
                MostrarError("Seleccione una habitación para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar la habitación " +
                _numeroSeleccionado.Value + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _habitacionService.Eliminar(
                    _numeroSeleccionado.Value,
                    _usuarioActual);

                MostrarInformacion("La habitación fue eliminada.");
                LimpiarFormulario();
                CargarDatos();
            }
            catch (PermisoDenegadoException error)
            {
                MostrarError(error.Message);
            }
            catch (InvalidOperationException error)
            {
                MostrarError(error.Message);
            }
            catch (SqlException)
            {
                MostrarError(
                    "No se pudo eliminar la habitación. " +
                    "Puede tener datos relacionados.");
            }
            catch (Exception)
            {
                MostrarError(
                    "Ocurrió un error al eliminar la habitación.");
            }
        }

        private void filtros_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (_cargandoFiltros)
            {
                return;
            }

            CargarHabitacionesFiltradas();
        }

        private void txtNumero_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            if (char.IsDigit(e.KeyChar) == false)
            {
                e.Handled = true;
            }
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Zormat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void MostrarInformacion(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Hotel Zormat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
