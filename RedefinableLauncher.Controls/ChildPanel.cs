using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// LauncherPanel直下に追加され、一時的に手前に表示されるパネルコントロールです。
    /// </summary>
    public class ChildPanel : VariableScaleableControl
    {
        // 非公開フィールド
        ICollection<Control> hideControls;
        
        LauncherPanel ParentPanel
        {
            get { return (LauncherPanel) this.Parent; }
        }


        // 非公開フィールド :: コントロール
        LauncherButton closeButton;


        // 公開フィールド

        
        // 公開イベント

        public event EventHandler ChildPanelClosed;



        // コンストラクタ

        public ChildPanel()
            : base(new Point(0, 0), LauncherPanel.LauncherPanelSize)
        {
            // データフィールドの初期化
            this.hideControls = new List<Control>();

            // コントロールの開始
            this._initializeControls();

            // イベントデリゲートの初期化
            this.ChildPanelClosed = (sender, e) => { };

            // イベントの追加
            this.ControlAdded += ChildPanel_ControlAdded;
            this.closeButton.Click += (sender, e) => { this.ChildPanelHide(); };
        }
        

        // 非公開メソッド

        private void _initializeControls()
        {
            this.BackColor = Color.Black;
            this.Visible = false;
            
            this.closeButton = new LauncherButton(new Point(10, 10), new Size(100, 40));
            this.closeButton.Text = "閉じる";
            this.Controls.Add(this.closeButton);
        }

        private void _showPanel()
        {
            this.Parent.SuspendLayout();
            this.Visible = true;

            foreach (Control c in this.Parent.Controls)
            {
                if (c.Visible)
                {
                    c.Visible = false;
                    this.hideControls.Add(c);
                }
            }

            this.Parent.ResumeLayout();
        }

        private void _hidePanel()
        {
            foreach (Control c in this.hideControls)
            {
                c.Visible = true;
            }

            this.hideControls.Clear();
            this.Visible = false;
        }

        private void _createdCheck()
        {
            if (this.Parent == null)
                throw new InvalidOperationException("この操作は、ChildPanelがLaucnherPanel上に追加される前に実行することはできません。");
        }


        // 非公開メソッド :: イベント
        
        private void ChildPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            // このコントロールはLauncherPanel直下にしか追加できない
            if (!(this.Parent is LauncherPanel))
                throw new InvalidOperationException("ChildPanelはLauncherPanelコントロールの直下以外の場所に追加することはできません。");
        }


        // 公開メソッド

        /// <summary>
        /// このChildPanelを非表示にします。
        /// </summary>
        public void ChildPanelShow()
        {
            this._createdCheck();
            this._showPanel();
        }

        /// <summary>
        /// このChildPanelを表示します。
        /// </summary>
        public void ChildPanelHide()
        {
            this._createdCheck();
            this._hidePanel();
        }
    }
}
