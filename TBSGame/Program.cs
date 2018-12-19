using System;

namespace TBSGame
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Settings settings = new Settings("config.ini");
            Error.Initalize(settings.LogFile);
            Resources.Load(settings.Language);
            using (var game = new Game1(settings))
                game.Run();
        }
    }
#endif
}
