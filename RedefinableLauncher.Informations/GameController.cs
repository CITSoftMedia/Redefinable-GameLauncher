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
    /// コントローラー情報のコレクション機能を提供します。
    /// </summary>
    public class GameControllerCollection : NativeEventDefinedList<GameController>
    {
        // 非公開フィールド

        // 非公開静的フィールド

        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher CONTROLLER GUIDS Binary "; }
        }


        // 公開フィールド


        // コンストラクタ


        // 非公開メソッド
        
        /// <summary>
        /// 完全な情報を含むコントローラー情報をディレクトリから一括で読み込みます。
        /// </summary>
        /// <param name="path"></param>
        private void _loadFromDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("コントローラー情報コレクションの読み込み元のディレクトリが見つかりませんでした。");

            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);

            string[] files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string f in files)
            {
                string file = /*path + "\\" +*/ f;

                try
                {
                    this.Add(GameController.Load(file));
                }
                catch (Exception ex)
                {
                    throw new IOException("コントローラー情報のファイルからの読み込みでエラーが発生しました。 " + file, ex);
                }
            }
        }

        /// <summary>
        /// 完全な情報を含むコントローラー情報をディレクトリへ一括で出力します。
        /// </summary>
        /// <param name="path">保存先のディレクトリ</param>
        private void _saveToDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("コントローラー情報コレクションの保存先のディレクトリが見つかりませんでした。");

            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);

            FileStream fs = new FileStream(path + "\\info.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine("以下に自動保存された情報のGUIDと中身の対応表を示します。");
            sw.WriteLine();
            foreach (GameController controller in this)
            {
                controller.Save(path + "\\" + controller.ControllerGuid.ToString() + ".xml");
                sw.WriteLine("* {0}: {1}", controller.ControllerGuid.ToString(), controller.Name);
            }
            sw.WriteLine();
            sw.WriteLine("以上");
            sw.Close();
        }

        // 公開メソッド
        
        /// <summary>
        /// 指定したディレクトリから完全なコントローラー情報を読み取ります。
        /// </summary>
        /// <param name="dir"></param>
        public void AddFromDirectory(string dir)
        {
            this._loadFromDirectory(dir);
        }

        /// <summary>
        /// 指定したディレクトリへ完全なコントローラー情報を保存します。
        /// </summary>
        /// <param name="dir"></param>
        public void SaveToDirectory(string dir)
        {
            this._saveToDirectory(dir);
        }

        /// <summary>
        /// Guidのコレクションに含まれるGuid値を持つGameController情報だけを集めた新しいコレクションを生成します。
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public GameControllerCollection GetControllers(ICollection<Guid> guids)
        {
            return this.GetControllers(guids, false);
        }

        /// <summary>
        /// Guidのコレクションに含まれるGuid値を持つGameController情報だけを集めた新しいコレクションを生成します。
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="throwException">trueを指定すると、無効なGuidを見つけた際、無視せず例外をスローします。</param>
        /// <returns></returns>
        public GameControllerCollection GetControllers(ICollection<Guid> guids, bool throwException)
        {
            GameControllerCollection controllers = new GameControllerCollection();
            foreach (Guid guid in guids)
            {
                bool contains = this.ContainsGuid(guid);

                if (throwException && !contains)
                    throw new KeyNotFoundException("無効なGuidを検出しました。" + guid.ToString());
                
                if (contains)
                    controllers.Add(this.GetController(guid));
            }

            return controllers;
        }

        /// <summary>
        /// このコレクションに含まれるすべてのGameControllerのGuidのみを集めた配列を取得します。
        /// </summary>
        /// <returns></returns>
        public Guid[] GetGuids()
        {
            Guid[] guids = new Guid[this.Count];
            for (int i = 0; i < this.Count; i++)
                guids[i] = this[i].ControllerGuid;

            return guids;
        }

        /// <summary>
        /// 指定したGuidを保持するGameControllerのインスタンスを取得します。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameController GetController(Guid guid)
        {
            foreach (GameController controller in this)
                if (controller.ControllerGuid == guid)
                    return controller;

            throw new KeyNotFoundException("指定したGuidを持つGameControllerはコレクションに含まれていません。");
        }

        /// <summary>
        /// 指定した名前を持つGameControllerのインスタンスを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameController GetController(string name)
        {
            foreach (GameController controller in this)
                if (controller.Name == name)
                    return controller;

            throw new KeyNotFoundException("指定したnameを持つGameControllerはコレクションに含まれていません。");
        }

        /// <summary>
        /// GameControllerをコレクションに追加します。
        /// </summary>
        /// <param name="item"></param>
        public override void Add(GameController item)
        {
            if (this.ContainsGuid(item))
                throw new ArgumentException("指定されたGameControllerと同一のGuidを保持するインスタンスがすでにコレクションに含まれています。");

            base.Add(item);
        }

        /// <summary>
        /// 指定したGuidを保有するGameControllerがこのコレクションに含まれているか判断します。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool ContainsGuid(Guid guid)
        {
            foreach (GameController controller in this)
                if (controller.ControllerGuid == guid)
                    return true;

            return false;
        }

        /// <summary>
        /// 指定したGameControllerと同一のGuidを保有するGameControllerがこのコレクションに含まれているか判断します。
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool ContainsGuid(GameController controller)
        {
            return this.ContainsGuid(controller.ControllerGuid);
        }

        /// <summary>
        /// 指定したストリームへ現在のコレクションのGuid一覧を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void SaveGuids(Stream stream)
        {
            GameControllerCollection.SaveGuids(stream, this.GetGuids());
        }
        

        // 公開静的メソッド

        /// <summary>
        /// Guidの配列をGameControllerのGuidデータの一覧として書き込みます。
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
            bw.Write(bc.GetBytes(GameControllerCollection._MagicCode));

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
        /// 指定したストリームからGameControllerのGuidデータの一覧として読み込みます。
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
            int magicCodeLength = bc.GetBytes(GameControllerCollection._MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicCodeLength)) != GameControllerCollection._MagicCode)
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
        /// 指定したストリームからGuidデータの一覧を読み取り、fullInformationsから完全なGameController情報を取得して、新たなコレクションを作成します。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fullInformations"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static GameControllerCollection LoadCollection(Stream stream, GameControllerCollection fullInformations, bool throwException)
        {
            Guid[] guids = LoadGuids(stream);
            return fullInformations.GetControllers(guids);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static GameControllerCollection GetDefaultControllers()
        {
            GameControllerCollection controllers = new GameControllerCollection();
            GameController gc = null;
            FieldInfo guidField = null;
            BindingFlags bFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            // Empty
            gc = GameController.CreateEmpty();
            controllers.Add(gc);

            // マウス
            gc = new GameController("マウス", "ポインティングデバイスです。");
            guidField = gc.GetType().GetField("controllerGuid", bFlags);
            guidField.SetValue(gc, Guid.Parse("1D076FE3-ED05-4BF8-9EAD-F48D05835DA8"));
            controllers.Add(gc);

            // キーボード
            gc = new GameController("キーボード", "デフォルトの入力デバイスです。");
            guidField = gc.GetType().GetField("controllerGuid", bFlags);
            guidField.SetValue(gc, Guid.Parse("AA0DDEB2-66D2-4198-B9F1-3B43B84FC86C"));
            controllers.Add(gc);

            // ゲームパッド
            gc = new GameController("ゲームパッド", "ゲーム用のコントローラーデバイスです。");
            guidField = gc.GetType().GetField("controllerGuid", bFlags);
            guidField.SetValue(gc, Guid.Parse("A555A683-B336-4691-BB23-6B939E26F461"));
            controllers.Add(gc);

            // オリジナルコントローラ
            gc = new GameController("オリジナルコントローラ", "他サークルと共同で開発した独自のコントローラーです。");
            guidField = gc.GetType().GetField("controllerGuid", bFlags);
            guidField.SetValue(gc, Guid.Parse("9E94B087-188A-49C2-88A5-713575B3DC07"));
            controllers.Add(gc);

            return controllers;
        }
    }


    /// <summary>
    /// ゲーム作品のコントローラーに関する情報を格納します。
    /// </summary>
    [Serializable]
    public class GameController
    {
        // 非公開フィールド
        private string name;
        private string description;
        private Guid controllerGuid;


        // 公開フィールド

        /// <summary>
        /// コントローラー名を取得・設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// コントローラーに関する説明を取得・設定します。
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Guidを取得します。
        /// </summary>
        public Guid ControllerGuid
        {
            get { return this.controllerGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいGameControllerクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public GameController(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.controllerGuid = Guid.NewGuid();
        }


        // 非公開メソッド

        /// <summary>
        /// 指定したストリームからSoap形式でコントローラー情報を読み取ります。
        /// </summary>
        /// <param name="stream"></param>
        private void _loadFrom(Stream stream)
        {
            SoapFormatter sf = new SoapFormatter();
            GameController gc = (GameController)sf.Deserialize(stream);

            this.name = gc.name;
            this.description = gc.description;
            this.controllerGuid = gc.controllerGuid;
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
            return (((GameController)obj).controllerGuid == this.controllerGuid);
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
        /// 指定したストリームにコントローラー情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }

        /// <summary>
        /// 指定したファイルへコントローラー情報を保存します。
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
        /// 指定したストリームからコントローラー情報をロードします。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static GameController Load(Stream stream)
        {
            GameController result = new GameController(null, null);
            result._loadFrom(stream);
            return result;
        }

        /// <summary>
        /// 指定したファイルからコントローラー情報をロードします。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameController Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            GameController result = new GameController(null, null);

            result._loadFrom(fs);
            fs.Close();

            return result;
        }

        public static GameController CreateEmpty()
        {
            GameController gc = new GameController("Empty", "Empty");
            gc.controllerGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            return gc;
        }
    }
}