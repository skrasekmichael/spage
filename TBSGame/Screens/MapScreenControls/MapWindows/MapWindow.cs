using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls.MapWindows
{
    public abstract class MapWindow : MapControl
    {
        public bool Visible { get; set; } = false;

        protected Texture2D black;

        private VertexPositionColorTexture[] bg;

        public override void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite, TextureDriver driver, SpriteFont font)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;
            this.driver = driver;
            this.font = font;

            black = sprite.GetColorFill(Color.Black);
            Color transparent = new Color(Color.Black, 0.5f);
            bg = get_vertex_ct(new Vector2[] {
                new Vector2(-Width / 2, Height / 2),
                new Vector2(Width / 2, Height / 2),
                new Vector2(Width / 2, -Height / 2),
                new Vector2(-Width / 2, -Height / 2)
            }, transparent);

            load();
        }

        protected VertexPositionTexture[] get_vertex(Vector2[] points)
        {
            return new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(points[1], 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(points[0], 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(points[3], 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(points[2], 0), new Vector2(1, 0))
            };
        }

        protected VertexPositionColor[] get_vertex(Vector2[] points, Color color)
        {
            return new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(points[1], 0), color),
                new VertexPositionColor(new Vector3(points[0], 0), color),
                new VertexPositionColor(new Vector3(points[3], 0), color),
                new VertexPositionColor(new Vector3(points[2], 0), color)
            };
        }

        protected VertexPositionColorTexture[] get_vertex_ct(Vector2[] points, Color color)
        {
            return new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture(new Vector3(points[2], 0), color, new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(points[3], 0), color, new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(points[1], 0), color, new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(points[0], 0), color, new Vector2(1, 0)),
                new VertexPositionColorTexture(new Vector3(points[2], 0), color, new Vector2(0, 1))
            };
        }

        protected abstract void draw();
        public void Draw()
        {
            if (Visible)
            {
                sprite.FillArea(bg, black);
                draw();
            }
        }
    }
}
