using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Controls
{
    public class ImagePanel : Control
    {
        public VerticalAligment VAligment { get; set; }
        public HorizontalAligment HAligment { get; set; }

        public Rectangle MaxBounds { get; private set; }
        public Rectangle ImageBounds { get; set; }

        public double WidthCoef => ImageBounds.Width / texture.Width;
        public double HeightCoef => ImageBounds.Height / texture.Height;

        public override Border Border { get; set; } = new Border() { IsVisible = true };
        public override Color Frame { get; set; } = Color.Pink;

        private Texture2D texture;

        public ImagePanel(Texture2D texture)
        {
            this.texture = texture;
        }

        protected override void draw()
        {
            sprite.Draw(texture, new Rectangle(bounds.X + ImageBounds.X, bounds.Y + ImageBounds.Y, ImageBounds.Width, ImageBounds.Height), Color.White);
        }

        protected override void load()
        {
            load_bounds();
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            load_bounds();
        }

        private void load_bounds()
        {
            double wc = bounds.Width / texture.Width;
            double hc = bounds.Height / texture.Height;

            int width = 0, height = 0;

            if (wc < hc)
            {
                width = bounds.Width;
                height = (int)(bounds.Height * wc);
            }
            else
            {
                width = (int)(bounds.Width * hc);
                height = bounds.Height;
            }

            MaxBounds = new Rectangle(0, 0, width, height);

            if (ImageBounds.Width > width || ImageBounds.Height > height)
                ImageBounds = MaxBounds;            

            Rectangle rec = new Rectangle(0, 0, ImageBounds.Width, ImageBounds.Height);

            if (VAligment == VerticalAligment.Top)
                rec.Y = 0;
            else if (VAligment == VerticalAligment.Center)
                rec.Y = (bounds.Height - ImageBounds.Height) / 2;
            else if (VAligment == VerticalAligment.Bottom)
                rec.Y = bounds.Height - ImageBounds.Height;

            if (HAligment == HorizontalAligment.Left)
                rec.X = 0;
            else if (HAligment == HorizontalAligment.Center)
                rec.X = (bounds.Width - ImageBounds.Width) / 2;
            else if (HAligment == HorizontalAligment.Right)
                rec.X = bounds.Width - ImageBounds.Width;

            ImageBounds = rec;
        }
    }
}
