using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// ゲーム実行ファイルの実行に関する情報を格納します。
    /// </summary>
    public class ExecInfo
    {
        // 非公開フィールド
        private string relativePath;
        private string arguments;
        private bool autoCurrentDirectory;
        private Guid execInfoGuid;


        // 非公開静的フィールド

        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher EXEC INFORMATION Binary "; }
        }

        
        // 公開フィールド
        
        /// <summary>
        /// ゲーム情報ファイルからゲーム本体の実行ファイルへのパスを取得・設定します。
        /// </summary>
        public string RelativePath
        {
            get { return this.relativePath; }
            set { this.relativePath = value; }
        }

        /// <summary>
        /// ゲーム実行時のコマンドライン引数を取得・設定します。Launcherの提供するRedefinableScripting記法が利用可能です。
        /// </summary>
        public string Arguments
        {
            get { return this.arguments; }
            set { this.arguments = value; }
        }

        /// <summary>
        /// ゲーム実行時にプロセスのカレントディレクトリをゲーム本体の実行ファイルのディレクトリにするかどうかを示す値を取得・設定します。
        /// </summary>
        public bool AutoCurrentDirectory
        {
            get { return this.autoCurrentDirectory; }
            set { this.autoCurrentDirectory = value; }
        }

        /// <summary>
        /// この情報に割り当てられたGuidを取得・設定します。
        /// </summary>
        public Guid ExecInfoGuid
        {
            get { return this.execInfoGuid; }
            set { this.execInfoGuid = value; }
        }

        
        // コンストラクタ

        /// <summary>
        /// 新しいExecInfoクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="arguments"></param>
        /// <param name="autoCurrentDirectory"></param>
        public ExecInfo(string relativePath, string arguments, bool autoCurrentDirectory)
        {
            this.relativePath = relativePath;
            this.arguments = arguments;
            this.autoCurrentDirectory = autoCurrentDirectory;
            this.execInfoGuid = Guid.NewGuid();
        }


        // 非公開メソッド

        private void _saveTo(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("指定されたストリームは書き込みが許可されていません。");

            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコード
            bw.Write(bc.GetBytes(ExecInfo._MagicCode));

            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"RelativePath", this.relativePath},
                {"Arguments", this.arguments},
                {"AutoCurrentDirectory", this.autoCurrentDirectory.ToString()},
                {"ExecInfoGuid", this.execInfoGuid.ToString()},
                {"AssemblyType", "Win32|WinFX"},
                {"AssemblySubsystem", "GUI"},
                {"ApplicationHelper", "launcher internal"},
                {"ApplicationHelperPath", ""},
                {"ApplicationHelperArgument", ""}
            };

            bc.WriteDictionary(dict, stream);
        }

        private void _loadFrom(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("指定されたストリームは読み取りが許可されていません。");

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコードの検査
            int magicCodeLength = bc.GetBytes(ExecInfo._MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicCodeLength)) != ExecInfo._MagicCode)
                throw new NotSupportedException("ゲーム起動情報の読み込みに失敗しました。ヘッダの値が不正です。");

            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>();
            bc.ReadDictionary(dict, stream);
            
            // 値の適用
            this.relativePath = dict["RelativePath"];
            this.arguments = dict["Arguments"];
            this.autoCurrentDirectory = Boolean.Parse(dict["AutoCurrentDirectory"]);
            this.execInfoGuid = Guid.Parse(dict["ExecInfoGuid"]);
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
        /// 指定したストリームからExecInfoクラスの情報を取得します。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ExecInfo Load(Stream stream)
        {
            ExecInfo execInfo = new ExecInfo("", "", false);
            execInfo._loadFrom(stream);

            return execInfo;
        }
    }
}
