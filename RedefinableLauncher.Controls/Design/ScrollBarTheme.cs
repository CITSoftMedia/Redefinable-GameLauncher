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
    public class ScrollBarTheme
    {
        // 非公開フィールド
        private Color trayColor;
        private Color upButtonColor;
        private Color downButtonColor;


        // 公開フィールド

        /// <summary>
        /// トレイパネルの背景色を取得します。
        /// </summary>
        public Color TrayColor
        {
            get { return this.trayColor; }
        }

        /// <summary>
        /// 上昇ボタンの背景色を取得します。
        /// </summary>
        public Color UpButtonColor
        {
            get { return this.upButtonColor; }
        }

        /// <summary>
        /// 降下ボタンの背景色を取得します。
        /// </summary>
        public Color DownButtonColor
        {
            get { return this.downButtonColor; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいインスタンスを初期化しますが、メンバ変数は初期化されません。 (内部用)
        /// </summary>
        private ScrollBarTheme()
        {

        }

        /// <summary>
        /// 新しいScrollBarThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="tray"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public ScrollBarTheme(Color tray, Color up, Color down)
        {
            this.trayColor = tray;
            this.upButtonColor = up;
            this.downButtonColor = down;
        }


        // 非公開メソッド


        // 公開メソッド

        
        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static ScrollBarTheme GetSampleTheme()
        {
            ScrollBarTheme result = new ScrollBarTheme();
            result.trayColor = Color.FromArgb(100, 180, 180, 180);
            result.upButtonColor = result.downButtonColor = Color.FromArgb(100, 100, 100, 100);

            return result;
        }
    }
}
