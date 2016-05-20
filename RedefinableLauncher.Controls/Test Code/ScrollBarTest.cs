using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.Controls.Test_Code
{
    public class ScrollBarTest
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private Panel panel1;

        public MainForm()
        {
            this.ClientSize = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.AutoScroll = true;
            
            this.panel1 = new Panel();
            this.panel1.Location = new Point(10, 0);
            this.panel1.Size = new Size(300, 600);
            this.Controls.Add(this.panel1);
        }
    }
}
