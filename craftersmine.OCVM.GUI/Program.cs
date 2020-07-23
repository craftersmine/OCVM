using craftersmine.OCVM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.GUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger l = new Logger(Environment.GetEnvironmentVariable("TEMP"), "OCVM");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Settings.InitializeFont();
            Application.Run(new VMForm(Core.Base.Tier.Base));
        }
    }
}
