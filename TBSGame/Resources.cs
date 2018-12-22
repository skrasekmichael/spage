﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TBSGame
{
    public static class Resources
    {
        private static Dictionary<string, string> data = new Dictionary<string, string>();

        public static void Load(string text_path)
        {
            List<string> content = new List<string>();
            using (StreamReader sr = new StreamReader(text_path))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                    content.Add(line.Trim());
            }

            string key = null, data = null;
            foreach (string line in content)
            {
                if (line[0] == '#')
                {
                    if (key != null && data != null)
                        Resources.data.Add(key, data);

                    string[] split = line.Split(':');
                    key = split[0].Trim().Substring(1);
                    data = split[1].Trim() + "\n";
                }
                else
                    data += line + "\n";
            }

            if (key != null && data != null)
                Resources.data.Add(key, data);
        }

        public static string GetString(string key)
        {
            if (data.ContainsKey(key))
                return data[key].Trim();
            else
            {
                Error.Log("klíč k textu nebyl nalezen, soubor s texty je pravděpodobně poškozen.");
                return key;
            }
        }
    }
}