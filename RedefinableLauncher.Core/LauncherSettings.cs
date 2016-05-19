using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IniHandler;
using Redefinable.Text.Scripting;


namespace Redefinable.Applications.Launcher.Core
{
    public class LauncherSettings
    {
        // 非公開フィールド
        private string windowText;
        private float windowScale;
        
        private string gameFilesDirectory;
        private bool showPathErrorGame;
        private bool warningPathError;
        private bool useAutoInitializer;
        
        private string genreFilesDirectory;
        private string controllersFilesDirectory;

        
        // 公開フィールド

        /// <summary>
        /// ウィンドウのテキストを取得・設定します。
        /// </summary>
        public string WindowText
        {
            get { return this.windowText; }
            set { this.windowText = value; }
        }

        /// <summary>
        /// ウィンドウのスケールを取得・設定します。負の値を設定すると、自動調整になります。
        /// </summary>
        public float WindowScale
        {
            get { return this.windowScale; }
            set { this.windowScale = value; }
        }

        /// <summary>
        /// GameFilesディレクトリのパスを取得・設定します。
        /// </summary>
        public string GameFilesDirectory
        {
            get { return this.gameFilesDirectory; }
            set { this.gameFilesDirectory = value; }
        }
        
        /// <summary>
        /// パスエラーの発生している作品を事前に非表示にするかどうかを示す値を取得・設定します。
        /// </summary>
        public bool ShowPathErrorGame
        {
            get { return this.showPathErrorGame; }
            set { this.showPathErrorGame = value; }
        }

        /// <summary>
        /// パスエラーが発生している作品の一覧をロードに警告するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool WarningPathError
        {
            get { return this.warningPathError; }
            set { this.warningPathError = value; }
        }

        /// <summary>
        /// 有効なディレクトリが０である場合に、全ての無効なディレクトリを初期化します。 (デバッグ用)
        /// </summary>
        public bool UseAutoInitializer
        {
            get { return this.useAutoInitializer; }
            set { this.useAutoInitializer = value; }
        }

        /// <summary>
        /// GenreFilesのディレクトリを取得・設定します。
        /// </summary>
        public string GenreFilesDirectory
        {
            get { return this.genreFilesDirectory; }
            set { this.genreFilesDirectory = value; }
        }

        /// <summary>
        /// ControllerFilesのディレクトリを取得・設定します。
        /// </summary>
        public string ControllersFilesDirectory
        {
            get { return this.controllersFilesDirectory; }
            set { this.controllersFilesDirectory = value; }
        }
        

        // 公開静的フィールド

        /// <summary>
        /// 
        /// </summary>
        public static string LauncherSettingsFilePath
        {
            get { return AssemblyInformationUtility.EntryAssemblyInformation.Dir + "\\config.ini"; }
        }


        // コンストラクタ

        /// <summary>
        /// デフォルトの値で新しいLauncherSettingsクラスのインスタンスを初期化します。
        /// </summary>
        public LauncherSettings()
        {
            this.windowText = "Redefinable GameLauncher";
            this.windowScale = -1.0f;

            this.gameFilesDirectory = "{var: Ini_AssemblyDirectory}\\Game Files";
            
            this.showPathErrorGame = false;
            this.warningPathError = false;
            this.useAutoInitializer = false;

            this.genreFilesDirectory = "{var: Ini_AssemblyDirectory}\\Genre Informations";
            this.controllersFilesDirectory = "{var: Ini_AssemblyDirectory}\\Controllers Informations";
        }


        // 公開メソッド

        /// <summary>
        /// ファイルへ保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            IniFile ini = new IniFile();
            
            ini.Sections.Add(new IniSection("MetaInfo", new IniValue[] {
                new IniValue("Create", DateTime.Now.ToString("r")),
                new IniValue("Guid", Guid.NewGuid().ToString()),
            }, new string[] {
                " ",
                " Redefinable Launcher Settings",
                " ",
            }, 2));

            ini.Sections.Add(new IniSection("Window", new IniValue[] {
                new IniValue("Text", this.windowText),
                new IniValue("Scale", this.windowScale.ToString()),
            }, new string[] {
                " Window Settings"
            }, 1));

            ini.Sections.Add(new IniSection("GameManager", new IniValue[] {
                new IniValue("Directory", this.gameFilesDirectory),
                new IniValue("ShowPathErrorGame", this.showPathErrorGame.ToString()),
                new IniValue("WarningPathError", this.warningPathError.ToString()),
                new IniValue("UseAutoInitializer", this.useAutoInitializer.ToString()),
            }, new string[] {
                " Game Information Settings"
            }, 1));

            ini.Sections.Add(new IniSection("ClassificationInformation", new IniValue[] {
                new IniValue("GenreFiles", this.genreFilesDirectory),
                new IniValue("ControllerFiles", this.controllersFilesDirectory),
            }, new string[] {
                " Genre and Controllers informations"
            }, 1));


            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            ini.Save(fs, new IniFileSaveOption() { PresectionLine = 2 });
            fs.Close();
        }

        /// <summary>
        /// ファイルへ保存します。
        /// </summary>
        public void Save()
        {
            this.Save(LauncherSettingsFilePath);
        }


        // 静的公開メソッド

        /// <summary>
        /// ファイルから設定を読み込みます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static LauncherSettings Load(string path)
        {
            IniFile ini = new IniFile(path);
            LauncherSettings ls = new LauncherSettings();

            try
            {
                IniSection sec = ini.Sections["Window"];
                ls.windowText = sec.Values["Text"].Value;
                ls.windowScale = float.Parse(sec.Values["Scale"].Value);

                sec = ini.Sections["GameManager"];
                ls.gameFilesDirectory = sec.Values["Directory"].Value;
                ls.showPathErrorGame = Boolean.Parse(sec.Values["ShowPathErrorGame"].Value);
                ls.warningPathError = Boolean.Parse(sec.Values["WarningPathError"].Value);
                ls.useAutoInitializer = Boolean.Parse(sec.Values["UseAutoInitializer"].Value);

                sec = ini.Sections["ClassificationInformation"];
                ls.genreFilesDirectory = sec.Values["GenreFiles"].Value;
                ls.controllersFilesDirectory = sec.Values["ControllerFiles"].Value;
            }
            catch (Exception ex)
            {
                throw new IOException("設定ファイルからLauncherSettingsをロードできませんでした。", ex);
            }

            return ls;
        }

        /// <summary>
        /// ファイルから設定を読み込みます。
        /// </summary>
        /// <returns></returns>
        public static LauncherSettings Load()
        {
            return Load(LauncherSettingsFilePath);
        }
    }
}
