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
    public class LauncherButton : VariableScaleableControl
    {
        // 非公開フィールド
        private bool applyThemesHeight;
        private string text;


        // 非公開フィールド :: コントロール
        private NormalScaleableColorPanel hilightPanel;
        private Color focusBorderColor;
        private Image backgroundImage;


        // 公開フィールド

        /// <summary>
        /// このLauncherButtonの高さをテーマが推奨するサイズへ設定するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool ApplyThemesHeight
        {
            get { return this.applyThemesHeight; }
            set { this.applyThemesHeight = value; }
        }

        /// <summary>
        /// コントロールのテキストを取得・設定します。
        /// </summary>
        public override string Text
        {
            get { return this._getText(); }
            set { this._setText(value); }
        }

        /// <summary>
        /// コントロールの背景を取得・設定します。
        /// </summary>
        public new Image BackgroundImage
        {
            get { return this._getBackgroundImage(); }
            set { this._setBackgroundImage(value); }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいLauncherButtonクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public LauncherButton(Point location, Size size)
            : base(location, size)
        {
            // データフィールドの初期化
            this.Text = "";
            this.Font = new Font("MS UI Gothic", 12, FontStyle.Bold);

            // コントロールの初期化
            this._initializeControls();
            
            // イベントハンドラの初期化

            
            // イベントの追加
            this.ScaleChanged += (sender, e) => { this.RefreshTheme(); };
            this.MouseMove += (sender, e) => { this.hilightPanel.Visible = true; };
            this.hilightPanel.MouseLeave += (sender, e) => { this.hilightPanel.Visible = false; };
            this.hilightPanel.MouseClick += (sender, e) => { var p = this.GetLauncherPanel(); if (p != null) p.SetFocus(this); };
            this.hilightPanel.MouseClick += (sender, e) => { this.OnMouseClick(e); };
            this.hilightPanel.MouseDown += (sender, e) => { this.OnMouseDown(e); };
            this.hilightPanel.Click += (sender, e) => { this.OnClick(e); };


            // テーマの適用
            this.RefreshTheme();
        }

        
        // 非公開メソッド

        private void _initializeControls()
        {
            this.BackColor = Color.Gray;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Cursor = Cursors.Hand;

            // ハイライトパネル
            this.hilightPanel = new NormalScaleableColorPanel(0, 0, this.DefaultControlSize.Width, this.DefaultControlSize.Height, Color.FromArgb(128, 255, 255, 255));
            this.hilightPanel.Cursor = Cursors.Hand;
            this.hilightPanel.Visible = false;
            this.Controls.Add(this.hilightPanel);
        }

        private string _getText()
        {
            return this.text;
        }

        private void _setText(string value)
        {
            this.text = value;
            this.RefreshTheme();
        }

        private Image _getBackgroundImage()
        {
            return this.backgroundImage;
        }

        private void _setBackgroundImage(Image image)
        {
            this.backgroundImage = image;
            base.BackgroundImage = new Bitmap(image, this.Size);
            this._drawText();
        }

        private void _drawText()
        {
            Graphics g = Graphics.FromImage(base.BackgroundImage);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            Font font = new Font(this.Font.FontFamily, (this.Font.Size * this.currentScale), this.Font.Style);

            SizeF m = g.MeasureString(this.Text, font);
            PointF p = new PointF((this.Size.Width / 2) - (m.Width / 2), (this.Size.Height / 2) - (m.Height / 2));
            g.DrawString(this.Text, font, new SolidBrush(this.ForeColor), p);

            font.Dispose();
        }

        
        
        // 公開メソッド

        /// <summary>
        /// コントロールを現在のテーマで再描画します。
        /// </summary>
        public override void RefreshTheme()
        {
            // 先に処理
            base.RefreshTheme();
            
            // テーマの適用
            // テーマが取得できない場合 → 終わり
            LauncherTheme theme = this.GetLauncherTheme();
            if (theme == null)
            {
                //Console.WriteLine("テーマ取得失敗");
                return;
            }

            LauncherButtonTheme bt = theme.ButtonTheme;
            
            this.focusBorderColor = bt.FocusBorderColor;

            if (this.applyThemesHeight)
            {
                // テーマが推奨する高さを使用する
                this.SetDefaultControlSize(new Size(this.Width, bt.RecommendedHeight));
                //this.ChangeScale(this.currentScale);
            }

            // 背景描画
            base.BackgroundImage = new Bitmap(this.Width, this.Height);
            
            int leftw = (int)((float)bt.LeftPaddingSize * this.currentScale);
            int rightw = (int)((float)bt.RightPaddingSize * this.currentScale);

            Graphics g = Graphics.FromImage(base.BackgroundImage);
            g.DrawImage(bt.CenterDecoration, leftw, 0, this.Width, this.Height);
            g.DrawImage(bt.LeftDecoration, 0, 0, leftw, this.Height);
            g.DrawImage(bt.RightDecoration, this.Width - rightw, 0, rightw, this.Height);

            g.Dispose();

            this._drawText();
        }

        public override void RefreshFocusState()
        {
            // フォーカスがある場合は、テーマのイメージを描画し、
            // その上にフォーカスを表すイメージを描画
            if (this.LauncherControlFocused)
            {
                //Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

                this.RefreshTheme();

                float borderWidth = (float)((double) 3 * (double)this.currentScale);
                Graphics g = Graphics.FromImage(base.BackgroundImage);
                Pen p = new Pen(this.focusBorderColor, borderWidth);
                //p = Pens.Black;
                g.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                g.Dispose();
                //this.OnParentBackgroundImageChanged(new EventArgs());
            }
            else
            {
                this.RefreshTheme();
            }

            // 子コントロールへも適用
            base.RefreshFocusState();
        }
    }
}
