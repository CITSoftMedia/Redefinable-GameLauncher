using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.IO;


namespace Redefinable.Applications.Launcher.Controls.Design
{
    /// <summary>
    /// LauncherButtonのテーマを格納します。
    /// </summary>
    public class LauncherButtonTheme
    {
        // 非公開フィールド
        private Image leftDecoration;
        private Image centerDecoration;
        private Image rightDecoration;

        private int recommendedHeight;
        private int leftPaddingSize;
        private int rightPaddingSize;


        // 公開フィールド

        /// <summary>
        /// ボタンの左端側の装飾のイメージを取得します。
        /// </summary>
        public Image LeftDecoration
        {
            get { return this.leftDecoration; }
        }

        /// <summary>
        /// ボタンの中央部の装飾のイメージを取得します。
        /// </summary>
        public Image CenterDecoration
        {
            get { return this.centerDecoration; }
        }

        /// <summary>
        /// ボタンの右端側の装飾のイメージを取得します。
        /// </summary>
        public Image RightDecoration
        {
            get { return this.rightDecoration; }
        }
        
        /// <summary>
        /// このテーマが推奨するボタンの高さを取得します。
        /// </summary>
        public int RecommendedHeight
        {
            get { return this.recommendedHeight; }
        }

        /// <summary>
        /// 左端の内部余白サイズを取得します。
        /// </summary>
        public int LeftPaddingSize
        {
            get { return this.leftPaddingSize; }
        }

        /// <summary>
        /// 右端の内部余白サイズを取得します。
        /// </summary>
        public int RightPaddingSize
        {
            get { return this.rightPaddingSize; }
        }


        // コンストラクタ

        


        // 公開メソッド

        /// <summary>
        /// イメージのインスタンスを更新します。nullを指定した項目は更新されません。すでにこのテーマが使用中の場合は、このボタンテーマを利用しているコントロールで再描画を実施しなくてはなりません。
        /// </summary>
        /// <param name="left">左端イメージ</param>
        /// <param name="center">中央イメージ</param>
        /// <param name="right">右端イメージ</param>
        public void SetImage(Image left, Image center, Image right)
        {
            if (left != null) this.leftDecoration = left;
            if (center != null) this.centerDecoration = center;
            if (right != null) this.rightDecoration = right;
        }
        
        /// <summary>
        /// このボタンテーマの各種サイズなどの値を更新します。
        /// </summary>
        /// <param name="height"></param>
        /// <param name="leftPadding"></param>
        /// <param name="rightPadding"></param>
        public void SetSizes(int height, int leftPadding, int rightPadding)
        {

        }
    }
}
