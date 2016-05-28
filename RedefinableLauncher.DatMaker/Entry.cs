using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Redefinable;
using Redefinable.Applications.Launcher.Core;
using Redefinable.Applications.Launcher.Informations;
using Redefinable.IniHandler;

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
            GameControllerCollection fullControllers = new GameControllerCollection();
            fullControllers.AddFromDirectory(ls.ControllersFilesDirectory);
            
            Console.WriteLine("Program is loading game files...");
            GameFilesDirectory gfd = new GameFilesDirectory(ls.GameFilesDirectory, fullGenres, fullControllers);

            string infodirName = "launcher";

            string settingsIni = "settings.ini";
            string operationTxt = "operation.txt";
            string descriptionTxt = "description.txt";
            string bannerPng = "banner.png";

            string screenshots = "screenshots";


            Console.WriteLine("You can make dat file on next directories.");
            GameDirectoryCollection regDirs = new GameDirectoryCollection();
            foreach (GameDirectory dir in gfd.Directories)
            {
                if (dir.CheckInitialized())
                    continue;

                if (Directory.Exists(dir.Path + "\\" + infodirName) && File.Exists(dir.Path + "\\" + infodirName + "\\" + settingsIni))
                {
                    Console.WriteLine("+ " + dir.DirectoryName);
                    regDirs.Add(dir);
                }
            }


            Console.WriteLine("Please Y key to continue registration.");
            int count = 0;
            keyinput:
            if (count >= 3)
            {
                Console.WriteLine("");
                Console.WriteLine("Exit.");
                Environment.Exit(0);
            }
            var keyinfo = Console.ReadKey();
            if (keyinfo.Key != ConsoleKey.Y)
            {
                count++;
                goto keyinput;
            }
            Console.WriteLine();


            foreach (GameDirectory dir in regDirs)
            {
                Console.WriteLine("*" + dir.DirectoryName);
                IniFile ini = new IniFile(dir.Path + "\\" + infodirName + "\\" + settingsIni);

                string title = ini.Sections["Exe"].Values["title"].Value;
                string filename = ini.Sections["Exe"].Values["filename"].Value;
                string argument = ini.Sections["Exe"].Values["argument"].Value;
                if (argument == "none") argument = "";

                Console.WriteLine("Title = " + title);
            }
        }
    }
}
