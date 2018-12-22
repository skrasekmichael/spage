using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class BallisticTrajectoryControl : MapControl
    {
        private BallisticTrajectory trajetory;
        private Texture2D arrow;
        private int index = 0, next = 10;
        private Point ap, op;
        private float rotation = 0;
        private TimeSpan start = TimeSpan.Zero;

        public BallisticTrajectoryControl(Map map, Engine engine, Point a, Point b)
        {
            trajetory = new BallisticTrajectory(map, engine, a, b);
            index = next;
        }

        protected override void load()
        {
            arrow = sprite.GetColorFill(Color.Red);
        }

        public bool Update(Engine engine, GameTime time)
        {
            if (index < trajetory.Count)
            {
                if (start == TimeSpan.Zero)
                    start = time.TotalGameTime;

                if (time.TotalGameTime - start >= TimeSpan.FromMilliseconds(10))
                {
                    Point p1 = trajetory.GetPoint(engine, index);
                    Point p2 = trajetory.GetPoint(engine, index - next);
                    index += next;

                    ap = parse(p1.ToVector2()).ToPoint();
                    op = parse(p2.ToVector2()).ToPoint();
                    rotation = (float)Math.Atan2((ap.Y - op.Y), (ap.X - op.X));

                    start = time.TotalGameTime;
                }
                return false;
            }
            else
                return true;
        }

        public void Draw()
        {
            #pragma warning disable CS0618 // Typ nebo člen je zastaralý.
            sprite.Draw(
                texture: arrow,
                destinationRectangle: new Rectangle(ap, new Point(10, 2)), 
                rotation: rotation, 
                color: Color.White
                );
            #pragma warning restore CS0618 // Typ nebo člen je zastaralý.
        }
    }
}
