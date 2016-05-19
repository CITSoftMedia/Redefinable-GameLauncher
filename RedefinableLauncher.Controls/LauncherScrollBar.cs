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
    public class LauncherScrollBar : VariableScaleableControl
    {
        // 非公開フィールド
        private int maxValue;
        private int value;

        private int buttonsDefaultHeight;


        // 非公開フィールド :: コントロール
        private Panel upButton;
        private Panel downButton;
        private Panel tray;
        private Panel knob;


        // コンストラクタ

        public LauncherScrollBar(Point location, Size size)
            : base(location, size)
        {
            // データフィールドの初期化
            this.maxValue = 100;
            this.value = 0;
            this.buttonsDefaultHeight = 20;

            // コントロールの初期化
            this._initializeElements();
            this._arrangementElements();

            // イベントデリゲートの初期化

            // イベントの追加
            this.ScaleChanged += (sender, e) => { this._arrangementElements(); };
        }


        // 非公開メソッド

        /// <summary>
        /// このコントロール上に配置されるチャイルドコントロールを初期化します。
        /// </summary>
        private void _initializeElements()
        {
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


            // デバッグ用の着色
            Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Violet, Color.RosyBrown };
            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].BackColor = colors[i];
            }
        }

        /// <summary>
        /// 現在のフィールドデータに合わせて、このコントロール上のチャイルドコントロールを再配置します。
        /// </summary>
        private void _arrangementElements()
        {
            this.upButton.Width = this.Width;
            this.downButton.Width = this.Width;

            int buttonsHeight = (int)((float)this.buttonsDefaultHeight * this.CurrentScale);
            this.upButton.Height = buttonsHeight;
            this.downButton.Height = buttonsHeight;

            int trayTop = buttonsHeight;
            int trayHeight = this.Height - (buttonsHeight * 2);

            this.tray.SetBounds(0, trayTop, this.Width, trayHeight);
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
