using System;
using System.Windows.Forms;

namespace Tron
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Ventana()); // Cambia esto si usas otro nombre para tu formulario principal
        }
    }
}
