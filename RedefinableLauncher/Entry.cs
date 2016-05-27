using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redefinable.Applications.Launcher
{
    using Core;
    
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Entry point

            #region Redefinable初期化処理
            ApplicationMetaData apMetaData;
#if DEBUG
            // デバッグアセンブリの場合は出力
            apMetaData = new ApplicationMetaData("Redefinable Launcher", "http://www.a32kita.tk/", "%mod_dir%\\%mod_name%_debuglog.txt");
            apMetaData.Save();
#endif
            apMetaData = new ApplicationMetaData();
            DebugConsole.NewLine = "\r\n";
#if DEBUG
            apMetaData.DebugMode = true;
#endif
            RedefinableUtility.Initialize(apMetaData);
            #endregion



            try
            {
                LauncherAssemblyUtility.ConsoleHelper = new DebugConsoleHelper();

                if (!LauncherAssemblyUtility.CheckGuid())
                    throw new Exception("ランチャーを構成するライブラリの一部でGuid値の一致しないコンポーネントが検出されました。");

                LauncherStarter.Start();

                //Console.WriteLine("停止");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RunHandler(ex, " ご不便とご迷惑をお掛けしております。ランチャーアプリケーションの実行中に致命的なエラーが発生したため、ソフトウェアを強制終了致しました。\n コンポーネントやパラメータをお確かめの上、管理者へお問い合わせください。");
            }
        }
    }
}
