using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TBSGame
{
    public class Settings
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();

        public Settings(string file)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(':');
                    data.Add(split[0].Trim(), split[1].Trim());
                }
            }
        }

        public string Language => this["lang"];
        public int ResolutionWidth => int.Parse(this["resolution"].Split('x')[0].Trim());
        public int ResolutionHeight => int.Parse(this["resolution"].Split('x')[1].Trim());
        public string LogFile => this["logfile"];
        public string CommonApp { get; set; }
        public string App { get; set; }

        public string this[string key]
        {
            get
            {
                if (data.ContainsKey(key))
                    return data[key];
                else
                {
                    Error.Log("Nastavení nebylo nalezeno, soubor s nastavením byl pravděpodobně poškozen.");
                    return "";
                }
            }
        }
    }
}
