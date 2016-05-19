using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    /// <summary>
    /// LauncherPanel自体のテーマデータを格納します。
    /// </summary>
    public class LauncherPanelTheme
    {
        // 非公開フィールド
        private Image backgroundImage;

        
        // 公開フィールド

        /// <summary>
        /// 背景画像のイメージのインスタンスを取得します。
        /// </summary>
        public Image BackgroundImage
        {
            get { return this.backgroundImage; }
        }


        // コンストラクタ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundImage"></param>
        public LauncherPanelTheme(Image backgroundImage)
        {
            this.SetImage(backgroundImage);
        }


        // 公開メソッド

        /// <summary>
        /// イメージのインスタンスを更新します。すでにこのテーマが使用中の場合は、このボタンテーマを利用しているコントロールで再描画を実施しなくてはなりません。
        /// </summary>
        /// <param name="backgroundImage"></param>
        public void SetImage(Image backgroundImage)
        {
            this.backgroundImage = backgroundImage;
        }


        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static LauncherPanelTheme GetSampleTheme()
        {
            Bitmap bmp = new Bitmap(1080, 810);
            Graphics g = Graphics.FromImage(bmp);
            LinearGradientBrush gb = new LinearGradientBrush(
                g.VisibleClipBounds,
                Color.White,
                Color.Black,
                LinearGradientMode.Vertical);
            g.FillRectangle(Brushes.White, g.VisibleClipBounds);
            g.FillRectangle(gb, g.VisibleClipBounds);

            gb.Dispose();
            g.Dispose();

            return new LauncherPanelTheme(bmp);
        }
    }
}
