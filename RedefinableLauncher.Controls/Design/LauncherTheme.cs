using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;


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

        private string _MagicCode
        {
            get { return "Redefinable GameLauncher Theme File :: "; }
        }
        

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



        // コンストラクタ

        /// <summary>
        /// デバッグ用のサンプルテーマとしてLauncherThemeクラスのインスタンスを初期化します。
        /// </summary>
        public LauncherTheme()
        {
            this.info = new LauncherThemeInfo("Sample Theme for DEBUG", null);
            this.panelTheme = LauncherPanelTheme.GetSampleTheme();
            this.buttonTheme = LauncherButtonTheme.GetSampleTheme();
        }


        // 非公開メソッド
        
        private void _saveTo(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
        }

        private void _loadFrom()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LauncherThemeInfo
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
        public void _saveTo(Stream stream)
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
        public void _loadFrom(Stream stream)
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
    }
}
