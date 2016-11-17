using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.InfoEditor.Forms
{
    public class LoadingForm : Form
    {
        // 非公開フィールド
        private Label messageLabel;


        // 公開プロパティ

        public string Message
        {
            get { return this.messageLabel.Text; }
            set { this.messageLabel.Text = value; }
        }

        // コンストラクタ
        
        /// <summary>
        /// 新しいLoadingFormクラスのインスタンスを初期化します。
        /// </summary>
        public LoadingForm()
        {
            // フォーム自体の設定
            this.Text = "";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(500, 150);

            // ラベルの設置
            this.messageLabel = new Label();
            this.messageLabel.Location = new Point(10, 10);
            this.messageLabel.Text = "";
            this.messageLabel.Font = new Font("MS UI Gothic", 14, FontStyle.Bold);
            this.messageLabel.AutoSize = false;
            this.messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.messageLabel.Dock = DockStyle.Fill;
            this.Controls.Add(this.messageLabel);
        }
    }
}
