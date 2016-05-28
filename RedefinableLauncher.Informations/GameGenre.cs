using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IO;

using FieldInfo = System.Reflection.FieldInfo;
using BindingFlags = System.Reflection.BindingFlags;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// ジャンル情報のコレクション機能を提供します。
    /// </summary>
    public class GameGenreCollection : NativeEventDefinedList<GameGenre>
    {
        // 非公開フィールド

        // 非公開静的フィールド

        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher GENRE GUIDS Binary "; }
        }


        // 公開フィールド


        // コンストラクタ


        // 非公開メソッド
        
        /// <summary>
        /// 完全な情報を含むジャンル情報をディレクトリから一括で読み込みます。
        /// </summary>
        /// <param name="path"></param>
        private void _loadFromDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("ジャンル情報コレクションの読み込み元のディレクトリが見つかりませんでした。");

            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);

            string[] files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string f in files)
            {
                string file = /*path + "\\" +*/ f;

                try
                {
                    this.Add(GameGenre.Load(file));
                }
                catch (Exception ex)
                {
                    throw new IOException("ジャンル情報のファイルからの読み込みでエラーが発生しました。 " + file, ex);
                }
            }
        }

        /// <summary>
        /// 完全な情報を含むジャンル情報をディレクトリへ一括で出力します。
        /// </summary>
        /// <param name="path">保存先のディレクトリ</param>
        private void _saveToDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("ジャンル情報コレクションの保存先のディレクトリが見つかりませんでした。");

            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);
            
            FileStream fs = new FileStream(path + "\\info.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine("以下に自動保存された情報のGUIDと中身の対応表を示します。");
            sw.WriteLine();
            foreach (GameGenre genre in this)
            {
                genre.Save(path + "\\" + genre.GenreGuid.ToString() + ".xml");
                sw.WriteLine("* {0}: {1}", genre.GenreGuid.ToString(), genre.Name);
            }
            sw.WriteLine();
            sw.WriteLine("以上");
            sw.Close();
        }

        // 公開メソッド
        
        /// <summary>
        /// 指定したディレクトリから完全なジャンル情報を読み取ります。
        /// </summary>
        /// <param name="dir"></param>
        public void AddFromDirectory(string dir)
        {
            this._loadFromDirectory(dir);
        }

        /// <summary>
        /// 指定したディレクトリへ完全なジャンル情報を保存します。
        /// </summary>
        /// <param name="dir"></param>
        public void SaveToDirectory(string dir)
        {
            this._saveToDirectory(dir);
        }

        /// <summary>
        /// Guidのコレクションに含まれるGuid値を持つGameGenre情報だけを集めた新しいコレクションを生成します。
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public GameGenreCollection GetGenres(ICollection<Guid> guids)
        {
            return this.GetGenres(guids, false);
        }

        /// <summary>
        /// Guidのコレクションに含まれるGuid値を持つGameGenre情報だけを集めた新しいコレクションを生成します。
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="throwException">trueを指定すると、無効なGuidを見つけた際、無視せず例外をスローします。</param>
        /// <returns></returns>
        public GameGenreCollection GetGenres(ICollection<Guid> guids, bool throwException)
        {
            GameGenreCollection genres = new GameGenreCollection();
            foreach (Guid guid in guids)
            {
                bool contains = this.ContainsGuid(guid);

                if (throwException && !contains)
                    throw new KeyNotFoundException("無効なGuidを検出しました。" + guid.ToString());
                
                if (contains)
                    genres.Add(this.GetGenre(guid));
            }

            return genres;
        }

        /// <summary>
        /// このコレクションに含まれるすべてのGameGenreのGuidのみを集めた配列を取得します。
        /// </summary>
        /// <returns></returns>
        public Guid[] GetGuids()
        {
            Guid[] guids = new Guid[this.Count];
            for (int i = 0; i < this.Count; i++)
                guids[i] = this[i].GenreGuid;

            return guids;
        }

        /// <summary>
        /// 指定したGuidを保持するGameGenreのインスタンスを取得します。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameGenre GetGenre(Guid guid)
        {
            foreach (GameGenre genre in this)
                if (genre.GenreGuid == guid)
                    return genre;

            throw new KeyNotFoundException("指定したGuidを持つGameGenreはコレクションに含まれていません。");
        }

        /// <summary>
        /// 指定した名前を持つGameGenreのインスタンスを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameGenre GetGenre(string name)
        {
            foreach (GameGenre genre in this)
                if (genre.Name == name)
                    return genre;

            throw new KeyNotFoundException("指定したnameを持つGameGenreはコレクションに含まれていません。");
        }

        /// <summary>
        /// GameGenreをコレクションに追加します。
        /// </summary>
        /// <param name="item"></param>
        public override void Add(GameGenre item)
        {
            if (this.ContainsGuid(item))
                throw new ArgumentException("指定されたGameGenreと同一のGuidを保持するインスタンスがすでにコレクションに含まれています。");

            base.Add(item);
        }

        /// <summary>
        /// 指定したGuidを保有するGameGenreがこのコレクションに含まれているか判断します。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool ContainsGuid(Guid guid)
        {
            foreach (GameGenre genre in this)
                if (genre.GenreGuid == guid)
                    return true;

            return false;
        }

        /// <summary>
        /// 指定したGameGenreと同一のGuidを保有するGameGenreがこのコレクションに含まれているか判断します。
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        public bool ContainsGuid(GameGenre genre)
        {
            return this.ContainsGuid(genre.GenreGuid);
        }

        /// <summary>
        /// 指定したストリームへ現在のコレクションのGuid一覧を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void SaveGuids(Stream stream)
        {
            GameGenreCollection.SaveGuids(stream, this.GetGuids());
        }
        

        // 公開静的メソッド

        /// <summary>
        /// Guidの配列をGameGenreのGuidデータの一覧として書き込みます。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="guids"></param>
        public static void SaveGuids(Stream stream, ICollection<Guid> guids)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("指定されたストリームは書き込みが許可されていません。");

            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコード
            bw.Write(bc.GetBytes(GameGenreCollection._MagicCode));

            // mainData
            string[] guidsArray = new string[guids.Count];
            int i = 0;
            foreach (Guid guid in guids)
            {
                guidsArray[i] = guid.ToString();
                i++;
            }

            bc.WriteArray(guidsArray, stream);
        }

        /// <summary>
        /// 指定したストリームからGameGenreのGuidデータの一覧として読み込みます。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Guid[] LoadGuids(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("指定されたストリームは読み取りが許可されていません。");

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコードの検査
            int magicCodeLength = bc.GetBytes(GameGenreCollection._MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicCodeLength)) != GameGenreCollection._MagicCode)
                throw new NotSupportedException("ゲーム起動情報の読み込みに失敗しました。ヘッダの値が不正です。");

            // mainData
            string[] guidsArray = bc.ReadArray(stream);

            // Guidの配列へ変換
            Guid[] guids = new Guid[guidsArray.Length];
            for (int i = 0; i < guidsArray.Length; i++)
                guids[i] = Guid.Parse(guidsArray[i]);

            return guids;
        }

        /// <summary>
        /// 指定したストリームからGuidデータの一覧を読み取り、fullInformationsから完全なGameGenre情報を取得して、新たなコレクションを作成します。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fullInformations"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static GameGenreCollection LoadCollection(Stream stream, GameGenreCollection fullInformations, bool throwException)
        {
            Guid[] guids = LoadGuids(stream);
            return fullInformations.GetGenres(guids);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static GameGenreCollection GetDefaultGenres()
        {
            GameGenreCollection genres = new GameGenreCollection();
            GameGenre gg = null;
            FieldInfo guidField = null;
            BindingFlags bFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            // Empty
            gg = GameGenre.CreateEmpty();
            genres.Add(gg);

            // RPG
            //gg = new GameGenre("ＲＰＧ", "ローププレイング方式のゲーム作品です。");
            //guidField = gg.GetType().GetField("genreGuid", bFlags);
            //guidField.SetValue(gg, Guid.Parse("{37D04978-1D6D-4D02-99DF-E417B41BABCC}"));
            //genres.Add(gg);

            // シューティング
            //gg = new GameGenre("シューティング", "シューティング方式のゲーム作品です。");
            //guidField = gg.GetType().GetField("genreGuid", bFlags);
            //guidField.SetValue(gg, Guid.Parse("{A91F7292-6A43-46D5-AF6E-D9A8582DFC40}"));
            //genres.Add(gg);

            // Guid値絶対にいじるなよ！？？
            // 絶対にいじるなよ！？？
            // 絶対にだからな！！！！！！

            // 2Dシューティングゲーム
            gg = new GameGenre("2Dシューティングゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{0BB90942-D04A-4ACD-B4A8-CCF21A42C640}"));
            genres.Add(gg);

            // 3Dシューティングゲーム
            gg = new GameGenre("3Dシューティングゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{9EF887A7-8F70-4B84-9AE1-746A8FDFC1C2}"));
            genres.Add(gg);

            // 2Dアクションゲーム
            gg = new GameGenre("2Dアクションゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{35F9421E-60E1-43B9-ADD0-73EC866F5FA7}"));
            genres.Add(gg);

            // 3Dアクションゲーム
            gg = new GameGenre("3Dアクションゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{45F9421E-30A3-53B9-B6D0-73EC866F5CB3}"));
            genres.Add(gg);

            // アドベンチャーゲーム
            gg = new GameGenre("アドベンチャーゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{E6CF9704-B12C-4099-A201-25BE981E9EED}"));
            genres.Add(gg);

            // 脱出ゲーム・謎解き・迷路
            gg = new GameGenre("謎解き・迷路", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{A6D532AB-F293-41BF-9001-E6FCBD605666}"));
            genres.Add(gg);

            // ノベルゲーム
            gg = new GameGenre("ノベルゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{CBDBBA70-936A-4D26-BCF6-97DF78E5A541}"));
            genres.Add(gg);

            // パズルゲーム
            gg = new GameGenre("パズルゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{A9562BB8-1F84-49C0-9441-B4A4646693D0}"));
            genres.Add(gg);

            // シミュレーションゲーム
            gg = new GameGenre("シミュレーションゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{F0B230E3-DBFD-4527-9537-BB19327A0BF7}"));
            genres.Add(gg);

            // 育成シミュレーション
            gg = new GameGenre("育成シミュレーション", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{083630F5-3A36-4291-87CB-14A0019DCC95}"));
            genres.Add(gg);

            // 音楽ゲーム
            gg = new GameGenre("音楽ゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{2955F9A2-6F1F-47EF-88B8-1B96C736AC33}"));
            genres.Add(gg);

            // クイズゲーム
            gg = new GameGenre("クイズゲーム", "ローププレイング方式のゲーム作品です。");
            guidField = gg.GetType().GetField("genreGuid", bFlags);
            guidField.SetValue(gg, Guid.Parse("{5DC7932F-2AD3-4159-9D0B-8E43E74F72F9}"));
            genres.Add(gg);

            return genres;
        }
    }

    /// <summary>
    /// ゲーム作品のジャンルに関する情報を格納します。
    /// </summary>
    [Serializable]
    public class GameGenre
    {
        // 非公開フィールド
        private string name;
        private string description;
        private Guid genreGuid;
        

        // 公開フィールド
        
        /// <summary>
        /// ジャンル名を取得・設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// ジャンルに関する説明を取得・設定します。
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Guidを取得します。
        /// </summary>
        public Guid GenreGuid
        {
            get { return this.genreGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいGameGenreクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public GameGenre(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.genreGuid = Guid.NewGuid();
        }

        
        // 非公開メソッド

        /// <summary>
        /// 指定したストリームからSoap形式でジャンル情報を読み取ります。
        /// </summary>
        /// <param name="stream"></param>
        private void _loadFrom(Stream stream)
        {
            SoapFormatter sf = new SoapFormatter();
            GameGenre gg = (GameGenre) sf.Deserialize(stream);

            this.name = gg.name;
            this.description = gg.description;
            this.genreGuid = gg.genreGuid;
        }
        
        /// <summary>
        /// Soap形式で指定したストリームにGuidを含む全データを出力します。
        /// </summary>
        /// <param name="stream"></param>
        private void _saveTo(Stream stream)
        {
            SoapFormatter sf = new SoapFormatter();
            sf.Serialize(stream, this);
        }


        // 公開メソッド

        /// <summary>
        /// Guidが一致しているかどうかを判断します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (((GameGenre) obj).genreGuid == this.genreGuid);
        }

        /// <summary>
        /// 使用しないでください。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 指定したストリームにジャンル情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }

        /// <summary>
        /// 指定したファイルへジャンル情報を保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveTo(fs);
            fs.Close();
        }


        // 公開静的メソッド

        /// <summary>
        /// 指定したストリームからジャンル情報をロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static GameGenre Load(Stream stream)
        {
            GameGenre result = new GameGenre(null, null);
            result._loadFrom(stream);
            return result;
        }

        /// <summary>
        /// 指定したファイルからジャンル情報をロードします。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameGenre Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            GameGenre result = new GameGenre(null, null);

            result._loadFrom(fs);
            fs.Close();

            return result;            
        }

        public static GameGenre CreateEmpty()
        {
            GameGenre gg = new GameGenre("Empty", "Empty");
            gg.genreGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            return gg;
        }
    }
}
