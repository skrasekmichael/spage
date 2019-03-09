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
    public class Graphics
    {
        public GraphicsDevice GraphicsDevice => GraphicsDeviceManager.GraphicsDevice;

        public TextureDriver TextureDriver { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public ContentManager Content { get; private set; }
        public CustomSpriteBatch Sprite { get; private set; }

        public SpriteFont Normal { get; set; }
        public SpriteFont Small { get; set; }

        public int ScreenWidth => 1920;
        public int ScreenHeight => 1080;

        public void Load(GraphicsDeviceManager manager, ContentManager content, CustomSpriteBatch sprite, TextureDriver texture)
        {
            GraphicsDeviceManager = manager;
            Content = content;
            TextureDriver = texture;
            Sprite = sprite;

            Normal = content.Load<SpriteFont>("fonts/text");
            Small = content.Load<SpriteFont>("fonts/small");
        }
    }
}
