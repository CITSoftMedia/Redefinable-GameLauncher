using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;


namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// ランチャーのメインパネル機能を提供します。
    /// </summary>
    public class LauncherPanel : UserControl, IScaleableControl
    {
        // 非公開フィールド
        private float currentScale;
        private LauncherTheme theme;
        private int focusIndex;


        // 非公開フィールド :: コントロール
        


        // 公開フィールド

        /// <summary>
        /// デフォルトスケール時の位置を取得します。
        /// </summary>
        public Point DefaultControlLocation
        {
            get { return new Point(0, 0); }
        }

        /// <summary>
        /// デフォルトスケール時のサイズを取得します。
        /// </summary>
        public Size DefaultControlSize
        {
            get { return new Size(1080, 810); }
        }

        /// <summary>
        /// スケールを取得・設定します。
        /// </summary>
        public float CurrentScale
        {
            get { return this._getScale(); }
            set { this._setScale(value); }
        }

        /// <summary>
        /// テーマを取得・設定します。
        /// </summary>
        public LauncherTheme Theme
        {
            get { return this._getLaucncherTheme(); }
            set { this._setLauncherTheme(value); }
        }

        /// <summary>
        /// 現在LauncherPanel上のどのコントロールにフォーカスがあるかどうかをControlsのインデックス番号で取得します。
        /// </summary>
        public int FocusIndex
        {
            get { return this.focusIndex; }
        }
        

        // 公開フィールド・プロパティ :: 明示的な実装

        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.UpControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.DownControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.LeftControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.RightControl
        {
            get { return this; }
        }


        // 公開イベント

        /// <summary>
        /// スケールが変更された際に発生します。
        /// </summary>
        public event ScaleChangedEventHandler ScaleChanged;


        // コンストラクタ

        /// <summary>
        /// 新しいLauncherPanelクラスのインスタンスを初期化します。
        /// </summary>
        public LauncherPanel()
        {
            // データフィールドの初期化
            this.currentScale = 1.0f;

            // コントロールの初期化
            this._initializeControls();

            // イベントハンドラの初期化
            this.ScaleChanged = (sender, e) => { };

            // イベントの追加
            this.Click += (sender, e) => { this.ChangeScale(this.currentScale -= 0.1f); };
        }


        // 非公開メソッド

        private void _initializeControls()
        {
            // ランチャーパネル
            this.Location = this.DefaultControlLocation;
            this.Size = this.DefaultControlSize;

            // テーマ
            this.theme = new LauncherTheme(); // デバッグ用, サンプルテーマ

            // テストコントロール
            this.Controls.Add(new NormalScaleableColorPanel(new Point(20, 20), new Size(100, 100), Color.Blue));
            this.Controls.Add(new NormalScaleableColorPanel(new Point(140, 50), new Size(100, 100), Color.Green));
            this.Controls.Add(new LauncherButton(new Point(200, 200), new Size(130, 40)));
            this.Controls.Add(new LauncherButton(new Point(200, 250), new Size(200, 40)));

        }
        
        private void _setScale(float value)
        {
            this.currentScale = value;

            // 自身のサイズ変更
            this.Width = (int)((float)this.DefaultControlSize.Width * value);
            this.Height = (int)((float)this.DefaultControlSize.Height * value);
            
            // 子コントロールのサイズ変更
            foreach (Control cont in this.Controls)
            {
                if (cont is IScaleableControl)
                {
                    IScaleableControl scont = (IScaleableControl) cont;
                    scont.ChangeScale(value);
                }
            }

            this.ScaleChanged(this, new ScaleChangedEventArgs());
        }
        
        private float _getScale()
        {
            return this.currentScale;
        }

        private LauncherTheme _getLaucncherTheme()
        {
            return this.theme;
        }

        private void _setLauncherTheme(LauncherTheme value)
        {
            this.theme = value;
            this.RefreshTheme();
        }


        // 公開メソッド

        /// <summary>
        /// スケールを設定します。
        /// </summary>
        /// <param name="scale"></param>
        public void ChangeScale(float scale)
        {
            this._setScale(scale);
        }

        /// <summary>
        /// コントロールのテーマを現在のテーマで再描画します。
        /// </summary>
        public void RefreshTheme()
        {
            foreach (Control cont in this.Controls)
                if (cont is IScaleableControl)
                    ((IScaleableControl) cont).RefreshTheme();
        }

        /// <summary>
        /// FocusIndexが変化した際に実行され、
        /// </summary>
        public void RefreshFocusState()
        {

        }


        // 公開メソッド :: インタフェースの明示的な実装
        
    }
}
