using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.InfoEditor.Core;
using Redefinable.Applications.Launcher.Controls.Design;


namespace Redefinable.Applications.Launcher.InfoEditor.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectModeForm : Form
    {
        // 非公開フィールド
        

        // 非公開フィールド :: コントロール
        private Panel basePanel;
        private Label titleLabel;
        private Button gameCollectionButton;
        private Button genreAndControllersButton;
        private Button configButton;
        

        // 公開フィールド・プロパティ


        // コンストラクタ

        /// <summary>
        /// 新しいSelectModeFormクラスのインスタンスを初期化します。
        /// </summary>
        public SelectModeForm()
        {
            // データフィールドの初期化

            // コントロールの初期化
            this._initializeComponents();
            this._autoLoadBackground();
        }


        // 非公開メソッド

        /// <summary>
        /// このフォーム上に配置される各種コントロールを初期化します。
        /// </summary>
        private void _initializeComponents()
        {
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // basePanel
            // → 背景を薄くするためのパネル
            this.basePanel = new Panel();
            this.basePanel.Location = new Point(20, 20);
            this.basePanel.Width = this.ClientSize.Width - (20 * 2);
            this.basePanel.Height = this.ClientSize.Height - (20 * 2);
            this.basePanel.BackColor = Color.FromArgb(127, Color.White);
            this.Controls.Add(this.basePanel);

            this.titleLabel = new Label();
            this.titleLabel.Text = "Info Editor  -- 作品の追加と削除, 各種コンフィグ";
            this.titleLabel.Location = new Point(10, 10);
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = Color.Transparent;
            this.titleLabel.Font = new Font("MS UI Gothic", 14, FontStyle.Bold);
            this.basePanel.Controls.Add(this.titleLabel);

            this.gameCollectionButton = new Button();
            this.gameCollectionButton.Text = "作品情報の追加と削除";
            this.gameCollectionButton.Location = new Point(50, 80);
            this.gameCollectionButton.Width = this.basePanel.Width - (50 * 2);
            this.gameCollectionButton.Height = 50;
            this.gameCollectionButton.Font = new Font("MS UI Gothic", 14, FontStyle.Bold);
            this.basePanel.Controls.Add(this.gameCollectionButton);

            this.genreAndControllersButton = new Button();
            this.genreAndControllersButton.Text = "ジャンルとコントローラ";
            this.genreAndControllersButton.Location = new Point(50, 190);
            this.genreAndControllersButton.Width = this.basePanel.Width - (50 * 2);
            this.genreAndControllersButton.Height = 50;
            this.genreAndControllersButton.Font = new Font("MS UI Gothic", 14, FontStyle.Bold);
            this.basePanel.Controls.Add(this.genreAndControllersButton);

            this.configButton = new Button();
            this.configButton.Text = "システム設定";
            this.configButton.Location = new Point(50, 300);
            this.configButton.Width = this.basePanel.Width - (50 * 2);
            this.configButton.Height = 50;
            this.configButton.Font = new Font("MS UI Gothic", 14, FontStyle.Bold);
            this.basePanel.Controls.Add(this.configButton);

        }

        /// <summary>
        /// 現在のランチャーのテーマ設定から適当な背景をロードします。
        /// </summary>
        private void _autoLoadBackground()
        {
            // Configからテーマファイルを取得
            string themeDir = ConfigHandler.Settings.ThemeDirectory;
            string themePath = themeDir + "\\" + ConfigHandler.Settings.ThemeFile;
            LauncherTheme currentTheme = LauncherTheme.Load(themePath);

            // ウィンドウの背景をテーマの背景に
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImage = currentTheme.PanelTheme.BackgroundImage;
        }


        // 公開メソッド


        // 静的公開メソッド

        public static int ShowSelecter()
        {
            SelectModeForm smForm = new SelectModeForm();
            int result = -1;
            bool canClose = false;

            smForm.FormClosing += (sender, e) =>
            {
                if (!canClose)
                {
                    result = 0;
                    canClose = true;
                    smForm.Close();
                }
            };

            smForm.gameCollectionButton.Click += (sender, e) =>
            {
                result = 1;
                canClose = true;
                smForm.Close();
            };

            smForm.genreAndControllersButton.Click += (sender, e) =>
            {
                result = 2;
                canClose = true;
                smForm.Close();
            };

            smForm.configButton.Click += (sender, e) =>
            {
                result = 3;
                canClose = true;
                smForm.Close();
            };

            smForm.ShowDialog();

            return result;
        }
    }
}
