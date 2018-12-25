using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Saver
{
    [Serializable]
    public class MapInfo
    {
        public string Name { get; set; }
        public Visibility[,] MapVisibilities { get; set; }
        public bool IsEnemy { get; set; }
        public int RoundsToDeplete { get; set; }
        public int Layer { get; set; }
    }
}
