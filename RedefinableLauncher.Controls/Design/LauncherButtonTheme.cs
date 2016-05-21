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
    /// LauncherButtonのテーマを格納します。
    /// </summary>
    public class LauncherButtonTheme : ILauncherThemeElement
    {
        // 非公開フィールド
        private Image leftDecoration;
        private Image centerDecoration;
        private Image rightDecoration;

        private int recommendedHeight;
        private int leftPaddingSize;
        private int rightPaddingSize;


        // 公開フィールド

        /// <summary>
        /// ボタンの左端側の装飾のイメージを取得します。
        /// </summary>
        public Image LeftDecoration
        {
            get { return this.leftDecoration; }
        }

        /// <summary>
        /// ボタンの中央部の装飾のイメージを取得します。
        /// </summary>
        public Image CenterDecoration
        {
            get { return this.centerDecoration; }
        }

        /// <summary>
        /// ボタンの右端側の装飾のイメージを取得します。
        /// </summary>
        public Image RightDecoration
        {
            get { return this.rightDecoration; }
        }
        
        /// <summary>
        /// このテーマが推奨するボタンの高さを取得します。
        /// </summary>
        public int RecommendedHeight
        {
            get { return this.recommendedHeight; }
        }

        /// <summary>
        /// 左端の内部余白サイズを取得します。
        /// </summary>
        public int LeftPaddingSize
        {
            get { return this.leftPaddingSize; }
        }

        /// <summary>
        /// 右端の内部余白サイズを取得します。
        /// </summary>
        public int RightPaddingSize
        {
            get { return this.rightPaddingSize; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいLauncherButtonThemeクラスのインスタンスを初期化します。このコンストラクタの仕様は推奨されません。
        /// </summary>
        public LauncherButtonTheme()
        {
            this.leftDecoration = null;
            this.centerDecoration = null;
            this.rightDecoration = null;

            this.recommendedHeight = 30;

            this.leftPaddingSize = 10;
            this.rightPaddingSize = 10;
        }

        /// <summary>
        /// 新しいLauncherButtonThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="left">左端の装飾イメージ</param>
        /// <param name="center">中央部の装飾イメージ</param>
        /// <param name="right">右端の装飾イメージ</param>
        /// <param name="clone">各インスタンスを複製するかどうか</param>
        public LauncherButtonTheme(Image left, Image center, Image right, bool clone)
        {
            if (clone)
            {
                left = (Image) left.Clone();
                center = (Image) center.Clone();
                right = (Image) right.Clone();
            }

            this.leftDecoration = left;
            this.centerDecoration = center;
            this.rightDecoration = right;
        }

        /// <summary>
        /// 新しいLauncherButtonThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="left">左端イメージ</param>
        /// <param name="center">中央イメージ</param>
        /// <param name="right">右端イメージ</param>
        /// <param name="height">このテーマで推奨されるデフォルトのボタンの高さ</param>
        /// <param name="leftPadding">左端部分の幅 (内部余白)</param>
        /// <param name="rightPadding">右端部分の幅 (内部余白)</param>
        public LauncherButtonTheme(Image left, Image center, Image right, int height, int leftPadding, int rightPadding)
            : this(left, center, right, false)
        {
            this.SetSizes(height, leftPadding, rightPadding);
        }
        

        // 非公開メソッド


        // 公開メソッド

        /// <summary>
        /// イメージのインスタンスを更新します。nullを指定した項目は更新されません。すでにこのテーマが使用中の場合は、このボタンテーマを利用しているコントロールで再描画を実施しなくてはなりません。
        /// </summary>
        /// <param name="left">左端イメージ</param>
        /// <param name="center">中央イメージ</param>
        /// <param name="right">右端イメージ</param>
        public void SetImage(Image left, Image center, Image right)
        {
            if (left != null) this.leftDecoration = left;
            if (center != null) this.centerDecoration = center;
            if (right != null) this.rightDecoration = right;
        }
        
        /// <summary>
        /// このボタンテーマの各種サイズなどの値を更新します。すでにこのテーマが使用中の場合は、このボタンテーマを利用しているコントロールで再描画を実施しなくてはなりません。
        /// </summary>
        /// <param name="height">このテーマで推奨されるデフォルトのボタンの高さ</param>
        /// <param name="leftPadding">左端部分の幅 (内部余白)</param>
        /// <param name="rightPadding">右端部分の幅 (内部余白)</param>
        public void SetSizes(int height, int leftPadding, int rightPadding)
        {
            this.recommendedHeight = height;
            this.leftPaddingSize = leftPadding;
            this.rightPaddingSize = rightPadding;
        }

        /// <summary>
        /// 指定したストリームへ現在このインスタンスが保持している情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("type", "LauncherButtonTheme");
            headers.Add("guid", Guid.NewGuid().ToString());
            

            // 各種サイズ
            headers.Add("RecommendedHeight", this.recommendedHeight.ToString());
            headers.Add("LeftPaddingSize", this.leftPaddingSize.ToString());
            headers.Add("RightPaddingSize", this.rightPaddingSize.ToString());


            // ヘッダの書き込み
            bc.WriteDictionary(headers, stream);


            // 各種画像の書き込み
            MemoryStream ms;
            byte[] buf;
            ulong bufLen;


            // leftDecoration
            ms = new MemoryStream();
            this.leftDecoration.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);
            

            // centerDecoration
            ms = new MemoryStream();
            this.centerDecoration.Save(ms, ImageFormat.Png);
            buf = ms.ToArray();
            bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);
            

            // rightDecoration
            ms = new MemoryStream();
            this.rightDecoration.Save(ms, ImageFormat.Png);
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
        public static LauncherButtonTheme GetSampleTheme()
        {
            Image left, center, right;
            Graphics g;

            left = new Bitmap(10, 10);
            g = Graphics.FromImage(left);
            g.FillRectangle(Brushes.Blue, 0, 0, 10, 10);
            g.Dispose();

            center = new Bitmap(10, 10);
            g = Graphics.FromImage(center);
            g.FillRectangle(Brushes.White, 0, 0, 10, 10);
            g.Dispose();

            right = new Bitmap(10, 10);
            g = Graphics.FromImage(right);
            g.FillRectangle(Brushes.Red, 0, 0, 10, 10);
            g.Dispose();

            return new LauncherButtonTheme(left, center, right, 40, 10, 10);
        }

        /// <summary>
        /// 指定したストリームからロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static LauncherButtonTheme Load(Stream stream)
        {
            LauncherButtonTheme result = new LauncherButtonTheme();
            
            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            bc.ReadDictionary(headers, stream);

            if (headers["type"] != "LauncherButtonTheme")
                throw new NotSupportedException("LauncherButtonThemeの読み込みに失敗しました。ヘッダの値が不正です。");

            
            
            // 各種サイズの読み込み
            result.recommendedHeight = Int32.Parse(headers["RecommendedHeight"]);
            result.leftPaddingSize = Int32.Parse(headers["LeftPaddingSize"]);
            result.rightPaddingSize = Int32.Parse(headers["RightPaddingSize"]);


            // 各種画像の読み込み
            ulong imageLen;    // バイト長
            byte[] buf;        // バイト長だけ読み込まれるバッファ
            MemoryStream ms;   // バッファからイメージを作成するのに使用するMemoryStream

            
            // leftDecoration
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.leftDecoration = Image.FromStream(ms);


            // centerDecoration
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.centerDecoration = Image.FromStream(ms);


            // rightDecoration
            imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            buf = br.ReadBytes((int) imageLen);
            ms = new MemoryStream(buf);
            ms.Position = 0;
            result.rightDecoration = Image.FromStream(ms);


            // おわり
            return result;
        }
    }
}
