using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redefinable.Applications.Launcher.Core;

using This = Redefinable.Applications.Launcher.InfoEditor.Core.ConfigHandler;


namespace Redefinable.Applications.Launcher.InfoEditor.Core
{
    /// <summary>
    /// ランチャーのConfigへのアクセスを提供します。
    /// </summary>
    public static class ConfigHandler
    {
        // 非公開静的フィールド
        private static LauncherSettings launcherSettings;


        // 公開静的フィールド・プロパティ

        /// <summary>
        /// ランチャーのConfigの内容を取得します。
        /// </summary>
        public static LauncherSettings Settings
        {
            get { return This.launcherSettings; }
        }


        // 静的コンストラクタ

        /// <summary>
        /// ConfigHandlerクラスを初期化します。
        /// </summary>
        static ConfigHandler()
        {
            This.Load();
        }


        // 非公開静的メソッド
        

        // 公開静的メソッド

        /// <summary>
        /// 現在のConfigHandlerの各種値を保存します。
        /// </summary>
        public static void Save()
        {
            This.launcherSettings.Save();
        }

        /// <summary>
        /// 現在の設定をConfigHandlerへ読み込みます。
        /// </summary>
        public static void Load()
        {
            This.launcherSettings = LauncherSettings.Load();
        }
    }
}
