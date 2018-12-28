using System;
using System.IO;

namespace TBSGame
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "spage\\";
            string temp = Path.GetTempPath() + "spage\\";

            if (!Directory.Exists(appdata))
            {
                Directory.CreateDirectory(appdata);
                File.Copy("default.ini", appdata + "config.ini");
            }

            if (!Directory.Exists(appdata + "saves\\"))
                Directory.CreateDirectory(appdata + "saves\\");

            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            Settings settings = new Settings(appdata + "config.ini");
            settings.AppData = appdata;
            settings.Temp = temp;

            Error.Initalize(settings.LogFile);
            Resources.Load(settings.Language);

            using (var game = new Spage(settings))
                game.Run();
        }
    }
#endif
}
