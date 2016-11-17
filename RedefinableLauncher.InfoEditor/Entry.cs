using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redefinable.Applications.Launcher.InfoEditor.Core;
using Redefinable.Applications.Launcher.InfoEditor.Forms;


namespace Redefinable.Applications.Launcher.InfoEditor
{
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Main Entry Point
            EditorStarter.Start();
        }
    }
}
