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
            this.listView.Size = new Size(760, 530);
            this.listView.View = View.Details;
            this.listView.Columns.Add("状態", 100);
            this.listView.Columns.Add("作品No", 100);
            this.listView.Columns.Add("ディレクトリ名", 200);
            this.listView.Columns.Add("作品名", 200);
            this.Controls.Add(this.listView);
        }


        // 公開静的メソッド

        public static Game ShowSelecter(GameGenreCollection allGenres, GameControllerCollection allControllers)
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

            // 表示
            glForm.ShowDialog();
            ldForm.Close();

            return null;
        }
    }
}
