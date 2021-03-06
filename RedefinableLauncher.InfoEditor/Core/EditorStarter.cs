﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable.Applications.Launcher.Informations;

using Redefinable.Applications.Launcher.InfoEditor.Forms;


namespace Redefinable.Applications.Launcher.InfoEditor.Core
{
    public static class EditorStarter
    {
        // 非公開静的メソッド

        private static bool _isAlreadyRun(string filePath)
        {
            return _isAlreadyRun(filePath, -1);
        }

        /// <summary>
        /// 指定された実行ファイルのプロセスがすでに存在するか調べます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static bool _isAlreadyRun(string filePath, int pid)
        {
            // 実行中のすべてのプロセスを取得
            Process[] prcs = Process.GetProcesses();

            // 自分のファイル名とプロセスIDを取得
            string path = filePath;
            //int pid = Process.GetCurrentProcess().Id;

            foreach (Process ps in prcs)
            {
                string psPath = null;
                try
                {
                    // プロセスのファイル名を取得
                    psPath = ps.MainModule.FileName;
                }
                catch (Win32Exception ex)
                {
                    // 取得失敗
                    // → スルー
                    psPath = null;
                }

                if (psPath == path && ps.Id != pid)
                    // パスが一致 かつ PIDが現在のプロセスと異なる
                    // → すでに同じ実行ファイルのプロセスが存在している
                    return true;
            }

            // 同じ実行ファイルのプロセスを検出できなかった
            return false;
        }

        // 公開静的メソッド

        /// <summary>
        /// 指定したパスの実行ファイルが現在実行中かどうかを調べます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckRun(string path)
        {
            // ローカルホスト上のすべてのプロセスを取得
            Process[] psList = Process.GetProcesses();
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            int mode;
            while ((mode = SelectModeForm.ShowSelecter()) != 0)
            {
                switch (mode)
                {
                    case 1:
                        ExecuteGameRegister();
                        break;
                    case 2:
                        MessageBox.Show("コントローラ、ジャンルエディタは未実装");
                        break;
                    case 3:
                        MessageBox.Show("コンフィグエディタは未実装");
                        break;
                    default:
                        MessageBox.Show("あああ");
                        Environment.Exit(-1);
                        break;
                }
            }
        }

        public static void ExecuteGameRegister()
        {
            // ジャンル情報・コントローラ情報の取得
            GameGenreCollection genreFullInformations = new GameGenreCollection();
            genreFullInformations.AddFromDirectory(ConfigHandler.Settings.GenreFilesDirectory);
            GameControllerCollection controllerFullInformations = new GameControllerCollection();
            controllerFullInformations.AddFromDirectory(ConfigHandler.Settings.ControllersFilesDirectory);

            while (true)
            {
                // 対象を選択させる
                var res = GameListForm.ShowSelecter(genreFullInformations, controllerFullInformations);

                // もし意図的に閉じた場合は、戻る
                if (res.Close)
                    return;

                // 編集させる
                //MessageBox.Show("res={Close: " + res.Close + ", Remove: " + res.Remove + ", Directory: " + res.Directory?.DirectoryName + "}");
                if (res.Remove)
                {
                    // ここで情報を削除して終了
                    File.Delete(res.Directory.GameInformationFilePath);
                }
                else
                {
                    //MessageBox.Show(res.Directory.DirectoryName + "を編集");
                    EditForm.ShowEditor(res.Directory, genreFullInformations, controllerFullInformations);
                }
            }
        }
    }
}
