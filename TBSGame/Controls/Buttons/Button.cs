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
        public override Color Frame { get; set; } = Color.Transparent;
        public override Color MouseOverFrame { get; set; } = Color.Transparent;
        public Color LockedColor { get; set; } = Color.Gray;

        public int Key { get; set; } = 0;
        public string Title { get; set; }
        public float Opacity { get; set; } = 1f;

        public override bool BorderTop { get; set; } = true;
        public override bool BorderLeft { get; set; } = true;
        public override bool BorderRight { get; set; } = true;
        public override bool BorderBottom { get; set; } = true;

        protected Texture2D background_fill, background_over_fill, locked_bg;

        public Button(string title)
        {
            Title = title;
        }

        protected override void load()
        {
            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
            locked_bg = sprite.GetColorFill(Color.Lerp(Fill, Color.White, 0.2f));
        }

        protected override void draw()
        {
            sprite.Draw(IsLocked ? locked_bg : (is_mouse_hover ? background_over_fill : background_fill), bounds, Color.White);
            _draw();
            Vector2 middle = new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(Title).X) / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 - 1);
            sprite.DrawString(Font, Title, middle, IsLocked ? LockedColor : (is_mouse_hover ? MouseOverTextColor : TextColor) * Opacity);
        }

        protected virtual void _draw() { }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            sprite.SetColorFill(ref background_fill, Fill);
            sprite.SetColorFill(ref background_over_fill, MouseOverFill);
        }
    }
}
