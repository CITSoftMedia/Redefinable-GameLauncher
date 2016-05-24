using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;
using Redefinable.StreamArchive;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    /// <summary>
    /// ランチャーパネルのテーマを管理します。
    /// </summary>
    public class LauncherTheme
    {
        // 非公開フィールド
        private LauncherThemeInfo info;
        
        private LauncherPanelTheme panelTheme;
        private LauncherButtonTheme buttonTheme;
        private ScrollBarTheme scrollBarTheme;
        


        // 公開フィールド・プロパティ

        /// <summary>
        /// テーマ情報を格納するインスタンスを取得します。
        /// </summary>
        public LauncherThemeInfo Info
        {
            get { return this.info; }
        }

        /// <summary>
        /// LauncherPanel用のテーマデータのインスタンスを取得します。
        /// </summary>
        public LauncherPanelTheme PanelTheme
        {
            get { return panelTheme; }
        }

        /// <summary>
        /// LauncherButton用のテーマデータのインスタンスを取得します。
        /// </summary>
        public LauncherButtonTheme ButtonTheme
        {
            get { return this.buttonTheme; }
        }

        /// <summary>
        /// LauncherScrollBar用のテーマデータを取得します。
        /// </summary>
        public ScrollBarTheme ScrollBarTheme
        {
            get { return this.scrollBarTheme; }
        }



        // コンストラクタ

        /// <summary>
        /// 空のテーマ情報のインスタンスを初期化します。
        /// </summary>
        private LauncherTheme()
        {
            this.info = null;
            this.panelTheme = null;
            this.buttonTheme = null;
            this.scrollBarTheme = null;
        }


        // 非公開メソッド
        
        private Dictionary<string, string> _getInternalItems()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("info", "__redef_launcher_theme_info.dat");
            dict.Add("panel", "__redef_launcher_theme_panel.dat");
            dict.Add("button", "__redef_launcher_theme_button.dat");
            dict.Add("scrollbar", "__redef_launcher_theme_scrollbar.dat");

            return dict;
        }

        /// <summary>
        /// nullでない項目をアーカイブへ追加します。
        /// </summary>
        /// <param name="maker"></param>
        private void _saveTo(ArchiveMaker maker)
        {
            MemoryStream ms = null;
            StoringStreamItem item = null;
            Dictionary<string, string> nameDict = this._getInternalItems();

            // info
            if (this.info != null)
            {
                ms = new MemoryStream();
                this.info.Save(ms);
                item = new StoringStreamItem(nameDict["info"], ms);
                maker.ItemList.Add(item);
            }

            // panel
            if (this.panelTheme != null)
            {
                ms = new MemoryStream();
                this.panelTheme.Save(ms);
                item = new StoringStreamItem(nameDict["panel"], ms);
                maker.ItemList.Add(item);
            }

            // button
            if (this.buttonTheme != null)
            {
                ms = new MemoryStream();
                this.buttonTheme.Save(ms);
                item = new StoringStreamItem(nameDict["button"], ms);
                maker.ItemList.Add(item);
            }

            // button
            if (this.scrollBarTheme != null)
            {
                ms = new MemoryStream();
                this.scrollBarTheme.Save(ms);
                item = new StoringStreamItem(nameDict["scrollbar"], ms);
                maker.ItemList.Add(item);
            }
        }

        /// <summary>
        /// 指定したアーカイブからthemeを読み込みます。アーカイブ内に存在しない項目はnullが設定されます。
        /// </summary>
        /// <param name="archive"></param>
        private void _loadFrom(ArchiveReader archive)
        {
            // アイテム名一覧
            IDictionary<string, string> nameDict = this._getInternalItems();

            // アイテムのオフセット情報を利用して、直に読み込む
            FileStream stream = new FileStream(archive.ArchivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IArchiveItem item;


            // info
            if (archive.Items.Contains(nameDict["info"]))
            {
                item = archive.Items.GetItem(nameDict["info"]);
                this.info = LauncherThemeInfo.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));
            }
            else
                this.info = null;

            // panel
            if (archive.Items.Contains(nameDict["panel"]))
            {
                item = archive.Items.GetItem(nameDict["panel"]);
                this.panelTheme = LauncherPanelTheme.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));
            }
            else
                this.panelTheme = null;

            // button
            if (archive.Items.Contains(nameDict["button"]))
            {
                item = archive.Items.GetItem(nameDict["button"]);
                this.buttonTheme = LauncherButtonTheme.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));
            }
            else
                this.buttonTheme = null;

            // scrollbar
            if (archive.Items.Contains(nameDict["scrollbar"]))
            {
                item = archive.Items.GetItem(nameDict["scrollbar"]);
                this.scrollBarTheme = ScrollBarTheme.Load(new SubStream(stream, (long)item.StartOffset, (long)item.Length));
            }
            else
                this.scrollBarTheme = null;
        }

        /// <summary>
        /// 指定したthemeのうち、info以外の情報で現在のthemeを上書きします。指定したthemeの中でnullの項目は、上書きされません。
        /// </summary>
        /// <param name="theme"></param>
        private void _overrideFrom(LauncherTheme theme)
        {
            if (theme.panelTheme != null)
                this.panelTheme = theme.panelTheme;
            
        }


        // 公開メソッド

        public void Save(ArchiveMaker archive)
        {
            this._saveTo(archive);
        }

        public void Save(string path)
        {
            ArchiveMaker maker = new ArchiveMaker(path);
            this._saveTo(maker);

            maker.Save();
        }

        
        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のデフォルトテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static LauncherTheme GetDefaultTheme()
        {
            LauncherTheme theme = new LauncherTheme();
            theme.info = new LauncherThemeInfo("Default", null);
            theme.panelTheme = LauncherPanelTheme.GetSampleTheme();
            theme.buttonTheme = LauncherButtonTheme.GetSampleTheme();
            theme.scrollBarTheme = ScrollBarTheme.GetSampleTheme();

            return theme;
        }
        
        public static LauncherTheme Load(ArchiveReader archive)
        {
            LauncherTheme theme = new LauncherTheme();
            theme._loadFrom(archive);
            
            return theme;
        }

        public static LauncherTheme Load(string path)
        {
            ArchiveReader reader = new ArchiveReader(path);
            LauncherTheme theme = LauncherTheme.Load(reader);
            reader.Close();

            return theme;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LauncherThemeInfo : ILauncherThemeElement
    {
        // 非公開フィールド
        private string name;
        private string baseThemeFile;


        // 公開フィールド

        /// <summary>
        /// テーマの名前を取得・設定します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 継承元のテーマファイルを取得・設定します。継承しない場合はnullを表します。
        /// </summary>
        public string BaseThemeFile
        {
            get { return this.baseThemeFile; }
            set { this.baseThemeFile = value; }
        }

        /// <summary>
        /// このテーマは継承元のテーマが有効であるかどうかを示す値を取得します。
        /// </summary>
        public bool IsInherited
        {
            get { return this.baseThemeFile != null; }
        }



        // コンストラクタ

        /// <summary>
        /// 空のインスタンスを初期化します。
        /// </summary>
        private LauncherThemeInfo()
        {
            this.name = null;
            this.baseThemeFile = null;
        }

        /// <summary>
        /// LauncherThemeInfoクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">テーマの名前</param>
        /// <param name="baseThemeFile">継承元のテーマファイル。継承しない場合は、nullを設定します。</param>
        public LauncherThemeInfo(string name, string baseThemeFile)
        {
            this.name = name;
            this.baseThemeFile = baseThemeFile;
        }


        // 非公開メソッド

        /// <summary>
        /// 指定したストリームの現在の位置にこのインスタンスのデータを出力します。
        /// 内部でRedefinable Dictionary Binaryを使用しています。
        /// </summary>
        /// <param name="stream"></param>
        private void _saveTo(Stream stream)
        {
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict["FileId"] = "Launcher Theme Information";
            dict["Name"] = this.name;
            
            if (this.IsInherited)
            {
                dict["Inherited"] = true.ToString();
                dict["BaseThemeFile"] = this.baseThemeFile;
            }
            else
            {
                dict["Inherited"] = false.ToString();
                dict["BaseThemeFile"] = "undefined";
            }

            bc.WriteDictionary(dict, stream);
        }

        /// <summary>
        /// 指定したストリームの現在の位置からこのインスタンスに各種データを読み込みます。
        /// </summary>
        /// <param name="steam"></param>
        private void _loadFrom(Stream stream)
        {
            BinaryConverter bc = new BinaryConverter(BinaryConverterByteOrder.LittleEndian, Encoding.UTF8);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            bc.ReadDictionary(dict, stream);

            if (!dict.ContainsKey("FileId") && dict["FileId"] != "Launcher Theme Information")
                throw new NotSupportedException("Launcher Theme Informationが不正です。");

            this.name = dict["Name"];

            if (Boolean.Parse(dict["Inherited"]))
                this.baseThemeFile = dict["BaseThemeFile"];
            else
                this.baseThemeFile = null;
        }


        // 公開メソッド

        /// <summary>
        /// 現在のインスタンスが保持するデータを指定したストリームへ出力します。
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            this._saveTo(stream);
        }


        // 公開静的メソッド

        /// <summary>
        /// 指定したストリームからLauncherThemeInfoを読み込みます。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static LauncherThemeInfo Load(Stream stream)
        {
            LauncherThemeInfo info = new LauncherThemeInfo();
            info._loadFrom(stream);

            return info;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ILauncherThemeElement
    {
        /// <summary>
        /// 
        /// </summary>
        void Save(Stream stream);
    }
}
