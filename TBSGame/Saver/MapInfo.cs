using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TBSGame.Saver
{
	[Serializable]
    public class MapInfo
    {
        public string Name { get; set; }
        public Visibility[,] MapVisibilities { get; set; }
        public Dictionary<Point, Unit> Units { get; set; } = null;
        public bool IsEnemy { get; set; }
        public int RoundsToDeplete { get; set; }
        public int Layer { get; set; }
    }
}
