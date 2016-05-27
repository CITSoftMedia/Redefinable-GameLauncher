using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;
using Redefinable.Collections;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

namespace Redefinable.Applications.Launcher.Controls
{
    public class ChildOpeationPanel : ChildPanel
    {
        // 非公開フィールド
        private Image image;
        private string titleField;
        private string message;

        
        // 非公開フィールド :: コントロール
        private VariableScaleableControl drawingPanel;


        // 公開フィールド

        public Image Image
        {
            get { return this.image; }
        }

        public string TitleField
        {
            get { return this.titleField; }
        }

        public string Message
        {
            get { return this.message; }
        }


        // コンストラクタ

        public ChildOpeationPanel()
        {
            // データフィールドの初期化
            this.image = null;
            this.titleField = "null";
            this.message = "null";

            // コントロールの初期化
            this._initializeControls();
            this._redraw();

            // イベントデリゲートの初期化


            // イベントの初期化


        }


        // 非公開メソッド

        private void _initializeControls()
        {
            this.drawingPanel = new VariableScaleableControl(new Point(0, 0), this.DefaultControlSize);
            this.drawingPanel.BackColor = Color.Transparent;
            //this.drawingPanel.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(this.drawingPanel);
        }

        private void _redraw()
        {
            Size defSize = this.DefaultControlSize;
            this.drawingPanel.BackgroundImage = new Bitmap(defSize.Width, defSize.Height);
            Image image = this.drawingPanel.BackgroundImage;
            Graphics g = Graphics.FromImage(image);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GraphicsPath gp;
            FontFamily ff = new FontFamily("MS UI Gothic");

            Size drawingSize;
            Point drawingPoint;

            // titleBack
            drawingSize = new Size(980, 50).GetScaledSize(this.currentScale);
            drawingPoint = new Point(50, 50).GetScaledPoint(this.currentScale);
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), new Rectangle(drawingPoint, drawingSize));
            
            // title
            drawingPoint = new Point(60, 55).GetScaledPoint(this.currentScale);
            int titleSize = (int)(35f * this.currentScale);
            
            gp = new GraphicsPath();
            gp.AddString(this.titleField, ff, 0, titleSize, new Point(drawingPoint.X + 2, drawingPoint.Y + 2), StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(Color.FromArgb(100, Color.Black)), gp);
            g.DrawPath(new Pen(Color.FromArgb(30, Color.Black), 2), gp);
            gp.Dispose();
            gp = new GraphicsPath();
            gp.AddString(this.titleField, ff, 0, titleSize, drawingPoint, StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(Color.White), gp);
            g.DrawPath(new Pen(Color.FromArgb(30, Color.Black), 2), gp);
            gp.Dispose();
            
            // message
            drawingSize = new Size(980, 800).GetScaledSize(this.currentScale);
            drawingPoint = new Point(60, 120).GetScaledPoint(this.currentScale);
            int messageSize = (int)(22f * this.currentScale);
            
            gp = new GraphicsPath();
            gp.AddString(this.message, ff, 0, messageSize, new Rectangle(drawingPoint, drawingSize), StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(Color.White), gp);
            g.DrawPath(new Pen(Color.FromArgb(30, Color.Black), 2), gp);
            gp.Dispose();
        }


        // 公開メソッド

        public void SetFields(string title, string message)
        {
            this.titleField = title;
            this.message = message;
            this._redraw();
        }

        public override void ChangeScale(float scale)
        {
            base.ChangeScale(scale);
            this._redraw();
        }
    }
}
