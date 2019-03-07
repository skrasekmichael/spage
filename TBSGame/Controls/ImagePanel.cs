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
    [Flags]
    public enum ImageSize
    {
        Min, Normal, Max, Special
    }

    public class ImagePanel : Control
    {
        private Rectangle min_bounds = Rectangle.Empty, max_bounds = Rectangle.Empty;
        public Rectangle MaxBounds
        {
            get => max_bounds;
            set
            {
                max_bounds = value;
                if (min_bounds != Rectangle.Empty && (min_bounds.Width > value.Width || min_bounds.Height > value.Height))
                    min_bounds = value;

                base.Bounds = value;
                if (IsLoaded)
                    load_bounds();
            }
        }
        public Rectangle MinBounds
        {
            get => min_bounds;
            set
            {
                min_bounds = value;
                if (max_bounds != Rectangle.Empty && (max_bounds.Width < value.Width || MaxBounds.Height < value.Height))
                    max_bounds = value;
                if (bounds.Width < value.Width || bounds.Height < value.Height)
                    base.Bounds = value;
                if (IsLoaded)
                    load_bounds();
            }
        }
        public override Rectangle Bounds
        {
            get => base.Bounds;
            set
            {
                base.Bounds = value;
                max_bounds = value;
                min_bounds = value;
                if (IsLoaded)
                    load_bounds();
            }
        }
        public Rectangle ImageBounds { get; private set; }

        private ImageSize size = ImageSize.Normal;
        public ImageSize ImageSize
        {
            get => size; set
            {
                size = value;
                if (IsLoaded)
                    load_bounds();
            }
        }

        private Texture2D texture;
        public Texture2D Image
        {
            get => texture;
            set
            {
                texture = value;
                ImageBounds = texture.Bounds;
                if (IsLoaded)
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
            sprite.Draw(texture, new Rectangle(bounds.X + ImageBounds.X, bounds.Y + ImageBounds.Y, ImageBounds.Width, ImageBounds.Height), Color.White * Opacity);
        }

        protected override void load()
        {
            load_bounds();
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            load_position();
        }

        private void load_size()
        {
            if (bounds.Height != 0)
            {

                int wmax = 0, wmin = 0, hmax = 0, hmin = 0;
                double w_coef = (double)texture.Height / texture.Width;
                double h_coef = (double)texture.Width / texture.Height;

                if ((double)bounds.Width / texture.Width > (double)bounds.Height / texture.Height)
                {
                    wmin = MinBounds.Width;
                    hmin = (int)(wmin * w_coef);

                    wmax = MaxBounds.Width;
                    hmax = (int)(wmax * w_coef);
                }
                else
                {
                    hmin = MinBounds.Height;
                    wmin = (int)(hmin * h_coef);

                    hmax = MaxBounds.Height;
                    wmax = (int)(hmax * h_coef);
                }

                Rectangle min = new Rectangle(0, 0, wmin, hmin);
                Rectangle max = new Rectangle(0, 0, wmax, hmax);

                switch (ImageSize)
                {
                    case ImageSize.Min:
                        ImageBounds = min;
                        break;
                    case ImageSize.Max:
                        ImageBounds = max;
                        break;
                    case ImageSize.Normal:
                        if (MinBounds != Rectangle.Empty && (texture.Width < wmin || texture.Height < hmin))
                            ImageBounds = min;
                        else if (MaxBounds != Rectangle.Empty && (texture.Width > wmax || texture.Height > hmax))
                            ImageBounds = max;
                        else
                            ImageBounds = texture.Bounds;
                        break;
                    case ImageSize.Special:
                        if (ImageBounds.Width < wmin || ImageBounds.Height < hmin)
                            ImageBounds = min;
                        else if (ImageBounds.Width > wmax || ImageBounds.Height > hmax)
                            ImageBounds = max;
                        break;
                }
            }
        }

        private void load_position()
        {
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

        private void load_bounds()
        {
            load_size();
            load_position();
        }

        public void SetSize(double coef)
        {
            ImageBounds = new Rectangle(0, 0, (int)(texture.Width * coef), (int)(texture.Height * coef));
            ImageSize = ImageSize.Special;
        }

        public void SetSize(Rectangle bounds)
        {
            ImageBounds = bounds;
            ImageSize = ImageSize.Special;
        }
    }
}
