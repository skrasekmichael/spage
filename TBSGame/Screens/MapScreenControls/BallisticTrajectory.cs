using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class BallisticTrajectory
    {
        private List<Point> sequence = new List<Point>();
        public int Count => sequence.Count;

        public BallisticTrajectory(Engine engine, Point a, Point b)
        {
            System.Drawing.PointF A = engine.GetPoint(a.X, a.Y);
            System.Drawing.PointF B = engine.GetPoint(b.X, b.Y);

            int len = (int)Math.Pow(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2), 0.5);
            int h = 50;
            Point C = new Point((int)(A.X + B.X) / 2, (int)(A.Y + B.Y) / 2 + h);

            int ca_sx = (int)Math.Abs(C.X - A.X);
            int ca_sy = (int)Math.Abs(C.Y - A.Y);

            int bc_sx = (int)Math.Abs(B.X - C.X);
            int bc_sy = (int)Math.Abs(B.Y - C.Y);

            double coef_x1 = ca_sx / (len / 2);
            double coef_y1 = ca_sy / h;

            double coef_x2 = bc_sx / (len / 2);
            double coef_y2 = bc_sy / h;

            for (int i = len / 2; i >= 0; i--)
            {
                int px = (int)(A.X + i * coef_x1);
                int py = (int)(A.Y + Math.Pow(i, 3) / 10 * coef_y1);
                sequence.Add(new Point(px, py));
            }

            for (int i = len / 2; i >= 0; i--)
            {
                int px = (int)(C.X + i * coef_x2);
                int py = (int)(C.Y - Math.Pow(i, 3) / 10 * coef_y2);
                sequence.Add(new Point(px, py));
            }
        }

        public Point this[int index] => sequence[index];
    }
}
