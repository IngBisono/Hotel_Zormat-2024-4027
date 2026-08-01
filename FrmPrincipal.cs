using System;
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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            //Los Tools del formulario se deben meter dentro de una variable para programarlos
            string Usuario = txtUsuario.Text;
            string Password = txtPassword.Text;
            
            try
            {
                if (Usuario == "Admin" && Password == "1234")
                {
                    MessageBox.Show("Bienvenido Admin");
                    
                    FrmHabitacion frmHab = new FrmHabitacion();
                    frmHab.ShowDialog();
                }
            }
            catch { MessageBox.Show("Solo el admin puede Ingresar"); }
            //Show es para que se muestre en pantalla la ventana de mensaje
            
        }

        private void MostrarBienvenida(string rol)
        {
            switch (rol)
            {
                case "Admin":
                    MessageBox.Show("Bienvenido Administrador");
                break;

                case "Recepcionista":
                    MessageBox.Show("Hola, recepcionista");
                break;

                case "Gerente":
                    MessageBox.Show("Buenas, gerente");
                break;

                default:
                    MessageBox.Show("Rol desconocido");
                break;


            }
        }
    }
}
