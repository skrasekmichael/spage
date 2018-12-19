using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TBSGame
{
    public static class Error
    {
        private static string path, last_log;
        private static TimeSpan last_log_time = TimeSpan.Zero, time;

        public static void Initalize(string logfile)
        {
            if (!File.Exists(logfile))
                using (StreamWriter stream = File.CreateText(logfile)) stream.Close();

            path = logfile;
        }

        private static void write(string log)
        {
            using (StreamWriter stream = File.AppendText(path))
                stream.WriteLine($"Error [{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}]: {log}");
        }

        public static void Log(string log)
        {
            TimeSpan duration = Error.time - Error.last_log_time;
            if (duration.TotalSeconds >= 1 || last_log != log)
            {
                Error.last_log_time = Error.time;
                last_log = log;
                Console.WriteLine($"Error: {log}");
                write(log);
            }
        }

        public static void Update(GameTime time)
        {
            Error.time = time.TotalGameTime;
        }
    }
}
