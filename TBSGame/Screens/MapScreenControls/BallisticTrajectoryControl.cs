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
        private int index = 0;
        private Point p;

        private TimeSpan start = TimeSpan.Zero;

        public BallisticTrajectoryControl(Engine engine, Point a, Point b)
        {
            trajetory = new BallisticTrajectory(engine, a, b);
        }

        protected override void load()
        {
            arrow = sprite.GetColorFill(Color.Red);
        }

        public void Update(GameTime time)
        {
            if (index < trajetory.Count)
            {
                p = trajetory[index];
                index++;
            }
        }

        public void Draw()
        {
            Vector2 point = parse(p.ToVector2());
            sprite.Draw(arrow, new Rectangle((int)point.X - 2, (int)point.Y - 2, 4, 4), Color.White);
            Console.WriteLine(p);
        }
    }
}
