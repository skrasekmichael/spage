using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Controls.TextBoxes
{
    public class NormalTextBox : TextBox
    {
        private Texture2D bg, frame, frame_over;

        public override Color BackColor { get; set; } = Color.Black;
        public override Color BorderColor { get; set; } = Color.Aqua;
        public override Color BorderHoverColor { get; set; } = Color.Silver;
        public override Color TextColor { get; set; } = Color.White;

        protected override void draw_background()
        {
            sprite.Draw(bg, bounds, Color.White);
        }

        protected override void draw_foreground()
        {
            if (BorderTop)
                sprite.Draw(is_mouse_hover || IsFocused ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Bounds.Width, Border), Color.White);
            if (BorderLeft)
                sprite.Draw(is_mouse_hover || IsFocused ? frame_over : frame, new Rectangle(bounds.X, bounds.Y, Border, Bounds.Height), Color.White);
            if (BorderBottom)
                sprite.Draw(is_mouse_hover || IsFocused ? frame_over : frame, new Rectangle(bounds.X, bounds.Y + Bounds.Height - Border, Bounds.Width, Border), Color.White);
            if (BorderRight)
                sprite.Draw(is_mouse_hover || IsFocused ? frame_over : frame, new Rectangle(bounds.X + Bounds.Width - Border, bounds.Y, Border, Bounds.Height), Color.White);
        }

        protected override void load()
        {
            bg = sprite.GetColorFill(BackColor);
            frame = sprite.GetColorFill(BorderColor);
            frame_over = sprite.GetColorFill(BorderHoverColor);
        }

        protected override void update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            
        }
    }
}
