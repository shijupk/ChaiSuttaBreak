using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;


namespace ChaiSuttaBreak
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            _ = new Mutex(true, "ChaiSuttaBreak", out bool createdNew);

            if (!createdNew)
            {
                _ = MessageBox.Show("Application already running", "ChaiSuttaBreak", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayContext());
        }
    }
}
