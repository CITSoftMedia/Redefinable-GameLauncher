using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;

namespace Redefinable.Applications.Launcher.Informations
{
    public class GameFilesDirectory
    {
        // 非公開フィールド
        private string path;
        private GameDirectoryCollection directories;
        private GameGenreCollection genreFullInformations;


        // 公開フィールド

        /// <summary>
        /// 現在このインスタンスで管理されいるGameDirectoryのコレクションを取得します。
        /// </summary>
        public GameDirectoryCollection Directories
        {
            get { return this.directories; }
        }


        // コンストラクタ

        /// <summary>
        /// 使用できません
        /// </summary>
        private GameFilesDirectory()
        {
            this.path = null;
            this.directories = new GameDirectoryCollection();
            this.genreFullInformations = new GameGenreCollection();
        }

        /// <summary>
        /// 指定したGameDirectoryのコレクションから、新しいGameFilesDirectoryクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="directories"></param>
        public GameFilesDirectory(ICollection<GameDirectory> directories, GameGenreCollection genreFullInformations)
            : this()
        {
            foreach (var dir in directories)
                this.directories.Add(dir);

            this.genreFullInformations = genreFullInformations;
        }

        /// <summary>
        /// 指定したGameFilesディレクトリのパスから、新しいGameFilesDirectoryのインスタンスを初期化します。
        /// </summary>
        /// <param name="path"></param>
        public GameFilesDirectory(string path, GameGenreCollection genreFullInformations)
            : this()
        {
            this.genreFullInformations = genreFullInformations;


            // 厳密なディレクトリ形式へ修正
            if (path[path.Length - 1] == '\\')
                path = path.Substring(0, path.Length - 1);

            // フルパスへ
            path = IOUtility.GetFullPath(Environment.CurrentDirectory, path);
            
            // 設定と追加
            this.path = path;
            this._addFrom(path);
        }


        // 非公開メソッド

        /// <summary>
        /// 指定したパスの中に含まれるディレクトリをゲームディレクトリとしてDirectoriesへ追加します。
        /// </summary>
        /// <param name="dirPath"></param>
        private void _addFrom(string dirPath)
        {
            // 厳密なディレクトリ形式へ修正
            if (dirPath[dirPath.Length - 1] == '\\')
                dirPath = dirPath.Substring(0, dirPath.Length - 1);

            // フルパスへ
            dirPath = IOUtility.GetFullPath(Environment.CurrentDirectory, dirPath);

            // 指定されたディレクトリが存在するかどうか検査
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException("指定されたディレクトリは存在しません。");

            // ディレクトリ内の項目一覧を取得
            string[] directories = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            
            // 追加処理
            foreach (string dir in directories)
                this.directories.Add(new GameDirectory(dir, this.genreFullInformations));
        }

        /// <summary>
        /// 現在DirectoriesにあるGameDirectoryの中で有効な項目のみをコレクションで取得します。
        /// </summary>
        /// <returns></returns>
        private ICollection<GameDirectory> _getValidDirectories()
        {
            List<GameDirectory> validList = new List<GameDirectory>();
            foreach(var dir in this.directories)
                if (dir.CheckInitialized())
                    validList.Add(dir);

            return validList;
        }


        // 公開メソッド

        /// <summary>
        /// 現在DirectoriesにあるGameDirectoryの中で有効な項目のみをコレクションで取得します。
        /// </summary>
        /// <returns></returns>
        public ICollection<GameDirectory> GetValidDirectories()
        {
            return this._getValidDirectories();
        }

        /// <summary>
        /// 無効なディレクトリをすべて初期化します。 (デバッグ用)
        /// </summary>
        public void InitializeAllDirectory()
        {
            foreach (var dir in this.directories)
            {
                if (!dir.CheckInitialized())
                    dir.Initialize(true);
            }
        }
    }
    
}
