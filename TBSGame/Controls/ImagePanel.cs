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
        public Rectangle MaxBounds { get; private set; }
        public Rectangle ImageBounds { get; set; }

        public double WidthCoef => ImageBounds.Width / texture.Width;
        public double HeightCoef => ImageBounds.Height / texture.Height;

        private Texture2D texture;
        public Texture2D Image
        {
            get => texture;
            set
            {
                texture = value;
                ImageBounds = texture.Bounds;
                load_bounds();
            }
        }

        public ImagePanel(Texture2D texture)
        {
            this.texture = texture;
            ImageBounds = texture.Bounds;
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
            int width = 0, height = 0;
            if ((double)bounds.Width / texture.Width < (double)bounds.Height / texture.Height)
            {
                width = bounds.Width;
                height = width * (texture.Width / texture.Height);
            }
            else
            {
                height = bounds.Height;
                width = height * (texture.Height / texture.Width);
            }

            MaxBounds = new Rectangle(0, 0, width, height);

            if (ImageBounds.Width > width || ImageBounds.Height > height)
                ImageBounds = new Rectangle(0, 0, width, height);

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
