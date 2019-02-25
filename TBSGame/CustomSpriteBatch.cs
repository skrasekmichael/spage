using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame
{
    [Flags]
    public enum HorizontalAligment { Left = 0, Center = 1, Right = 2 }

    [Flags]
    public enum VerticalAligment { Top = 0, Center = 1, Bottom = 2 }

    public class CustomSpriteBatch : SpriteBatch
    {
        private BasicEffect effect;
        private GraphicsDeviceManager manager;

        public CustomSpriteBatch(GraphicsDeviceManager manager) : base(manager.GraphicsDevice)
        {
            this.manager = manager;
            Load();
        }

        public void Load()
        {
            effect = new BasicEffect(GraphicsDevice);
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 540), new Vector3(0, 0, 0), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.DisplayMode.AspectRatio, 1, 1000);
        }

        public void DrawMultiLineText(SpriteFont font, string[] lines, Rectangle bounds, HorizontalAligment h, VerticalAligment v, int space, Color c, int hspace = 0)
        {
            lines = lines.ToList().Select(line => WrapText(font, line, bounds.Width - 2 * space)).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                Vector2 loc = Vector2.Zero;
                string text = lines[i].Trim();

                if (h == HorizontalAligment.Left)
                    loc.X = bounds.X + space;
                else if (h == HorizontalAligment.Center)
                    loc.X = bounds.X + (bounds.Width - font.MeasureString(text).X) / 2;
                else if (h == HorizontalAligment.Right)
                    loc.X = bounds.X + (bounds.Width - font.MeasureString(text).X) - space;

                if (v == VerticalAligment.Top)
                    loc.Y = bounds.Y + space + i * (font.LineSpacing + hspace);
                else if (v == VerticalAligment.Center)
                    loc.Y = bounds.Y + (bounds.Height - lines.Length * (font.LineSpacing + hspace)) / 2 - 1 + i * (font.LineSpacing + hspace);
                else if (v == VerticalAligment.Bottom)
                    loc.Y = bounds.Y + bounds.Height - space - (lines.Length * (font.LineSpacing + hspace)) + i * (font.LineSpacing + hspace);

                this.DrawString(font, text, loc, c);
            }
        }

        public string WrapText(SpriteFont font, string text, float max)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float line = 0f;
            float space = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (line + size.X < max)
                {
                    sb.Append(word + " ");
                    line += size.X + space;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    line = size.X + space;
                }
            }

            return sb.ToString();
        }

        public Texture2D GetColorFill(Color color, int width = 1, int height = 1)
        {
            Texture2D t = new Texture2D(this.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            t.SetData(data);
            return t;
        }

        public void SetColorFill(ref Texture2D t, Color color)
        {
            Color[] data = new Color[t.Width * t.Height];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            t.SetData(data);
        }

        public Texture2D GetColorFill(System.Drawing.Color color, int width = 1, int height = 1)
        {
            Color c = new Color(color.R, color.G, color.B);
            return GetColorFill(c, width, height);
        }

        public void DrawLine(Color color, Vector2 start, Vector2 end)
        {
            Texture2D dot = this.GetColorFill(color);
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            this.Draw(dot, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1), null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void FillArea(VertexPositionTexture[] vertex, Texture2D texture)
        {
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;

            effect.Texture = texture;

            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex, 0, vertex.Length - 2);
        }

        public void FillArea(VertexPositionColorTexture[] vertex, Texture2D texture)
        {
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;

            effect.Texture = texture;

            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex, 0, vertex.Length - 2);
        }

        public void FillArea(VertexPositionColor[] vertex)
        {
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex, 0, vertex.Length - 2);
        }

        public void DrawLines(Vector2[] lines, Color c)
        {
            for (int i = 0; i < lines.Length - 1; i++)
                DrawLine(c, lines[i], lines[i + 1]);
        }

        public void DrawLine(VertexPositionColor[] vertex)
        {
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertex, 0, vertex.Length - 1);
        }

        public Texture2D Tint(Texture2D texture, Color tint, Color color = new Color())
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);                    

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int index = x + y * texture.Width;
                    if (colors[index].A != 0)
                        colors[index] = tint;
                    else
                        colors[index] = color;
                }
            }
            Texture2D tinted = new Texture2D(manager.GraphicsDevice, texture.Width, texture.Height);
            tinted.SetData(colors);
            return tinted;
        }
        
        public Texture2D Shadow(Texture2D texture, Color color, float a)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int index = x + y * texture.Width;
                    if (colors[index].A != 0)
                        colors[index] = Color.Lerp(colors[index], color, a);
                }
            }
            Texture2D nt = new Texture2D(manager.GraphicsDevice, texture.Width, texture.Height);
            nt.SetData(colors);
            return nt;
        }

        public void Clip (Texture2D s, ref Texture2D t, Rectangle destination, SpriteEffects effect = SpriteEffects.None)
        {
            Color[] source = new Color[s.Width * s.Height];
            s.GetData(source);

            Color[] colors = new Color[t.Width * t.Height];

            for (int x = destination.X; x < destination.X + t.Width; x++)
            {
                for (int y = destination.Y; y < destination.Y + t.Height; y++)
                {
                    int index = x + y * s.Width;
                    int px = x - destination.X;
                    if (effect == SpriteEffects.FlipHorizontally)
                        px = destination.Width - 1 - (x - destination.X);
                    int py = y - destination.Y;
                    if (effect == SpriteEffects.FlipVertically)
                        py = destination.Height - 1 - (y - destination.Y);
                    int pindex = px + py * t.Width;
                    colors[pindex] = source[index];
                }
            }

            t.SetData(colors);
        }

        public Texture2D GetTexture(System.Drawing.Bitmap bmp)
        {
            Texture2D tx = new Texture2D(manager.GraphicsDevice, bmp.Width, bmp.Height);
            Color[] data = new Color[tx.Width * tx.Height];

            BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            unsafe
            {
                for (int y = 0; y < bd.Height; y++)
                {
                    byte* row = (byte*)bd.Scan0 + y * bd.Stride;
                    for (int x = 0; x < bd.Width; x++)
                    {
                        int index = x + y * tx.Width;

                        byte r = row[2];
                        byte g = row[1];
                        byte b = row[0];

                        data[index] = new Color(r, g, b);

                        row += 4;
                    }
                }
            }

            bmp.UnlockBits(bd);

            tx.SetData(data);
            return tx;
        }

        public Texture2D Clone(Texture2D texture)
        {
            Texture2D t = new Texture2D(this.GraphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            t.SetData(data);
            return t;
        }
    }
}
