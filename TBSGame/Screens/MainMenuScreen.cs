using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens
{
    public class MainMenuScreen : Screen
    {
        private List<Button> buttons = new List<Button>();

        public MainMenuScreen() : base()
        {
            buttons.AddRange(new Button[] {
                new MenuButton(Resources.GetString("campaign")),
                new MenuButton($"{Resources.GetString("custom")} {Resources.GetString("scenario")}"),
                new MenuButton(Resources.GetString("about")),
                new MenuButton(Resources.GetString("settings")),
                new MenuButton(Resources.GetString("exit"))
            });

            ButtonClickedEventHandler[] handlers = new ButtonClickedEventHandler[] {
                sender => Dispose(new GameScreen("levels/level1/level1.dat")),
                sender => Dispose(null),
                sender => Dispose(null),
                sender => Dispose(new SettingsScreen()),
                sender => Dispose(null)
            };

            for (int i = 0; i < buttons.Count; i++)
                buttons[i].OnButtonClicked += new ButtonClickedEventHandler(handlers[i]);
        }

        protected override void draw()
        {
            buttons.ForEach(btn => btn.Draw());
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            buttons.ForEach(btn => btn.Load(graphics, content, sprite));
        }

        protected override void loadpos()
        {
            int h = 50;
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Bounds = new Rectangle((Width - 300) / 2, (Height - h - buttons.Count * h) / 2 + i * h, 300, h - 1);
        }

        protected override void update(GameTime time)
        {
            buttons.ForEach(btn => btn.Update(time));
        }
    }
}
