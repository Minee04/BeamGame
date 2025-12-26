using System;
using System.Windows.Forms;

namespace BeamGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the Balance Beam Game application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
