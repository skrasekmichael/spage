using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class MoveUnit
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Stamina { get; set; }
        public UnitControl UnitControl { get; private set; }

        public MoveUnit(UnitControl unit, int x, int y, double stamina)
        {
            this.X = x;
            this.Y = y;
            this.Stamina = stamina;
            this.UnitControl = unit;
        }

        public void Update(Map map, int oldx, int oldy)
        {
            int dx = oldx - X, dy = oldy - Y;
            int dir = GetDirection(dx, dy);

            if (dir != -1)
            {
                UnitControl.Unit.Direction = (byte)dir;
                map.SetUnit(oldx, oldy, null);
                map.SetUnit(X, Y, UnitControl.Unit);
            }
        }

        public static int GetDirection(int dx, int dy)
        {
            List<string> dirs = "ru,rc,rd,cd,ld,cu,lu,lc".Split(',').ToList();

            string xk = "c", yk = "c";
            if (dx < 0)
                xk = "r";
            else if (dx > 0)
                xk = "l";
            if (dy < 0)
                yk = "u";
            else if (dy > 0)
                yk = "d";

            if (xk != yk) return (byte)dirs.IndexOf(xk + yk);
            return -1;
        }
    }
}
