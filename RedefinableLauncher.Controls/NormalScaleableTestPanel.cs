using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// IScaleableControlを実装するコンストラクタで位置と大きさを固定するタイプの単色表示コントロールです。
    /// </summary>
    public sealed class NormalScaleableColorPanel : VariableScaleableControl
    {
        /// <summary>
        /// 新しいNormalScaleableTestPanelクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="location">デフォルトスケール時の位置</param>
        /// <param name="size">デフォルトスケール時のサイズ</param>
        /// <param name="color">色</param>
        public NormalScaleableColorPanel(Point location, Size size, Color color)
            : base(location, size)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// 新しいNormalScaleableTestPanelクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public NormalScaleableColorPanel(int left, int top, int width, int height, Color color)
            : this(new Point(left, top), new Size(width, height), color)
        {
            // 実装なし
        }
    }
}
