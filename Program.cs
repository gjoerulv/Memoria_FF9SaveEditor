using System;
using System.Windows.Forms;

namespace Memoria
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DialogResult res = DialogResult.No;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SelectSave());
            }
            catch (Exception ex)
            {
                res = MessageBox.Show(ex.Message + "\r\nRestart?", "Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            }

            if (res == DialogResult.Yes)
            {
                Application.ExitThread();
                Application.Restart();
            }
        }

        public static string Version
        {
            get
            {
                try
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                    return fvi.FileVersion;
                }
                catch(Exception ex) { } return "";
            }
        }
    }
}