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
        // 非公開フィールド (protected)
        
        /// <summary>
        /// コントロールの現在の比率を格納します。
        /// </summary>
        protected float currentScale;


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

            // コントロールの初期化

            // イベントハンドラの初期化
            this.ScaleChanged = (e, sender) => { };
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
    }

    /// <summary>
    /// コンストラクタで位置とサイズを固定するタイプのIScaleableControl実装コントロールです。
    /// </summary>
    public class VariableScaleableControl : NormalScaleableControl
    {
        // 非公開フィールド
        private Point defaultControlLocation;
        private Size defaultControlSize;

        
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


        // 公開メソッド

        /// <summary>
        /// コントロールのテーマを現在のテーマで再描画します。
        /// </summary>
        public override void RefreshTheme()
        {
            // 継承先で実装
        }
    }
}
