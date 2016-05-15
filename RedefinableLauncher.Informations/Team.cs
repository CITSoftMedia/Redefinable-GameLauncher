using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;
using Redefinable.StreamArchive;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// 開発チームの情報
    /// </summary>
    public class Team
    {
        // 非公開フィールド
        private string name;
        private DeveloperCollection developers;
        private int leaderIndex;
        private string description;
        private Guid teamGuid;


        // 非公開静的フィールド
        
        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher TEAM Information Binary  "; }
        }


        // 公開フィールド
        
        /// <summary>
        /// チームの名前を取得・設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 開発者情報のコレクションを取得します。
        /// </summary>
        public DeveloperCollection Developers
        {
            get { return this.developers; }
        }

        /// <summary>
        /// チームの代表者のDeveloperインスタンスをDevelopersのインデックス番号で取得・設定します。
        /// </summary>
        public int LeaderIndex
        {
            get { return this.leaderIndex; }
            set { this.leaderIndex = value; }
        }

        /// <summary>
        /// チームの代表者のDeveloperインスタンスを取得・設定します。設定する場合、そのインスタンスはDevelopersに含まれていなければなりません。
        /// </summary>
        public Developer Leader
        {
            get { return this._getLeader(); }
            set { this._setLeader(value); }
        }

        /// <summary>
        /// チームの説明を取得・設定します。
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// このTeam情報に割り当てられたGuidの値を取得します。
        /// </summary>
        public Guid TeamGuid
        {
            get { return this.teamGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいTeamクラスのインスタンスを初期化します。
        /// </summary>
        public Team()
        {
            this.name = "";
            this.leaderIndex = 0;
            this.description = "";
            this.teamGuid = Guid.NewGuid();
            this.developers = new DeveloperCollection();
        }

        /// <summary>
        /// ストリームから新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stream"></param>
        public Team(Stream stream)
            : this()
        {
            this._loadFrom(stream);
        }


        // 非公開メソッド

        public Developer _getLeader()
        {
            return this.developers[this.leaderIndex];
        }

        public void _setLeader(Developer developer)
        {
            int index = this.developers.GetIndex(developer);
            if (index == -1)
                throw new KeyNotFoundException("指定されたDeveloperインスタンスはコレクションに含まれていません。");

            this.leaderIndex = index;
        }

        private void _saveTo(Stream stream)
        {
            this._saveTo(stream, DeveloperFileType.DictionaryBinaryUTF8);
        }

        private void _saveTo(Stream stream, DeveloperFileType developerDataType)
        {
            lock (stream)
            {
                BinaryWriter bw = new BinaryWriter(stream);
                BinaryConverter bc = new BinaryConverter();

                // マジックコード
                bw.Write(bc.GetBytes(_MagicCode));

                // 基本情報
                byte[] basicInfoBytes = bc.GetBytesWithUInt16LengthAndUInt32ValueLengthInfo(new string[]
                {
                    this.name,
                    this.leaderIndex.ToString(),
                    this.description,
                    this.teamGuid.ToString()
                });

                uint basicInfoLength = (uint)basicInfoBytes.Length;

                bw.Write(bc.GetBytes(basicInfoLength));
                bw.Write(basicInfoBytes);

                // 開発者情報
                bw.Write(bc.GetBytes((uint) this.developers.Count));
                foreach (Developer dev in this.developers)
                {
                    // データタイプ
                    bw.Write(bc.GetBytes((uint) developerDataType));
                    
                    // バイトデータの生成
                    MemoryStream ms = new MemoryStream();
                    dev.Save(ms, developerDataType);
                    byte[] developerData = ms.ToArray();

                    // データサイズ書き込み
                    bw.Write(bc.GetBytes((uint) developerData.Length));

                    // データ書き込み
                    bw.Write(developerData);
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
                int magicCodeLength = Team._MagicCode.Length;
                if (bc.GetString(br.ReadBytes(magicCodeLength)) != Team._MagicCode)
                    throw new NotSupportedException("Teamデータの読み込みに失敗しました。ヘッダの値が不正です。");

                // 基本情報の取得
                uint basicInfoLength = bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));
                byte[] basicInfoData = br.ReadBytes((int) basicInfoLength);
                string[] basicInfo = bc.GetArrayWithUInt16LengthAndUInt32ValueLengthInfo(basicInfoData);

                this.name = basicInfo[0];
                this.leaderIndex = Int32.Parse(basicInfo[1]);
                this.description = basicInfo[2];
                this.teamGuid = Guid.Parse(basicInfo[3]);
                
                // 開発者情報
                int devCount = (int) bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));
                for(int i = 0; i < devCount; i++)
                {
                    // データタイプ
                    DeveloperFileType fileType = (DeveloperFileType) bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));

                    // データサイズ
                    uint dataSize = bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));
                    
                    // バイトデータ取得
                    byte[] devData = br.ReadBytes((int) dataSize);
                    MemoryStream ms = new MemoryStream(devData);

                    this.developers.Add(Developer.Load(ms, fileType));
                }
            }
        }


        // 公開メソッド
        
        /// <summary>
        /// 指定したファイルへチーム情報を保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            this._saveTo(fs);
            fs.Close();
        }

        /// <summary>
        /// 指定したストリームへチーム情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }

        /// <summary>
        /// 指定したストリームへ指定したデータ形式でチーム情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="developerFileType"></param>
        public void Save(Stream stream, DeveloperFileType developerFileType)
        {
            this._saveTo(stream, developerFileType);
        }


        // 公開静的メソッド

        public static Team Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            Team result = Team.Load(fs);
            fs.Close();

            return result;
        }
        
        public static Team Load(Stream stream)
        {
            return new Team(stream);
        }
    }
}
