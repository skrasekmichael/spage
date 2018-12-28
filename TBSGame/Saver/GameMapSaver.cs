using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TBSGame.Saver
{
    public class GameMapSaver
    {
        public int Saves { get; private set; } = 0;

        private Settings settings;

        public GameMapSaver(Settings settings)
        {
            this.settings = settings;
        }

        public void Save(Map map)
        {
            if (!Directory.Exists(settings.MapSaves))
                Directory.CreateDirectory(settings.MapSaves);

            map.Save($"{settings.MapSaves}{Saves}.dat");
            Saves++;
        }

        public Map Load(int index) => Map.Load($"{settings.MapSaves}{index.ToString()}.dat");
    }
}
