using System;
using System.Collections.Generic;
using System.Text;

namespace Redefinable.Applications.Launcher.ScrollTester
{
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            System.Windows.Forms.Application.Run(new MainForm());
        }
    }
}
