using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;
using Redefinable.Collections;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

namespace Redefinable.Applications.Launcher.Controls
{
    public class ChildSelectPanel : ChildPanel
    {
        // 非公開フィールド
        private ChildSelectPanelItemCollection items;


        // 
    }

    public class ChildSelectPanelItemCollection : NativeEventDefinedList<ChildSelectPanelItem>
    {
        // 実装なし
    }

    public class ChildSelectPanelItem
    {
        // 非公開メンバ
        private string text;
        private bool _checked;


        // 公開メンバ

        public string Text
        {
            get { return this._getText(); }
            set { this._setText(value); }
        }

        public bool Checked
        {
            get { return this._getChecked(); }
            set { this._setChecked(value); }
        }
        
        
        // 公開イベント

        /// <summary>
        /// Textの値が変化した際に発生します。
        /// </summary>
        public event EventHandler TextChanged;
        
        /// <summary>
        /// Checkedの値が変化した際に発生します。
        /// </summary>
        public event EventHandler CheckStateChanged;

        
        // コンストラクタ

        /// <summary>
        /// 使用できません
        /// </summary>
        private ChildSelectPanelItem()
        {
            // 実装なし
            this.TextChanged = (sender, e) => { };
            this.CheckStateChanged = (sender, e) => { };
        }

        /// <summary>
        /// 新しいChildSelectPanelItemクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_checked"></param>
        public ChildSelectPanelItem(string text, bool _checked)
            : this()
        {
            this.text = text;
            this._checked = _checked;
        }

        /// <summary>
        /// 新しいChildSelectPanelItemクラスのインスタンスをCheckedがfalseの状態で初期化します。
        /// </summary>
        /// <param name="text"></param>
        public ChildSelectPanelItem(string text)
            : this(text, false)
        {
            // 実装なし
        }


        // 非公開メソッド

        private string _getText()
        {
            return this.text;
        }

        private void _setText(string value)
        {
            this.text = value;
            this.TextChanged(this, EventArgs.Empty);
        }

        private bool _getChecked()
        {
            return this._checked;
        }

        private void _setChecked(bool value)
        {
            this._checked = value;
            this.CheckStateChanged(this, EventArgs.Empty);
        }
    }
}
