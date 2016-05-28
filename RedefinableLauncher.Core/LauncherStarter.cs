using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.IO;

using Redefinable.Applications.Launcher;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Forms;
using Redefinable.Applications.Launcher.Informations;


namespace Redefinable.Applications.Launcher.Core
{
    public static class LauncherStarter
    {
        private static GameGenreCollection genreFullInfo = null;
        private static GameControllerCollection controllerFullInfo = null;

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
            // ジャンル情報
            DebugConsole.Push("COR", "Genre> GenreFilesディレクトリの検索を開始します。");
            DebugConsole.Push("COR", "Genre> " + settings.GenreFilesDirectory);

            if (!Directory.Exists(settings.GenreFilesDirectory))
                throw new DirectoryNotFoundException("GenreFilesDirectoryが見つかりません。 " + settings.GenreFilesDirectory);

            GameGenreCollection genreFullInformations = new GameGenreCollection();
            genreFullInfo = genreFullInformations; 
            genreFullInformations.AddFromDirectory(settings.GenreFilesDirectory);

            if (genreFullInformations.Count == 0)
            {
                genreFullInformations = GameGenreCollection.GetDefaultGenres();
                genreFullInformations.SaveToDirectory(settings.GenreFilesDirectory);
            }


            // コントローラ情報
            DebugConsole.Push("COR", "Cntrl> ControllerFilesディレクトリの検索を開始します。");
            DebugConsole.Push("COR", "Cntrl> " + settings.GenreFilesDirectory);

            if (!Directory.Exists(settings.ControllersFilesDirectory))
                throw new DirectoryNotFoundException("ControllerFilesDirectoryが見つかりません。 " + settings.GenreFilesDirectory);

            GameControllerCollection controllerFullInformations = new GameControllerCollection();
            controllerFullInfo = controllerFullInformations;
            controllerFullInformations.AddFromDirectory(settings.ControllersFilesDirectory);

            if (controllerFullInformations.Count == 0)
            {
                controllerFullInformations = GameControllerCollection.GetDefaultControllers();
                controllerFullInformations.SaveToDirectory(settings.ControllersFilesDirectory);
            }


            // GameFilesディレクトリ
            DebugConsole.Push("COR", "Load> GameFilesディレクトリの検索を開始します。");
            DebugConsole.Push("COR", "Load> " + settings.GameFilesDirectory);

            if (!Directory.Exists(settings.GameFilesDirectory))
                throw new DirectoryNotFoundException("GameFilesDirectoryが見つかりません。 " + settings.GameFilesDirectory);
            
            var gfDir = new GameFilesDirectory(settings.GameFilesDirectory, genreFullInformations, controllerFullInformations);
            var gDirs = gfDir.GetValidDirectories();
            DebugConsole.Push("COR", "Load> 有効なディレクトリは、" + gDirs.Count + "個です。");
            
            if (gDirs.Count == 0 && settings.UseAutoInitializer)
            {
                DebugConsole.Push("COR", "Load> UseAutoInitializerがtrueです。");
                gfDir.InitializeAllDirectory();
                gDirs = gfDir.GetValidDirectories();
                DebugConsole.Push("COR", "Load> 有効なディレクトリは、" + gDirs.Count + "個です。");
            }

            if (gDirs.Count == 0)
                throw new Exception("登録ディレクトリが0件の状態でランチャーを起動することはできません。");

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

                string fpath = game.GetGameExeFullPath();
                if (File.Exists(fpath))
                {
                    // 本体ファイルが存在する
                    games.Add(game);
                }
                else
                {
                    DebugConsole.Push("COR", "Load> 参照エラー: " + game.Title);
                    DebugConsole.Push("COR", "Load> " + fpath);

                    if (settings.ShowPathErrorGame)
                    {
                        // 本体ファイルが存在しないが、存在しない作品も表示する設定になっている
                        games.Add(game);
                        DebugConsole.Push("COR", "Load> 参照エラーも表示する設定になっています。");
                    }
                    else
                    {
                        DebugConsole.Push("COR", "Load> この作品は非表示になります。");
                    }
                    
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

            games.SortWithDisplayNumber();
            return games;
        }
        
        private static LauncherTheme _getLauncherTheme(LauncherSettings settings)
        {
            if (settings.ThemeDirectory[settings.ThemeDirectory.Length - 1] == '\\')
                settings.ThemeDirectory = settings.ThemeDirectory.Substring(0, settings.ThemeDirectory.Length - 1);

            DebugConsole.Push("COR", "Theme> Themesディレクトリの検索を開始します。");
            DebugConsole.Push("COR", "Theme> " + settings.ThemeDirectory);

            string[] files = Directory.GetFiles(settings.ThemeDirectory, "*.rlt", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);

            if (files.Length == 0)
            {
                // 検出なし
                DebugConsole.Push("COR", "Theme> デフォルトテーマを初期化します。");
                LauncherTheme def = LauncherTheme.GetDefaultTheme();
                def.Save(settings.ThemeDirectory + "\\" + settings.ThemeFile);
            }
            else
                DebugConsole.Push("COR", "Theme> " + String.Join(", ", files));

            return LauncherTheme.Load(settings.ThemeDirectory + "\\" + settings.ThemeFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        private static void _formInitialize(MainForm form, ICollection<Game> games, LauncherSettings settings)
        {
            var enm = games.GetEnumerator();
            enm.MoveNext();
            var currentGame = enm.Current;
            
            var launcherPanel = form.GetLauncherPanel();


            // ジャンル情報一覧の追加
            Dictionary<Controls.ChildSelectPanelItem, GameGenre> genreDict = new Dictionary<Controls.ChildSelectPanelItem, GameGenre>();
            foreach (GameGenre gg in genreFullInfo)
            {
                if (gg.GenreGuid == Guid.Empty)
                    continue;

                Controls.ChildSelectPanelItem item = new Controls.ChildSelectPanelItem(gg.Name, true);
                launcherPanel.GenreSelectPanel.Items.Add(item);
                genreDict.Add(item, gg);
            }

            // コントローラ情報一覧の追加
            Dictionary<Controls.ChildSelectPanelItem, GameController> controllerDict = new Dictionary<Controls.ChildSelectPanelItem, GameController>();
            foreach (GameController gc in controllerFullInfo)
            {
                if (gc.ControllerGuid == Guid.Empty)
                    continue;

                Controls.ChildSelectPanelItem item = new Controls.ChildSelectPanelItem(gc.Name, true);
                launcherPanel.ControllerSelectPanel.Items.Add(item);
                controllerDict.Add(item, gc);
            }

            // NoImage画像の作成
            Image noImage = new Bitmap(1024, 576);
            Graphics noImage_g = Graphics.FromImage(noImage);
            noImage_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            noImage_g.FillRectangle(Brushes.DarkGray, 0, 0, noImage.Width, noImage.Height);

            Font f = new Font("MS UI Gohic", 20, FontStyle.Bold);
            SizeF strSize = noImage_g.MeasureString("NO IMAGE", f);
            noImage_g.DrawString("NO IMAGE", f, Brushes.White, new PointF((noImage.Width - strSize.Width) / 2, (noImage.Height - strSize.Height) / 2));
            noImage_g.Dispose();

            Func<ICollection<GameGenre>> getCheckedGenres = () =>
            {
                List<GameGenre> result = new List<GameGenre>();
                foreach (var item in launcherPanel.GenreSelectPanel.Items)
                    if (item.Checked)
                        result.Add(genreDict[item]);
                return result;
            };

            Func<ICollection<GameController>> getCheckedControllers = () =>
            {
                List<GameController> result = new List<GameController>();
                foreach (var item in launcherPanel.ControllerSelectPanel.Items)
                    if (item.Checked)
                        result.Add(controllerDict[item]);
                return result;
            };

            Func<GameImageCollection, Controls.SlideshowPanelImageCollection> createSlideshowImages = imgs =>
            {
                var result = new Controls.SlideshowPanelImageCollection();
                foreach (var item in imgs)
                    result.Add(item.Image);
                return result;
            };

            Func<GameImageCollection, Controls.SlideshowPanelImageCollection> createSlideshowImages_x2 = imgs =>
            {
                var result = new Controls.SlideshowPanelImageCollection();
                foreach (var item in imgs)
                    result.Add(item.Image);
                foreach (var item in imgs)
                    result.Add(item.Image);
                return result;
            };

            Func<Controls.SlideshowPanelImageCollection> createSlideshowImages_noimage = () =>
            {
                var result = new Controls.SlideshowPanelImageCollection();
                
                result.Add(noImage);
                result.Add(noImage);

                return result;
            };
            
            Action viewDetails = () =>
            {
                // currentGameについて表示
                Controls.TitleBar tb = launcherPanel.TitleBar;
                tb.RefreshFields(currentGame.DisplayNumber.FullNumber, currentGame.Title);
                Controls.DescriptionPanel dp = launcherPanel.DescriptionPanel;
                dp.Message = currentGame.Description;
                Controls.SlideshowPanel sp = launcherPanel.SlideshowPanel;
                sp.Stop();
                sp.ClearContext();
                if (currentGame.Images.Count >= 2)
                    sp.SetImages(createSlideshowImages(currentGame.Images));
                else if (currentGame.Images.Count == 1)
                {
                    sp.SetImages(createSlideshowImages_x2(currentGame.Images));
                }
                else
                    sp.SetImages(createSlideshowImages_noimage());
                sp.Start();
            };

            Action refreshGameList = () =>
            {
                Controls.GameBannerListView listview = launcherPanel.GameBannerListView;
                
                listview.SuspendRefreshItem();
                listview.Items.Clear();
                var chkdGenres = getCheckedGenres();
                var chkdControllers = getCheckedControllers();
                foreach (Game g in games)
                {
                    // g = GameFilesから読み込んだうちの１つ
                    bool flag1 = false;
                    bool flag2 = false;

                    foreach (GameGenre gg in chkdGenres)
                        // 現在のチェック済みジャンル一覧の中に、含まれるものがあれば表示フラグをオン
                        if (g.Genres.ContainsGuid(gg))
                        {
                            flag1 = true;
                            break;
                        }

                    foreach (GameController gc in chkdControllers)
                        // 現在のチェック済みコントローラ一覧の中に、含まれるものがあれば表示フラグをオン
                        if (g.Controllers.ContainsGuid(gc))
                        {
                            flag2 = true;
                            break;
                        }

                    if (flag1 && flag2)
                    {
                        Controls.GameBanner b = new Controls.GameBanner();
                        b.Text = g.Title;
                        b.Click += (sender, e) => { currentGame = g; viewDetails(); };
                        
                        if (g.Banner.UseBanner)
                            b.BackgroundImage = g.Banner.BannerImage;

                        listview.Items.Add(b);
                    }
                }
                
                listview.ResumeRefreshItem();
            };

            launcherPanel.GenreSelectPanel.ChildPanelClosed += (sender, e) =>
            {
                refreshGameList();
                launcherPanel.SetFocus(launcherPanel.GenreSelectButton);
            };

            launcherPanel.ControllerSelectPanel.ChildPanelClosed += (sender, e) =>
            {
                refreshGameList();

            };

            launcherPanel.HelpButton.Click += (sender, e) =>
            {
                var panel = launcherPanel.LauncherHelpPanel;
                panel.SetFields("Redefinable GameLauncher", File.ReadAllText(settings.DescriptionFile));
                panel.ChildPanelShow();
            };

            launcherPanel.OperationButton.Click += (sender, e) =>
            {
                var panel = launcherPanel.LauncherHelpPanel;
                panel.SetFields(currentGame.Title, currentGame.Description + "\n\n\n" + currentGame.OperationDescription);
                panel.ChildPanelShow();
            };

            launcherPanel.MovieButton.Click += (sender, e) =>
            {
                var panel = launcherPanel.LauncherHelpPanel;
                panel.SetFields(currentGame.Title, "本作品には動画が関連付けられていません。");
                panel.ChildPanelShow();
            };

            launcherPanel.PlayButton.Click += (sender, e) =>
            {
                string path = currentGame.GetGameExeFullPath();
                if (!File.Exists(path))
                {
                    var panel = launcherPanel.LauncherHelpPanel;
                    panel.SetFields(currentGame.Title, "登録エラーを検出しました。\n本体のファイルのパスが無効です。\n\n" + path);
                    panel.ChildPanelShow();
                    return;
                }

                InfoForm iform = new InfoForm();
                iform.SetField(currentGame.DisplayNumber.Precode + currentGame.DisplayNumber.MainNumber.ToString("D3"));
                iform.Show();

                form.WindowState = FormWindowState.Minimized;
                launcherPanel.Visible = false;
                
                ProcessStartInfo psi = new ProcessStartInfo(path, currentGame.ExecInfo.Arguments);
                if (currentGame.ExecInfo.AutoCurrentDirectory)
                    psi.WorkingDirectory = Path.GetDirectoryName(path);
                Process p = Process.Start(psi);

                while (!p.HasExited)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1);
                }

                iform.Close();
                iform.Dispose();

                launcherPanel.Visible = true;
                form.WindowState = FormWindowState.Normal;
            };


            refreshGameList();
            viewDetails();
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

            // テーマのロード
            DebugConsole.Push("COR", "テーマ情報を読み込みます。");
            LauncherTheme theme = _getLauncherTheme(settings);

            // メインウィンドウの表示
            DebugConsole.Push("COR", "メインウィンドウを初期化します。");
            MainForm mainForm = new MainForm();
            _formInitialize(mainForm, games, settings);

            mainForm.LauncherTheme = theme;
            mainForm.Show();
            while (mainForm.Created)
            {
                //mainForm.GetLauncherPanel().Focus();
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }

            mainForm.Dispose();
            GC.Collect();
        }
    }
}
