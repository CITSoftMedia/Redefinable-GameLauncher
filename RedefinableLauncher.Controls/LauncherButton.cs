using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Redefinable.Applications.Launcher.Controls
{
    public class LauncherButton : VariableScaleableControl
    {
        // 非公開フィールド
        


        // 非公開フィールド :: コントロール
        private NormalScaleableColorPanel hilightPanel;


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
            this.MouseMove += (sender, e) => { this.hilightPanel.Visible = true; };
            this.hilightPanel.MouseLeave += (sender, e) => { this.hilightPanel.Visible = false; };
        }

        
        // 非公開メソッド

        private void _initializeControls()
        {
            this.BackColor = Color.Gray;
            this.Cursor = Cursors.Hand;

            // ハイライトパネル
            this.hilightPanel = new NormalScaleableColorPanel(0, 0, this.DefaultControlSize.Width, this.DefaultControlSize.Height, Color.FromArgb(128, 255, 255, 255));
            this.hilightPanel.Cursor = Cursors.Hand;
            this.hilightPanel.Visible = false;
            this.Controls.Add(this.hilightPanel);
        }


        // 
    }
}
