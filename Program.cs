// Cedula: 402-3047435-1
using HotelZormat.Modelo;
using System;
using System.Windows.Forms;

namespace Hotel_Zormat
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool mostrarLogin = true;

            while (mostrarLogin)
            {
                Usuario usuarioAutenticado;

                using (FrmLogin login = new FrmLogin())
                {
                    DialogResult resultadoLogin = login.ShowDialog();

                    if (resultadoLogin != DialogResult.OK)
                    {
                        return;
                    }

                    usuarioAutenticado = login.UsuarioAutenticado;
                }

                using (FrmDashboardHabitaciones dashboard =
                    new FrmDashboardHabitaciones(usuarioAutenticado))
                {
                    dashboard.ShowDialog();
                    mostrarLogin = dashboard.CerrarSesionSolicitado;
                }
            }
        }
    }
}
