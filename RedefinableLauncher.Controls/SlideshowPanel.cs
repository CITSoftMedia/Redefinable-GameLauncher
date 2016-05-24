using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Collections;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;


namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// 画像のスライドショーなどを表示するパネルです。
    /// </summary>
    public class SlideshowPanel : VariableScaleableControl
    {
        // 非公開フィールド
        private SlideshowPanelImageCollection images;
        private Color letterBoxColor;
        private bool running;
        

        // 公開フィールド・プロパティ

        /// <summary>
        /// 表示する画像の一覧を格納するコレクションを取得します。
        /// </summary>
        public SlideshowPanelImageCollection Images
        {
            get { return this.images; }
        }

        /// <summary>
        /// 画像の縦横サイズ調整時に、このコントロールとの縦横比の違いにより発生する隙間の領域の色を取得・設定します。
        /// </summary>
        public Color LetterBoxColor
        {
            get { return this.letterBoxColor; }
            set { this.letterBoxColor = value; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいSlideshowPanelのインスタンスを初期化します。
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public SlideshowPanel(Point location, Size size)
            : base(location, size)
        {
            // データフィールドの初期化
            this.images = new SlideshowPanelImageCollection();
            this.letterBoxColor = Color.Black;
            this.running = false;
        }


        // 公開メソッド
        
        /// <summary>
        /// スライドショーを開始します。
        /// </summary>
        public void Start()
        {
            this.running = true;
        }

        /// <summary>
        /// スライドショーを停止します。
        /// </summary>
        public void Stop()
        {
            this.running = false;
        }
    }

    public class SlideshowPanelImageCollection : NativeEventDefinedList<Image>
    {
        /// <summary>
        /// 現在のコレクションのイメージをすべて指定したサイズに調整し、調整後のイメージのコレクションを取得します。
        /// </summary>
        /// <param name="size"></param>
        /// <param name="letterBoxColor"></param>
        /// <returns></returns>
        public SlideshowPanelImageCollection GetAdjustedImages(Size size, Color letterBoxColor)
        {
            Image colormad = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(colormad);


        }
    }
}
