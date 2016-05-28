using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;
using Redefinable.IO;

using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Ini = Redefinable.IniHandler.IniFile;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    public class DescriptionPanelTheme : ILauncherThemeElement
    {
        // 非公開フィールド
        private Image upperLeft;
        private Image lowerLeft;
        private Image upperRight;
        private Image lowerRight;

        private Image topLine;
        private Image bottomLine;
        private Image leftLine;
        private Image rightLine;
        
        private Image middle;

        private Padding padding;

        private string fontName;
        private int fontSize;
        private Color fontColor;

        private bool fontBorder;
        private Color fontBorderColor;


        // 公開フィールド

        public Image UpperLeft
        {
            get { return this.upperLeft; }
        }

        public Image LowerLeft
        {
            get { return this.lowerLeft; }
        }

        public Image UpperRight
        {
            get { return this.upperRight; }
        }

        public Image LowerRight
        {
            get { return this.lowerRight; }
        }

        public Image TopLine
        {
            get { return this.topLine; }
        }

        public Image BottomLine
        {
            get { return this.bottomLine; }
        }

        public Image LeftLine
        {
            get { return this.leftLine; }
        }

        public Image RightLine
        {
            get { return this.rightLine; }
        }

        public Image Middle
        {
            get { return this.middle; }
        }

        public Padding Padding
        {
            get { return this.padding; }
        }

        public string FontName
        {
            get { return this.fontName; }
        }

        public int FontSize
        {
            get { return this.fontSize; }
        }

        public Color FontColor
        {
            get { return this.fontColor; }
        }

        public bool FontBorder
        {
            get { return this.fontBorder; }
        }

        public Color FontBorderColor
        {
            get { return this.fontBorderColor; }
        }
        

        // コンストラクタ

        /// <summary>
        /// 利用できません。
        /// </summary>
        private DescriptionPanelTheme()
        {
            // 実装なし
        }

        /// <summary>
        /// 新しいDescriptionPanelThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="upperLeft"></param>
        /// <param name="lowerLeft"></param>
        /// <param name="upperRight"></param>
        /// <param name="lowerRight"></param>
        /// <param name="topLine"></param>
        /// <param name="bottomLine"></param>
        /// <param name="leftLine"></param>
        /// <param name="rightLine"></param>
        /// <param name="middle"></param>
        /// <param name="padding"></param>
        public DescriptionPanelTheme(Image upperLeft, Image lowerLeft, Image upperRight, Image lowerRight, Image topLine, Image bottomLine, Image leftLine, Image rightLine, Image middle, Padding padding, string fontName, int fontSize, Color fontColor)
        {
            this.upperLeft = upperLeft;
            this.lowerLeft = lowerLeft;
            this.upperRight = upperRight;
            this.lowerRight = lowerRight;

            this.topLine = topLine;
            this.bottomLine = bottomLine;
            this.leftLine = leftLine;
            this.rightLine = rightLine;

            this.middle = middle;

            this.padding = padding;

            this.fontName = fontName;
            this.fontSize = fontSize;
            this.fontColor = fontColor;
        }


        // 公開メソッド

        /// <summary>
        /// 指定したストリームへ現在このインスタンスが保持している情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            // headers
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                // 基礎情報
                {"type", "DescriptionPanelTheme"},
                {"type-guid", "9BE1FE79-6C25-4AE0-95B3-C883EBDD1A01"},
                {"guid", Guid.NewGuid().ToString()},

                // padding
                {"Padding-Top", this.padding.Top.ToString()},
                {"Padding-Bottom", this.padding.Bottom.ToString()},
                {"Padding-Left", this.padding.Left.ToString()},
                {"Padding-Right", this.padding.Right.ToString()},

                // font
                {"FontName", this.fontName},
                {"FontSize", this.fontSize.ToString()},
                {"FontColor", this.fontColor.ToArgb().ToString()},
                {"FontBorder", this.fontBorder.ToString()},
                {"FontBorderColor", this.fontBorderColor.ToArgb().ToString()},
            };

            // ヘッダの書き込み
            bc.WriteDictionary(headers, stream);


            // 各種画像の書き込み
            MemoryStream ms;
            byte[] buf;
            ulong bufLen;
            
            // upperLeft
            ms = new MemoryStream();
            this.upperLeft.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // lowerLeft
            ms = new MemoryStream();
            this.lowerLeft.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // upperRight
            ms = new MemoryStream();
            this.upperRight.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // lowerRight
            ms = new MemoryStream();
            this.lowerRight.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // topLine
            ms = new MemoryStream();
            this.topLine.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // bottomLine
            ms = new MemoryStream();
            this.bottomLine.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // leftLine
            ms = new MemoryStream();
            this.leftLine.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // rightLine
            ms = new MemoryStream();
            this.rightLine.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);

            // middle
            ms = new MemoryStream();
            this.middle.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);
        }


        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static DescriptionPanelTheme GetSampleTheme()
        {
            DescriptionPanelTheme result = new DescriptionPanelTheme();
            
            result.padding = new Padding(15);
            Padding p = result.padding;
            
            Func<int, int, Color, Bitmap> createColormad = (w, h, c) =>
            {
                Bitmap bmp = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(c), 0, 0, w, h);
                g.Dispose();
                return bmp;
            };
            
            result.upperLeft = createColormad(p.Left, p.Top, Color.DarkGray);
            result.lowerLeft = createColormad(p.Left, p.Bottom, Color.DarkGray);
            result.upperRight = createColormad(p.Right, p.Top, Color.DarkGray);
            result.lowerRight = createColormad(p.Right, p.Bottom, Color.DarkGray);
            
            const int width = 800;
            const int height = 300;
            Size clientSize = p.GetClientSize(new Size(width, height));
            result.topLine = createColormad(p.Left, clientSize.Width, Color.FromArgb(150, Color.Silver));
            result.bottomLine = createColormad(p.Left, clientSize.Width, Color.FromArgb(150, Color.Silver));
            result.leftLine = createColormad(p.Left, clientSize.Height, Color.FromArgb(150, Color.Silver));
            result.rightLine = createColormad(p.Left, clientSize.Height, Color.FromArgb(150, Color.Silver));

            result.middle = new Bitmap(clientSize.Width, clientSize.Height);
            Graphics gr = Graphics.FromImage(result.middle);
            LinearGradientBrush gb = new LinearGradientBrush(
                    gr.VisibleClipBounds,
                    Color.Silver,
                    Color.Gray,
                    LinearGradientMode.Vertical);
                gr.FillRectangle(Brushes.White, gr.VisibleClipBounds);
                gr.FillRectangle(gb, gr.VisibleClipBounds);
                gb.Dispose();

            result.fontName = "MS UI Gothic";
            result.fontSize = 17;
            result.fontColor = Color.White;

            result.fontBorder = true;
            result.fontBorderColor = Color.Black;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DescriptionPanelTheme GetSampleThemeForInit()
        {
            DescriptionPanelTheme theme = DescriptionPanelTheme.GetSampleTheme();
            theme.fontColor = Color.Black;

            return theme;
        }

        /// <summary>
        /// 指定したストリームからロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DescriptionPanelTheme Load(Stream stream)
        {
            DescriptionPanelTheme result = new DescriptionPanelTheme();
            
            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            bc.ReadDictionary(headers, stream);

            if (headers["type"] != "DescriptionPanelTheme")
                throw new NotSupportedException("DescriptionPanelThemeの読み込みに失敗しました。ヘッダの値が不正です。");

            // paddingの読み取り
            result.padding = new Padding(
                Int32.Parse(headers["Padding-Top"]),
                Int32.Parse(headers["Padding-Bottom"]),
                Int32.Parse(headers["Padding-Left"]),
                Int32.Parse(headers["Padding-Right"]) );

            // fontの読み取り
            result.fontName = headers["FontName"];
            result.fontSize = Int32.Parse(headers["FontSize"]);
            result.fontColor = Color.FromArgb(Int32.Parse(headers["FontColor"]));
            result.fontBorder = Boolean.Parse(headers["FontBorder"]);
            result.fontBorderColor = Color.FromArgb(Int32.Parse(headers["FontBorderColor"]));

            
            // 各種画像の読み込み
            ulong imageLen;    // バイト長
            byte[] buf;        // バイト長だけ読み込まれるバッファ
            MemoryStream ms;   // バッファからイメージを作成するのに使用するMemoryStream

            // upperLeft
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.upperLeft = Image.FromStream(ms);

            // lowerLeft
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.lowerLeft = Image.FromStream(ms);

            // upperRight
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.upperRight = Image.FromStream(ms);

            // lowerRight
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.lowerRight = Image.FromStream(ms);

            // topLine
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.topLine = Image.FromStream(ms);

            // bottomLine
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.bottomLine = Image.FromStream(ms);

            // leftLine
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.leftLine = Image.FromStream(ms);

            // rightLine
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.rightLine = Image.FromStream(ms);

            // rightLine
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.middle = Image.FromStream(ms);


            return result;
        }
    }
}
