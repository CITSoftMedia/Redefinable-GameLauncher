using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IO;
using Redefinable.IniHandler;
using Redefinable.StreamArchive;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// Gameクラスのインスタンスのコレクション機能を提供します。
    /// </summary>
    public class GameCollection : NativeEventDefinedList<Game>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class Game
    {
        // 非公開フィールド
        private string title;
        private string description;
        private string operationDescription;
        private Guid gameGuid;
        private Team developerTeam;
        private GameServerConnectInfo clientInfo;
        private GameImageCollection images;
        private ExecInfo execInfo;
        private DisplayNumber displayNumber;
        private GameGenreCollection genres;

        private string informationFilePath;


        // 非公開静的フィールド

        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher GAME Basic Information  "; }
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
        /// ゲームの概要を取得・設定します。
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// ゲームの操作説明を取得・設定します。
        /// </summary>
        public string OperationDescription
        {
            get { return this.operationDescription; }
            set { this.operationDescription = value; }
        }

        /// <summary>
        /// このインスタンスに割り当てたGuidを取得します。
        /// </summary>
        public Guid GameGuid
        {
            get { return this.gameGuid; }
        }

        /// <summary>
        /// 開発チーム情報のインスタンスへの参照を取得します。
        /// </summary>
        public Team DeveloperTeam
        {
            get { return this.developerTeam; }
        }

        /// <summary>
        /// Redefinableゲームサーバへ接続するための情報を格納するインスタンスへの参照を取得します。
        /// </summary>
        public GameServerConnectInfo ClientInfo
        {
            get { return this.clientInfo; }
        }

        /// <summary>
        /// ゲームの画像のコレクションを取得します。
        /// </summary>
        public GameImageCollection Images
        {
            get { return this.images; }
        }

        /// <summary>
        /// 実行情報を取得します。
        /// </summary>
        public ExecInfo ExecInfo
        {
            get { return this.execInfo; }
        }

        /// <summary>
        /// ディスプレイ番号のインスタンスを取得します。
        /// </summary>
        public DisplayNumber DisplayNumber
        {
            get { return this.displayNumber; }
        }

        /// <summary>
        /// このゲーム作品に関連付けられているジャンルのジャンル情報インスタンスコレクションを取得します。
        /// </summary>
        public GameGenreCollection Genres
        {
            get { return this.genres; }
        }

        /// <summary>
        /// ファイルからゲーム情報が読み込まれたあるいは保存した場合に、そのファイルのフルパスを示す値を取得します。
        /// </summary>
        public string InformationFilePath
        {
            get { return this.informationFilePath; }
        }
        

        // コンストラクタ
        
        /// <summary>
        /// 新しいGameクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="operationDescription"></param>
        /// <param name="developerTeam"></param>
        /// <param name="clientInfo"></param>
        /// <param name="images"></param>
        /// <param name="execInfo"></param>
        /// <param name="number"></param>
        public Game(string title, string description, string operationDescription, Team developerTeam, GameServerConnectInfo clientInfo, ICollection<GameImage> images, ExecInfo execInfo, DisplayNumber number, ICollection<Guid> genreGuids, GameGenreCollection genreFullInformations)
        {
            this.title = title;
            this.description = description;
            this.operationDescription = operationDescription;

            this.developerTeam = developerTeam;
            this.clientInfo = clientInfo;
            
            this.images = new GameImageCollection();
            foreach (GameImage image in images)
                this.images.Add(image);

            this.execInfo = execInfo;
            this.displayNumber = number;
            
            this.gameGuid = Guid.NewGuid();
            this.informationFilePath = "";

            this.genres = new GameGenreCollection();
            foreach (Guid guid in genreGuids)
            {
                this.genres.Add(genreFullInformations.GetGenre(guid));
            }
        }

        

        // 非公開メソッド

        /// <summary>
        /// このインスタンスをStreamArchiveに保存する際の各種データアイテム名の辞書を取得します。
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> _getFileNameDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("NameInfo", "__redef_launcher__nameinfo.dict");
            dict.Add("DebugInfo", "__redef_launcher__debuginfo.dict");
            dict.Add("BasicInfo", "__redef_launcher__game__basicinfo.dat");
            dict.Add("TeamInfo", "__redef_launcher__game__teaminfo.dat");
            dict.Add("ClientInfo", "__redef_launcher__game__clientinfo.dat");
            dict.Add("ImageInfo", "__redef_launcher__game__imageinfo.dat");
            dict.Add("ExecInfo", "__redef_launcher__game__execinfo.dat");
            dict.Add("NumberInfo", "__redef_launcher__game__numberinfo.dat");
            dict.Add("GenreGuids", "__redef_launcher__game__genreguids.dat");
            //dict.Add("ControllersGuids", "__redef_launcher__game__controllerguids.dat");
            return dict;
        }
        
        /// <summary>
        /// 指定したArchiveMakerに現在のゲーム情報をアイテムとして追加します。
        /// </summary>
        /// <param name="maker"></param>
        private void _addToArchive(ArchiveMaker maker)
        {
            if (maker == null)
                throw new ArgumentNullException("Gameを出力アーカイブへ追加できませんでした。makerがnullです。");

            Dictionary<string, string> nameList = this._getFileNameDictionary();

            foreach (var name in nameList.Values)
                foreach (var item in maker.ItemList)
                    if (item.ItemFileName == name)
                        // アーカイブ内にすでに同名のファイルが存在
                        throw new Exception("指定されたmakerにはすでに追加ファイルと同名のアイテムが存在します。");

            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);
            MemoryStream ms;
            
            // 名前情報
            ms = new MemoryStream();
            bc.WriteDictionary(nameList, ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["NameInfo"], ms));

            // デバッグ情報
            ms = new MemoryStream();
            Dictionary<string, string> debugInfoDict = new Dictionary<string, string>();
            debugInfoDict.Add("DebugInfoGuid", Guid.NewGuid().ToString());
            debugInfoDict.Add("GlobalTime", DateTime.Now.ToString("r"));
            debugInfoDict.Add("RedefinableVersion", AssemblyInformationUtility.LibraryInformation.GetVersion().ToString());
            debugInfoDict.Add("EntryAssemblyVersion", AssemblyInformationUtility.EntryAssemblyInformation.GetVersion().ToString());
            debugInfoDict.Add("LauncherAssemblyGuid", LauncherAssemblyInformation.LauncherAssembly.Guid);
            debugInfoDict.Add("LauncherAssemblyDirectory", AssemblyInformationUtility.EntryAssemblyInformation.Dir);
            debugInfoDict.Add("GameInterfaceGuid", ""); // 未実装
            debugInfoDict.Add("GameInterfaceVersion", ""); // 未実装
            debugInfoDict.Add("GameInterfaceIPAddress", ""); // 未実装
            debugInfoDict.Add("GameInterfaceRequestToken", ""); // 未実装
            debugInfoDict.Add("FatalErrorAssembly", ""); // 未実装
            debugInfoDict.Add("FatalErrorAssemblyCommand", ""); // 未実装
            debugInfoDict.Add("FatalErrorAssemblyArguments", ""); // 未実装
            debugInfoDict.Add("FatalErrorAssemblyWait", "false"); // 未実装
            bc.WriteDictionary(debugInfoDict, ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["DebugInfo"], ms));

            // 基本情報
            ms = new MemoryStream();
            this._saveBasicInfoTo(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["BasicInfo"], ms));

            // 開発チーム情報
            ms = new MemoryStream();
            this.developerTeam.Save(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["TeamInfo"], ms));

            // ゲームサーバ接続情報
            if (this.clientInfo == null)
                ms = new MemoryStream(bc.GetBytes("GameServerInfo is Not Implemented."));
            else
            {
                ms = new MemoryStream();
                this.clientInfo.Save(ms);
            }

            maker.ItemList.Add(new StoringStreamItem(nameList["ClientInfo"], ms));

            // GameImages
            ms = new MemoryStream();
            this.images.Save(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["ImageInfo"], ms));

            // ExecInfo
            ms = new MemoryStream();
            this.execInfo.Save(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["ExecInfo"], ms));

            // DisplayNumberInfo
            ms = new MemoryStream();
            this.displayNumber.Save(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["NumberInfo"], ms));

            // GameGenreGuids
            ms = new MemoryStream();
            this.genres.SaveGuids(ms);
            maker.ItemList.Add(new StoringStreamItem(nameList["GenreGuids"], ms));
        }

        /// <summary>
        /// 指定したArchiveReaderからゲーム情報を読み取ります。
        /// </summary>
        /// <param name="reader"></param>
        private void _loadFromArchive(ArchiveReader reader, GameGenreCollection genreFullInformations)
        {
            if (reader == null)
                throw new ArgumentNullException("Gameを入力アーカイブから読み取れませんでした。readerがnullです。");

            Dictionary<string, string> nameList = this._getFileNameDictionary();
            

            // すべてのデータがあるか検査
            foreach (var name in nameList.Values)
                if (!reader.Items.Contains(name))
                    // nameという名前のアイテムは含まれていない
                    throw new ArgumentException("Gameを入力アーカイブから読み取れませんでした。内部アイテムが含まれていません。 : " + name);
            
            // アイテムのオフセット情報を利用して、直に読み込む
            FileStream stream = new FileStream(reader.ArchivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IArchiveItem item;

            try
            {
                // 基本情報
                item = reader.Items.GetItem(nameList["BasicInfo"]);
                this._loadBasicInfoFrom(new SubStream(stream, (long)item.StartOffset, (long)item.Length));

                // 開発チーム情報
                item = reader.Items.GetItem(nameList["TeamInfo"]);
                this.developerTeam = Team.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));

                // ゲームサーバ接続情報
                item = reader.Items.GetItem(nameList["ClientInfo"]);
                try
                {
                    this.clientInfo = GameServerConnectInfo.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));
                }
                catch (NotSupportedException)
                {
                    this.clientInfo = null;
                }

                // GameImages
                item = reader.Items.GetItem(nameList["ImageInfo"]);
                this.images = GameImageCollection.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));

                // ExecInfo
                item = reader.Items.GetItem(nameList["ExecInfo"]);
                this.execInfo = ExecInfo.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));

                // DisplayNumberInfo
                item = reader.Items.GetItem(nameList["NumberInfo"]);
                this.displayNumber = DisplayNumber.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));

                // GameGenreGuids
                item = reader.Items.GetItem(nameList["GenreGuids"]);
                this.genres = GameGenreCollection.LoadCollection(new SubStream(stream, (long)item.StartOffset, (long)item.Length), genreFullInformations, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }
        
        /// <summary>
        /// title、description、operationDescriptionを指定したストリームへ出力します。
        /// </summary>
        /// <param name="stream"></param>
        private void _saveBasicInfoTo(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter();

            // マジックコード
            bw.Write(bc.GetBytes(_MagicCode));
            
            // (新方式)
            bc.WriteArray(new string[]
            {
                this.title,
                this.description,
                this.operationDescription,
                this.gameGuid.ToString()
            }, stream);

            // ダミーデータ
            bw.Write(this.gameGuid.ToByteArray());
        }
        
        /// <summary>
        /// title、description、operationDescriptionを指定したストリームから読み込みます。
        /// </summary>
        /// <param name="stream"></param>
        private void _loadBasicInfoFrom(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter();
            
            // マジックコードの検査
            int magicLength = bc.GetBytes(_MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicLength)) != _MagicCode)
                throw new NotSupportedException("Game基本情報の読み込みに失敗しました。ヘッダの値が不正です。");
            
            // (新方式)
            string[] mainData = bc.ReadArray(stream);

            this.title = mainData[0];
            this.description = mainData[1];
            this.operationDescription = mainData[2];
            this.gameGuid = Guid.Parse(mainData[3]);
        }


        // 公開メソッド

        /// <summary>
        /// 指定したストリームに基本情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void SaveBasicInfo(Stream stream)
        {
            this._saveBasicInfoTo(stream);
        }

        /// <summary>
        /// 指定したファイルに基本情報を保存します。
        /// </summary>
        /// <param name="path"></param>
        public void SaveBasicInfo(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveBasicInfoTo(fs);
            fs.Close();
        }

        /// <summary>
        /// ゲームの情報をアーカイブのアイテムとして指定されたArchiveMakerクラスのアイテムリストに追加します。
        /// </summary>
        /// <param name="maker"></param>
        public void SaveToArchive(ArchiveMaker maker)
        {
            this._addToArchive(maker);
        }

        /// <summary>
        /// ゲームの情報をStreamArchive形式で保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            ArchiveMaker maker = new ArchiveMaker(path);
            this._addToArchive(maker);
            maker.Title = "Redefinable Game Launcher Game Information";
            maker.Author = "Redefinable Game Launcher";
            maker.Save();

            this.informationFilePath = Path.GetFullPath(path);
        }

        /// <summary>
        /// 指定したストリームから読み込んだ基本情報で現在のインスタンスの基本情報を上書きします。
        /// </summary>
        /// <param name="stream"></param>
        public void OverrideBasicInfo(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ゲームの実行ファイル本体のパスをフルパスで取得します。
        /// </summary>
        /// <returns></returns>
        public string GetGameExeFullPath()
        {
            string relative = this.execInfo.RelativePath;
            return IOUtility.GetFullPathFromFile(this.InformationFilePath, relative);
        }


        // 公開静的メソッド

        /// <summary>
        /// ゲームの基礎情報だけを読み込んだインスタンスを作成します (デバッグ用)
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="genreFullInformations"></param>
        /// <returns></returns>
        public static Game LoadBasicInfo(Stream stream, GameGenreCollection genreFullInformations)
        {
            Game game = new Game("", "", "", null, null, new GameImage[0], null, null, new Guid[0], genreFullInformations);
            game._loadBasicInfoFrom(stream);
            return game;
        }

        /// <summary>
        /// ゲームの基礎情報だけを読み込んだインスタンスを作成します (デバッグ用)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="genreFullInformations"></param>
        /// <returns></returns>
        public static Game LoadBasicInfo(string path, GameGenreCollection genreFullInformations)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            Game result = Game.LoadBasicInfo(fs, genreFullInformations);

            fs.Close();
            return result;
        }

        /// <summary>
        /// 指定したStreamArchiveのReaderを使用して、StreamArchiveファイルからゲーム情報を読み込みます。
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="genreFullInformations"></param>
        /// <returns></returns>
        public static Game LoadFromArchive(ArchiveReader reader, GameGenreCollection genreFullInformations)
        {
            Game result = new Game("", "", "", null, null, new GameImage[0], null, null, new Guid[0], genreFullInformations);
            result._loadFromArchive(reader, genreFullInformations);
            return result;
        }

        /// <summary>
        /// 指定したStreamArchiveファイルからゲーム情報を読み込みます。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="genreFullInformations"></param>
        /// <returns></returns>
        public static Game Load(string path, GameGenreCollection genreFullInformations)
        {
            ArchiveReader reader = null;
            Game result = null;

            try
            {
                reader = new ArchiveReader(path);
                result = Game.LoadFromArchive(reader, genreFullInformations);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }

            result.informationFilePath = Path.GetFullPath(path);
            return result;
        }
    }

}
