using System;

using This = Redefinable.Applications.Launcher.LauncherAssemblyInformation.LauncherAssembly;


namespace Redefinable.Applications.Launcher.LauncherAssemblyInformation
{
    /// <summary>
    /// ランチャー構成アセンブリである現在のアセンブリに関する情報を取得します。
    /// これと同名のクラスが各アセンブリに定義されています。
    /// </summary>
    public static class LauncherAssembly
    {
        // 公開フィールド

        /// <summary>
        /// GUID値を取得します。
        /// </summary>
        public static string Guid
        {
            get { return "{6A4225D8-FA1B-446D-99E9-AC4F02DBBB00}"; }
        }

        
        // 公開メソッド

        /// <summary>
        /// GUID値を取得します。
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return This.Guid;
        }
    }


    public static class VailidDatas
    {
        public static readonly string XASM_0492_001 = "qc43otiyv2n0934tn0v29894r";
        public static readonly string XASM_0492_002 = "34p9g-0rg9er9u05y9buq2v-3";
        public static readonly string XASM_0492_003 = "ertohjm3p9gvf2-22q34r2crt";
        public static readonly string XASM_0492_004 = "t230t9vuweifjwgpov45tyt3f";

        public static readonly string XASM_0492_011 = "werigc45sgicjmpotgv3m5tp9";
        public static readonly string XASM_0492_012 = "ericmw54gmvtyhjytgorf2950";
    }
}