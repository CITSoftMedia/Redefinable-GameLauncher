using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls;

namespace Redefinable.Applications.Launcher.ScrollTester
{
    public class MainForm : Form
    {
        // 非公開フィールド

        
        // 非公開フィールド :: コントロール
        private LauncherScrollBar scrollBar;
        private Label label1;

        
        // 公開フィールド


        // コンストラクタ

        /// <summary>
        /// 新しいMainFormクラスのインスタンスを初期化します。
        /// </summary>
        public MainForm()
        {
            this.Text = "hogeee";

            this.scrollBar = new LauncherScrollBar(new Point(0, 0), new Size(30, 100));
            this.Controls.Add(this.scrollBar);
            
            this.label1 = new Label();
            this.label1.Text = "";
            this.label1.Location = new Point(40, 0);
            this.label1.AutoSize = true;
            this.Controls.Add(this.label1);

            this.scrollBar.TargetControl = this.label1;
            
            for (int i = 0; i < 100; i++)
            {
                this.label1.Text += String.Format("試験ラベル: {0:00}行目\n", i);
            }

            this.MainForm_SizeChanged(null, null);

            this.SizeChanged += MainForm_SizeChanged;

            this.scrollBar.ValueChanged += (sender, e) =>
            {
                this.label1.Top = this.scrollBar.Value * (-1);
            };
        }

        
        // 非公開メソッド

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            this.scrollBar.Height = this.ClientSize.Height;
        }


        // 公開メソッド
    }
}
