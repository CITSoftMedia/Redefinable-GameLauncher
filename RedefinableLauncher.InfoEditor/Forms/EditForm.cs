using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.Controls;
using Redefinable.Applications.Launcher.Informations;

using Redefinable.Applications.Launcher.InfoEditor.Core;


namespace Redefinable.Applications.Launcher.InfoEditor.Forms
{
    public class EditForm : Form
    {
        // 非公開フィールド :: コントロール


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
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }


        // 静的公開メソッド
        
        public static void ShowEditor(GameDirectory gdir)
        {
            EditForm edForm = new EditForm();
            edForm.ShowDialog();
        }
    }
}
