using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Redefinable.Applications.Launcher.Controls.DrawingExtensions
{
    public static class SizeExtension
    {
        public static Size GetScaledSize(this Size size, double value)
        {
            return new Size(
                (int)((double) size.Width * value),
                (int)((double) size.Height * value) );
        }

        public static SizeF GetScaledSize(this SizeF size, double value)
        {
            return new SizeF(
                (float)((double) size.Width * value),
                (float)((double) size.Height * value) );
        }
    }

    public static class PointExtension
    {
        public static Point GetScaledPoint(this Point point, double value)
        {
            return new Point(
                (int)((double) point.X * value),
                (int)((double) point.Y * value) );
        }

        public static PointF GetScaledPoint(this PointF point, double value)
        {
            return new PointF(
                (float)((double) point.X * value),
                (float)((double) point.Y * value) );
        }
    }
}
