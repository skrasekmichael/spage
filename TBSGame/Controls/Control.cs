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

        public virtual bool BorderTop { get; set; } = false;
        public virtual bool BorderLeft { get; set; } = false;
        public virtual bool BorderRight { get; set; } = false;
        public virtual bool BorderBottom { get; set; } = false;
        public int FrameWidth { get; set; } = 1;

        public bool IsVisible { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public bool IsMouseOver => is_mouse_hover;
        public object Tag { get; set; }

        public virtual Color Frame { get; set; } = Color.Transparent;
        public virtual Color MouseOverFrame { get; set; } = Color.Transparent;

        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        protected Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);

        protected Texture2D frame, frame_over;

        public virtual SpriteFont Font { get; set; }

        public void SetPos(Vector2 p) => start = p;

        public void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite, Vector2? start = null)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;

            if (start == null)
                this.start = new Vector2(0, 0);
            else
                this.start = start.Value;

            this.Font = load_font();
            frame = sprite.GetColorFill(Frame);
            frame_over = sprite.GetColorFill(MouseOverFrame);

            load();
        }

        protected virtual SpriteFont load_font() => content.Load<SpriteFont>("fonts/text");

        protected abstract void load();

        public void Draw()
        {
            if (IsVisible)
            {
                draw();
                draw_border();
            }
        }

        protected virtual void draw_border()
        {
            if (BorderTop)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Bounds.Width, FrameWidth), Color.White);
            if (BorderLeft)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, FrameWidth, Bounds.Height), Color.White);
            if (BorderBottom)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y + Bounds.Height - FrameWidth, Bounds.Width, FrameWidth), Color.White);
            if (BorderRight)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X + Bounds.Width - FrameWidth, bounds.Y, FrameWidth, Bounds.Height), Color.White);
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

                sprite.SetColorFill(ref frame, Frame);
                sprite.SetColorFill(ref frame_over, MouseOverFrame);

                update(gametime, keyboard, mouse);
            }
        }
    }
}
