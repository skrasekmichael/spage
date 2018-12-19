using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TBSGame.Controls
{
    public delegate void ButtonClickedEventHandler(object sender);

    public abstract class Button : Control
    {
        public event ButtonClickedEventHandler OnButtonClicked;

        protected virtual void ButtonClick()
        {
            OnButtonClicked?.Invoke(this);
        }

        public Color TextColor { get; set; } = Color.Black;
        public Color MouseOverTextColor { get; set; } = Color.Black;
        public Color Fill { get; set; } = Color.Transparent;
        public Color MouseOverFill { get; set; } = Color.Transparent;
        public Color Frame { get; set; } = Color.Transparent;
        public Color MouseOverFrame { get; set; } = Color.Transparent;

        protected Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);
        public int FrameWidth { get; set; } = 1;
        public int Key { get; set; } = 0;
        public string Title { get; set; }
        public float Opacity { get; set; } = 1f;
        public object Tag { get; set; }

        public bool BorderTop { get; set; } = true;
        public bool BorderLeft { get; set; } = true;
        public bool BorderRight { get; set; } = true;
        public bool BorderBottom { get; set; } = true;

        protected bool is_mouse_hover = false, is_mouse_down = false;
        protected Texture2D frame, frame_over, background_fill, background_over_fill;

        public Button(string title)
        {
            Title = title;
        }

        protected abstract void draw();

        protected override void load()
        {
            frame = sprite.GetColorFill(Frame);
            frame_over = sprite.GetColorFill(MouseOverFrame);
            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
        }

        public override void Draw()
        {
            sprite.Draw(is_mouse_hover ? background_over_fill : background_fill, bounds, Color.White);
            draw();
            Vector2 middle = new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(Title).X) / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1);
            sprite.DrawString(Font, Title, middle, (is_mouse_hover ? MouseOverTextColor : TextColor) * Opacity);

            if (BorderTop)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Bounds.Width, FrameWidth), Color.White);
            if (BorderLeft)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, FrameWidth, Bounds.Height), Color.White);
            if (BorderBottom)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y + Bounds.Height - FrameWidth, Bounds.Width, FrameWidth), Color.White);
            if (BorderRight)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X + Bounds.Width - FrameWidth, bounds.Y, FrameWidth, Bounds.Height), Color.White);
        }

        public void Update(MouseState mouse)
        {
            check_mouse(mouse);

            sprite.SetColorFill(ref frame, Frame);
            sprite.SetColorFill(ref frame_over, MouseOverFrame);
            sprite.SetColorFill(ref background_fill, Fill);
            sprite.SetColorFill(ref background_over_fill, MouseOverFill);
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse) => Update(mouse == null ? Mouse.GetState() : mouse.Value);

        protected void check_mouse(MouseState mouse)
        {
            if (this.bounds.Contains(mouse.X, mouse.Y))
            {
                this.is_mouse_hover = true;
                if (mouse.LeftButton != ButtonState.Pressed && is_mouse_down)
                {
                    ButtonClick();
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
        }
    }
}