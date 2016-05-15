using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.IO;

using Redefinable.Applications.Launcher;
using Redefinable.Applications.Launcher.Forms;
using Redefinable.Applications.Launcher.Informations;

namespace Redefinable.Applications.Launcher.Core
{
    public static class LauncherStarter
    {
        private static LauncherSettings _initializeSettings()
        {
            // 基本設定
            LauncherSettings settings;

            try
            {
                // 読み込んでみる
                settings = LauncherSettings.Load();
            }
            catch
            {
                // 壊れてるので初期設定を利用
                settings = new LauncherSettings();
                settings.Save();

                // 再度読み込み
                settings = LauncherSettings.Load();
            }

            return settings;
        }

        private static GameCollection _getTargetGames(LauncherSettings settings)
        {
            DebugConsole.Push("COR", "Load> GameFilesディレクトリの検索を開始します。");
            DebugConsole.Push("COR", "Load> " + settings.GameFilesDirectory);

            if (!Directory.Exists(settings.GameFilesDirectory))
                throw new DirectoryNotFoundException("GameFilesDirectoryが見つかりません。 " + settings.GameFilesDirectory);
            
            var gfDir = new GameFilesDirectory(settings.GameFilesDirectory);
            var gDirs = gfDir.GetValidDirectories();
            DebugConsole.Push("COR", "Load> 有効なディレクトリは、" + gDirs.Count + "個です。");
            
            if (gDirs.Count == 0 && settings.UseAutoInitializer)
            {
                DebugConsole.Push("COR", "Load> UseAutoInitializerがtrueです。");
                gfDir.InitializeAllDirectory();
                gDirs = gfDir.GetValidDirectories();
                DebugConsole.Push("COR", "Load> 有効なディレクトリは、" + gDirs.Count + "個です。");
            }

            if (settings.ShowPathErrorGame)
                DebugConsole.Push("COR", "Load> 参照エラーが発生している作品も表示する設定になっています。");
            else
                DebugConsole.Push("COR", "Load> 参照エラーが発生しているゲームを検索します。");

            GameCollection games = new GameCollection();
            GameCollection warnings = new GameCollection();
            foreach (var dir in gDirs)
            {
                Game game = dir.Load();
                DebugConsole.Push("COR", "Load> ロード: " + game.Title);

                if (File.Exists(game.GetGameExeFullPath()))
                {
                    // 本体ファイルが存在する
                    games.Add(game);
                }
                else
                {
                    DebugConsole.Push("COR", "Load> 参照エラー: " + game.Title);

                    if (settings.ShowPathErrorGame)
                        // 本体ファイルが存在しないが、存在しない作品も表示する設定になっている
                        games.Add(game);

                    if (settings.WarningPathError)
                        // 本体ファイルが存在しない場合に、警告する設定になっている
                        warnings.Add(game);
                }
            }

            if (warnings.Count != 0)
            {
                DebugConsole.Push("COR", "Load> 参照エラーの警告が有効になっています。");

                string message = "";
                foreach (var item in warnings)
                    message += "・" + item.Title + "\n";
                MessageBox.Show("次の作品は、ゲーム本体の実行ファイルの参照が正しく設定されていません。\n\n" + message, settings.WindowText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return games;
        }

        /// <summary>
        /// ランチャーを開始します。
        /// </summary>
        public static void Start()
        {
            DebugConsole.Push("Core::Start()");
            DebugConsole.Push("COR", "ソフトウェアを開始します。");

            // 基本設定
            DebugConsole.Push("COR", "基本設定をロードします。");
            LauncherSettings settings = _initializeSettings();

            // 対象作品一覧のロード
            DebugConsole.Push("COR", "ゲーム情報を読み込みます。");
            GameCollection games = _getTargetGames(settings);

            // メインウィンドウの表示
            DebugConsole.Push("COR", "メインウィンドウを初期化します。");
            MainForm mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}
