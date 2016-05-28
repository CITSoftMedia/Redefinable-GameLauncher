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
        private Timer timer;
        private int imageInterval;
        

        // 非公開フィールド
        private int contextCurrentImageIndex;
        private CurrentStage contextCurrentStage;
        private int contextIntervalCount;
        private float contextTrans;
        private Image contextOutputImage;
        private int contextGCCount;


        // 公開フィールド・プロパティ
        
        /// <summary>
        /// 画像の縦横サイズ調整時に、このコントロールとの縦横比の違いにより発生する隙間の領域の色を取得・設定します。
        /// </summary>
        public Color LetterBoxColor
        {
            get { return this.letterBoxColor; }
            set { this.letterBoxColor = value; }
        }

        /// <summary>
        /// １つの画像を表示し続ける時間を取得・設定します。
        /// </summary>
        public int ImageInterval
        {
            get { return this.imageInterval; }
            set { this.imageInterval = value; }
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
            this.imageInterval = 50;

            this._initializeTimer();


            // イメージコレクションを初期化 (初期イメージを２つ追加しておく)
            Image image = new Bitmap(this.Width, this.Height);
            using (var g = Graphics.FromImage(image))
            {
                g.FillRectangle(new SolidBrush(this.letterBoxColor), 0, 0, this.Width, this.Height);
            }
            this.images.Add(image);
            this.images.Add(image);


            // コントロールの初期化
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        

        // 非公開メソッド

        private void _initializeTimer()
        {
            this.timer = new Timer();
            this.timer.Interval = 120;
            this.timer.Tick += Timer_Tick;

            this._initializeContext();
        }

        private void _initializeContext()
        {
            this.contextCurrentImageIndex = 0;
            this.contextCurrentStage = CurrentStage.View;
            this.contextIntervalCount = 0;
            this.contextTrans = 0;
            this.contextOutputImage = new Bitmap(this.Width, this.Height);
            this.contextGCCount = 0;
        }

        /// <summary>
        /// 現在の画像に別の画像を透過合成した画像を生成します。
        /// </summary>
        /// <param name="back">背景となる画像</param>
        /// <param name="image">合成する画像</param>
        /// <param name="trans">合成する画像の不透明度</param>
        /// <returns></returns>
        private Image _imageOverray(Image back, Image image, float trans)
        {
            //Bitmap backBmp = (Bitmap) this.ArrangeImage(back);
            //Bitmap imageBmp = (Bitmap) this.ArrangeImage(image);

            Bitmap backBmp = (Bitmap) back.Clone();
            Bitmap imageBmp = (Bitmap) image;
            
            Graphics g = Graphics.FromImage(backBmp);

            System.Drawing.Imaging.ColorMatrix cm = 
                new System.Drawing.Imaging.ColorMatrix();

            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix33 = trans;
            cm.Matrix44 = 1;

            System.Drawing.Imaging.ImageAttributes imgAtt =
                new System.Drawing.Imaging.ImageAttributes();
            imgAtt.SetColorMatrix(cm);

            g.DrawImage(
                imageBmp, 
                new Rectangle(0, 0, imageBmp.Width, imageBmp.Height), 
                0, 
                0, 
                imageBmp.Width, 
                imageBmp.Height, 
                GraphicsUnit.Pixel, 
                imgAtt );

            g.Dispose();

            return (Image) backBmp.Clone();
        }


        // 非公開メソッド :: イベント系
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.Created)
            {
                // コントロールが作成されていないため続行不能
                this.timer.Stop();
                return;
            }

            Image current = this.images[this.contextCurrentImageIndex];
            Image next = this.images[0];
            if (this.contextCurrentImageIndex + 1 < this.images.Count)
                // まだ次がある
                next = this.images[this.contextCurrentImageIndex + 1];
            
            Graphics g = this.CreateGraphics();

            if (this.contextCurrentStage == CurrentStage.View)
            {
                g.DrawImage(current, 0, 0, this.Width, this.Height);
                this.contextIntervalCount++;

                if (this.contextIntervalCount >= this.imageInterval)
                {
                    // カウント満タン
                    // →Transitioningへ移行
                    this.contextTrans = 0;
                    this.contextCurrentStage = CurrentStage.Transitioning;
                }
            }
            else if (this.contextCurrentStage == CurrentStage.Transitioning)
            {
                Image image = this._imageOverray(current, next, this.contextTrans);
                g.DrawImage(image, 0, 0, this.Width, this.Height);
                image.Dispose();
                
                this.contextTrans += 0.1f;
                this.contextGCCount++;

                if (this.contextGCCount >= 4)
                {
                    GC.Collect();
                    this.contextGCCount = 0;
                }

                if (this.contextTrans >= 1.0f)
                {
                    // 切り替え完了
                    // →Viewへ移行
                    this.contextCurrentImageIndex++;
                    if (this.contextCurrentImageIndex >= this.images.Count)
                        // インデックス値行き過ぎィ！
                        this.contextCurrentImageIndex = 0;
                    this.contextIntervalCount = 0;
                    this.contextCurrentStage = CurrentStage.View;
                }
            }
            else
                throw new NotImplementedException("スライドショーで未定義の遷移段階に移行しました。");

            g.Dispose();
        }


        // 公開メソッド
        
        /// <summary>
        /// スライドショーを開始します。
        /// </summary>
        public void Start()
        {
            this.timer.Start();
        }

        /// <summary>
        /// スライドショーを停止します。
        /// </summary>
        public void Stop()
        {
            this.timer.Stop();
        }

        /// <summary>
        /// スライドショーを初期状態に戻します。停止中にのみ有効です。
        /// </summary>
        public void ClearContext()
        {
            if (this.timer.Enabled)
                throw new InvalidOperationException("ClearContextはスライドショーが停止中の時のみ有効です。");
            this._initializeContext();
        }

        /// <summary>
        /// スライドショーで表示するイメージのコレクションを設定します。停止中にのみ有効です。
        /// </summary>
        /// <param name="images"></param>
        public void SetImages(SlideshowPanelImageCollection images)
        {
            if (this.timer.Enabled)
                throw new InvalidOperationException("ClearContextはスライドショーが停止中の時のみ有効です。");
            if (images.Count == 0)
                throw new ArgumentException("スライドショーには２つ以上の画像を持つコレクションを設定する必要があります。");
            this.images = images.GetAdjustedImages(this.DefaultControlSize, this.letterBoxColor);
        }


        // 内部クラス

        private enum CurrentStage
        {
            View,
            Transitioning,
        }
    }

    public class SlideshowPanelImageCollection : NativeEventDefinedList<Image>
    {
        /// <summary>
        /// 現在のコレクションのイメージをすべて指定したサイズに調整し、調整後のイメージのコレクションを取得します。
        /// </summary>
        /// <param name="size">目標サイズを指定します。</param>
        /// <param name="madColor">リサイズ時にできた隙間の領域の色を指定します。</param>
        /// <returns></returns>
        public SlideshowPanelImageCollection GetAdjustedImages(Size size, Color madColor)
        {
            Graphics g;

            int dw = size.Width;
            int dh = size.Height;

            SlideshowPanelImageCollection collection = new SlideshowPanelImageCollection();

            foreach (Image image in this)
            {
                double hi;
                double imagew = image.Width;
                double imageh = image.Height;

                if ((dh / dw) <= (imageh / imagew))
                {
                    hi = dh / imageh;
                }
                else
                {
                    hi = dw / imagew;
                }
                int w = (int)(imagew * hi);
                int h = (int)(imageh * hi);

                Bitmap result = new Bitmap(w, h);
                g = Graphics.FromImage(result);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, result.Width, result.Height);
                g.Dispose();

                Console.WriteLine("Resize: {0:000}x{1:000} => {2:000}x{3:000}", image.Width, image.Height, result.Width, result.Height);

                Bitmap result2 = new Bitmap(dw, dh);
                g = Graphics.FromImage(result2);
                g.FillRectangle(new SolidBrush(madColor), 0, 0, result2.Width, result2.Height);
                g.DrawImageUnscaled(result, (result2.Width / 2) - (result.Width / 2), (result2.Height / 2) - (result.Height / 2));
                g.Dispose();

                collection.Add(result2);
            }

            return collection;
        }
    }
}
