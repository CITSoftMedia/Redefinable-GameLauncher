using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Collections;
using Redefinable.IO;
using Redefinable.Applications.Launcher;
using Redefinable.Applications.Launcher.Core;
using Redefinable.Applications.Launcher.Informations;
using Redefinable.Applications.Launcher.LauncherAssemblyInformation;

namespace Redefinable.Applications.Launcher
{
    public static class DebugCode
    {
        /// <summary>
        /// ゲーム情報の入出力試験
        /// </summary>
        public static void Debug01()
        {
            DebugConsole.Push(" ");
            DebugConsole.Push("生成試験");

            // GameImageとGameImageCollectionのテスト
            GameImageCollection images = new GameImageCollection(new GameImage[]
            {
                new GameImage("TestTitle01", System.Drawing.Image.FromFile("testImage.bmp")),
                new GameImage("TestTitle02", System.Drawing.Image.FromFile("testImage.bmp")),
            });

            // Team情報・Developer情報のテスト
            Team team = new Team();
            team.Name = "Explorers of the Binary World";
            team.Description = "ツール・ソフトを中心に開発をおこなっています。";
            team.Developers.Add(new Developer("あおと", "プログラム班", 3, "http://www.a32kita.tk"));
            team.Developers.Add(new Developer("nanase", "外部", 0, "http://www.a32kita.tk"));
            team.Developers.Add(new Developer("ドンモツ", "マルチメディア班", 3, ""));
            team.Developers.Add(new Developer("EyEsya", "マルチメディア班", 3, ""));
            team.Developers.Add(new Developer("二熊", "マルチメディア班", 3, ""));
            team.Developers.Add(new Developer("fblack", "外部", 3, ""));
            team.Developers.Add(new Developer("ヒデタカ", "プログラム班", 3, ""));
            team.Developers.Add(new Developer("新製品", "プログラム班", 3, ""));
            team.LeaderIndex = 0;
            
            // Game基本情報のテスト
            Game game = new Game("ほげほげーむ", "hello, description!!", "ﾊﾞｧﾝｯ", team, new GameServerConnectInfo("aaaaaaaaaaaccccccccccccounttttttttttNaaaaaaaaammmmeeeeeeee", "pppppppaaaaaaaaaaaaaaaaaaaaaaaaaaaasssssssssswwwoooooooooord", "gaaaaaaaameIdeeeeeeeentiiiiifiiieeeeeeerrrrr"), images, new ExecInfo("rewv2w3@f5vuy", "\\/v345v3", false));
            game.Save("GameInfo.dat");


            // 読み込み試験
            DebugConsole.Push(" ");
            DebugConsole.Push("読み込み試験");

            game = null;

            Game game2 = Game.Load("GameInfo.dat");
            DebugConsole.Push("作品名: " + game2.Title);
            DebugConsole.Push("概　要: " + game2.Description);

            
            DebugConsole.Push(" ");
            DebugConsole.Push("開発チーム \"" + game2.DeveloperTeam.Name + "\"");
            foreach (Developer dev in game2.DeveloperTeam.Developers)
            {
                DebugConsole.Push(String.Format("* {0}, {1}", dev.DeveloperGuid, dev.Name));
            }

            DebugConsole.Push(" ");
            DebugConsole.Push("GameImageCollection");
            foreach (GameImage image in game2.Images)
            {
                DebugConsole.Push(String.Format("* {0}: {1}", image.Title, image.ImageGuid));
            }

            DebugConsole.Push(" ");
            DebugConsole.Push("ExecInfo");
            DebugConsole.Push("* RPath  : " + game2.ExecInfo.RelativePath);
            DebugConsole.Push("* Args   : " + game2.ExecInfo.Arguments);


            /*
            game.SaveBasicInfo("gameBasicInfo.txt");

            Game game2 = Game.LoadBasicInfo("gameBasicInfo.txt");
            DebugConsole.Push("game2.Title = " + game2.Title);
            DebugConsole.Push("game2.Descr = " + game2.Description);
            DebugConsole.Push("game2.Opera = " + game2.OperationDescription);
            */
        }

        /// <summary>
        /// 辞書コンバータ
        /// </summary>
        public static void Debug02()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("key01", "value01");
            dict.Add("key02", "ばりゅー２");
            dict.Add("key03", "あぁぅえ03");
            BinaryConverter bc = new BinaryConverter(IO.BinaryConverterByteOrder.LittleEndian, Encoding.GetEncoding(932));
            bc.WriteDictionary(dict, "dictBin.txt");
                
            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            bc.ReadDictionary(dict2, "dictBin.txt");
            foreach (var item in dict2)
                DebugConsole.Push(item.Key + "=" + item.Value);
        }
    }
}
