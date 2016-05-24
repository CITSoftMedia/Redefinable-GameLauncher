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
        

        // 非公開静的フィールド
        private static int defaultHeight = 60;

        
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
        /// 新しいTitleBarクラスのインスタンスを初期化します。このコントロールの高さは固定されています。
        /// </summary>
        /// <param name="location"></param>
        /// <param name="width"></param>
        public TitleBar(Point location, int width)
            : base(location, new Size(width, defaultHeight))
        {
            // データフィールドの初期化
            this.displayNumberField = "";
            this.titleField = "";

            
        }
    }
}
