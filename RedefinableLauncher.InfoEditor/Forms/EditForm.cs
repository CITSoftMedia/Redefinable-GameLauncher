using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.Controls;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Informations;

using Redefinable.Applications.Launcher.InfoEditor.Core;


namespace Redefinable.Applications.Launcher.InfoEditor.Forms
{
    public class EditForm : Form
    {
        // 非公開フィールド :: コントロール
        private LauncherPanel launcherPanel;
        private Panel editMenuPanel;
        private Button[] editMenuPanelButtons;
        private Panel[] operationPanel;

        private TextBox basicInfo_numTextBox;
        private TextBox basicInfo_titleTextBox;

        private TextBox description_textBox;


        // コンストラクタ

        public EditForm()
        {
            this._initializeControls();
        }


        // 非公開メソッド

        private void _initializeControls()
        {
            // フォームそのもの
            this.Text = "作品情報の編集";
            this.ClientSize = new Size(850, 640);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            // プレビューパネル
            this.launcherPanel = new LauncherPanel();
            this.launcherPanel.Theme = LauncherTheme.Load(ConfigHandler.Settings.ThemeDirectory + "\\" + ConfigHandler.Settings.ThemeFile);
            this.launcherPanel.Location = new Point(290, 10);
            this.launcherPanel.ChangeScale(0.5f);
            this.Controls.Add(this.launcherPanel);

            // 編集メニューパネル
            this.editMenuPanel = new Panel();
            this.editMenuPanel.Location = new Point(10, 10);
            this.editMenuPanel.Size = new Size(270, 500);
            this.editMenuPanel.AutoScroll = true;
            this.Controls.Add(this.editMenuPanel);

            // 編集メニュー
            this.editMenuPanelButtons = new Button[]
            {
                new Button() { Text = "基本情報" },
                new Button() { Text = "概要" },
                new Button() { Text = "操作説明" },
                new Button() { Text = "対応コントローラー" },
                new Button() { Text = "該当ジャンル" },
                new Button() { Text = "実行ファイル" },
                new Button() { Text = "スクリーンショット" },
                new Button() { Text = "バナー" },
            };

            this.operationPanel = new Panel[this.editMenuPanelButtons.Length];
            for (int i = 0; i < this.editMenuPanelButtons.Length; i++)
            {
                Button target = this.editMenuPanelButtons[i];
                target.Top = i * 90;
                target.Left = 0;
                target.Width = this.editMenuPanel.ClientSize.Width - 30;
                target.Height = 80;
                this.editMenuPanel.Controls.Add(target);

                this.operationPanel[i] = new Panel();
                this.operationPanel[i].Location = new Point(290, 450);
                this.operationPanel[i].Size = new Size(480, 150);
                this.operationPanel[i].BackColor = Color.White;
                this.Controls.Add(this.operationPanel[i]);

                target.Click += (sender, e) =>
                {
                    int count = 0;
                    for (count = 0; count < this.editMenuPanelButtons.Length; count++)
                    {
                        if (target == this.editMenuPanelButtons[count])
                        {
                            this.operationPanel[count].Visible = true;
                            // MessageBox.Show(count + "");
                        }
                        else
                            this.operationPanel[count].Visible = false;
                    }
                };
            }


            // 基本情報
            this.basicInfo_numTextBox = new TextBox();
            this.basicInfo_numTextBox.Location = new Point(10, 10);
            this.basicInfo_numTextBox.Size = new Size(100, 35);
            this.operationPanel[0].Controls.Add(this.basicInfo_numTextBox);
            this.basicInfo_titleTextBox = new TextBox();
            this.basicInfo_titleTextBox.Location = new Point(120, 10);
            this.basicInfo_titleTextBox.Size = new Size(300, 35);
            this.operationPanel[0].Controls.Add(this.basicInfo_titleTextBox);
            this.operationPanel[0].Controls.Add(new Label() { Location = new Point(10, 60), AutoSize = true, Text = "左側に作品番号、右側に作品タイトル" });

            // 概要
            this.operationPanel[1].Controls.Add(new Label() { Location = new Point(10, 10), AutoSize = true, Text = "作品概要を簡単に！！" });
            this.description_textBox = new TextBox();
            this.description_textBox.Location = new Point(10, 40);
            this.description_textBox.Size = new Size(420, 100);
            this.description_textBox.Multiline = true;
            this.operationPanel[1].Controls.Add(this.description_textBox);

            
        }


        // 静的公開メソッド
        
        /// <summary>
        /// ゲーム情報を編集する画面を表示します。
        /// </summary>
        /// <param name="gdir"></param>
        /// <param name="genres"></param>
        /// <param name="controllers"></param>
        public static void ShowEditor(GameDirectory gdir, GameGenreCollection genres, GameControllerCollection controllers)
        {
            EditForm edForm = new EditForm();
            edForm.ShowDialog();
        }
    }
}
