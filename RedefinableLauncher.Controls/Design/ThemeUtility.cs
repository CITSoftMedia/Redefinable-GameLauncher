using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AU = Redefinable.AssemblyInformationUtility.EntryAssemblyInformation;
using This = Redefinable.Applications.Launcher.Controls.Design.ThemeUtility;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    public static class ThemeUtility
    {
        // 公開フィールド・プロパティ
        
        /// <summary>
        /// Theme Debugディレクトリのフルパスを取得します。
        /// </summary>
        public static string ThemeDebugDir
        {
            get { return This._getThemeDebugDir(); }
        }


        // 非公開メソッド

        private static string _getThemeDebugDir()
        {
            return AU.Dir + "\\Launcher System\\Theme Debug";
        }
    }
}
