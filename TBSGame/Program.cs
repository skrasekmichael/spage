using MapDriver;
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
            string app = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\spage";
            string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\spage";

            if (!Directory.Exists(app))
            {
                Directory.CreateDirectory(app);
                File.Copy("default.ini", app + "\\config.ini");
            }

            if (!Directory.Exists(app + "\\saves"))
                Directory.CreateDirectory(app + "\\saves");

            if (!Directory.Exists(common))
                Directory.CreateDirectory(common);

            if (!Directory.Exists(common + "\\scenario"))
                Directory.CreateDirectory(common + "\\scenario");

            Settings settings = new Settings($"{app}\\config.ini");
            settings.App = app;
            settings.CommonApp = common;
            Error.Initalize(settings.LogFile);
            Resources.Load(settings.Language);

            using (var game = new Game1(settings))
                game.Run();
        }
    }
#endif
}
