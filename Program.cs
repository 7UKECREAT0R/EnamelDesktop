using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var f1 = new Form1();
            if(args.Length > 0)
            {
                f1.fromEnamel = true;
                string a0 = args[0];
                if(int.TryParse(a0, out int monitor))
                    f1.attemptToUseMonitor = monitor;
            } else { f1.attemptToUseMonitor = 0; }
            Application.Run(f1);
        }
    }
}
