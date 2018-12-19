using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls.Buttons;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Screens.ScreenTabPanels
{
    public abstract class ScreenTabPanel
    {
        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected GameButton button;

        public int Width => graphics.PreferredBackBufferWidth;
        public int Height => graphics.PreferredBackBufferHeight;

        public SpriteFont Font { get; set; }
        public bool Selected { get; set; } = false;
        public int Index { get; private set; }

        public ScreenTabPanel(string icon, int index)
        {
            button = new GameButton(icon, icon);
            Index = index;
        }

        public void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;
            this.Font = content.Load<SpriteFont>("fonts/text");

            button.Load(graphics, content, sprite);
            button.Bounds = new Rectangle(graphics.PreferredBackBufferWidth - 100, 100 * Index, 100, 100);

            load();
        }

        protected abstract void load();

        public void Update(GameTime time, KeyboardState keybord, MouseState mouse)
        {
            button.Update(time, keybord, mouse);
            update(time, keybord, mouse);
        }

        protected abstract void update(GameTime time, KeyboardState keybord, MouseState mouse);

        public void Draw()
        {
            button.Draw();
            draw();
        }

        protected abstract void draw();
    }
}
