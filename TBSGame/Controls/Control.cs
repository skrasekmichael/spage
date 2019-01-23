using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Controls
{
    public abstract class Control
    {
        protected GraphicsDeviceManager graphics;
        protected int Width => graphics.PreferredBackBufferWidth;
        protected int Height => graphics.PreferredBackBufferHeight;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected Vector2 start = new Vector2(0, 0);

        public bool IsVisible { get; set; } = true;
        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        public virtual SpriteFont Font { get; set; }

        public void SetPos(Vector2 p) => start = p;

        public void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;

            this.Font = load_font();
            load();
        }

        protected virtual SpriteFont load_font() => content.Load<SpriteFont>("fonts/text");

        protected abstract void load();

        public void Draw()
        {
            if (IsVisible)
                draw();
        }

        protected abstract void draw();

        public abstract void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse);

        public void Update(GameTime gametime, Vector2? start = null, KeyboardState? keyboard = null, MouseState? mouse = null)
        {
            if (IsVisible)
            {
                if (start == null)
                    this.start = new Vector2(0, 0);
                else
                    this.start = start.Value;

                Update(gametime, keyboard, mouse);
            }
        }
    }
}
