//
// ランチャーのカスタムコントロールにおけるキーイベントの判定実験
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.Controls.Test_Code
{
    public class TestCode
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // エントリポイント
            TestForm form = new TestForm();
            Application.Run(form);
        }
    }

    /// <summary>
    /// 試験用フォーム
    /// </summary>
    class TestForm : Form
    {
        // 非公開フィールド
        private TestControl testControl;

        // コンストラクタ

        /// <summary>
        /// 新しいTestFormクラスのインスタンスを初期化します。
        /// </summary>
        public TestForm()
        {
            // データフィールドの初期化

            // コントロールの初期化
            this.ClientSize = new Size(1280, 720);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.Black;
            
            this.testControl = new TestControl();
            this.testControl.Location = new Point(10, 10);
            this.testControl.Size = new Size(1260, 700);
            this.Controls.Add(this.testControl);
        }
    }

    /// <summary>
    /// UserControlクラスを継承する試験用カスタムコントロール
    /// </summary>
    class TestControl : UserControl
    {
        /// <summary>
        /// 新しいTestControlクラスのインスタンスを初期化します。
        /// </summary>
        public TestControl()
        {
            // コントロールの初期化
            this.BackColor = Color.Navy;

            // イベントの初期化
            this.PreviewKeyDown += (sender, e) =>
            {
                Console.Write(e.KeyCode);
            };
        }
    }
}
