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
    public abstract class Button : Label
    {
        public Color MouseOverForeground { get; set; } = Color.Black;
        public Color Fill { get; set; } = Color.Transparent;
        public Color MouseOverFill { get; set; } = Color.Transparent;
        public Color LockedColor { get; set; } = Color.Gray;

        public int Key { get; set; } = 0;

        public override Border Border { get; set; } = new Border() { IsVisible = true };

        protected Texture2D background_fill, background_over_fill, locked_bg;

        public Button(string title) : base(title)
        {
            
        }

        protected override void load()
        {
            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
            locked_bg = sprite.GetColorFill(Color.Lerp(Fill, Color.White, 0.2f));
        }

        protected override void draw()
        {
            sprite.Draw(IsLocked ? locked_bg : (is_mouse_hover ? background_over_fill : background_fill), bounds, Color.White * Opacity);
            _draw();
            sprite.DrawMultiLineText(Font, Text.Split('\n'), bounds, HAligment, VAligment, Space, IsLocked ? LockedColor : (is_mouse_hover ? MouseOverForeground : Foreground) * Opacity, LineHeight);
        }

        protected virtual void _draw() { }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            sprite.SetColorFill(ref background_fill, Fill);
            sprite.SetColorFill(ref background_over_fill, MouseOverFill);
        }
    }
}
