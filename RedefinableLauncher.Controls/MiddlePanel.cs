using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Collections;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;


namespace Redefinable.Applications.Launcher.Controls
{
    public class MiddlePanel : VariableScaleableControl
    {
        // 非公開フィールド
        private MiddlePanelStyle middlePanelStyle;
        private Padding middlePanelPadding;


        // 非公開フィールド :: コントロール
        private SlideshowPanel slideshowPanel;
        

        // 非公開静的フィールド
        private static int defaultWidth = 780;  // 固定幅
        private static int defaultHeight = 400; // 固定高さ

        
        // 公開フィールド・プロパティ
        
        /// <summary>
        /// SlideshowPanelのインスタンスを取得します。
        /// </summary>
        public SlideshowPanel SlideshowPanel
        {
            get { return this.slideshowPanel; }
        }


        // 公開静的フィールド

        public static Size MiddlePanelDefaultSize
        {
            get { return new Size(defaultWidth, defaultHeight); }
        }

        
        // コンストラクタ
        
        /// <summary>
        /// 新しいMiddlePanelクラスのインスタンスを初期化します。コントロールのサイズはMiddlePanelDefaultSizeで固定されています。
        /// </summary>
        /// <param name="point"></param>
        public MiddlePanel(Point point)
            : base(point, MiddlePanelDefaultSize)
        {
            // データフィールドの初期化
            this.middlePanelStyle = MiddlePanelStyle.SlideshowOnly;
            this.middlePanelPadding = new Padding(15);

            // コントロールの初期化
            this._styleInitialize();

            // デバッグ
            this.BackColor = Color.Yellow;
        }


        // 非公開メソッド

        private Padding _getScaledPadding()
        {
            int left = this.middlePanelPadding.Left;
            int top = this.middlePanelPadding.Top;
            int right = this.middlePanelPadding.Right;
            int bottom = this.middlePanelPadding.Bottom;

            left = (int)((float) left * this.currentScale);
            top = (int)((float) top * this.currentScale);
            right = (int)((float) right * this.currentScale);
            bottom = (int)((float) bottom * this.currentScale);

            return new Padding(left, top, right, bottom);
        }

        /// <summary>
        /// スタイル変更後に各コントロールを初期化します。
        /// </summary>
        private void _styleInitialize()
        {
            // 既存のコントロールを削除
            if (this.Controls.Contains(this.slideshowPanel))
                this.Controls.Remove(this.slideshowPanel);

            Padding padding = this._getScaledPadding();

            // 現在のスタイルに合わせて再追加
            if (this.middlePanelStyle == MiddlePanelStyle.SlideshowOnly)
            {
                this.slideshowPanel = new SlideshowPanel(
                    padding.GetClientPoint(new Point(0, 0)),
                    padding.GetClientSize(this.Size));
                this.Controls.Add(this.slideshowPanel);
            }
            else
                throw new NotImplementedException("現在MiddlePanelではMiddlePanelStyle \"" + this.middlePanelStyle + "\" をサポートしていません。");
        }
    }

    public enum MiddlePanelStyle
    {
        SlideshowOnly,

        WithRanking,
    }
}
