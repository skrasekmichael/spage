using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame
{
    public class FPSCounter
    {
        private TimeSpan start_span = TimeSpan.Zero;
        private SpriteFont font;
        private int count = 0;

        public int Count { get; private set; } = 0;

        public void Load(ContentManager content)
        {
            font = content.Load<SpriteFont>("fonts/text");
        }

        public void Update(GameTime time)
        {
            if (start_span == TimeSpan.Zero)
                start_span = time.TotalGameTime;

            if (time.TotalGameTime - start_span >= TimeSpan.FromSeconds(1))
            {
                start_span = time.TotalGameTime;
                Count = count;
                count = 0;
            }

            count++;
        }

        public void Draw(CustomSpriteBatch sprite)
        {
            sprite.DrawString(font, Count.ToString() + " FPS", new Vector2(0, 0), Color.Red);
        }
    }
}
