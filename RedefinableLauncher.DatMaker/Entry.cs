using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Applications.Launcher.Core;
using Redefinable.Applications.Launcher.Informations;
using Redefinable.IniHandler;
using Redefinable.IO;

using AU = Redefinable.AssemblyInformationUtility.EntryAssemblyInformation;


namespace Redefinable.Applications.Launcher.DatMaker
{
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Entry point

            Console.WriteLine("Redefinable GameLauncher -- DatFileMaker");
            Console.WriteLine("Version: 1.0.0.0");
            Console.WriteLine();

            Console.WriteLine("Program is loading settings...");
            LauncherSettings ls = LauncherSettings.Load(AU.Dir + "\\config.ini");
            Console.WriteLine("GameFiles = " + ls.GameFilesDirectory);
            Console.WriteLine();

            Console.WriteLine("Program is loading genre and controller templates...");
            GameGenreCollection fullGenres = new GameGenreCollection();
            fullGenres.AddFromDirectory(ls.GenreFilesDirectory);
            Console.WriteLine("Genres;");
            foreach (var item in fullGenres)
            {
                Console.WriteLine("* {0} {1}", item.GenreGuid, item.Name);
            }
            Console.WriteLine();

            GameControllerCollection fullControllers = new GameControllerCollection();
            fullControllers.AddFromDirectory(ls.ControllersFilesDirectory);
            Console.WriteLine("Controllers;");
            foreach (var item in fullControllers)
            {
                Console.WriteLine("* {0} {1}", item.ControllerGuid, item.Name);
            }
            Console.WriteLine();
            
            Console.WriteLine("Program is loading game files...");
            GameFilesDirectory gfd = new GameFilesDirectory(ls.GameFilesDirectory, fullGenres, fullControllers);

            string infodirName = "launcher";

            string settingsIni = "settings.ini";
            string operationTxt = "opration.txt";
            string descriptionTxt = "description.txt";
            string bannerPng = "banner.png";

            string screenshots = "screenshots";


            Console.WriteLine("You can make dat file on next directories;");
            GameDirectoryCollection regDirs = new GameDirectoryCollection();
            foreach (GameDirectory dir in gfd.Directories)
            {
                //if (dir.CheckInitialized())
                //    continue;

                string infodir = dir.Path + "\\" + infodirName;
                string infoini = dir.Path + "\\" + infodirName + "\\" + settingsIni;
                //Console.WriteLine(infoini);
                if (Directory.Exists(infodir) && File.Exists(infoini))
                {
                    Console.WriteLine("+ " + dir.DirectoryName);
                    regDirs.Add(dir);
                }
                else if (!File.Exists(dir.GameInformationFilePath))
                {
                    Console.WriteLine("- " + dir.DirectoryName + " (not information)");
                }
                else
                {
                    Console.WriteLine("- " + dir.DirectoryName + " (registered)");
                }
            }
            
            if (regDirs.Count == 0)
            {
                Console.WriteLine("All directory are registered.");
                Console.Write("Please press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }


            Console.WriteLine("Please press Y key to continue registration.");
            int kcount = 0;
            while (true)
            {
                ConsoleKeyInfo kinfo = Console.ReadKey();
                if (kinfo.Key == ConsoleKey.Y)
                    break;
                kcount += 1;

                if (kcount >= 3)
                {
                    Console.WriteLine("Cancelled.");
                    Environment.Exit(0);
                }
            }

            Console.WriteLine();

            foreach (GameDirectory dir in regDirs)
            {
                Console.WriteLine("* " + dir.DirectoryName);
                IniFile ini = new IniFile(dir.Path + "\\" + infodirName + "\\" + settingsIni);

                string title = ini.Sections["Exe"].Values["title"].Value;
                int num = 0;
                string description = File.ReadAllText(dir.Path + "\\" + infodirName + "\\" + descriptionTxt, Encoding.GetEncoding("shift_jis"));
                string operation = File.ReadAllText(dir.Path + "\\" + infodirName + "\\" + operationTxt, Encoding.GetEncoding("shift_jis"));
                string filename = IOUtility.GetFullPath(dir.Path, ini.Sections["Exe"].Values["filename"].Value);
                string banner = IOUtility.GetFullPath(dir.Path + "\\" + infodirName, bannerPng);
                if (!File.Exists(banner)) banner = null;
                string argument = ini.Sections["Exe"].Values["argument"].Value;
                if (argument == "none") argument = "";
                string scdir = dir.Path + "\\" + infodirName + "\\" + screenshots;

                GameGenreCollection targetGenres = new GameGenreCollection();
                if (Boolean.Parse(ini.Sections["Genre"].Values["shooting2d"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{0BB90942-D04A-4ACD-B4A8-CCF21A42C640}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["shooting3d"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{9EF887A7-8F70-4B84-9AE1-746A8FDFC1C2}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["action2d"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{35F9421E-60E1-43B9-ADD0-73EC866F5FA7}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["action3d"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{45F9421E-30A3-53B9-B6D0-73EC866F5CB3}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["adventure"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{E6CF9704-B12C-4099-A201-25BE981E9EED}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["guessing"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{A6D532AB-F293-41BF-9001-E6FCBD605666}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["novel"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{CBDBBA70-936A-4D26-BCF6-97DF78E5A541}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["puzzle"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{A9562BB8-1F84-49C0-9441-B4A4646693D0}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["simulation"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{F0B230E3-DBFD-4527-9537-BB19327A0BF7}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["training_and_develop"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{083630F5-3A36-4291-87CB-14A0019DCC95}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["music"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{2955F9A2-6F1F-47EF-88B8-1B96C736AC33}")));
                if (Boolean.Parse(ini.Sections["Genre"].Values["quiz"].Value))
                    targetGenres.Add(fullGenres.GetGenre(Guid.Parse("{5DC7932F-2AD3-4159-9D0B-8E43E74F72F9}")));

                GameControllerCollection targetController = new GameControllerCollection();
                if (Boolean.Parse(ini.Sections["Controller"].Values["keyboard"].Value))
                    targetController.Add(fullControllers.GetController(Guid.Parse("AA0DDEB2-66D2-4198-B9F1-3B43B84FC86C")));
                if (Boolean.Parse(ini.Sections["Controller"].Values["gamepad"].Value))
                    targetController.Add(fullControllers.GetController(Guid.Parse("A555A683-B336-4691-BB23-6B939E26F461")));
                if (Boolean.Parse(ini.Sections["Controller"].Values["mouse"].Value))
                    targetController.Add(fullControllers.GetController(Guid.Parse("1D076FE3-ED05-4BF8-9EAD-F48D05835DA8")));
                if (Boolean.Parse(ini.Sections["Controller"].Values["original"].Value))
                    targetController.Add(fullControllers.GetController(Guid.Parse("9E94B087-188A-49C2-88A5-713575B3DC07")));
                

                Console.WriteLine("Title        = " + title);
                
                Console.Write    ("Number       = ");
                while (!Int32.TryParse(Console.ReadLine(), out num))
                {
                    Console.WriteLine("作品番号は数値で入力してください");
                    Console.Write("> ");
                }

                Console.WriteLine("Description  = " + description.Split('\n').Length + " lines.");
                Console.WriteLine("Operation    = " + operation.Split('\n').Length + " lines.");
                
                Console.Write    ("FileName     = " + filename);
                if (File.Exists(filename))
                    Console.WriteLine(" (Exist)");
                else
                    Console.WriteLine(" (Not Exist)");

                Console.WriteLine("Argument     = " + argument);

                Console.WriteLine("Banner       = " + (banner != null).ToString());
                Console.Write    ("Genres       = ");
                foreach (var item in targetGenres)
                {
                    Console.Write(item.Name + ", ");
                }
                Console.WriteLine();
                Console.Write    ("Controlloers = ");
                foreach (var item in targetController)
                {
                    Console.Write(item.Name + ", ");
                }
                Console.WriteLine();
                
                Console.Write    ("ScreenShots  = ");
                string[] files = Directory.GetFiles(scdir);
                GameImageCollection images = new GameImageCollection();
                foreach (var f in files)
                {
                    if (Path.GetExtension(f).ToLower() != ".png")
                        continue;
                    
                    GameImage image = new GameImage(f, Image.FromFile(f));
                    images.Add(image);
                    
                    Console.Write(Path.GetFileName(f) + ", ");
                }
                Console.WriteLine();

                Image img;
                if (banner == null)
                {
                    // 自動生成
                    img = new Bitmap(220, 50);
                }
                else
                {
                    img = Image.FromFile(banner);
                }

                Game game = new Game(
                    title,
                    description,
                    operation,
                    new Team(),
                    new GameServerConnectInfo("", "", ""),
                    images,
                    new ExecInfo(IOUtility.GetRelativePath(dir.Path, filename), argument, true),
                    new DisplayNumber("P-", num, ""),
                    targetGenres.GetGuids(),
                    fullGenres,
                    targetController.GetGuids(),
                    fullControllers,
                    new Banner(banner != null, img));
                game.Save(dir.GameInformationFilePath);
            }

            Console.WriteLine("Done.");
            Console.Write("Please press any key to exit.");
            Console.ReadKey();
        }
    }
}
