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
        private Point displayNumberPoint;
        private int displayNumberSize;
        private Color displayNumberColor;
        private string displayNumberFontName;
        private bool displayNumberShadow;
        private bool displayNumberBorder;
        private Color displayNumberBorderColor;

        private Point titlePoint;
        private int titleSize;
        private Color titleColor;
        private string titleFontName;
        private bool titleShadow;
        private bool titleBorder;
        private Color titleBorderColor;

        private Image background;


        // 公開フィールド
        
        /// <summary>
        /// DisplayNumberの文字描画の装飾に関するパラメータを取得します。
        /// </summary>
        public TitleBarThemeTextDrawingOption DisplayNumberDrawingOption
        {
            get
            {
                return new TitleBarThemeTextDrawingOption(
                    this.displayNumberPoint,
                    this.displayNumberSize,
                    this.displayNumberColor,
                    this.displayNumberFontName,
                    this.displayNumberShadow,
                    this.displayNumberBorder,
                    this.displayNumberBorderColor );
            }
        }

        /// <summary>
        /// DisplayNumberの文字描画の装飾に関するパラメータを取得します。
        /// </summary>
        public TitleBarThemeTextDrawingOption TitleDrawingOption
        {
            get
            {
                return new TitleBarThemeTextDrawingOption(
                    this.titlePoint,
                    this.titleSize,
                    this.titleColor,
                    this.titleFontName,
                    this.titleShadow,
                    this.titleBorder,
                    this.titleBorderColor );
            }
        }

        /// <summary>
        /// 背景のイメージを取得します。
        /// </summary>
        public Image Background
        {
            get { return this.background; }
        }


        // コンストラクタ

        
        // 公開メソッド

        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            // headers
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"type", "TitleBarTheme"},
                {"guid", Guid.NewGuid().ToString()},

                {"DisplayNumberPoint-X", this.displayNumberPoint.X.ToString()},
                {"DisplayNumberPoint-Y", this.displayNumberPoint.Y.ToString()},
                {"DisplayNumberSize", this.displayNumberSize.ToString()},
                {"DisplayNumberColor", this.displayNumberColor.ToArgb().ToString()},
                {"DisplayNumberFontName", this.displayNumberFontName},
                {"DisplayNumberShadow", this.displayNumberShadow.ToString()},
                {"DisplayNumberBorder", this.displayNumberBorder.ToString()},
                {"DisplayNumberBorderColor", this.displayNumberBorderColor.ToArgb().ToString()},
                
                {"TitlePoint-X", this.titlePoint.X.ToString()},
                {"TitlePoint-Y", this.titlePoint.Y.ToString()},
                {"TitleSize", this.titleSize.ToString()},
                {"TitleColor", this.titleColor.ToArgb().ToString()},
                {"TitleFontName", this.titleFontName},
                {"TitleShadow", this.titleShadow.ToString()},
                {"TitleBorder", this.titleBorder.ToString()},
                {"TitleBorderColor", this.titleBorderColor.ToArgb().ToString()},
            };
            bc.WriteDictionary(dict, stream);

            // 背景
            MemoryStream ms = new MemoryStream();
            this.background.Save(ms, ImageFormat.Png);
            byte[] buf = ms.ToArray();
            bw.Write(bc.GetBytes((UInt64) buf.Length));
            bw.Write(buf);
        }


        // 公開静的メソッド

        /// <summary>
        /// 指定したストリームからロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static TitleBarTheme Load(Stream stream)
        {
            TitleBarTheme result = new TitleBarTheme();

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            bc.ReadDictionary(headers, stream);

            if (headers["type"] != "TitleBarTheme")
                throw new NotSupportedException("TitleBarThemeの読み込みに失敗しました。ヘッダの値が不正です。");

            result.displayNumberPoint = new Point(
                Int32.Parse(headers["DisplayNumberPoint-X"]),
                Int32.Parse(headers["DisplayNumberPoint-Y"]));
            result.displayNumberSize = Int32.Parse(headers["DisplayNumberSize"]);
            result.displayNumberColor = Color.FromArgb(Int32.Parse(headers["DisplayNumberColor"]));
            result.displayNumberFontName = headers["DisplayNumberFontName"];
            result.displayNumberShadow = Boolean.Parse(headers["DisplayNumberShadow"]);
            result.displayNumberBorder = Boolean.Parse(headers["DisplayNumberBorder"]);
            result.displayNumberBorderColor = Color.FromArgb(Int32.Parse(headers["DisplayNumberBorderColor"]));

            result.titlePoint = new Point(
                Int32.Parse(headers["TitlePoint-X"]),
                Int32.Parse(headers["TitlePoint-Y"]));
            result.titleSize = Int32.Parse(headers["TitleSize"]);
            result.titleColor = Color.FromArgb(Int32.Parse(headers["TitleColor"]));
            result.titleFontName = headers["TitleFontName"];
            result.titleShadow = Boolean.Parse(headers["TitleShadow"]);
            result.titleBorder = Boolean.Parse(headers["TitleBorder"]);
            result.displayNumberBorderColor = Color.FromArgb(Int32.Parse(headers["TitleBorderColor"]));

            ulong imageBufLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            MemoryStream imageStream = new MemoryStream(br.ReadBytes((int) imageBufLen));
            result.background = (Image)Image.FromStream(imageStream).Clone();
            imageStream.Close();
            imageStream.Dispose();

            return result;
        }

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static TitleBarTheme GetSampleTheme()
        {
            TitleBarTheme result = new TitleBarTheme();

            Size defSize = TitleBar.DefaultTitleBarSize;
            Image img = new Bitmap(defSize.Width, defSize.Height);
            Graphics g = Graphics.FromImage(img);
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), 0, 0, img.Width, img.Height);
            g.Dispose();

            result.background = img;

            result.displayNumberPoint = new Point(10, 10);
            result.displayNumberSize = 37;
            result.displayNumberColor = Color.LightGray;
            result.displayNumberFontName = "MS Gothic";
            result.displayNumberShadow = true;
            result.displayNumberBorder = true;
            result.displayNumberBorderColor = Color.Black;

            result.titlePoint = new Point(70, 10);
            result.titleSize = 40;
            result.titleColor = Color.White;
            result.titleFontName = "MS UI Gothic";
            result.titleShadow = true;
            result.titleBorder = true;
            result.titleBorderColor = Color.Black;

            return result;
        }

        /// <summary>
        /// コントロールでデフォルトとして使用するデバッグのテーマ (このテーマが使用されているということは、テーマが正常に読み込まれていないということです)
        /// </summary>
        /// <returns></returns>
        public static TitleBarTheme GetSampleThemeForInit()
        {
            TitleBarTheme result = GetSampleTheme();
            result.displayNumberColor = Color.Red;
            result.displayNumberBorderColor = Color.Blue;
            result.titleColor = Color.Red;
            result.titleBorderColor = Color.Blue;

            return result;
        }
    }

    public struct TitleBarThemeTextDrawingOption
    {
        // 非公開フィールド
        private Point point;
        private int size;
        private Color color;
        private string fontName;
        private bool shadow;
        private bool border;
        private Color borderColor;

        
        // 公開フィールド・プロパティ

        /// <summary>
        /// 描画位置を表すPointを取得します。
        /// </summary>
        public Point Point
        {
            get { return this.point; }
        }

        /// <summary>
        /// 描画時のフォントサイズを表す値を取得します。
        /// </summary>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// 描画時の前景色を表すColorを取得します。
        /// </summary>
        public Color Color
        {
            get { return this.color; }
        }

        /// <summary>
        /// 描画時に使用するフォントの名前を取得します。
        /// </summary>
        public string FontName
        {
            get { return this.fontName; }
        }

        /// <summary>
        /// 描画時に文字装飾として影を使用するかどうかを示す値を取得します。
        /// </summary>
        public bool Shadow
        {
            get { return this.shadow; }
        }

        /// <summary>
        /// 描画時に文字装飾として縁取り線を使用するかどうかを示す値を取得します。
        /// </summary>
        public bool Border
        {
            get { return this.border; }
        }

        /// <summary>
        /// 描画時に文字装飾として使用する縁取り線の色を表すColorを取得します。
        /// </summary>
        public Color BorderColor
        {
            get { return this.borderColor; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいTitleBarThemeTextOption構造体のインスタンスを初期化します。
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="fontName"></param>
        /// <param name="shadow"></param>
        /// <param name="border"></param>
        /// <param name="borderColor"></param>
        public TitleBarThemeTextDrawingOption(Point point, int size, Color color, string fontName, bool shadow, bool border, Color borderColor)
        {
            this.point = point;
            this.size = size;
            this.color = color;
            this.fontName = fontName;
            this.shadow = shadow;
            this.border = border;
            this.borderColor = borderColor;
        }
    }
}
