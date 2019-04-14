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
        private double h1 = 0, alpha, beta, gama, vx, vy, p;

        public int Count { get; private set; }
        public Point A { get; private set; }

        public BallisticTrajectory(Map map, Engine engine, Point a, Point b, double c1, double c2)
        {
            A = a;
            h1 = c1;

            //reálná velikost čtverce
            double size = engine.Size * 1.235;
            double height = (engine.Size / 2.5) * 1.235;

            double elev1 = map.GetTerrain(a.X, a.Y).Elevation + c1;
            double elev2 = map.GetTerrain(b.X, b.Y).Elevation + c2;

            //posunutí ze startu na cíl
            Vector3 s = new Vector3((float)((b.X - a.X) * size), (float)((b.Y - a.Y) * size), (float)((elev2 - elev1) * height));

            //vzdálenost startu k cíli
            double distance = Math.Pow(Math.Pow(s.X, 2) + Math.Pow(s.Y, 2) + Math.Pow(s.Z, 2), 0.5);
            double d = Math.Pow(Math.Pow(s.X, 2) + Math.Pow(s.Y, 2), 0.5);

            //rotační úhly
            alpha = Math.PI / 4 - (Math.Atan2(0, 1) - Math.Atan2(s.Y, s.X)); 
            gama = Math.Atan2(0, 1) - Math.Atan2(s.Z, d);
            //beta = Math.PI / 5.1;
            beta = Math.PI / 6; //30°

            if (gama % Math.PI == 0)
                gama = 0;

            Count = (int)distance;

            //vrchol paraboly
            vx = distance / 2;
            vy = distance / 4;

            if (Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1)
                vy = 10;

            //parametr
            p = Math.Pow(vx, 2) / (-2 * vy);
        }

        public Point GetPoint(Engine engine, int index)
        {
            int t = index - Count / 2;
            //parametrická rovnice paraboly
            double px = t + vx;
            double py = Math.Pow(t, 2) / (2 * p) + vy;

            //otočení podle osy z
            double rx = px * Math.Cos(gama) + py * Math.Sin(gama);
            double ry = py * Math.Cos(gama) - px * Math.Sin(gama);

            //otočení paraboly do správne pozice
            double x = rx * Math.Cos(alpha);
            double y = rx * Math.Sin(alpha) * Math.Sin(beta) + ry * Math.Cos(beta);

            System.Drawing.PointF s = engine.GetCenter(A.X, A.Y);
            return new Point((int)(s.X + x), (int)(s.Y + y + h1));
        }
    }
}
