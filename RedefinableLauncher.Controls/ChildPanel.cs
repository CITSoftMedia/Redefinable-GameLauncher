using System;
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
        List<Control> hideControls;


        // 非公開フィールド :: コントロール
        LauncherButton closeButton;


        // 公開フィールド


        // コンストラクタ

        public ChildPanel()
            : base(new Point(0, 0), LauncherPanel.LauncherPanelSize)
        {

        }


        // 非公開メソッド

        private void _initializeControls()
        {

        }

        private void _showPanel()
        {

        }

        private void _hidePanel()
        {

        }
    }
}
