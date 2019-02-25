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
    public class MapObjectControl : MapControl
    {
        public MapObject Object { get; set; }
        private Rectangle bounds;
        private Color texture;

        public MapObjectControl(MapObject obj, int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Object = obj;
        }

        protected override void load()
        {
            if (driver[Object.Texture] == null)
            {
                driver.LoadObject(Object.Texture);
            }
        }

        public void Update(Map map, Engine engine)
        {
            Rectangle get_object_bounds(Rectangle bounds)
            {
                double width = 2 * Math.Cos(Math.PI / 6) * engine.Size * Object.DrawScale;
                double height = width / bounds.Width * bounds.Height;
                Vector2 p = to_vector(engine.GetCenter(bounds.X, bounds.Y));

                switch (Object.DrawType)
                {
                    case DrawType.Bottom:
                        p = to_vector(engine.GetPoint(bounds.X, bounds.Y));
                        break;
                    case DrawType.Third:
                        p = new Vector2(p.X, (float)(p.Y - engine.Height / 2));
                        break;
                }
                return new Rectangle((int)p.X, (int)p.Y, (int)(width / 2), (int)height);
            }

            Texture2D tx = driver[Object.Texture];
            bounds = get_object_bounds(new Rectangle(X, Y, tx.Width, tx.Height));

            texture = Color.White;
            if (engine.GetVisibility(X, Y) == Visibility.Hidden)
                texture = shadow;
        }

        public void Draw()
        {
            VertexPositionColorTexture[] vertex = new VertexPositionColorTexture[5]
            { 
                new VertexPositionColorTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), texture, new Vector2(1, 0)),
                new VertexPositionColorTexture(new Vector3(bounds.X + bounds.Width, bounds.Y + bounds.Height, 0), texture, new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(bounds.X + bounds.Width, bounds.Y, 0), texture, new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(bounds.X - bounds.Width, bounds.Y, 0), texture, new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), texture, new Vector2(1, 0))
            };

            sprite.FillArea(vertex, driver[Object.Texture]);
        }
    }
}
