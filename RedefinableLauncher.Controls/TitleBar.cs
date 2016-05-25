using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;

namespace Redefinable.Applications.Launcher.Controls
{
    public class TitleBar : VariableScaleableControl
    {
        // 非公開フィールド
        private string displayNumberField;
        private string titleField;
        private TitleBarTheme currentTheme;
        

        // 非公開静的フィールド
        private static int defaultWidth = 780; // 固定幅
        private static int defaultHeight = 60; // 固定高さ
        
        
        // 公開フィールド

        /// <summary>
        /// DisplayNumberの表示テキストを取得します。更新と新規描画はRefreshFieldsメソッドでおこなってください。
        /// </summary>
        public string DisplayNumberField
        {
            get { return this.displayNumberField; }
        }

        /// <summary>
        /// Titleの表示テキストを取得します。更新と新規描画はRefreshFieldsメソッドでおこなってください。
        /// </summary>
        public string TitleField
        {
            get { return this.titleField; }
        }

        
        // 公開静的フィールド

        /// <summary>
        /// デフォルトのTitleBarのサイズを取得します。
        /// </summary>
        public static Size DefaultTitleBarSize
        {
            get { return new Size(defaultWidth, defaultHeight); }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいTitleBarクラスのインスタンスを初期化します。このコントロールのサイズは固定されています。
        /// </summary>
        /// <param name="location"></param>
        public TitleBar(Point location)
            : base(location, new Size(defaultWidth, defaultHeight))
        {
            // データフィールドの初期化
            this.displayNumberField = "";
            this.titleField = "";
            this.currentTheme = TitleBarTheme.GetSampleThemeForInit();

            // コントロールの初期化
            this.BackColor = Color.Transparent;

            this._redraw();

            // デバッグ
            //this.BackColor = Color.Beige;
        }


        // 非公開メソッド

        /// <summary>
        /// 再描画します。
        /// </summary>
        private void _redraw()
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            TitleBarTheme theme = this.currentTheme;
            TitleBarThemeTextDrawingOption opt;
            Font font;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawImage(theme.Background, 0, 0, this.BackgroundImage.Width, this.BackgroundImage.Height);

            // 注意: 縁取り実装前

            opt = theme.DisplayNumberDrawingOption;
            font = new Font(opt.FontName, (float)((double)opt.Size * this.currentScale));
            g.DrawString(this.displayNumberField, font, new SolidBrush(Color.FromArgb(100, Color.Black)), new Point(opt.Point.X + 2, opt.Point.Y + 2));
            g.DrawString(this.displayNumberField, font, new SolidBrush(opt.Color), new Point(opt.Point.X, opt.Point.Y));
            font.Dispose();

            opt = theme.TitleDrawingOption;
            font = new Font(opt.FontName, (float)((double)opt.Size * this.currentScale));
            g.DrawString(this.titleField, font, new SolidBrush(Color.FromArgb(100, Color.Black)), new Point(opt.Point.X + 2, opt.Point.Y + 2));
            g.DrawString(this.titleField, font, new SolidBrush(opt.Color), new Point(opt.Point.X, opt.Point.Y));
            font.Dispose();

            g.Dispose();
        }


        // 公開メソッド

        /// <summary>
        /// 各種フィールドの値を更新し、再描画を実行します。
        /// </summary>
        /// <param name="displayNumber"></param>
        /// <param name="title"></param>
        public void RefreshFields(string displayNumber, string title)
        {
            this.displayNumberField = displayNumber;
            this.titleField = title;

            this._redraw();
        }

        /// <summary>
        /// テーマを更新し、再描画を実行します。
        /// </summary>
        public override void RefreshTheme()
        {
            LauncherTheme t = this.GetLauncherTheme();
            if (t != null && t.TitleBarTheme != null)
                this.currentTheme = t.TitleBarTheme;
            this._redraw();
            base.RefreshTheme();
        }
    }
}
