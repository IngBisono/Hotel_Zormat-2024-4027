using HotelZormat.Negocio;
using HotelZormat.Negocio.Modelo;
using System;
using Habitacion = HotelZormat.Modelo.Habitacion;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    public partial class FrmGestionHabitaciones : Form
    {
        private readonly HabitacionService _habitacionService;
        private HabitacionService _habitacionActual;
        
        public FrmGestionHabitaciones()
        {
            InitializeComponent();
            _habitacionService = new HabitacionService();
        }

        private void frmGestionHabitaciones_Load(object sender, EventArgs e)
        {
            List<string> tipos = new List<string>()
            {
                "Sencilla", "Doble", "Suite"
            };

            foreach (var tipo in tipos)
            {
                cboTipo.Items.Add(tipo);
            }

            cboAccion.SelectedIndex = 0;
        }

        private void CargarPiso3()
        {
            lstHabitaciones.Items.Clear();

            List<Habitacion> todas = _habitacionService.ObtenerTodas();

            foreach (var hab in todas)
            {
                if (hab.Piso == 3)
                {
                    string linea = "#" + hab.Numero + " - " + hab.Tipo;
                    lstHabitaciones.Items.Add(linea);
                }
            }
        }

    }
}
