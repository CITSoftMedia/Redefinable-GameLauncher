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
        /// 内部用コンストラクタ
        /// </summary>
        private LauncherPanelTheme()
        {
            // 実装なし
        }

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
        
        /// <summary>
        /// 指定したストリームへ現在このインスタンスが保持している情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("type", "LauncherPanelTheme");
            headers.Add("guid", Guid.NewGuid().ToString());

            // 画像をバイトバッファへ
            MemoryStream ms = new MemoryStream();
            this.backgroundImage.Save(ms, ImageFormat.Png);
            byte[] buf = ms.ToArray();

            // バッファ長の書き込み
            ulong bufLen = (ulong) buf.Length;
            bw.Write(bc.GetBytes(bufLen));

            // バッファの書き込み
            bw.Write(buf);
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

        /// <summary>
        /// 指定したストリームからLauncherPanelThemeクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static LauncherPanelTheme Load(Stream stream)
        {
            LauncherPanelTheme result = new LauncherPanelTheme();
            
            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            bc.ReadDictionary(headers, stream);

            if (headers["type"] != "LauncherPanelTheme")
                throw new NotSupportedException("LauncherPanelThemeの読み込みに失敗しました。ヘッダの値が不正です。");

            // 画像バッファの長さを取得
            ulong imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));

            // 画像バッファを取得
            byte[] buf = br.ReadBytes((int) imageLen);

            // 画像へ変換し、インスタンスへ設定
            MemoryStream ms = new MemoryStream(buf);
            ms.Position = 0;

            result.backgroundImage = Image.FromStream(ms);

            // おわり
            return result;
        }
    }
}
