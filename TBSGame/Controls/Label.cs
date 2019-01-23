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
        public Color TextColor { get; set; } = Color.Black;
        public string Text { get; set; }
        private Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);
        public HorizontalAligment HAligment { get; set; } = HorizontalAligment.Center;
        public VerticalAligment VAligment { get; set; } = VerticalAligment.Center;
        public int Space { get; set; } = 10;
        public int LineHeight { get; set; } = 0;
        public float Opacity { get; set; } = 1f;

        private bool IsMultiLine => Text.Split('\n').Length > 1;
        private Vector2 loc = Vector2.Zero;

        public Label(string text)
        {
            this.Text = text;
        }

        protected override void load()
        { 
            loc = new Vector2(bounds.X, bounds.Y);
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse) => Update();

        public void Update()
        {
            int space = Space;
            if (HAligment == HorizontalAligment.Left)
                loc.X = bounds.X + space;
            else if (HAligment == HorizontalAligment.Center)
                loc.X = bounds.X + (Bounds.Width - Font.MeasureString(Text).X) / 2;
            else if (HAligment == HorizontalAligment.Right)
                loc.X = bounds.X + (Bounds.Width - Font.MeasureString(Text).X) - space;

            if (VAligment == VerticalAligment.Top)
                loc.Y = bounds.Y + space;
            else if (VAligment == VerticalAligment.Center)
                loc.Y = bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1;
            else if (VAligment == VerticalAligment.Bottom)
                loc.Y = bounds.Y + Bounds.Height - space;
        }

        protected override void draw()
        {
            sprite.DrawMultiLineText(Font, Text.Split('\n'), bounds, HAligment, VAligment, Space, TextColor * Opacity, LineHeight);
        }
    }
}
