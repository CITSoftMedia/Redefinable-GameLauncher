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
    
    public class GameServerConnectInfo
    {
        // 非公開フィールド
        private string accountName;
        private string password;
        private string gameIdentifier;

        
        // 非公開静的フィールド
        
        private static string _MagicCode
        {
            get { return " Redefinable GameLauncher GAMESERVER CLIENT INFORMATION Binary "; }
        }


        // 公開フィールド

        /// <summary>
        /// サーバログオン時のアカウント名を取得・設定します。
        /// </summary>
        public string AccountName
        {
            get { return this.accountName; }
            set { this.accountName = value; }
        }

        /// <summary>
        /// サーバログオン時のパスワードを取得・設定します。
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        /// <summary>
        /// GameIdentifierを取得・設定します。
        /// </summary>
        public string GameIdentifier
        {
            get { return this.gameIdentifier; }
            set { this.gameIdentifier = value; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいGameServerConnetInfoクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <param name="gameIdentifier"></param>
        public GameServerConnectInfo(string accountName, string password, string gameIdentifier)
        {
            this.accountName = accountName;
            this.password = password;
            this.gameIdentifier = gameIdentifier;
        }


        // 非公開メソッド

        /// <summary>
        /// 指定したストリームへ現在のインスタンスの情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        private void _saveTo(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("ゲームサーバの接続情報を出力できませんでした。指定されたstreamは書き込みが許可されていません。", "stream");

            BinaryWriter bw = new BinaryWriter(stream);
            BinaryConverter bc = new BinaryConverter();
            

            // マジックコード
            bw.Write(bc.GetBytes(_MagicCode));

            // MainDataの生成
            byte[] mainData = bc.GetBytesWithUInt16LengthAndUInt32ValueLengthInfo(new string[]
            {
                accountName,
                password,
                gameIdentifier,
                Guid.NewGuid().ToString()
            });

            // MainData長の書き込み
            bw.Write(bc.GetBytes((uint) mainData.Length));

            // MainDataの書き込み
            bw.Write(mainData);
        }

        /// <summary>
        /// 指定したストリームから情報を読み取ります。
        /// </summary>
        /// <param name="stream"></param>
        private void _loadFrom(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("ゲームサーバの接続情報を読み込めませんでした。指定されたstreamは読み取りが許可されていません。", "stream");

            BinaryReader br = new BinaryReader(stream);
            BinaryConverter bc = new BinaryConverter();


            // マジックコードの検査
            int magicLength = bc.GetBytes(_MagicCode).Length;
            if (bc.GetString(br.ReadBytes(magicLength)) != _MagicCode)
                    throw new NotSupportedException("ゲームサーバの接続情報の読み込みに失敗しました。ヘッダの値が不正です。");

            // MainDataのバイト長の読み取り
            uint mainDataLength = bc.ToUInt32(br.ReadBytes(sizeof(UInt32)));

            // MainDataの読み取り
            string[] mainData = bc.GetArrayWithUInt16LengthAndUInt32ValueLengthInfo(br.ReadBytes((int) mainDataLength));

            this.accountName = mainData[0];
            this.password = mainData[1];
            this.gameIdentifier = mainData[2];
        }


        // 公開メソッド
        
        /// <summary>
        /// 指定したストリームへ現在のインスタンスの保有する接続情報を出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }
        

        // 非公開メソッド


        // 公開静的メソッド

        public static GameServerConnectInfo Load(Stream stream)
        {
            GameServerConnectInfo result = new GameServerConnectInfo("", "", "");
            result._loadFrom(stream);
            return result;
        }
    }
}
