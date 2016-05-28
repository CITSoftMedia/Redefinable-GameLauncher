using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;


namespace Redefinable.Applications.Launcher.Forms
{
    public class LoadingForm : Form
    {
        private Label label1;

        public LoadingForm()
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(400, 150);
            this.TopMost = true;

            this.label1 = new Label();
            this.label1.Text = "起動中...";
            this.label1.Font = new Font("MS UI Gothic", 18);
            this.label1.AutoSize = true;
            this.Controls.Add(this.label1);

            this.label1.Location = new Point((this.ClientSize.Width - this.label1.Width) / 2, (this.ClientSize.Height - this.label1.Height) / 2);
        }
    }
}
