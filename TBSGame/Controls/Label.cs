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
    public class Label : Control
    {
        public string Text { get; set; }
        public override HorizontalAligment HAligment { get; set; } = HorizontalAligment.Center;
        public override VerticalAligment VAligment { get; set; } = VerticalAligment.Center;
        public int Space { get; set; } = 10;
        public int LineHeight { get; set; } = 0;
        public float Opacity { get; set; } = 1f;

        private bool IsMultiLine => Text.Split('\n').Length > 1;

        public Label(string text)
        {
            this.Text = text;
        }

        protected override void load() { }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse) { }

        protected override void draw()
        {
            sprite.DrawMultiLineText(Font, Text.Split('\n'), bounds, HAligment, VAligment, Space, Foreground * Opacity, LineHeight);
        }
    }
}
