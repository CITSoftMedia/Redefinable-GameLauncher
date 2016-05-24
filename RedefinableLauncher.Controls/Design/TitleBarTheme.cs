using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;

using ImageFormat = System.Drawing.Imaging.ImageFormat;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    /// <summary>
    /// タイトルバーのテーマを格納します。
    /// </summary>
    public class TitleBarTheme : ILauncherThemeElement
    {
        // 非公開フィールド
        private int displayNumberWidth;
        private Image displayNumberBackground;
        private Image titleBackground;


        // 公開フィールド
        


        // コンストラクタ

        
        // 公開メソッド

        public void Save(Stream stream)
        {

        }


        // 公開静的メソッド
    }
}
