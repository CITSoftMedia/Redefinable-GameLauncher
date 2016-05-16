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

        /// <summary>
        /// 新しいLauncherButtonThemeクラスのインスタンスを初期化します。このコンストラクタの仕様は推奨されません。
        /// </summary>
        public LauncherButtonTheme()
        {
            this.leftDecoration = null;
            this.centerDecoration = null;
            this.rightDecoration = null;

            this.recommendedHeight = 30;

            this.leftPaddingSize = 10;
            this.rightPaddingSize = 10;
        }

        /// <summary>
        /// 新しいLauncherButtonThemeクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="left">左端の装飾イメージ</param>
        /// <param name="center">中央部の装飾イメージ</param>
        /// <param name="right">右端の装飾イメージ</param>
        /// <param name="clone">各インスタンスを複製するかどうか</param>
        public LauncherButtonTheme(Image left, Image center, Image right, bool clone)
        {
            if (clone)
            {
                left = (Image) left.Clone();
                center = (Image) center.Clone();
                right = (Image) right.Clone();
            }

            this.leftDecoration = left;
            this.centerDecoration = center;
            this.rightDecoration = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">左端イメージ</param>
        /// <param name="center">中央イメージ</param>
        /// <param name="right">右端イメージ</param>
        /// <param name="height">このテーマで推奨されるデフォルトのボタンの高さ</param>
        /// <param name="leftPadding">左端部分の幅 (内部余白)</param>
        /// <param name="rightPadding">右端部分の幅 (内部余白)</param>
        public LauncherButtonTheme(Image left, Image center, Image right, int height, int leftPadding, int rightPadding)
            : this(left, center, right, false)
        {
            this.SetSizes(height, leftPadding, rightPadding);
        }
        


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
        /// このボタンテーマの各種サイズなどの値を更新します。すでにこのテーマが使用中の場合は、このボタンテーマを利用しているコントロールで再描画を実施しなくてはなりません。
        /// </summary>
        /// <param name="height">このテーマで推奨されるデフォルトのボタンの高さ</param>
        /// <param name="leftPadding">左端部分の幅 (内部余白)</param>
        /// <param name="rightPadding">右端部分の幅 (内部余白)</param>
        public void SetSizes(int height, int leftPadding, int rightPadding)
        {
            this.recommendedHeight = height;
            this.leftPaddingSize = leftPadding;
            this.rightPaddingSize = rightPadding;
        }


        // 公開静的メソッド

        /// <summary>
        /// デバッグ用のサンプルテーマを取得します。
        /// </summary>
        /// <returns></returns>
        public static LauncherButtonTheme GetSampleTheme()
        {
            Image left, center, right;
            Graphics g;

            left = new Bitmap(10, 10);
            g = Graphics.FromImage(left);
            g.FillRectangle(Brushes.Blue, 0, 0, 10, 10);
            g.Dispose();

            center = new Bitmap(10, 10);
            g = Graphics.FromImage(center);
            g.FillRectangle(Brushes.White, 0, 0, 10, 10);
            g.Dispose();

            right = new Bitmap(10, 10);
            g = Graphics.FromImage(right);
            g.FillRectangle(Brushes.Red, 0, 0, 10, 10);
            g.Dispose();

            return new LauncherButtonTheme(left, center, right, 40, 10, 10);
        }
    }
}
