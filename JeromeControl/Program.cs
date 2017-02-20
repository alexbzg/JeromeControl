using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace JeromeControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isSingle;
            using (var mutex = new System.Threading.Mutex(true, "JeromeControlAppId", out isSingle))
                if (isSingle)
                {
                    GC.KeepAlive(mutex);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    try
                    {
                        var applicationContext = new JCAppContext();
                        Application.Run(applicationContext);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Program Terminated Unexpectedly",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
        }
    }
}
