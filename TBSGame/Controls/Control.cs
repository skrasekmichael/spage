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
    public delegate void ControlMouseMoveEventHandler(object sender, Point position);
    public delegate void ControlBoundsChnagedEventHanlder(object sender, Rectangle bounds);

    public abstract class Control
    {
        public virtual event ControlClickedEventHandler OnControlClicked;
        public virtual event ControlMouseMoveEventHandler OnMouseMoved;
        public virtual event ControlBoundsChnagedEventHanlder OnBoundsChaned;

        protected GraphicsDeviceManager graphics;
        protected int Width => graphics.PreferredBackBufferWidth;
        protected int Height => graphics.PreferredBackBufferHeight;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected Vector2 start = new Vector2(0, 0);
        protected bool is_mouse_hover = false, is_mouse_down = false;

        public virtual SpriteFont Font { get; set; }
        public virtual HorizontalAligment HAligment { get; set; } = HorizontalAligment.Left;
        public virtual VerticalAligment VAligment { get; set; } = VerticalAligment.Top;
        public virtual Border Border { get; set; } = new Border();
        protected Color _fore = Color.Black;
        public virtual Color Foreground
        {
            get => _fore;
            set => _fore = value;
        }

        public float Opacity { get; set; } = 1f;
        public bool IsLoaded { get; private set; } = false;
        public bool IsVisible { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public bool IsMouseOver => is_mouse_hover;
        public object Tag { get; set; }

        private Rectangle _bounds = Rectangle.Empty;
        public Rectangle Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                OnBoundsChaned?.Invoke(this, value);
            }
        }
        protected Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);

        protected Texture2D frame, frame_over;
        private Point last = new Point(-1, -1);

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
            frame = sprite.GetColorFill(Border.Color);
            frame_over = sprite.GetColorFill(Border.MouseOverColor);

            load();
            IsLoaded = true;
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
            if (Border.Top)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Bounds.Width, Border.Width), Color.White * Opacity);
            if (Border.Left)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Border.Width, Bounds.Height), Color.White * Opacity);
            if (Border.Bottom)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y + Bounds.Height - Border.Width, Bounds.Width, Border.Width), Color.White * Opacity);
            if (Border.Right)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X + Bounds.Width - Border.Width, bounds.Y, Border.Width, Bounds.Height), Color.White * Opacity);
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
                    Point npoint = mouse.Position;
                    if (npoint != last)
                        OnMouseMoved?.Invoke(this, npoint);
                    last = npoint;

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

                sprite.SetColorFill(ref frame, Border.Color);
                sprite.SetColorFill(ref frame_over, Border.MouseOverColor);

                update(gametime, keyboard, mouse);
            }
        }
    }

    public class Border
    {
        public Color Color { get; set; } = Color.Transparent;
        public Color MouseOverColor { get; set; } = Color.Transparent;

        public bool Top { get; set; } = false;
        public bool Left { get; set; } = false;
        public bool Right { get; set; } = false;
        public bool Bottom { get; set; } = false;

        public int Width { get; set; } = 1;

        public bool IsVisible
        {
            set
            {
                Top = value;
                Left = value;
                Right = value;
                Bottom = value;
            }
        }
    }
}
