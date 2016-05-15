using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Informations
{
    public class DisplayNumber
    {
        // 非公開フィールド
        private string precode;
        private int mainNumber;
        private string postcode;
        private Guid numberGuid;


        // 非公開静的フィールド

        public static string _MagicCode
        {
            get { return " Redefinable GameLauncher DISPLAY NUMBER Binary "; }
        }


        // 公開フィールド

        /// <summary>
        /// 番号の前に付加する文字列を取得・設定します。
        /// </summary>
        public string Precode
        {
            get { return this.precode; }
            set { this.precode = value; }
        }

        /// <summary>
        /// 番号を取得・設定します。この番号がソートで使用されます。
        /// </summary>
        public int MainNumber
        {
            get { return this.mainNumber; }
            set { this.mainNumber = value; }
        }

        /// <summary>
        /// 番号の後に付加する文字列を取得・設定します。
        /// </summary>
        public string Postcode
        {
            get { return this.postcode; }
            set { this.postcode = value; }
        }

        /// <summary>
        /// このDisplayNumberに割り当てられたGUIDを取得します。
        /// </summary>
        public Guid NumberGuid
        {
            get { return this.numberGuid; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいDisplayNumberクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="precode">番号の前に付加する文字列</param>
        /// <param name="mainNumber">ソートで使用する番号</param>
        /// <param name="postcode">番号の後に付加する文字列</param>
        public DisplayNumber(string precode, int mainNumber, string postcode)
        {
            this.precode = precode;
            this.mainNumber = mainNumber;
            this.postcode = postcode;

            this.numberGuid = Guid.NewGuid();
        }

        /// <summary>
        /// 新しいDisplayNumberクラスのインスタンスを初期化します。
        /// </summary>
        public DisplayNumber()
            : this("", 0, "")
        {
            // 実装なし
        }


        // 非公開メソッド

        public void _saveTo(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("指定されたストリームは書き込みが許可されていません。");

            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコード
            bw.Write(bc.GetBytes(DisplayNumber._MagicCode));

            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"Precode", this.precode},
                {"MainNumber", this.mainNumber.ToString()},
                {"Postcode", this.postcode},
                {"NumberGuid", this.numberGuid.ToString()}
            };

            bc.WriteDictionary(dict, stream);
        }

        public void _loadFrom(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("指定されたストリームは読み取りが許可されていません。");

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);


            // マジックコードの検査
            int magicCodeLength = bc.GetBytes(DisplayNumber._MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicCodeLength)) != DisplayNumber._MagicCode)
                throw new NotSupportedException("ゲーム起動情報の読み込みに失敗しました。ヘッダの値が不正です。");

            // mainData
            Dictionary<string, string> dict = new Dictionary<string, string>();
            bc.ReadDictionary(dict, stream);
            
            // 値の適用
            this.precode = dict["Precode"];
            this.mainNumber = Int32.Parse(dict["MainNumber"]);
            this.postcode = dict["Postcode"];
            this.numberGuid = Guid.Parse(dict["NumberGuid"]);
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
        public static DisplayNumber Load(Stream stream)
        {
            DisplayNumber execInfo = new DisplayNumber();
            execInfo._loadFrom(stream);

            return execInfo;
        }
    }
}
