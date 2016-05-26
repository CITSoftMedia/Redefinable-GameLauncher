using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.Controls.DrawingExtensions
{
    /// <summary>
    /// Paddingに対して拡張メソッドを提供します。
    /// </summary>
    public static class PaddingExtension
    {
        /// <summary>
        /// コントロールのSizeからPaddingを考慮したClientSizeを算出します。
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Size GetClientSize(this Padding padding, Size size)
        {
            return new Size(
                size.Width - (padding.Left + padding.Right),
                size.Height - (padding.Top + padding.Bottom) );
        }

        public static Point GetClientPoint(this Padding padding, Point point)
        {
            return new Point(
                padding.Left + point.X,
                padding.Left + point.Y );
        }
    }
}
