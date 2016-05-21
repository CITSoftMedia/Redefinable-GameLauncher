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


        // 非公開フィールド :: コントロール
        private NormalScaleableColorPanel hilightPanel;


        // 公開フィールド

        /// <summary>
        /// このLauncherButtonの高さをテーマが推奨するサイズへ設定するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool ApplyThemesHeight
        {
            get { return this.applyThemesHeight; }
            set { this.applyThemesHeight = value; }
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
            

            if (this.applyThemesHeight)
            {
                // テーマが推奨する高さを使用する
                this.SetDefaultControlSize(new Size(this.Width, bt.RecommendedHeight));
                //this.ChangeScale(this.currentScale);
            }

            // 背景描画
            this.BackgroundImage = new Bitmap(this.Width, this.Height);

            Graphics g = Graphics.FromImage(this.BackgroundImage);
            g.DrawImage(bt.CenterDecoration, 0, 0, this.Width, this.Height);

            int leftw = (int)((float)bt.LeftPaddingSize * this.currentScale);
            int rightw = (int)((float)bt.RightPaddingSize * this.currentScale);

            g.DrawImage(bt.LeftDecoration, 0, 0, leftw, this.Height);
            g.DrawImage(bt.RightDecoration, this.Width - rightw, 0, rightw, this.Height);

            g.Dispose();
        }

        public override void RefreshFocusState()
        {
            // フォーカスがある場合は、テーマのイメージを描画し、
            // その上にフォーカスを表すイメージを描画
            if (this.LauncherControlFocused)
            {
                this.RefreshTheme();

                float borderWidth = (float)((double) 3 * (double)this.currentScale);
                Graphics g = Graphics.FromImage(this.BackgroundImage);
                g.DrawRectangle(new Pen(Color.LightBlue, borderWidth), 0, 0, this.Width - 1, this.Height - 1);
                g.Dispose();
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
