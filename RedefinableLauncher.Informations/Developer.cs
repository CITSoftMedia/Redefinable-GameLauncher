using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IniHandler;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// Developer型オブジェクトのインスタンスのコレクションです。同一のインスタンスを２重で追加することはできません。
    /// </summary>
    public class DeveloperCollection : NativeEventDefinedList<Developer>
    {
        // コンストラクタ

        /// <summary>
        /// 新しいDeveloperCollectionクラスのインスタンスを初期化します。
        /// </summary>
        public DeveloperCollection()
            : this(new Developer[0])
        {
            // 実装なし
        }

        /// <summary>
        /// 指定したDeveloperのコレクションから新しいDeveloperCollectionクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="developers"></param>
        public DeveloperCollection(ICollection<Developer> developers)
            : base()
        {
            this.AddRange(developers);
        }


        // 公開メソッド

        /// <summary>
        /// 任意のDeveloperインスタンスのコレクション内のインデックス番号を取得します。指定されたDeveloperインスタンスがコレクションに含まれていない場合、-1を返します。
        /// </summary>
        /// <param name="developer"></param>
        /// <returns></returns>
        public int GetIndex(Developer developer)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == developer)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 指定されたDeveloperのコレクションのすべての要素をDeveloperCollectionに追加します。
        /// </summary>
        /// <param name="developers"></param>
        public void AddRange(ICollection<Developer> developers)
        {
            foreach (Developer dev in developers)
                this.Add(dev);
        }

        /// <summary>
        /// 指定されたDeveloperをコレクションに追加します。
        /// </summary>
        /// <param name="item"></param>
        public override void Add(Developer item)
        {
            if (this.Contains(item))
                throw new ArgumentException("追加しようとしたDeveloperインスタンスはすでにコレクションに含まれています。");

            base.Add(item);
        }
    }

    /// <summary>
    /// Developer情報を保存しているファイルのタイプを取得します。
    /// </summary>
    public enum DeveloperFileType : UInt32
    {
        /// <summary>
        /// INI 構成設定ファイル
        /// </summary>
        INI,

        /// <summary>
        /// XML
        /// </summary>
        XML,

        /// <summary>
        /// バイナリ
        /// </summary>
        Binary,

        /// <summary>
        /// Redefinable BinaryConverterを利用した辞書のバイナリ (UTF-8)
        /// </summary>
        DictionaryBinaryUTF8,

        /// <summary>
        /// Redefinable BinaryConverterを利用した辞書のバイナリ (ShiftJIS)
        /// </summary>
        DictionaryBinaryShiftJIS,
    }

    /// <summary>
    /// 
    /// </summary>
    public class Developer : ICloneable
    {
        // 非公開フィールド
        private string name;
        private string affiliation;
        private int grade;
        private string url;
        private Guid developerGuid;


        // 公開フィールド

        /// <summary>
        /// 開発者の名前を取得・設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 開発者の所属先を取得・設定します。
        /// </summary>
        public string Affiliation
        {
            get { return this.affiliation; }
            set { this.affiliation = value; }
        }

        /// <summary>
        /// 回生を取得・設定します。
        /// </summary>
        public int Grade
        {
            get { return this.grade; }
            set { this.grade = value; }
        }

        /// <summary>
        /// URLを取得・設定します。
        /// </summary>
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        /// <summary>
        /// この開発者情報に割り当てられたGuidを取得します。
        /// </summary>
        public Guid DeveloperGuid
        {
            get { return this.developerGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいDeveloperクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="affiliation"></param>
        /// <param name="grade"></param>
        /// <param name="url"></param>
        public Developer(string name, string affiliation, int grade, string url)
        {
            this.name = name;
            this.affiliation = affiliation;
            this.grade = grade;
            this.url = url;
            this.developerGuid = Guid.NewGuid();
        }

        /// <summary>
        /// 新しいDeveloperクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="affiliation"></param>
        public Developer(string name, string affiliation)
            : this(name, affiliation, 0, null)
        {
            // 実装なし
        }

        /// <summary>
        /// 使用しないでください
        /// </summary>
        public Developer()
        {
            // 実装なし
        }


        // 非公開メソッド

        private Developer _createClone()
        {
            return (Developer) this.MemberwiseClone();
        }

        
        // 公開メソッド

        /// <summary>
        /// このインスタンスの複製を取得します。
        /// </summary>
        /// <returns></returns>
        public Developer Clone()
        {
            return this._createClone();
        }

        /// <summary>
        /// このインスタンスの情報を保存します。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileType"></param>
        public void Save(Stream stream, DeveloperFileType fileType)
        {
            lock (stream)
            {
                switch (fileType)
                {
                    case DeveloperFileType.INI:
                        IniFile ini = new IniFile();
                        IniSection developerSection = new IniSection("Developer", new IniValue[]
                        {
                            new IniValue("name", this.name),
                            new IniValue("affiliation", this.affiliation),
                            new IniValue("grade", this.grade.ToString()),
                            new IniValue("url", this.url),
                            new IniValue("guid", this.developerGuid.ToString()),
                        });
                        ini.Sections.Add(developerSection);
                        ini.Save(stream);
                        break;
                    case DeveloperFileType.XML:
                        XmlSerializer xs = new XmlSerializer(this.GetType());
                        xs.Serialize(stream, this);
                        break;
                    case DeveloperFileType.Binary:
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(stream, this);
                        break;
                    case DeveloperFileType.DictionaryBinaryUTF8:
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("Name", this.name);
                        dict.Add("Affiliation", this.affiliation);
                        dict.Add("Grade", this.grade.ToString());
                        dict.Add("Url", this.url);
                        dict.Add("Guid", this.developerGuid.ToString());
                        BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);
                        bc.WriteDictionary(dict, stream);
                        break;
                    case DeveloperFileType.DictionaryBinaryShiftJIS:
                        Dictionary<string, string> dict2 = new Dictionary<string, string>();
                        dict2.Add("Name", this.name);
                        dict2.Add("Affiliation", this.affiliation);
                        dict2.Add("Grade", this.grade.ToString());
                        dict2.Add("Url", this.url);
                        dict2.Add("Guid", this.developerGuid.ToString());
                        BinaryConverter bc2 = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.GetEncoding("shift_jis"));
                        bc2.WriteDictionary(dict2, stream);
                        break;
                    default:
                        throw new NotImplementedException("無効なFileTypeが指定されました。");
                }
            }
        }


        // 公開メソッド :: インタフェースの明示的な実装

        Object ICloneable.Clone()
        {
            return this._createClone();
        }


        // 静的公開メソッド
        
        /// <summary>
        /// ファイルから開発者情報を読み取ります。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Developer Load(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            DeveloperFileType fileType = DeveloperFileType.Binary;
            
            switch (ext)
            {
                case ".ini":
                    fileType = DeveloperFileType.INI;
                    break;
                case ".xml":
                    fileType = DeveloperFileType.XML;
                    break;
            }
            
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Developer result = Developer.Load(fs, fileType);
                return result;
            }
        }

        /// <summary>
        /// データを保持するストリームから、開発者情報を読み取ります。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Developer Load(Stream stream, DeveloperFileType type)
        {
            if (!stream.CanRead)
                throw new ArgumentException("指定されたストリームは読み取ることができません。");

            Developer result = null;
            switch (type)
            {
                case DeveloperFileType.INI:
                    {
                        IniFile ini = new IniFile(stream);
                        if (!ini.Sections.ContainsName("Developer"))
                            throw new RedefinableIniHandlerException("指定されたINIデータには開発者情報が含まれていません。");

                        IniSection section = ini.Sections["Developer"];
                        IniValueCollection values = section.Values;

                        result = new Developer("unknown", "", 0, null);
                        

                        if (values.ContainsName("name"))
                            result.name = values["name"].Value;

                        if (values.ContainsName("affiliation"))
                            result.affiliation = values["affiliation"].Value;

                        int grade = 0;
                        if (values.ContainsName("grade") && Int32.TryParse(values["grade"].Value, out grade))
                        {
                            result.grade = grade;
                        }

                        if (values.ContainsName("url"))
                            result.url = values["url"].Value;

                        if (values.ContainsName("guid"))
                            result.developerGuid = Guid.Parse(values["guid"].Value);
                        else
                            result.developerGuid = Guid.NewGuid();

                        
                        break;
                    }
                case DeveloperFileType.XML:
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(Developer));
                        result = (Developer) xs.Deserialize(stream);

                        break;
                    }
                case DeveloperFileType.Binary:
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        result = (Developer) bf.Deserialize(stream);
                        
                        break;
                    }
                case DeveloperFileType.DictionaryBinaryUTF8:
                    {
                        BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        bc.ReadDictionary(dict, stream);

                        result = new Developer("unknown", "", 0, null);
                        result.Name = dict["Name"];
                        result.Affiliation = dict["Affiliation"];
                        result.Grade = Int32.Parse(dict["Grade"]);
                        result.Url = dict["Url"];
                        result.developerGuid = Guid.Parse(dict["Guid"]);

                        break;
                    }
                case DeveloperFileType.DictionaryBinaryShiftJIS:
                    {
                        BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.GetEncoding("shift_jis"));
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        bc.ReadDictionary(dict, stream);

                        result = new Developer("unknown", "", 0, null);
                        result.Name = dict["Name"];
                        result.Affiliation = dict["Affiliation"];
                        result.Grade = Int32.Parse(dict["Grade"]);
                        result.Url = dict["Url"];
                        result.developerGuid = Guid.Parse(dict["Guid"]);

                        break;
                    }
                default:
                    throw new NotSupportedException("指定された開発者情報のデータ形式 '" + type + "' には対応していません。");
            }

            return result;
        }
    }
}
