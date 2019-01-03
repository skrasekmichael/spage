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

            //úhly otočení
            alpha = Math.Atan2(0, 1) - Math.Atan2(s.Y, s.X); 
            gama = (Math.Atan2(0, 1) - Math.Atan2(s.Z, d)); 
            beta = Math.PI / 5.2;

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
            double x = cx(rx, 0, ry, Math.PI / 4 - alpha, beta);
            double y = cy(rx, 0, ry, Math.PI / 4 - alpha, beta);

            System.Drawing.PointF s = engine.GetCenter(A.X, A.Y);
            return new Point((int)(s.X + x), (int)(s.Y + y + h1));
        }

        private double cxx(double a, double b, double c) => Math.Cos(c) * Math.Cos(a) - Math.Sin(c) * Math.Sin(a) * Math.Sin(b);
        private double cxy(double a, double b, double c) => Math.Cos(c) * Math.Sin(a) * Math.Sin(b) + Math.Sin(c) * Math.Cos(a);
        private double cyx(double a, double b, double c) => -Math.Cos(c) * Math.Sin(a) - Math.Sin(c) * Math.Cos(a) * Math.Sin(b);
        private double cyy(double a, double b, double c) => Math.Cos(c) * Math.Cos(a) * Math.Sin(b) - Math.Sin(c) * Math.Sin(a);
        private double czx(double a, double b, double c) => -Math.Sin(c) * Math.Cos(b);
        private double czy(double a, double b, double c) => Math.Cos(c) * Math.Cos(b);

        private double cx(double x, double y, double z, double a, double b, double c = 0) => x * cxx(a, b, c) + y * cyx(a, b, c) + z * czx(a, b, c);
        private double cy(double x, double y, double z, double a, double b, double c = 0) => x * cxy(a, b, c) + y * cyy(a, b, c) + z * czy(a, b, c);
    }
}
