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
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public delegate void SelectedTabEventHandler(object sender);

    public abstract class ScreenTabPanel
    {
        public event SelectedTabEventHandler OnSelectedTab;
        private void SelectTab()
        {
            OnSelectedTab?.Invoke(this);
        }

        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected GameButton button;
        protected Settings settings;
        protected GameSave game;

        public int Width => graphics.PreferredBackBufferWidth;
        public int Height => graphics.PreferredBackBufferHeight;

        public SpriteFont Font { get; set; }
        public int Index { get; set; }

        public ScreenTabPanel(Settings settings, GameSave game, string icon)
        {
            button = new GameButton(icon, icon);
            this.game = game;
            this.settings = settings;
        }

        public void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;
            this.Font = content.Load<SpriteFont>("fonts/text");

            button.Load(graphics, content, sprite);
            button.Bounds = new Rectangle(graphics.PreferredBackBufferWidth - 100, 100 * Index, 100, 100);
            button.OnButtonClicked += new Controls.ButtonClickedEventHandler(sender => SelectTab());

            load();
        }

        protected abstract void load();

        public abstract void LoadPosition();

        public void UpdateButton(GameTime time, KeyboardState keyboard, MouseState mouse) => button.Update(time, keyboard, mouse);
        
        public void Update(GameTime time, KeyboardState keyboard, MouseState mouse) => update(time, keyboard, mouse);

        protected abstract void update(GameTime time, KeyboardState keyboard, MouseState mouse);

        public void DrawButton() => button.Draw();

        public void Draw() => draw();

        protected abstract void draw();
    }
}
