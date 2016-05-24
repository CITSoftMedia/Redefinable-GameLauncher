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

        private int largeChange;


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

        /// <summary>
        /// このスクロールバーが現在示している値を取得します。
        /// </summary>
        public int Value
        {
            get { return this._getValue(); }
            set { this._setValue(value); }
        }

        /// <summary>
        /// 取得または設定に追加したりから減算する値、 Value プロパティ スクロール ボックスがある場合は、大規模な距離を移動します。
        /// </summary>
        public int LargeChange
        {
            get { return this.largeChange; }
            set { this.largeChange = value; }
        }


        // 公開イベント

        public event EventHandler ValueChanged;


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
            this.largeChange = 20;

            // コントロールの初期化
            this._initializeElements();
            this._arrangementElements();

            // イベントデリゲートの初期化
            this.targetControlSizeChangedEventHandler = (sender, e) =>
            {
                // targetLengthを更新する
                this._setTargetLength(-1);
            };
            
            this.ValueChanged += (sender, e) => { this.GetDebugWriter().WriteLine("value = {0}", this._getValue()); };

            // イベントの追加
            this.ScaleChanged += (sender, e) => { this._arrangementElements(); };
            this.SizeChanged += (sender, e) => { this._arrangementElements(); if (this.targetControl != null) this._setTargetLength(-1); };
            this._initializeKnobEvents();
            this._initializeButtonsEvents();
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
                    int knobTop = this.knob.Top;
                    knobTop += e.Y - origin.Y;   // 縦方向のみ移動

                    this.GetDebugWriter().WriteLine("knobTop(計算前): {0}", knobTop);

                    if (knobTop <= 0)
                        knobTop = 0;
                    else if (knobTop + this.knob.Height >= this.tray.Height)
                        knobTop = this.tray.Height - this.knob.Height;
                    this.knob.Top = knobTop;

                    this.ValueChanged(this, new EventArgs());

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
        /// 上下ボタンに対するイベントを初期化します。
        /// </summary>
        private void _initializeButtonsEvents()
        {
            // knobの移動に関する実装は _initializeKnobEvents() を参照
            
            this.upButton.MouseClick += (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // マウスのボタンが押下された
                    // →knobを上に移動
                    int knobTop = this.knob.Top;

                    // 加算
                    knobTop -= this.largeChange;

                    // 壁抜け回避
                    if (knobTop < 0)
                        knobTop = 0;

                    // 設定と適用
                    this.knob.Top = knobTop;
                    this.ValueChanged(this, new EventArgs());
                }
            };

            this.downButton.MouseClick += (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // マウスのボタンが押下された
                    // →knobを上に移動
                    int knobTop = this.knob.Top;

                    // 加算
                    knobTop += this.largeChange;

                    // 壁抜け回避
                    if (knobTop + this.knob.Height >= this.tray.Height)
                        knobTop = this.tray.Height - this.knob.Height;

                    // 設定と適用
                    this.knob.Top = knobTop;
                    this.ValueChanged(this, new EventArgs());
                }
            };
        }

        /// <summary>
        /// 現在のフィールドデータに合わせて、このコントロール上のチャイルドコントロールを再配置します。
        /// </summary>
        private void _arrangementElements()
        {
            // 先に現在の値を記憶
            int currentValue = this._getValue();
            if (currentValue < 0) currentValue = 0;


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


            // 最後に現在の値を再設定
            this._setValue(currentValue);
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
                this.targetLength = this.targetControl.Height - this.Height;

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
                try
                {
                    this.targetControl.SizeChanged -= this.targetControlSizeChangedEventHandler;
                }
                catch
                {
                    // なにもしない
                }

            if (value == null)
            {
                // 削除
                this.targetControl = null;
            }
            else
            {
                // 新規設定・更新
                this.targetControl = value;
                this.targetControl.SizeChanged += this.targetControlSizeChangedEventHandler;
                this._setTargetLength(-1);
            }
        }

        /// <summary>
        /// 現在のKnobのTopを元にValueを算出します。
        /// </summary>
        /// <returns></returns>
        private int _getValue()
        {
            // knobのtopの値が取りうる範囲
            int knobMovableRange = this.tray.Height - this.knob.Height;

            // 現在のknobの位置
            int knobTop = this.knob.Top;

            // 算出した値
            int value = (int)((double) this.targetLength * ((double) knobTop / (double) knobMovableRange));
            
            return value;
        }

        /// <summary>
        /// Knobの位置を値を元に算出し設定します。
        /// </summary>
        /// <param name="value"></param>
        private void _setValue(int value)
        {
            // knobのtopの値が取りうる範囲
            int knobMovableRange = this.tray.Height - this.knob.Height;

            // 計算
            this.knob.Top = (int)((double) knobMovableRange * (double) value / (double) this.targetLength);

            //Console.WriteLine("v={0}, t={1}", value, this.knob.Top);
        }


        // 公開メソッド

        public override void RefreshTheme()
        {
            // 先に処理
            base.RefreshTheme();
            
            LauncherTheme lt = this.GetLauncherTheme();
            if (lt != null)
            {
                // テーマ有効
                ScrollBarTheme theme = lt.ScrollBarTheme;

                this.BackColor = Color.Black;
                this.tray.BackColor = theme.TrayColor;
                this.upButton.BackColor = theme.UpButtonColor;
                this.downButton.BackColor = theme.DownButtonColor;
                this.knob.BackColor = theme.KnobColor;
            }
        }

        public override void RefreshFocusState()
        {



            // 子コントロールへも適用
            base.RefreshFocusState();
        }
    }
}
