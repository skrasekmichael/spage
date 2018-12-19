using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public abstract class MapControl
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width => graphics.PreferredBackBufferWidth;
        public int Height => graphics.PreferredBackBufferHeight;

        protected CustomSpriteBatch sprite;
        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected TextureDriver driver;
        protected SpriteFont font;

        protected Color shadow = Color.Gray;

        public virtual void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite, TextureDriver driver, SpriteFont font)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;
            this.driver = driver;
            this.font = font;

            load();
        }

        protected abstract void load();

        protected bool is_in_area(Vector2[] area, Vector2 point) => is_in_polygon(new Vector2[] { area[0], area[1], area[3] }, point) || is_in_polygon(new Vector2[] { area[0], area[2], area[3] }, point);

        protected bool is_in_polygon(Vector2[] polygon, Vector2 point)
        {
            int len = polygon.Length;
            bool inside = false;

            float px = point.X, py = point.Y;
            float sx, sy, ex, ey;

            Vector2 ep = polygon[len - 1];
            ex = ep.X;
            ey = ep.Y;

            int i = 0;
            while (i < len)
            {
                sx = ex;
                sy = ey;

                ep = polygon[i++];
                ex = ep.X;
                ey = ep.Y;

                inside ^= (ey > py ^ sy > py) && ((px - ex) < (py - ey) * (sx - ex) / (sy - ey));
            }
            return inside;
        }

        protected Vector2 to_vector(System.Drawing.PointF point) => new Vector2(point.X, point.Y);
        protected Vector2 parse(Vector2 val) => new Vector2(val.X + Width / 2, Height / 2 - val.Y);
    }
}
