using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Saver
{
    [Serializable]
    public class GameMapSaver
    {
        public int Saves { get; private set; } = 0;

        public void Save(Map map)
        {
            map.Save($"temp\\{Saves}.dat");
            Saves++;
        }

        public Map Load(int index) => Map.Load("index.dat");
    }
}
