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
        private Point[] p;

        private TimeSpan start = TimeSpan.Zero;

        public BallisticTrajectoryControl(Map map, Engine engine, Point a, Point b)
        {
            trajetory = new BallisticTrajectory(map, engine, a, b);
            p = new Point[trajetory.Count];
        }

        protected override void load()
        {
            arrow = sprite.GetColorFill(Color.Red);
        }

        public void Update(Engine engine, GameTime time)
        {
            if (index < trajetory.Count)
            {
                for (int i = 0; i < trajetory.Count; i++)
                    p[i] = trajetory.GetPoint(engine, i);
                index = trajetory.Count;
            }
        }

        public void Draw()
        {
            for (int i = 0; i < trajetory.Count; i++)
            {
                Vector2 point = parse(p[i].ToVector2());
                sprite.Draw(arrow, new Rectangle((int)point.X - 1, (int)point.Y - 1, 2, 2), Color.White);
            }
        }
    }
}
