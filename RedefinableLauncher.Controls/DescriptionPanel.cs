using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Collections;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;


namespace Redefinable.Applications.Launcher.Controls
{
    public class DescriptionPanel : VariableScaleableControl
    {
        // 非公開フィールド
        private string message;
        private Padding descriptionPanelPadding;
        private DescriptionPanelTheme currentTheme;
        
        
        // 公開フィールド

        /// <summary>
        /// このコントロールで表示するメッセージを取得・設定します。
        /// </summary>
        public string Message
        {
            get { return this._getMessage(); }
            set { this._setMessage(value); }
        }


        // 静的公開フィールド
        
        public static Size DescriptionPanelSize
        {
            get { return new Size(780, 100); }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいDescriptionPanelクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="location"></param>
        public DescriptionPanel(Point location)
            : base(location, DescriptionPanelSize)
        {
            // データフィールドの初期化
            this.message = "";
            this.descriptionPanelPadding = new Padding(10);
            this.currentTheme = DescriptionPanelTheme.GetSampleThemeForInit();

            // コントロールの初期化
            this.BackColor = Color.Transparent;
        }


        // 非公開メソッド

        private string _getMessage()
        {
            return this.message;
        }

        private void _setMessage(string value)
        {
            this.message = value;
            this._redraw();
        }

        private Padding _getDescriptionPanelPadding()
        {
            return this.descriptionPanelPadding;
        }

        private void _setDescriptionPanelPading(Padding value)
        {
            this.descriptionPanelPadding = value;
            this._redraw();
        }

        private void _redraw()
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            DescriptionPanelTheme theme = this.currentTheme;
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            Padding p = theme.Padding.GetScaledPadding(this.currentScale);

            Size drawingSize;
            Point drawingPoint;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // upperLeft
            drawingSize = new Size(p.Left, p.Top);
            drawingPoint = new Point(0, 0);
            g.DrawImage(theme.UpperLeft, new Rectangle(drawingPoint, drawingSize));

            // lowerLeft
            drawingSize = new Size(p.Left, p.Bottom);
            drawingPoint = new Point(0, this.Height - p.Bottom);
            g.DrawImage(theme.LowerLeft, new Rectangle(drawingPoint, drawingSize));

            // upperRight
            drawingSize = new Size(p.Right, p.Top);
            drawingPoint = new Point(this.Width - p.Right, 0);
            g.DrawImage(theme.UpperLeft, new Rectangle(drawingPoint, drawingSize));

            // lowerRight
            drawingSize = new Size(p.Left, p.Top);
            drawingPoint = new Point(this.Width - p.Right, this.Height - p.Bottom);
            g.DrawImage(theme.UpperLeft, new Rectangle(drawingPoint, drawingSize));

            // top
            drawingSize = new Size(this.Width - (p.Left + p.Right), p.Top);
            drawingPoint = new Point(p.Left, 0);
            g.DrawImage(theme.TopLine, new Rectangle(drawingPoint, drawingSize));

            // bottom
            drawingSize = new Size(this.Width - (p.Left + p.Right), p.Bottom);
            drawingPoint = new Point(p.Left, this.Height - p.Bottom);
            g.DrawImage(theme.BottomLine, new Rectangle(drawingPoint, drawingSize));
            
            // left
            drawingSize = new Size(p.Left, this.Height - (p.Top + p.Bottom));
            drawingPoint = new Point(0, p.Top);
            g.DrawImage(theme.LeftLine, new Rectangle(drawingPoint, drawingSize));

            // right
            drawingSize = new Size(p.Right, this.Height - (p.Top + p.Bottom));
            drawingPoint = new Point(this.Width - p.Right, p.Top);
            g.DrawImage(theme.LeftLine, new Rectangle(drawingPoint, drawingSize));

            // middle
            drawingSize = p.GetClientSize(this.Size);
            drawingPoint = p.GetClientPoint(new Point(0, 0));
            g.DrawImage(theme.Middle, new Rectangle(drawingPoint, drawingSize));

            // text
            /*
            Font font = new Font(theme.FontName, (int)((double)theme.FontSize * this.currentScale));
            g.DrawString(this.message, font, new SolidBrush(theme.FontColor), new Rectangle(drawingPoint, drawingSize));
            font.Dispose();
            */

            FontFamily ff = new FontFamily(theme.FontName);
            GraphicsPath gp = new GraphicsPath();
            gp.AddString(this.message, ff, (int)FontStyle.Bold, ((float)theme.FontSize * this.currentScale), new Rectangle(drawingPoint, drawingSize), StringFormat.GenericDefault);
            g.FillPath(new SolidBrush(theme.FontColor), gp);
            if (theme.FontBorder)
            {
                g.DrawPath(new Pen(Color.FromArgb(200, theme.FontBorderColor), 0.1f), gp);

            }
            gp.Dispose();

            g.Dispose();
        }


        // 公開メソッド

        public override void RefreshTheme()
        {
            LauncherTheme theme = this.GetLauncherTheme();
            if (theme != null)
                this.currentTheme = theme.DescriptionPanelTheme;

            this._redraw();
            base.RefreshTheme();
        }

        public override void ChangeScale(float scale)
        {
            base.ChangeScale(scale);
            this._redraw();
        }
    }
}
