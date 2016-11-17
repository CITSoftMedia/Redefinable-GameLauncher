using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.Informations;

using Redefinable.Applications.Launcher.InfoEditor.Core;


namespace Redefinable.Applications.Launcher.InfoEditor.Forms
{
    public class GameListForm : Form
    {
        // 非公開フィールド
        private ListView listView;
        private Button editButton;
        private Button removeButton;
        private Button closeButton;

        // コンストラクタ

        /// <summary>
        /// 新しいGameListFormクラスのインスタンスを初期化します。
        /// </summary>
        public GameListForm()
        {
            this._initializeControls();
        }


        // 非公開メソッド

        /// <summary>
        /// コントロールを初期化します。
        /// </summary>
        private void _initializeControls()
        {
            // フォームの初期化
            this.Text = "ゲーム作品の追加と削除";
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            // リストビューの初期化
            this.listView = new ListView();
            this.listView.Location = new Point(20, 20);
            this.listView.Size = new Size(760, 510);
            this.listView.View = View.Details;
            this.listView.Font = new Font("MS UI Gothic", 12);
            this.listView.Columns.Add("ディレクトリ名",  200);
            this.listView.Columns.Add("状態", 100);
            this.listView.Columns.Add("作品No", 100);
            this.listView.Columns.Add("作品名", 250);
            this.listView.FullRowSelect = true;
            this.Controls.Add(this.listView);

            // 編集ボタン
            this.editButton = new Button();
            this.editButton.Text = "登録・編集";
            this.editButton.Location = new Point(680, 550);
            this.editButton.Size = new Size(100, 30);
            this.editButton.Enabled = false;
            this.Controls.Add(this.editButton);

            // 削除ボタン
            this.removeButton = new Button();
            this.removeButton.Text = "登録解除";
            this.removeButton.Location = new Point(560, 550);
            this.removeButton.Size = new Size(100, 30);
            this.removeButton.Enabled = false;
            this.Controls.Add(this.removeButton);
            
            // 閉じるボタン
            this.closeButton = new Button();
            this.closeButton.Text = "閉じる";
            this.closeButton.Location = new Point(440, 550);
            this.closeButton.Size = new Size(100, 30);
            this.Controls.Add(this.closeButton);
        }


        // 公開静的メソッド

        public static ShowSelecterResult ShowSelecter(GameGenreCollection allGenres, GameControllerCollection allControllers)
        {
            // ローディング画面の表示
            LoadingForm ldForm = new LoadingForm();
            ldForm.Show();

            // ディレクトリのロード
            ldForm.Message = "ゲーム情報ディレクトリを読み込んでいます...";
            GameFilesDirectory gfDir = new GameFilesDirectory(ConfigHandler.Settings.GameFilesDirectory, allGenres, allControllers);
            Application.DoEvents();

            // 画面生成
            ldForm.Message = "読み込んでいます...";
            GameListForm glForm = new GameListForm();
            Application.DoEvents();

            // 作品情報追加
            foreach (var dir in gfDir.Directories)
            {
                var item = glForm.listView.Items.Add(dir.DirectoryName);
                
                if (dir.CheckInitialized())
                {
                    // 登録済み
                    Game g = dir.Load();
                    item.SubItems.Add("登録済み");
                    item.SubItems.Add(g.DisplayNumber.FullNumber);
                    item.SubItems.Add(g.Title);
                }
                else
                {
                    item.SubItems.Add("未登録");
                    item.SubItems.Add("-");
                    item.SubItems.Add("-");
                }
            }

            // イベント
            bool canClose = false;
            bool userClose = false;
            bool remove = false;
            GameDirectory selectedDirectory = null;

            glForm.listView.Click += (sender, e) =>
            {
                var selected = glForm.listView.SelectedItems;
                if (selected.Count <= 0)
                {
                    glForm.editButton.Enabled = false;
                    glForm.removeButton.Enabled = false;
                    return;
                }

                //MessageBox.Show(selected[0].SubItems.Count.ToString());
                if (selected[0].SubItems[1].Text == "登録済み")
                {
                    // 登録済み
                    glForm.editButton.Enabled = true;
                    glForm.removeButton.Enabled = true;
                }
                else
                {
                    // 未登録
                    glForm.editButton.Enabled = true;
                    glForm.removeButton.Enabled = false;
                }
            };

            glForm.FormClosing += (sender, e) =>
            {
                if (!canClose)
                {
                    // ユーザーが意図的に閉じる
                    userClose = true;
                }
            };

            glForm.editButton.Click += (sender, e) =>
            {
                if (glForm.listView.SelectedItems.Count <= 0)
                {
                    MessageBox.Show("作品を選択してください。");
                    return;
                }

                canClose = true;
                remove = false;
                foreach (var dir in gfDir.Directories)
                {
                    if (dir.DirectoryName == glForm.listView.SelectedItems[0].Text)
                    {
                        selectedDirectory = dir;
                        break;
                    }
                }

                glForm.Close();
            };

            glForm.removeButton.Click += (sender, e) =>
            {
                if (glForm.listView.SelectedItems.Count <= 0)
                {
                    MessageBox.Show("作品を選択してください。");
                    return;
                }

                if (MessageBox.Show("一度削除した登録情報は、再登録しないと復元できません。\n本当に削除してもよろしいですか？", "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;

                canClose = true;
                remove = true;
                foreach (var dir in gfDir.Directories)
                {
                    if (dir.DirectoryName == glForm.listView.SelectedItems[0].Text)
                    {
                        selectedDirectory = dir;
                        break;
                    }
                }

                glForm.Close();
            };

            glForm.closeButton.Click += (sender, e) =>
            {
                glForm.Close();
            };

            // 表示
            ldForm.Close();
            glForm.ShowDialog();

            return new ShowSelecterResult(userClose, remove, selectedDirectory);
        }


        // 公開内部クラス

        public class ShowSelecterResult
        {
            /// <summary>
            /// ユーザーが意図的にフォームを閉じたかどうかを示す値を格納します。
            /// </summary>
            public bool Close
            {
                get;
                set;
            }

            /// <summary>
            /// 編集ではなく削除を希望しているかどうかを示す値を格納します。
            /// </summary>
            public bool Remove
            {
                get;
                set;
            }

            /// <summary>
            /// 選択されたディレクトリインスタンスを格納します。
            /// </summary>
            public GameDirectory Directory
            {
                get;
                set;
            }


            public ShowSelecterResult(bool close, bool remove, GameDirectory gameDir)
            {
                this.Close = close;
                this.Remove = remove;
                this.Directory = gameDir;
            }
        }
    }
}
