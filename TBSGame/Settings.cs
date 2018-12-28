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
        private string path;

        public Settings(string path)
        {
            this.path = path;
            using (StreamReader sr = new StreamReader(path))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(':');
                    data.Add(split[0].Trim(), split[1].Trim());
                }
            }
        }

        public string Language { get => this["lang"]; set => this["lang"] = value; }
        public int ResolutionWidth
        {
            get => int.Parse(this["resolution"].Split('x')[0].Trim());
            set => this["resolution"] = value.ToString() + "x" + ResolutionHeight.ToString();
        }
        public int ResolutionHeight
        {
            get => int.Parse(this["resolution"].Split('x')[1].Trim());
            set => this["resolution"] = ResolutionWidth.ToString() + "x" + value.ToString();
        }
        public string LogFile { get => this["logfile"]; set => this["logfile"] = value; }
        public string MissingFile { get => this["missfile"]; set => this["missfile"] = value; }

        //nenačítající se data
        public string AppData { get; set; }
        public string Temp { get; set; }
        public string Scenario => Temp + "scenario\\";
        public string MapSaves => Temp + "tempsaves\\";
        public string GameSaves => AppData + "saves\\";

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, string> kvp in data)
                    sw.WriteLine(kvp.Key + ":" + kvp.Value);
            }
        }

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
            private set
            {
                if (data.ContainsKey(key))
                    data[key] = value;
                else
                {
                    Error.Log("Nastavení nebylo nalezeno, soubor s nastavením byl pravděpodobně poškozen.");
                }
            }
        }
    }
}
