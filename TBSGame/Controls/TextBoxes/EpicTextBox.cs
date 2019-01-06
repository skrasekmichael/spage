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
    public class EpicTextBox : TextBox
    {
        public override Color BackColor { get; set; } = Color.Black;
        public override Color BorderColor { get; set; } = Color.DarkGray;
        public override Color BorderHoverColor { get; set; } = Color.Silver;
        public override Color TextColor { get; set; } = Color.White;
        public override Color PlaceHolderColor { get; set; } = new Color(Color.White, 0.3f);

        private string underline = "";

        protected override void draw_background()
        {
            
        }

        protected override void draw_foreground()
        {
            sprite.DrawString(Font, underline, new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(underline).X) / 2, bounds.Y + Bounds.Height - Font.LineSpacing + 8), Color.Black);
        }

        protected override void load()
        {
            for (int i = 0; i <= (int)(Bounds.Width / Font.MeasureString("_").X); i++)
                underline += "_";
        }

        protected override void update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            
        }
    }
}
