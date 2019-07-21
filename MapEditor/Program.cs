using System;
using System.Windows.Forms;

namespace MapEditor
{
    public class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        public static void Main(string[] args)
        {
            SetProcessDPIAware();
            Application.Run(new Form1());
        }
    }
}
