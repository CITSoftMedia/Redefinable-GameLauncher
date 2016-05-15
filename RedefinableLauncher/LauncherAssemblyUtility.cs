using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Redefinable;
using Redefinable.IO;

using This = Redefinable.Applications.Launcher.LauncherAssemblyUtility;


namespace Redefinable.Applications.Launcher
{
    /// <summary>
    /// ランチャー構成アセンブリに関する機能を提供します。
    /// </summary>
    public static class LauncherAssemblyUtility
    {
        // 非公開静的フィールド
        private static DebugConsoleHelper consoleHelper;
        private static StreamWriter debugOut;

        // 公開静的フィールド・プロパティ

        /// <summary>
        /// ConsoleHelperを設定・取得します。
        /// </summary>
        public static DebugConsoleHelper ConsoleHelper
        {
            get { return This.consoleHelper; }
            set { This.consoleHelper = value; }
        }


        // コンストラクタ

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static LauncherAssemblyUtility()
        {
            This.consoleHelper = null;
            
            StringEventStream stream = new StringEventStream(Encoding.UTF8);
            stream.Wrote += (sender, e) =>
            {
                if (This.consoleHelper == null)
                    return;

                This.consoleHelper.Out.Write("(LAU)" + e.Text);
            };
            
            This.debugOut = new StreamWriter(stream);
            This.debugOut.AutoFlush = true;
        }

        
        // 非公開メソッド

        /// <summary>
        /// 指定Assemblyから既定のランチャー構成アセンブリのGUIDを取得します。
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        private static string _getLauncherAssemblyGuid(Assembly asm)
        {
            //This.debugOut.WriteLine("_getLauncherAssemblyGuid");

            string asmName = asm.GetName().Name;
            string className = "LauncherAssembly";
            string methodName = "GetGuid";

            //This.debugOut.WriteLine("asm: {0}", asmName);
            //This.debugOut.WriteLine("cls: {0}", className);
            //This.debugOut.WriteLine("mtd: {0}", methodName);
            
            Type[] types = asm.GetTypes();
            foreach (Type type in types)
            {
                if (type.Name == className)
                {
                    This.debugOut.WriteLine("Detected Class: {0}", type.Name);

                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (MethodInfo info in methodInfos)
                    {
                        //This.debugOut.WriteLine("Detected: {0}.{1}", type.Name, info.Name);

                        if (info.Name == methodName)
                            return (string)info.Invoke(null, null);
                    }

                    string errorStr = "クラス '" + asmName + "." + className + "' 内にメソッド '" + methodName + "' が存在しません。";
                    This.debugOut.WriteLine(errorStr);
                    throw new MethodAccessException(errorStr);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void _dummyMethod()
        {
            var obj01 = new Redefinable.Applications.Launcher.Core.CoreTestClass();
            var obj02 = new Redefinable.Applications.Launcher.Informations.Developer();
            var obj03 = new Redefinable.Applications.Launcher.Forms.MainForm();
        }

        
        // 公開静的メソッド

        /// <summary>
        /// 現在のアプリケーションドメインにロードされているランチャー構成アセンブリのGUIDを検査します。
        /// </summary>
        /// <returns></returns>
        public static bool CheckGuid()
        {
            This.debugOut.WriteLine("CheckGuid");
            This._dummyMethod();


            // エントリアセンブリのGuidを取得
            //This.debugOut.WriteLine("エントリアセンブリからGuidを取得します。");
            Assembly entryAsm = Assembly.GetEntryAssembly();
            
            string entryGuid = This._getLauncherAssemblyGuid(entryAsm);
            if (entryGuid == null)
                throw new Exception("エントリアセンブリからLauncherAssemblyGuidを取得できませんでした。");

            // 現在のAppDomainにロードされているすべてのアセンブリを取得
            //This.debugOut.WriteLine("ドメイン内のランチャー構成アセンブリを検査します。");
            Assembly[] domainAsms = AppDomain.CurrentDomain.GetAssemblies();
            string entryAsmName = entryAsm.GetName().Name;
            foreach (Assembly asm in domainAsms)
            {
                // 名前からランチャー用アセンブリ出なかった場合はcontinue
                string targetAsmName = asm.GetName().Name;
                if (targetAsmName.Split('.')[0] != entryAsmName)
                    continue;

                This.debugOut.WriteLine("ランチャー構成アセンブリ: {0}", targetAsmName);
                
                string asmGuid = This._getLauncherAssemblyGuid(asm);
                if (asmGuid != entryGuid)
                {
                    // Guid不一致
                    This.debugOut.WriteLine("false:: {0}", targetAsmName);
                    return false;
                }

                //This.debugOut.WriteLine("true:: {0}", targetAsmName);
            }

            return true;
        }
    }
}
