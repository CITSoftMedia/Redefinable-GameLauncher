using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.Controls.Design;


namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// LauncherPanel上に配置される標準的なコントロールを表す抽象クラスです。
    /// </summary>
    public abstract class NormalScaleableControl : UserControl, IScaleableControl
    {
        // 非公開フィールド
        private bool callOwnersKeyEvents;


        // 限定公開フィールド (protected)
        
        /// <summary>
        /// コントロールの現在の比率を格納します。
        /// </summary>
        protected float currentScale;

        /// <summary>
        /// このコントロールでキー入力イベントを検知した際に、親コントロールでもイベントを発生させるかどうかを示す値を取得・設定します。
        /// </summary>
        protected bool CallOwnersKeyEvents
        {
            get { return this.callOwnersKeyEvents; }
            set { this.callOwnersKeyEvents = value; }
        }
        

        // 公開フィールド・プロパティ

        /// <summary>
        /// コントロールの初期の大きさを取得します。
        /// </summary>
        public abstract Point DefaultControlLocation { get; }

        /// <summary>
        /// コントロールの初期の大きさを取得します。
        /// </summary>
        public abstract Size DefaultControlSize { get; }

        /// <summary>
        /// コントロールの現在の比率を取得します。ChangeScaleによりスケールを変更できます。
        /// </summary>
        public float CurrentScale
        {
            get { return this.currentScale; }
        }

        /// <summary>
        /// 上方のコントロールを取得します。
        /// </summary>
        public abstract IScaleableControl UpControl { get; }
        
        /// <summary>
        /// 下方のコントロールを取得します。
        /// </summary>
        public abstract IScaleableControl DownControl { get; }
        
        /// <summary>
        /// 左方のコントロールを取得します。
        /// </summary>
        public abstract IScaleableControl LeftControl { get; }
        
        /// <summary>
        /// 右方のコントロールを取得します。
        /// </summary>
        public abstract IScaleableControl RightControl { get; }

        /// <summary>
        /// LauncherPanelのFocuedControlの値から、現在このコントロールにフォーカスがあるかどうかを取得します。
        /// </summary>
        public virtual bool LauncherControlFocused
        {
            get { return this._isLauncherControlFocused(); }
        }



        // 公開イベント

        /// <summary>
        /// ChangeScaleメソッドが実行された際に発生します。
        /// </summary>
        public event ScaleChangedEventHandler ScaleChanged;

        
        // コンストラクタ
        
        /// <summary>
        /// 新しいNormalScaleableControlクラスのインスタンスを初期化します。
        /// </summary>
        protected NormalScaleableControl()
        {
            // データフィールドの初期化
            this.currentScale = 1.0f;
            this.callOwnersKeyEvents = true;
            
            // コントロールの初期化
            this.DoubleBuffered = true;

            // イベントハンドラの初期化
            this.ScaleChanged = (e, sender) => { };

            // イベントの追加
            this.PreviewKeyDown += NormalScaleableControl_PreviewKeyDown;
            this.PreviewKeyDown += (sender, e) => { Console.WriteLine("{0}:{1}", this, e.KeyCode); };
        }


        // 非公開メソッド
        
        /// <summary>
        /// 現在このコントロールにフォーカスがあるかどうかをLauncherPanelに問い合わせます。
        /// </summary>
        private bool _isLauncherControlFocused()
        {
            LauncherPanel lp = this.GetLauncherPanel();
            if (lp == null)
                return false;

            return this == lp.FocuedControl;
        }
        

        // 非公開メソッド :: コントロールイベント

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NormalScaleableControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Object parent = this.Parent;

            if (this.callOwnersKeyEvents || parent != null)
            {
                // リフレクションを使用して強制的にイベント処理を実行
                // → 非常に強引な方法です。良い子の皆は真似しないでね！

                try
                {
                    // Controls.OnPreviewKeyDown (protected) を呼び出す
                    // その際、メソッドを呼び出すメソッドには parent を指定する
                    // → parent.OnPreviewKeyDown() を本来呼び出せない場所 (クラスの外側) から無理やり呼び出す
                    parent.GetType().InvokeMember(
                        "OnPreviewKeyDown",                             // メソッド名
                        System.Reflection.BindingFlags.InvokeMethod |   // 呼び出しの種類 (実行、非公開、インスタンスから呼び出す動的メソッド)
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance,
                        null,                                           // バインダ (デフォルトのためnull)
                        parent,                                         // 動的メソッドを呼び出すインスタンス
                        new object[] { e });                            // 引数 (このPreviewKeyDownの引数をそのまま渡す)
                }
                catch (MissingMethodException)
                {
                    // メソッドが見つからなかった
                    // →諦める
                }
            }
        }


        // 限定公開メソッド
        
        /// <summary>
        /// このコントロールが所属しているLauncherPanelを取得します。
        /// もし、このコントロールがLauncherPanel上に配置されていない場合は、nullを返します。
        /// </summary>
        /// <returns></returns>
        protected LauncherPanel GetLauncherPanel()
        {
            try
            {
                Control control = this;
                while (control != null)
                {
                    //Console.WriteLine(control.ToString());
                    control = control.Parent;
                    if (control is LauncherPanel)
                        return (LauncherPanel) control;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LauncherPanelの検出に失敗しました。", ex);
            }

            return null;
        }

        /// <summary>
        /// 現在のScaleの値を使用して、Size originのスケーリング後のサイズを計算します。
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        protected Size CalcurateScaledSize(Size origin)
        {
            float w = origin.Width;
            float h = origin.Height;

            w *= this.currentScale;
            h *= this.currentScale;

            return new Size((int) w, (int) h);
        }


        // 公開メソッド

        /// <summary>
        /// DefaultPoint、DefaultSizeを元に、scaleへスケールを変更します。
        /// このメソッドの最後でScaleChangedを発生します。オーバーライドする際は、ScaleChangedの発生処理に注意してください。
        /// </summary>
        /// <param name="scale"></param>
        public virtual void ChangeScale(float scale)
        {
            this.currentScale = scale;

            // 自分のサイズと大きさを調整
            this.Width = (int) ((float) this.DefaultControlSize.Width * scale);
            this.Height = (int) ((float) this.DefaultControlSize.Height * scale);
            this.Left = (int) ((float) this.DefaultControlLocation.X * scale);
            this.Top = (int) ((float) this.DefaultControlLocation.Y * scale);

            // IScaleableControlを実装する子コントロールのみChangeScale呼び出し
            foreach (Control control in this.Controls)
            {
                if (control is IScaleableControl)
                {
                    IScaleableControl scaleableControl = (IScaleableControl)control;
                    scaleableControl.ChangeScale(scale);
                }
            }

            // イベントの発生
            this.ScaleChanged(this, new ScaleChangedEventArgs());
        }

        /// <summary>
        /// 現在のテーマに合わせてコントロールを再描画します。
        /// </summary>
        public abstract void RefreshTheme();

        /// <summary>
        /// LauncherPanelのFocuedControlが変化した際に自動的に実行され、もし、このコントロールにフォーカスが当てられた際は、フォーカス処理を実行します。
        /// また、コントロール内部に配置されているチャイルドコントロールでもこのメソッドを実行するように実装してください。
        /// </summary>
        public abstract void RefreshFocusState();
    }

    /// <summary>
    /// コンストラクタで位置とサイズを固定するタイプのIScaleableControl実装コントロールです。
    /// </summary>
    public class VariableScaleableControl : NormalScaleableControl
    {
        // 非公開フィールド
        private Point defaultControlLocation;
        private Size defaultControlSize;

        private IScaleableControl upControl;
        private IScaleableControl downControl;
        private IScaleableControl leftControl;
        private IScaleableControl rightControl;

        
        // 公開フィールド

        /// <summary>
        /// コントロールのデフォルトスケール時の位置を取得します。
        /// </summary>
        public override Point DefaultControlLocation
        {
            get { return this.defaultControlLocation; }
        }

        /// <summary>
        /// コントロールのデフォルトスケール時の大きさを取得します。
        /// </summary>
        public override Size DefaultControlSize
        {
            get { return this.defaultControlSize; }
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


        // コンストラクタ

        /// <summary>
        /// 新しいVariableScaleableControlクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="defaultControlLocation"></param>
        /// <param name="defaultControlSize"></param>
        public VariableScaleableControl(Point defaultControlLocation, Size defaultControlSize)
        {
            this.defaultControlLocation = defaultControlLocation;
            this.defaultControlSize = defaultControlSize;

            this.Location = defaultControlLocation;
            this.Size = defaultControlSize;

            this.upControl = this;
            this.downControl = this;
            this.leftControl = this;
            this.rightControl = this;
        }


        // 限定公開メソッド
        
        /// <summary>
        /// このコントロールが所属しているLauncherPanelのテーマを取得します。
        /// もし、このコントロールがLauncherPanel上に配置されていない場合は、nullを返します。
        /// </summary>
        /// <returns></returns>
        protected LauncherTheme GetLauncherTheme()
        {
            LauncherPanel p = this.GetLauncherPanel();
            if (p != null)
                return p.Theme;
            
            return null;
        }

        /// <summary>
        /// DefaultControlSizeの値を変更します。推奨されません。
        /// </summary>
        /// <param name="size"></param>
        /// <remarks>テーマ付きコントロールでテーマに合わせたサイズに変更するための実装です。</remarks>
        protected void SetDefaultControlSize(Size size)
        {
            this.defaultControlSize = size;
        }


        // 公開メソッド

        /// <summary>
        /// コントロールのテーマを現在のテーマで再描画します。
        /// </summary>
        public override void RefreshTheme()
        {
            // 継承先で実装
        }

        /// <summary>
        /// LauncherPanelのFocuedControlが変化した際に自動的に実行され、もし、このコントロールにフォーカスが当てられた際は、フォーカス処理を実行します。
        /// また、コントロール内部に配置されているチャイルドコントロールでもこのメソッドを実行するように実装してください。
        /// </summary>
        public override void RefreshFocusState()
        {
            // 子コントロールで同様の処理を行わせる
            foreach (var c in this.Controls)
                if (c is IScaleableControl)
                    ((IScaleableControl) c).RefreshFocusState();
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
}
