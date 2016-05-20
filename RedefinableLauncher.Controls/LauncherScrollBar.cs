using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;

using PropertyInfo = System.Reflection.PropertyInfo;
using BindingFlags = System.Reflection.BindingFlags;


namespace Redefinable.Applications.Launcher.Controls
{
    public class LauncherScrollBar : VariableScaleableControl
    {
        // 非公開フィールド
        private int targetLength;
        private Control targetControl;

        private EventHandler targetControlSizeChangedEventHandler;

        private int buttonsDefaultHeight;


        // 非公開フィールド :: コントロール
        private Panel upButton;
        private Panel downButton;
        private Panel tray;
        private Panel knob;
        private Panel knobHighlight;


        // 公開フィールド

        /// <summary>
        /// このスクロールバーで操作するオブジェクトの高さを取得・設定します。
        /// TargetControlがnull以外を示す場合、このプロパティに対する値の設定は無効な操作となります。
        /// </summary>
        public int TargetLength
        {
            get { return this._getTargetLength(); }
            set { this._setTargetLength(value); }
        }

        /// <summary>
        /// このスクロールバーで操作するコントロールオブジェクトを取得・設定します。
        /// このプロパティにnull以外が設定されている場合、TargetLengthはTargetControlのHeightが自動的に適用されます。
        /// </summary>
        public Control TargetControl
        {
            get { return this._getTargetControl(); }
            set { this._setTargetControl(value); }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいLauncherScrollBarクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public LauncherScrollBar(Point location, Size size)
            : base(location, size)
        {
            // データフィールドの初期化
            this.targetLength = 1000;
            this.targetControl = null;
            this.buttonsDefaultHeight = 20;

            // コントロールの初期化
            this._initializeElements();
            this._arrangementElements();

            // イベントデリゲートの初期化
            this.targetControlSizeChangedEventHandler = (sender, e) =>
            {
                // targetLengthを更新する
                this._setTargetLength(-1);
            };

            // イベントの追加
            this.ScaleChanged += (sender, e) => { this._arrangementElements(); };
            this._initializeKnobEvents();
        }


        // 非公開メソッド

        /// <summary>
        /// このコントロール上に配置されるチャイルドコントロールを初期化します。
        /// </summary>
        private void _initializeElements()
        {
            // コントロールの初期化とコレクションへの追加
            this.BackColor = Color.Red;
            
            this.upButton = new Panel();
            this.upButton.Cursor = Cursors.Hand;
            this.Controls.Add(this.upButton);

            this.downButton = new Panel();
            this.downButton.Cursor = Cursors.Hand;
            this.Controls.Add(this.downButton);

            this.tray = new Panel();
            this.Controls.Add(this.tray);
            
            this.knob = new Panel();
            this.tray.Controls.Add(this.knob);

            this.knobHighlight = new Panel();
            this.knobHighlight.BackColor = Color.FromArgb(50, Color.Black);
            this.knobHighlight.Visible = false;
            this.knob.Controls.Add(this.knobHighlight);

            // このコントロール内のすべてのPanelコントロールにかなり強引なやり方でDoubleBufferedプロパティにtrueを設定
            // 良い子の皆は真似しないでね！
            Type panelType = typeof(Panel);
            PropertyInfo property = panelType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (Control c in this.Controls)
            {
                if (c is Panel)
                    // プロパティにtrueを設定
                    property.SetValue(c, true, null);
            }


            // デバッグ用の着色
            this.BackColor = Color.Red;
            this.upButton.BackColor = Color.Green;
            this.downButton.BackColor = Color.Blue;
            this.tray.BackColor = Color.Brown;
            this.knob.BackColor = Color.Yellow;
        }

        /// <summary>
        /// knobに対するイベントを初期化します。
        /// </summary>
        private void _initializeKnobEvents()
        {
            // ハイライトのオンオフ
            this.knob.MouseMove += (sender, e) => { this.knobHighlight.Visible = true; };
            this.knobHighlight.MouseLeave += (sender, e) => { this.knobHighlight.Visible = false; };

            // 移動
            Point origin = new Point();
            
            MouseEventHandler mDown = (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // マウスのボタンが押下された
                    // →位置を記憶
                    origin = new Point(e.X, e.Y);
                }
            };

            MouseEventHandler mMove = (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // マウスが押下されたまま移動
                    this.SuspendLayout();
                    //this.knob.Top = e.Y - origin.Y; // 縦方向のみ移動
                    int knobTop = e.Y - origin.Y;   // 縦方向のみ移動
                    origin = new Point(e.X, e.Y);

                    this.GetDebugWriter().WriteLine("knobTop(計算前): {0}", knobTop);

                    if (knobTop <= 0)
                        knobTop = 0;
                    else if (knobTop + this.knob.Height >= this.tray.Height)
                        knobTop = this.tray.Height - this.knob.Height;

                    this.knob.Top = knobTop;

                    this.GetDebugWriter().WriteLine("knobTop(計算後): {0}", this.knob.Top);
                    
                    this.ResumeLayout();
                }
            };


            //this.knob.MouseDown += mDown;
            //this.knob.MouseMove += mMove;

            this.knobHighlight.MouseDown += mDown;
            this.knobHighlight.MouseMove += mMove;
        }

        /// <summary>
        /// 現在のフィールドデータに合わせて、このコントロール上のチャイルドコントロールを再配置します。
        /// </summary>
        private void _arrangementElements()
        {
            // リサイズと再配置
            this.upButton.Width = this.Width;
            this.downButton.Width = this.Width;

            int buttonsHeight = (int)((float)this.buttonsDefaultHeight * this.CurrentScale);
            this.upButton.Height = buttonsHeight;
            this.downButton.Height = buttonsHeight;
            
            this.downButton.Top = this.Height - this.downButton.Height;

            int trayTop = buttonsHeight;
            int trayHeight = this.Height - (buttonsHeight * 2);
            this.tray.SetBounds(0, trayTop, this.Width, trayHeight);


            // knobの再計算
            int knobLeft = (int)((float)3 * this.CurrentScale);
            int knobWidth = this.Width - (knobLeft * 2);
            int knobHeight = (int)(((double)this.Height / (double)this.targetLength) * (double)this.tray.Height);
            this.GetDebugWriter().WriteLine("knobHeight: {0}", knobHeight);
            this.GetDebugWriter().WriteLine("knobTop: {0}", knob.Top);

            // knob配置
            this.knob.SetBounds(knobLeft, this.knob.Top, knobWidth, knobHeight);
            
            // knobHighlight
            this.knobHighlight.Size = this.knob.Size;
        }

        /// <summary>
        /// targetLengthを返します。
        /// </summary>
        /// <returns></returns>
        private int _getTargetLength()
        {
            return this.targetLength;
        }

        /// <summary>
        /// targetLengthを設定します。targetControlがnullでない場合、InvalidOperationExceptionをスローします。
        /// </summary>
        /// <param name="value"></param>
        private void _setTargetLength(int value)
        {
            if (value != -1 && this.targetControl != null)
                throw new InvalidOperationException("TargetControlが設定されているのに値が設定されようとしていました。TargetControlにもとづき、自動的に値を更新する場合は、-1を設定してください。");

            if (value <= 0 && this.targetControl == null)
                throw new ArgumentOutOfRangeException("value", value, "valueに無効な値が設定されました。");

            if (this.targetControl == null)
                this.targetLength = value;
            else
                this.targetLength = this.targetControl.Height;

            this._arrangementElements();
        }

        private Control _getTargetControl()
        {
            return this.targetControl;
        }

        private void _setTargetControl(Control value)
        {
            if (this.targetControl != null)
                // targetControlがすでに存在する場合、SizeChangedに設定したイベントハンドラを削除する
                this.targetControl.SizeChanged -= this.targetControlSizeChangedEventHandler;

            if (value == null)
            {
                // 削除
                this.targetControl = null;
            }
            else
            {
                // 新規設定・更新
                this.targetControl.SizeChanged += this.targetControlSizeChangedEventHandler;
            }
        }


        // 公開メソッド

        public override void RefreshTheme()
        {
            // 先に処理
            base.RefreshTheme();



        }

        public override void RefreshFocusState()
        {



            // 子コントロールへも適用
            base.RefreshFocusState();
        }
    }
}
