using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;

using ImageFormat = System.Drawing.Imaging.ImageFormat;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// バナーについての情報を格納します。
    /// </summary>
    public class Banner
    {
        // 非公開フィールド
        private bool useBanner;
        private Image bannerImage;
        private Guid bannerGuid;

        
        // 非公開静的フィールド

        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher BANNER INFORMATION Binary "; }
        }


        // 公開フィールド

        /// <summary>
        /// バナーを使用するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool UseBanner
        {
            get { return this.useBanner; }
            set { this.useBanner = value; }
        }

        /// <summary>
        /// バナーのイメージを指定します。nullを指定すると、UseBannerがfalseにされ、1x1の画像が使用されます。
        /// </summary>
        public Image BannerImage
        {
            get { return this._getBannerImage(); }
            set { this._setBannerImage(value); }
        }

        /// <summary>
        /// このインスタンスに割り当てられたGuidを取得します。
        /// </summary>
        public Guid BannerGuid
        {
            get { return this.bannerGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 
        /// </summary>
        private Banner()
        {
            this.useBanner = false;
            this.bannerImage = null;
            this.bannerGuid = Guid.Empty;
        }

        /// <summary>
        /// 新しいBannerクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="useBanner"></param>
        /// <param name="bannerImage"></param>
        public Banner(bool useBanner, Image bannerImage)
        {
            this.useBanner = useBanner;
            this.bannerImage = bannerImage;
            this.bannerGuid = Guid.NewGuid();
        }

        /// <summary>
        /// 新しいBannerクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="bannerImage"></param>
        public Banner(Image bannerImage)
            : this(bannerImage != null, bannerImage)
        {
            if (bannerImage == null)
                this.bannerImage = new Bitmap(1, 1);
        }


        // 非公開メソッド

        private Image _getBannerImage()
        {
            return this.bannerImage;
        }

        private void _setBannerImage(Image image)
        {
            if (image == null)
            {
                this.useBanner = false;
                image = new Bitmap(1, 1);
            }

            this.bannerImage = image;
        }

        private void _saveTo(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("指定されたストリームは書き込みが許可されていません。");

            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコード
            bw.Write(bc.GetBytes(Banner._MagicCode));

            
            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"UseBanner", this.useBanner.ToString()},
                {"BannerGuid", this.bannerGuid.ToString()},
                {"Uwaaaaaa", "つらいめううううううううううううう"},
            };

            bc.WriteDictionary(dict, stream);


            // イメージ
            MemoryStream ms = new MemoryStream();
            this.bannerImage.Save(ms, ImageFormat.Png);
            byte[] buf = ms.ToArray();
            ulong bufLen = (ulong) buf.Length;

            bw.Write(bc.GetBytes(bufLen));
            bw.Write(buf);
        }

        private void _loadFrom(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("指定されたストリームは読み取りが許可されていません。");

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコードの検査
            int magicCodeLength = bc.GetBytes(Banner._MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicCodeLength)) != Banner._MagicCode)
                throw new NotSupportedException("バナー情報の読み込みに失敗しました。ヘッダの値が不正です。");

            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>();
            bc.ReadDictionary(dict, stream);

            this.useBanner = Boolean.Parse(dict["UseBanner"]);
            this.bannerGuid = Guid.Parse(dict["BannerGuid"]);

            // イメージ
            ulong imageLen = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
            byte[] buf = br.ReadBytes((int) imageLen);
            MemoryStream ms = new MemoryStream(buf);
            ms.Position = 0;

            this.bannerImage = (Image) Image.FromStream(ms).Clone();
        }


        // 公開メソッド

        /// <summary>
        /// 現在のインスタンスの保有する情報を指定したストリームに出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }

        /// <summary>
        /// 現在のインスタンスの保有する情報を指定したファイルへ保存します。
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveTo(fs);

            fs.Close();
        }


        // 公開静的メソッド

        /// <summary>
        /// 指定したストリームからBannerクラスの情報を取得します。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Banner Load(Stream stream)
        {
            Banner banner = new Banner();
            banner._loadFrom(stream);

            return banner;
        }
    }
}
