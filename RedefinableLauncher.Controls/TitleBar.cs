using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;

namespace Redefinable.Applications.Launcher.Controls
{
    public class TitleBar : VariableScaleableControl
    {
        // 非公開フィールド
        private string displayNumberField;
        private string titleField;
        private TitleBarTheme currentTheme;
        

        // 非公開静的フィールド
        private static int defaultWidth = 780; // 固定幅
        private static int defaultHeight = 60; // 固定高さ

        
        // 公開フィールド

        /// <summary>
        /// DisplayNumberの表示テキストを取得します。更新と新規描画はRefreshFieldsメソッドでおこなってください。
        /// </summary>
        public string DisplayNumberField
        {
            get { return this.displayNumberField; }
        }

        /// <summary>
        /// Titleの表示テキストを取得します。更新と新規描画はRefreshFieldsメソッドでおこなってください。
        /// </summary>
        public string TitleField
        {
            get { return this.titleField; }
        }


        // コンストラクタ

        /// <summary>
        /// 新しいTitleBarクラスのインスタンスを初期化します。このコントロールのサイズは固定されています。
        /// </summary>
        /// <param name="location"></param>
        public TitleBar(Point location)
            : base(location, new Size(defaultWidth, defaultHeight))
        {
            // データフィールドの初期化
            this.displayNumberField = "";
            this.titleField = "";

            // デバッグ
            this.BackColor = Color.Beige;
        }


        // 非公開メソッド

        /// <summary>
        /// 現在のテーマで再描画します。
        /// </summary>
        private void _redraw()
        {

        }


        // 公開メソッド

        public override void RefreshTheme()
        {
            LauncherTheme t = this.GetLauncherTheme();
            if (t != null && t.TitleBarTheme != null)
                this.currentTheme = t.TitleBarTheme;
            base.RefreshTheme();
        }
    }
}
