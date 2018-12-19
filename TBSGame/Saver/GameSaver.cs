using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;

namespace TBSGame.Saver
{
    [Serializable]
    public class GameSaver
    {
        public int MapLevel { get; set; }
        public Dictionary<string, Visibility[,]> MapVisibilities { get; set; }
        public List<Unit> Units { get; set; } = null;
        public string Name { get; set; }
    }
}
