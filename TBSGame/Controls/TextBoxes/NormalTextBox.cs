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
        private Texture2D bg;

        public override Color BackColor { get; set; } = Color.Black;
        public override Color Frame { get; set; } = new Color(30, 30, 30);
        public override Color MouseOverFrame { get; set; } = Color.White;
        public override Color TextColor { get; set; } = Color.White;
        public override Color PlaceHolderColor { get; set; } = Color.Gray;

        protected override void draw_background()
        {
            sprite.Draw(bg, bounds, Color.White);
        }

        protected override void draw_border()
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
        }
    }
}
