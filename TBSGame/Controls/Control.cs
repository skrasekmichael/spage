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
    public delegate void ControlClickedEventHandler(object sender);

    public abstract class Control
    {
        public virtual event ControlClickedEventHandler OnControlClicked;

        protected GraphicsDeviceManager graphics;
        protected int Width => graphics.PreferredBackBufferWidth;
        protected int Height => graphics.PreferredBackBufferHeight;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected Vector2 start = new Vector2(0, 0);
        protected bool is_mouse_hover = false, is_mouse_down = false;

        public bool IsVisible { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public bool IsMouseOver => is_mouse_hover;

        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        protected Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);
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

        protected abstract void update(GameTime time, KeyboardState keyboard, MouseState mouse);

        public void Update(GameTime gametime, KeyboardState keyboard, MouseState mouse, Vector2? start = null)
        {
            if (IsVisible)
            {
                if (start == null)
                    this.start = new Vector2(0, 0);
                else
                    this.start = start.Value;

                if (!IsLocked && this.bounds.Contains(mouse.X, mouse.Y))
                {
                    this.is_mouse_hover = true;
                    if (mouse.LeftButton != ButtonState.Pressed && is_mouse_down)
                    {
                        OnControlClicked?.Invoke(this);
                        this.is_mouse_down = false;
                    }
                    else
                        this.is_mouse_down = (mouse.LeftButton == ButtonState.Pressed);
                }
                else
                {
                    this.is_mouse_hover = false;
                    this.is_mouse_down = false;
                }

                update(gametime, keyboard, mouse);
            }
        }
    }
}
