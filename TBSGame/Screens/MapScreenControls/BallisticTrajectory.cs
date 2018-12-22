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
        private List<Point> data;

        public int Count { get; private set; }
        public Point A { get; private set; }

        public BallisticTrajectory(Map map, Engine engine, Point a, Point b)
        {
            //reálná velikost čtverce
            double size = engine.Size * 1.235;
            double height = (engine.Size / 2.5) * 1.235;

            A = a;

            //posunutí ze startu na cíl
            Vector3 s = new Vector3((float)((b.X - a.X) * size), (float)((b.Y - a.Y) * size), (float)((map.GetElevation(b.X, b.Y) - map.GetElevation(a.X, a.Y)) * height));

            //úhly otočení
            double alpha = Math.Atan2(0, 1) - Math.Atan2(s.Y, s.X); 
            double gama = (Math.Atan2(0, 1) - Math.Atan2(s.Z, s.X)) % Math.PI; 
            double beta = Math.PI / 5.2;

            if (gama != 0)
                gama = Math.PI - gama;

            //vzdálenost startu k cíli
            double distance = Math.Pow(Math.Pow(s.X, 2) + Math.Pow(s.Y, 2) + Math.Pow(s.Z, 2), 0.5);

            Count = (int)distance;
            data = new List<Point>(Count);

            //vrchol paraboly
            double vx = distance / 2;
            double vy = distance / 4;

            if (Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1)
                vy = 10;

            //parametr
            double p = Math.Pow(vx, 2) / (-2 * vy);

            for (int t = -Count / 2; t <= Count / 2; t++)
            {
                //parametrická rovnice paraboly
                double px = t + vx;
                double py = Math.Pow(t, 2) / (2 * p) + vy;
                
                //otočení paraboly do správne pozice
                double x = cx(px, 0, py, Math.PI / 4 - alpha, beta, gama);
                double y = cy(px, 0, py, Math.PI / 4 - alpha, beta, gama);

                data.Add(new Point((int)x, (int)y));
            }
        }

        public Point GetPoint(Engine engine, int index)
        {
            System.Drawing.PointF s = engine.GetCenter(A.X, A.Y);
            int x = data[index].X;
            int y = data[index].Y;
            return new Point((int)(s.X + x), (int)(s.Y + y));
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
