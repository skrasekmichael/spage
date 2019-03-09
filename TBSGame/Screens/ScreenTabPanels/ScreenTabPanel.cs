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
using TBSGame.Controls;

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

        protected ContentManager content => graphics.Content;
        protected CustomSpriteBatch sprite => graphics.Sprite;
        protected Graphics graphics { get; private set; }
        protected GameButton button;
        protected Settings settings;
        protected GameSave game;

        protected Panel panel = new Panel(true);

        public int Width => graphics.ScreenWidth;
        public int Height => graphics.ScreenHeight;

        public SpriteFont Font { get; set; }
        public int Index { get; set; }

        public ScreenTabPanel(Settings settings, GameSave game, string icon)
        {
            button = new GameButton(icon, icon);
            button.Tint = Color.Lime;
            this.game = game;
            this.settings = settings;
        }

        public void Load(Graphics graphics)
        {
            this.graphics = graphics;

            panel.Bounds = new Rectangle(0, 0, Width - 100, Height);
            panel.Load(graphics);

            button.Load(graphics);
            button.Bounds = new Rectangle(Width - 100, 100 * Index, 100, 100);
            button.OnControlClicked += new ControlClickedEventHandler(sender => SelectTab());

            load();
        }

        protected abstract void load();

        public abstract void LoadPosition();

        public void UpdateButton(GameTime time, KeyboardState keyboard, MouseState mouse) => button.Update(time, keyboard, mouse);

        public void Update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
            update(time, keyboard, mouse);
        }

        protected abstract void update(GameTime time, KeyboardState keyboard, MouseState mouse);

        public void DrawButton() => button.Draw();

        public void Draw()
        {
            panel.Draw();
            draw();
        }

        protected abstract void draw();

        public abstract void Reload();
    }
}
