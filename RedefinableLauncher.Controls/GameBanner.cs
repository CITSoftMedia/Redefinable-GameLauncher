using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Collections;

using Redefinable.Applications.Launcher;
using Redefinable.Applications.Launcher.Controls.Design;

using PropertyInfo = System.Reflection.PropertyInfo;
using BindingFlags = System.Reflection.BindingFlags;


namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// GameListView内に配置するゲームのバナーを表示します。
    /// </summary>
    public class GameBanner : NormalScaleableControl
    {
        // 非公開フィールド
        private int defaultLeft;
        private int currentTop;
        private Size defaultSize;
        private string text;
        
        private IScaleableControl upControl;
        private IScaleableControl downControl;
        private IScaleableControl leftControl;
        private IScaleableControl rightControl;

        private Image backgroundImage;

        private bool suspendChangeTopOnChangeScale;


        // 非公開フィールド :: コントロール
        private NormalScaleableColorPanel hilightPanel;
        

        // 公開フィールド・プロパティ

        /// <summary>
        /// 
        /// </summary>
        public override string Text
        {
            get { return this._getText(); }
            set { this._setText(value); }
        }

        /// <summary>
        /// デフォルトのLeftと現在のTopからデフォルトの位置を算出します。
        /// </summary>
        public override Point DefaultControlLocation
        {
            get { return new Point(this.defaultLeft, this.currentTop); }
        }

        /// <summary>
        /// 既定のサイズを取得します。
        /// </summary>
        public override Size DefaultControlSize
        {
            get { return this.defaultSize; }
        }

        /// <summary>
        /// このコントロールの縦位置を取得・設定します。この値は常にDefaultControlLocation同様、スケーリングが1.0の場合の値が使用されます。
        /// </summary>
        public int CurrentTop
        {
            get { return this._getCurrentTop(); }
            set { this._setCurrentTop(value); }
        }

        /// <summary>
        /// 上方のコントロールを取得します。
        /// </summary>
        public override IScaleableControl UpControl
        {
            get { return this.upControl; }
        }

        /// <summary>
        /// 下方のコントロールを取得します。
        /// </summary>
        public override IScaleableControl DownControl
        {
            get { return this.downControl; }
        }

        /// <summary>
        /// 左方のコントロールを取得します。
        /// </summary>
        public override IScaleableControl LeftControl
        {
            get { return this.leftControl; }
        }

        /// <summary>
        /// 右方のコントロールを取得します。
        /// </summary>
        public override IScaleableControl RightControl
        {
            get { return this.rightControl; }
        }
        
        /// <summary>
        /// このコントロールで表示するイメージを取得します。nullでない場合、テキストの表示が無効化されます。
        /// </summary>
        public new Image BackgroundImage
        {
            get { return this._getBackgroundImage(); }
            set { this._setBackgroundImage(value); }
        }

        /// <summary>
        /// コントロールによって表示されるテキストのフォントを取得・設定します。サイズは、スケーリングの値が1.0の場合のものを使用します。
        /// </summary>
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// スケーリングの値が更新された際に、このTopの位置変更を抑止するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool SuspendChangeTopOnChangeScale
        {
            get { return this.suspendChangeTopOnChangeScale; }
            set { this.suspendChangeTopOnChangeScale = value; }
        }


        // イベント




        // コンストラクタ

        /// <summary>
        /// 新しいゲームバナークラスのインスタンスを初期化します。
        /// </summary>
        public GameBanner()
        {
            // データフィールドの初期化
            this.defaultLeft = 5;
            this.currentTop = 5;
            this.defaultSize = new Size(220, 50);
            this.text = "";
            this.suspendChangeTopOnChangeScale = true;
            this.Font = new Font("MS UI Gothic", 14f, FontStyle.Bold);

            // 近隣コントロールの設定
            this.upControl = this;
            this.downControl = this;
            this.leftControl = this;
            this.rightControl = this;

            // コントロールの初期化
            this._initializeControls();
            this.RefreshFocusState();

            // イベントの追加
            this.ScaleChanged += (sender, e) => { this.RefreshTheme(); };
            this.MouseMove += (sender, e) => { this.hilightPanel.Visible = true; };
            this.hilightPanel.MouseLeave += (sender, e) => { this.hilightPanel.Visible = false; };
            this.hilightPanel.MouseClick += (sender, e) => { var p = this.GetLauncherPanel(); if (p != null) p.SetFocus(this); };
            this.hilightPanel.MouseClick += (sender, e) => { this.OnMouseClick(e); };
            this.hilightPanel.MouseDown += (sender, e) => { this.OnMouseDown(e); };
            this.hilightPanel.Click += (sender, e) => { this.OnClick(e); };
        }


        // 非公開メソッド

        private void _initializeControls()
        {
            this.ForeColor = Color.White;
            this.BackColor = Color.Gray;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Cursor = Cursors.Hand;
            this.Size = this.DefaultControlSize;

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
            base.OnTextChanged(new EventArgs());
        }

        private Image _getBackgroundImage()
        {
            return this.backgroundImage;
        }

        private void _setBackgroundImage(Image value)
        {
            this.backgroundImage = value;
            this.RefreshTheme(); 
        }
        
        private int _getCurrentTop()
        {
            return this.currentTop;
        }

        private void _setCurrentTop(int value)
        {
            this.currentTop = value;
            this.Top = (int)((double) value * this.currentScale);
        }
        

        // 公開メソッド

        /// <summary>
        /// 現在のテーマを利用して再描画します。
        /// このコントロールではbackgroundImageとtextを使用した再描画が実施されます。
        /// </summary>
        public override void RefreshTheme()
        {
            LauncherButtonTheme bt = LauncherButtonTheme.GetSampleTheme();
            LauncherPanel p = this.GetLauncherPanel();
            if (p != null) bt = p.Theme.ButtonTheme;

            if (this.backgroundImage == null)
            {
                // デフォルトテーマの上にテキストを描画
                base.BackgroundImage = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(base.BackgroundImage);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // テーマ描画
                g.FillRectangle(Brushes.Gray, g.VisibleClipBounds);

                // テキスト描画
                Font f = new Font(this.Font.FontFamily, this.Font.Size * this.currentScale, this.Font.Style);
                SizeF size = g.MeasureString(this.text, f);

                // 縦位置のみを調整して描画
                g.DrawString(this.text, f, new SolidBrush(this.ForeColor), new PointF(10, (this.Height / 2) - (size.Height / 2)));
                g.Dispose();
                f.Dispose();
            }
            else
            {
                // イメージ描画
                base.BackgroundImage = new Bitmap(this.backgroundImage, this.Size);
            }

            if (this.LauncherControlFocused)
            {
                float borderWidth = (float)((double) 3 * (double)this.currentScale);
                Graphics g = Graphics.FromImage(base.BackgroundImage);
                g.DrawRectangle(new Pen(bt.FocusBorderColor, borderWidth), 0, 0, this.Width - 1, this.Height - 1);
                g.Dispose();
            }
        }

        /// <summary>
        /// フォーカスの状態をコントロールへ反映します。
        /// </summary>
        public override void RefreshFocusState()
        {
            this.ChangeScale(this.currentScale);
            this.RefreshTheme();

            // 子コントロールへも適用
            foreach (Control c in this.Controls)
                if (c is IScaleableControl)
                    ((IScaleableControl) c).RefreshFocusState();
        }

        public override void ChangeScale(float scale)
        {
            int top = this.Top;
            base.ChangeScale(scale);

            if (this.suspendChangeTopOnChangeScale)
                this.Top = top;
        }

        /// <summary>
        /// 隣接コントロールを設定します。nullを指定した項目には、このインスタンス自身が設定されます。
        /// </summary>
        /// <param name="up">上方のコントロール</param>
        /// <param name="down">下方のコントロール</param>
        /// <param name="left">左方のコントロール</param>
        /// <param name="right">右方のコントロール</param>
        public void SetNeighborControls(IScaleableControl up, IScaleableControl down, IScaleableControl left, IScaleableControl right)
        {
            if (up == null)
                this.upControl = this;
            else
                this.upControl = up;

            if (down == null)
                this.downControl = this;
            else
                this.downControl = down;

            if (left == null)
                this.leftControl = this;
            else
                this.leftControl = left;


            if (right == null)
                this.rightControl = this;
            else
                this.rightControl = right;
        }
    }

    /// <summary>
    /// GameBannerのコレクション機能を提供します。
    /// </summary>
    public class GameBannerCollection : NativeEventDefinedList<GameBanner>
    {

    }
}
