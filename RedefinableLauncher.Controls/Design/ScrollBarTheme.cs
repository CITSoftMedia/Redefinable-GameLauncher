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
    public class ScrollBarTheme : ILauncherThemeElement
    {
        // 非公開フィールド
        private Color trayColor;
        private Color upButtonColor;
        private Color downButtonColor;
        private Color knobColor;


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

        /// <summary>
        /// つまみの色を取得します。
        /// </summary>
        public Color KnobColor
        {
            get { return this.knobColor; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいインスタンスを初期化しますが、メンバ変数は初期化されません。 (内部用)
        /// </summary>
        private ScrollBarTheme()
        {
            // なにもしない
        }

        /// <summary>
        /// 新しいScrollBarThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="tray"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="knob"></param>
        public ScrollBarTheme(Color tray, Color up, Color down, Color knob)
        {
            this.trayColor = tray;
            this.upButtonColor = up;
            this.downButtonColor = down;
            this.knobColor = knob;
        }


        // 非公開メソッド


        // 公開メソッド

        /// <summary>
        /// 指定したストリームへ現在このインスタンスが保持している情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("type", "ScrollBarTheme");
            headers.Add("guid", Guid.NewGuid().ToString());
            

            // 各種値
            headers.Add("TrayColor", this.trayColor.ToArgb().ToString());
            headers.Add("UpButtonColor", this.upButtonColor.ToArgb().ToString());
            headers.Add("DownButtonColor", this.downButtonColor.ToArgb().ToString());
            headers.Add("KnobColor", this.knobColor.ToArgb().ToString());


            // ヘッダの書き込み
            bc.WriteDictionary(headers, stream);
            
        }
        
        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static ScrollBarTheme GetSampleTheme()
        {
            ScrollBarTheme result = new ScrollBarTheme();
            result.trayColor = Color.FromArgb(180, 180, 180);
            result.upButtonColor = result.downButtonColor = Color.FromArgb(100, 100, 100);
            result.knobColor = Color.FromArgb(150, 150, 150);

            return result;
        }

        /// <summary>
        /// 指定したストリームからロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ScrollBarTheme Load(Stream stream)
        {
            ScrollBarTheme result = new ScrollBarTheme();
            
            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            bc.ReadDictionary(headers, stream);

            if (headers["type"] != "ScrollBarTheme")
                throw new NotSupportedException("ScrollBarThemeの読み込みに失敗しました。ヘッダの値が不正です。");

            
            
            // 各種値の読み込み
            result.trayColor = Color.FromArgb(Int32.Parse(headers["TrayColor"]));
            result.upButtonColor = Color.FromArgb(Int32.Parse(headers["UpButtonColor"]));
            result.downButtonColor = Color.FromArgb(Int32.Parse(headers["DownButtonColor"]));
            result.knobColor = Color.FromArgb(Int32.Parse(headers["KnobColor"]));


            // おわり
            return result;
        }
    }
}
