﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls;

namespace Redefinable.Applications.Launcher.Forms
{
    public class MainForm : Form
    {
        // 非公開フィールド


        // 非公開フィールド :: コントロール
        private LauncherPanel launcherPanel;


        // 公開フィールド

        /// <summary>
        /// 新しいMainFormクラスのインスタンスを初期化します。
        /// </summary>
        public MainForm()
        {
            // データフィールドの初期化


            // コントロールの初期化 :: フォーム
            this.Text = RedefinableUtility.SoftwareTitle;
            this.ClientSize = new Size(1080, 810);
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // コントロールの初期化 :: launcherPanel;
            this.launcherPanel = new LauncherPanel();
            this.Controls.Add(launcherPanel);
            
            // イベントの追加
            this.launcherPanel.ScaleChanged += (sender, e) => { this.ClientSize = this.launcherPanel.Size; };
        }
    }
}