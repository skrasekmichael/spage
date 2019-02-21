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
    public abstract class Button : Control
    {
        public Color TextColor { get; set; } = Color.Black;
        public Color MouseOverTextColor { get; set; } = Color.Black;
        public Color Fill { get; set; } = Color.Transparent;
        public Color MouseOverFill { get; set; } = Color.Transparent;
        public Color Frame { get; set; } = Color.Transparent;
        public Color MouseOverFrame { get; set; } = Color.Transparent;
        public Color LockedColor { get; set; } = Color.Gray;

        public int FrameWidth { get; set; } = 1;
        public int Key { get; set; } = 0;
        public string Title { get; set; }
        public float Opacity { get; set; } = 1f;
        public object Tag { get; set; }

        public bool BorderTop { get; set; } = true;
        public bool BorderLeft { get; set; } = true;
        public bool BorderRight { get; set; } = true;
        public bool BorderBottom { get; set; } = true;

        protected Texture2D frame, frame_over, background_fill, background_over_fill, locked_bg;

        public Button(string title)
        {
            Title = title;
        }

        protected override void load()
        {
            frame = sprite.GetColorFill(Frame);
            frame_over = sprite.GetColorFill(MouseOverFrame);
            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
            locked_bg = sprite.GetColorFill(Color.Lerp(Fill, Color.White, 0.2f));
        }

        protected override void draw()
        {
            sprite.Draw(IsLocked ? locked_bg : (is_mouse_hover ? background_over_fill : background_fill), bounds, Color.White);
            _draw();
            Vector2 middle = new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(Title).X) / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1);
            sprite.DrawString(Font, Title, middle, IsLocked ? LockedColor : (is_mouse_hover ? MouseOverTextColor : TextColor) * Opacity);

            if (BorderTop)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Bounds.Width, FrameWidth), Color.White);
            if (BorderLeft)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, FrameWidth, Bounds.Height), Color.White);
            if (BorderBottom)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X, bounds.Y + Bounds.Height - FrameWidth, Bounds.Width, FrameWidth), Color.White);
            if (BorderRight)
                sprite.Draw(is_mouse_hover ? frame_over : frame, new Rectangle(bounds.X + Bounds.Width - FrameWidth, bounds.Y, FrameWidth, Bounds.Height), Color.White);
        }

        protected virtual void _draw() { }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            sprite.SetColorFill(ref frame, Frame);
            sprite.SetColorFill(ref frame_over, MouseOverFrame);
            sprite.SetColorFill(ref background_fill, Fill);
            sprite.SetColorFill(ref background_over_fill, MouseOverFill);
        }
    }
}