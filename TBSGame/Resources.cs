﻿using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TBSGame
{
	public static class Resources
    {
        private static readonly Dictionary<string, string> data = new Dictionary<string, string>();
        private static readonly List<string> missing = new List<string>();

        public static void Load(string text_path)
        {
            Resources.data.Clear();
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
                    data = string.Join(":", split.ToList().GetRange(1, split.Length - 1).ToArray()).Trim() + "\n";
                }
                else
                    data += line + "\n";
            }

            if (key != null && data != null)
                Resources.data.Add(key, data);
        }

        public static List<string> GetMissing() => missing;

        public static string GetString(string key, string[] args = null)
        {
            key = key.ToLower();
            if (data.ContainsKey(key))
            {
                string s = data[key].Trim();
                if (args != null)
                    for (int i = 0; i < args.Length; i++) s = s.Replace("{" + i + "}", args[i]);
                return s;
            }
            else
            {
                Error.Log($"Key [{key}] was not found in dictionary. ");
                if (!missing.Contains(key))
                    missing.Add(key);
                return key;
            }
        }
    }
}
