using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IO;

namespace Redefinable.Applications.Launcher.Informations
{
    /// <summary>
    /// GameDirectoryのコレクション機能を提供します。
    /// </summary>
    public class GameDirectoryCollection : NativeEventDefinedList<GameDirectory>
    {
        // 公開メソッド

        /// <summary>
        /// 指定された登録ディレクトリ名のGameDirectoryが含まれているか、確認します。
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public bool ContainsName(string dirName)
        {
            foreach (GameDirectory dir in this)
                if (dir.DirectoryName == dirName)
                    return true;
            return false;
        }
    }

    /// <summary>
    /// GameFiles内のディレクトリを管理します。
    /// </summary>
    public class GameDirectory
    {
        // 非公開フィールド
        private string path;
        private GameGenreCollection genreFullInformations;
        private GameControllerCollection controllerFullInformations;
        
        
        // 公開フィールド

        /// <summary>
        /// ディレクトリのパスを取得します。
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// 登録ディレクトリ名を取得します。
        /// </summary>
        public string DirectoryName
        {
            get { return this._getValueOf_directoryName(); }
        }

        /// <summary>
        /// ゲーム情報ファイル (Gameクラスのオブジェクトを保存したもの) のパス
        /// </summary>
        public string GameInformationFilePath
        {
            get { return this._getValueOf_gameInformationFilePath(); }
        }

        /// <summary>
        /// ディレクトリが存在するかどうかを示す値を取得します。
        /// </summary>
        public bool IsExist
        {
            get { return this._getValueOf_isExist(); }
        }


        // 公開静的フィールド

        /// <summary>
        /// 各ゲームディレクトリに配置されているゲーム情報ファイルのファイル名を取得します。
        /// </summary>
        public static string InformationFileName
        {
            get { return "RedefinableGameInformation.dat"; }
        }

        
        // コンストラクタ

        /// <summary>
        /// 新しいGameDirectoryクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="genreFullInformations">GameGenreのGuid値を完全な情報に変換する際に使用する完全なジャンル情報のコレクション</param>
        public GameDirectory(string path, GameGenreCollection genreFullInformations, GameControllerCollection controllerFullInformations)
        {
            this.genreFullInformations = genreFullInformations;
            this.controllerFullInformations = controllerFullInformations;

            if (path == null)
                throw new ArgumentNullException("path", "pathにnullを指定することは出来ません。");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("GameDirectoryクラスのインスタンスの初期化に失敗しました。存在しないディレクトリをpathに指定することは出来ません。");

            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);

            // ディレクトリパスをフルパスへ
            this.path = IOUtility.GetFullPath(Environment.CurrentDirectory, path);
        }


        // 非公開メソッド
        
        private string _getValueOf_directoryName()
        {
            return System.IO.Path.GetFileName(this.path);
        }

        private string _getValueOf_gameInformationFilePath()
        {
            return this.path + "\\" + InformationFileName;
        }

        private bool _getValueOf_isExist()
        {
            return Directory.Exists(this.path);
        }

        private bool _checkInitialized()
        {
            if (!_getValueOf_isExist())
                // ディレクトリがない
                return false;

            if (!File.Exists(this._getValueOf_gameInformationFilePath()))
                // ゲーム情報ファイルがない
                return false;

            try
            {
                Game gameInfo = Game.Load(this._getValueOf_gameInformationFilePath(), this.genreFullInformations, this.controllerFullInformations);
                if (gameInfo == null)
                    // なんかよくわからんけど読み込み失敗
                    return false;
                
                // 有効なファイルが存在した！
                return true;
            }
            catch
            {
                // ファイルが壊れてる
                return false;
            }
        }


        // 公開メソッド

        /// <summary>
        /// このディレクトリに有効なゲーム情報ファイルが存在しているか確認します。
        /// </summary>
        /// <returns></returns>
        public bool CheckInitialized()
        {
            return this._checkInitialized();
        }

        /// <summary>
        /// 実行ファイルのパスを指定して、このディレクトリにゲーム情報ファイルを設置します。
        /// </summary>
        /// <param name="exePath">ゲーム本体の実行ファイルのフルパス</param>
        public void Initialize(string exePath)
        {
            string relativePath = "";
            if (exePath != null && exePath != "")
                // ディレクトリからの相対パスへ変換
                relativePath = IOUtility.GetRelativePath(this.path, exePath);
            
            Team devTeam = new Team()
            {
                Name = "開発チーム",
                Description = "この作品は次のメンバーによって制作されました。",
                LeaderIndex = 0,
            };

            devTeam.Developers.Add(new Developer("開発者名 (ハンドルネーム)", "開発者所属先 (班)", 0, ""));

            Game gameInfo = new Game(
                "GameTitle",
                "GameDescription ゲームの概要",
                "GameOperationDescription 基本的な操作説明 (複数行)",
                devTeam,
                new GameServerConnectInfo("", "", ""),
                new GameImage[0],
                new ExecInfo(relativePath, "", true),
                new DisplayNumber(),
                new Guid[] { GameGenre.CreateEmpty().GenreGuid },
                this.genreFullInformations,
                new Guid[] { GameController.CreateEmpty().ControllerGuid },
                this.controllerFullInformations,
                new Banner(null) );

            gameInfo.Save(this._getValueOf_gameInformationFilePath());
        }

        /// <summary>
        /// 実行ファイルの自動検索を有効にするかどうかを指定して、このディレクトリにゲーム情報ファイルを設置します。
        /// </summary>
        /// <param name="autoDetect"></param>
        public void Initialize(bool autoDetect)
        {
            string exePath = this.path + "\\game.exe";

            if (autoDetect)
            {
                string[] fileList = Directory.GetFiles(this.path, "*.exe", SearchOption.TopDirectoryOnly);
                foreach (string filepath in fileList)
                {
                    //DebugConsole.Push("EXE: " + filepath);
                    if (filepath.IndexOf(".vshost.exe") == -1 && filepath.IndexOf(".ExHandler.exe") == -1)
                    {
                        // 構成アセンブリではない
                        // → メインアプリケーション
                        exePath = filepath;
                        break;
                    }
                }
            }
            
            this.Initialize(exePath);
        }

        /// <summary>
        /// 実行ファイルの自動検索を使用して、このディレクトリにゲーム情報ファイルを設置します。
        /// </summary>
        public void Initialize()
        {
            this.Initialize(true);
        }

        /// <summary>
        /// ゲーム情報ファイルを読み込み、Gameオブジェクトのインスタンスを作成します。
        /// </summary>
        /// <returns></returns>
        public Game Load()
        {
            if (!File.Exists(this._getValueOf_gameInformationFilePath()))
                throw new FileNotFoundException("ゲームの情報ファイルが見つかりません。初期化してください。", this._getValueOf_gameInformationFilePath());

            Game game = Game.Load(this._getValueOf_gameInformationFilePath(), this.genreFullInformations, this.controllerFullInformations);

            return game;
        }
    }
}
