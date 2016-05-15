using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Informations
{
    public class GameImageCollection : NativeEventDefinedList<GameImage>
    {
        // 非公開静的フィールド

        /// <summary>
        /// 
        /// </summary>
        private static string _MagicCode
        {
            get { return " Redefinable Launcher GAME IMAGE Collection "; }
        }


        // コンストラクタ

        /// <summary>
        /// 既存のコレクションを指定して、新しいGameImageCollectionクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="images"></param>
        public GameImageCollection(ICollection<GameImage> images)
            : this()
        {
            this.AddRange(images);
        }

        /// <summary>
        /// 新しいGameImageCollectionクラスのインスタンスを初期化します。
        /// </summary>
        public GameImageCollection()
            : base()
        {
            // 実装なし
        }

        /// <summary>
        /// ストリームから新しいGameImageCollectionクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="stream"></param>
        public GameImageCollection(Stream stream)
        {
            this._loadFrom(stream);
        }


        // 非公開メソッド

        private void _saveTo(Stream stream)
        {
            lock (stream)
            {
                BinaryWriter bw = new BinaryWriter(stream);
                BinaryConverter bc = new BinaryConverter();

                // マジックコード
                bw.Write(bc.GetBytes(_MagicCode));

                // 個数
                bw.Write(bc.GetBytes(((uint)this.Count)));

                // データの書き込み
                foreach (GameImage image in this)
                {
                    // 現在のストリームにイメージのデータを追加
                    image.Save(stream);
                }
            }
        }

        private void _loadFrom(Stream stream)
        {
            lock (stream)
            {
                BinaryReader br = new BinaryReader(stream);
                BinaryConverter bc = new BinaryConverter();

                // マジックコードの検査
                int magicCodeLength = GameImageCollection._MagicCode.Length;
                if (bc.GetString(br.ReadBytes(magicCodeLength)) != GameImageCollection._MagicCode)
                    throw new NotSupportedException("GameImageCollectionの読み込みに失敗しました。ヘッダの値が不正です。");

                // ストリームの個数
                uint imageCount = 0;
                try
                {
                    imageCount = bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));
                }
                catch (OverflowException ex)
                {
                    throw new NotSupportedException("GameImageCollectionの読み込みに失敗しました。データ個数のバイトデータが有効範囲外です。", ex);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                for (int i = 0; i < imageCount; i++)
                {
                    GameImage image = null;
                    try
                    {
                        image = GameImage.Load(stream);
                    }
                    catch (NotSupportedException ex)
                    {
                        throw new NotSupportedException("GameImageCollectionの読み込みに失敗しました。GameImageデータが壊れています。", ex);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    this.Add(image);
                }
            }
        }


        // 公開メソッド

        /// <summary>
        /// 既存のGameImageのコレクションのすべての内容を現在のコレクションに追加します。
        /// </summary>
        /// <param name="images"></param>
        public void AddRange(ICollection<GameImage> images)
        {
            foreach (GameImage image in images)
                this.Add(image);
        }

        /// <summary>
        /// コレクションをファイルへ保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveTo(fs);
            fs.Close();
        }

        /// <summary>
        /// コレクションをストリームへ出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }


        // 公開静的メソッド

        /// <summary>
        /// 指定したファイルを読み込んで新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameImageCollection Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            GameImageCollection result = GameImageCollection.Load(fs);
            return result;
        }

        /// <summary>
        /// 指定したストリームを読み込んで新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static GameImageCollection Load(Stream stream)
        {
            GameImageCollection collection = new GameImageCollection(stream);
            return collection;
        }
    }

    public class GameImage
    {
        // 非公開フィールド
        private string title;
        private Image image;
        private Guid imageGuid;

        
        // 非公開静的フィールド

        /// <summary>
        /// 
        /// </summary>
        private static string _MagicCode
        {
            get { return " Redefinable Launcher GAME IMAGE "; }
        }


        // 公開フィールド

        /// <summary>
        /// タイトルを取得・設定します。
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        /// イメージを取得・設定します。
        /// </summary>
        public Image Image
        {
            get { return this.image; }
            set { this.image = value; }
        }

        /// <summary>
        /// イメージに割り当てられた固有のGUIDを取得します。
        /// </summary>
        public Guid ImageGuid
        {
            get { return this.imageGuid; }
        }

        
        // コンストラクタ

        /// <summary>
        /// Streamから新しいGameImageクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="stream"></param>
        public GameImage(Stream stream)
        {
            this._loadFrom(stream);
        }

        /// <summary>
        /// 新しいGameImageクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="title"></param>
        /// <param name="image"></param>
        public GameImage(string title, Image image)
        {
            this.title = title;
            this.image = (Image)image.Clone();
            this.imageGuid = Guid.NewGuid();
        }


        // 非公開メソッド
        
        private void _saveTo(Stream stream)
        {
            lock (stream)
            {
                BinaryWriter bw = new BinaryWriter(stream);
                BinaryConverter bc = new BinaryConverter();

                // マジックコードの書き込み
                bw.Write(bc.GetBytes(GameImage._MagicCode));
                
                // タイトルの書き込み
                bw.Write(bc.GetBytesWithUInt16LengthInfo(this.title));

                // GUIDの書き込み
                bw.Write(bc.GetBytesWithUInt16LengthInfo(this.imageGuid.ToString()));

                // 画像データの書き込み
                MemoryStream ms = new MemoryStream();
                this.image.Save(ms, ImageFormat.Png);
                byte[] imageData = ms.ToArray();
                
                bw.Write(bc.GetBytes((ulong) (imageData.Length)));
                bw.Write(imageData);

                ms.Dispose();
            }
        }

        private void _loadFrom(Stream stream)
        {
            lock (stream)
            {
                BinaryReader br = new BinaryReader(stream);
                BinaryConverter bc = new BinaryConverter();

                // マジックコードの検査
                int magicLength = bc.GetBytes(GameImage._MagicCode).Length;
                if (bc.GetString(br.ReadBytes(magicLength)) != GameImage._MagicCode)
                    throw new NotSupportedException("GameImageの読み込みに失敗しました。ヘッダの値が不正です。");

                // タイトルの取得
                ushort titleLength = bc.ToUInt16(br.ReadBytes(sizeof(UInt16)));
                if (titleLength == 0)
                {
                    // タイトル無し
                    this.title = null;
                }
                else
                {
                    // タイトルあり
                    this.title = bc.GetString(br.ReadBytes(titleLength));
                }

                // Guidの取得
                ushort guidLength = bc.ToUInt16(br.ReadBytes(sizeof(UInt16)));
                this.imageGuid = Guid.Parse(bc.GetString(br.ReadBytes(guidLength)));

                // イメージデータの長さを取得
                ulong imageLength = bc.ToUInt64(br.ReadBytes(sizeof(UInt64)));
                byte[] imageData = br.ReadBytes((int)imageLength);

                // イメージの生成
                try
                {
                    MemoryStream ms = new MemoryStream(imageData);
                    this.image = Image.FromStream(ms);
                    ms.Dispose();
                }
                catch (ArgumentException ex)
                {
                    throw new NotSupportedException("GameImageの読み込みに失敗しました。画像データが壊れています。", ex);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        // 公開メソッド

        /// <summary>
        /// 指定されたパスにバイナリデータで保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveTo(fs);
            fs.Close();
        }

        /// <summary>
        /// 指定されたストリームにバイナリデータとして出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }


        // 公開静的メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameImage Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            GameImage result = GameImage.Load(fs);

            fs.Close();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static GameImage Load(Stream stream)
        {
            return new GameImage(stream);
        }
    }
}
