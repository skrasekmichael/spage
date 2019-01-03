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
        private BallisticTrajectory trajectory;
        private Texture2D arrow;
        private int index = 0, next = 10;
        private Point ap, op, p1, p2;
        private float rotation = 0;
        private TimeSpan start = TimeSpan.Zero;

        public BallisticTrajectoryControl(Map map, Engine engine, Point a, Point b, double c1, double c2)
        {
            trajectory = new BallisticTrajectory(map, engine, a, b, c1, c2);
            index = next;
        }

        protected override void load()
        {
            arrow = sprite.GetColorFill(Color.Silver);
        }

        public bool Update(Engine engine, GameTime time)
        {
            if (index < trajectory.Count)
            {
                if (start == TimeSpan.Zero)
                {
                    start = time.TotalGameTime - TimeSpan.FromMilliseconds(10);
                    p2 = trajectory.GetPoint(engine, 0);
                }

                if (time.TotalGameTime - start >= TimeSpan.FromMilliseconds(10))
                {
                    p1 = trajectory.GetPoint(engine, index);
                    index += next;

                    ap = parse(p1.ToVector2()).ToPoint();
                    op = parse(p2.ToVector2()).ToPoint();

                    rotation = (float)Math.Atan2((ap.Y - op.Y), (ap.X - op.X));

                    start = time.TotalGameTime;
                    p2 = p1;
                }
                return false;
            }
            else
                return true;
        }

        public void Draw()
        {
            if (start != TimeSpan.Zero)
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
}
