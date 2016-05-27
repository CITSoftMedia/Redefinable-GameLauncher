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
        ICollection<Control> hiddenControls;
        //ICollection<SlideshowPanel> stoppedSlideshowPanels;
        Color childPanelBackColor;
        
        LauncherPanel ParentPanel
        {
            get { return (LauncherPanel) this.Parent; }
        }


        // 非公開フィールド :: コントロール
        LauncherButton closeButton;


        // 公開フィールド

        public Color ChildPanelBackColor
        {
            get { return this._getChildPanelBackColor(); }
            set { this._setChildPanelBackColor(value); }
        }

        
        // 公開イベント

        /// <summary>
        /// このChildPanelが非表示になった時に発生します。
        /// </summary>
        public event EventHandler ChildPanelClosed;

        /// <summary>
        /// 背景を再描画した時に発生します。
        /// </summary>
        public event EventHandler DrawnBackground;


        // コンストラクタ

        public ChildPanel()
            : base(new Point(0, 0), LauncherPanel.LauncherPanelSize)
        {
            // データフィールドの初期化
            this.hiddenControls = new List<Control>();
            this.childPanelBackColor = Color.FromArgb(180, Color.Black);

            // コントロールの開始
            this._initializeControls();

            // イベントデリゲートの初期化
            this.ChildPanelClosed = (sender, e) => { };
            this.DrawnBackground = (sender, e) => { };

            // イベントの追加
            this.ControlAdded += ChildPanel_ControlAdded;
            this.closeButton.Click += (sender, e) => { this.ChildPanelHide(); };
        }
        

        // 非公開メソッド

        private void _initializeControls()
        {
            this.BackColor = Color.Black;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Visible = false;
            
            this.closeButton = new LauncherButton(new Point(930, 740), new Size(120, 40));
            this.closeButton.Text = "閉じる";
            this.Controls.Add(this.closeButton);
        }

        private Color _getChildPanelBackColor()
        {
            return this.childPanelBackColor;
        }

        public void _setChildPanelBackColor(Color value)
        {
            if (this.Visible)
                throw new InvalidOperationException("ChildPanel表示中にChildPanelBackColorを変更することはできません。一度非表示にしてください。");
            this.childPanelBackColor = value;
        }

        private void _showPanel()
        {
            this.Parent.SuspendLayout();
            this._drawBack();
            this.Visible = true;

            foreach (Control c in this.Parent.Controls)
            {
                if (c.Visible && c != this)
                {
                    c.Visible = false;
                    this.hiddenControls.Add(c);
                }
            }

            this.Parent.ResumeLayout();
        }

        private void _hidePanel()
        {
            foreach (Control c in this.hiddenControls)
            {
                c.Visible = true;
            }

            this.hiddenControls.Clear();
            this.Visible = false;
            this.ChildPanelClosed(this, EventArgs.Empty);
        }

        /// <summary>
        /// 背景を描画します。このコントロールが表示前で他のコントロールが表示されている間に実行してください。（キャプチャのため）
        /// </summary>
        private void _drawBack()
        {
            Bitmap launcherBmp = new Bitmap(this.ParentPanel.Width, this.ParentPanel.Height);
            this.ParentPanel.DrawToBitmap(launcherBmp, new Rectangle(0, 0, launcherBmp.Width, launcherBmp.Height));

            Graphics g = Graphics.FromImage(launcherBmp);
            g.FillRectangle(new SolidBrush(this.childPanelBackColor),0, 0, launcherBmp.Width, launcherBmp.Height);
            g.Dispose();

            this.BackgroundImage = launcherBmp;
            this.DrawnBackground(this, EventArgs.Empty);
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
            Console.WriteLine("表示しました: {0}x{1}", this.Width, this.Height);
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
