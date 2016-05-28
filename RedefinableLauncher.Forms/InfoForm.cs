using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;


namespace Redefinable.Applications.Launcher.Forms
{
    public class InfoForm : Form
    {
        // 非公開フィールド
        private string message;

        
        // 公開フィールド

        /// <summary>
        /// 現在表示しているテキストを取得します。
        /// </summary>
        public string Message
        {
            get { return this.message; }
        }


        // コンストラクタ

        public InfoForm()
        {
            Rectangle dispBounds = Screen.FromControl(this).Bounds;

            // データフィールドの初期化
            this.message = "null";

            // コントロールの初期化
            this.Text = RedefinableUtility.SoftwareTitle;
            this.Location = new Point(0, 0);
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(550, 60);
            //this.Visible = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.6f;
            this.BackColor = Color.Black;
            this.TopMost = true;

            // イベントの追加
            this.MouseMove += (sender, e) =>
            {
                if (this.Location.X < 10)
                    this.Location = new Point(dispBounds.Width - this.ClientSize.Width, 0);
                else
                    this.Location = new Point(0, 0);
            };
        }


        // 非公開メソッド

        private void _redraw()
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            GraphicsPath gp;
            FontFamily ff = new FontFamily("MS UI Gothic");
            Point dp = new Point(10, 10);
            float fs = 30f;

            gp = new GraphicsPath();
            gp.AddString(this.message, ff, 0, fs, new Point(dp.X + 2, dp.Y + 2), StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(Color.FromArgb(100, Color.Black)), gp);
            g.DrawPath(new Pen(Color.FromArgb(30, Color.Black), 2), gp);
            gp.Dispose();

            gp = new GraphicsPath();
            gp.AddString(this.message, ff, 0, fs, dp, StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(Color.White), gp);
            g.DrawPath(new Pen(Color.FromArgb(150, Color.Black), 1), gp);
            gp.Dispose();

            g.Dispose();
            ff.Dispose();
        }

        
        // 公開メソッド

        public void SetField(string message)
        {
            this.message = message;
            this._redraw();
        }
    }
}
