using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Saver;

namespace TBSGame.Screens
{
    public class MainMenuScreen : Screen
    {
        private List<Button> buttons = new List<Button>();
        private ImagePanel background;

        public MainMenuScreen() : base()
        {
            buttons.AddRange(new Button[] {
                new MenuButton(Resources.GetString("campaign")),
                new MenuButton(Resources.GetString("custom_scenario")) { IsLocked = true },
                new MenuButton(Resources.GetString("loadgame")),
                new MenuButton(Resources.GetString("about")),
                new MenuButton(Resources.GetString("settings")),
                new MenuButton(Resources.GetString("exit"))
            });

            ControlClickedEventHandler[] handlers = new ControlClickedEventHandler[] {
                sender => {
                    Scenario scenario = Scenario.Load("scenario\\campaign.dat", Settings.Scenario + "campaign\\");
                    GameSave game = new GameSave(scenario, Settings);
                    Dispose(new GameScreen(game));
                },
                sender => Dispose(null),
                sender => Dispose(new GameSavesScreen()),
                sender => Dispose(new AboutScreen()),
                sender => Dispose(new SettingsScreen()),
                sender => Dispose(null)
            };

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].OnControlClicked += new ControlClickedEventHandler(handlers[i]);
                buttons[i].Opacity = 0.8f;
            }
        }

        protected override void draw()
        {
            background.Draw();
            buttons.ForEach(btn => btn.Draw());
        }

        protected override void load()
        {
            buttons.ForEach(btn => btn.Load(graphics));
            background = new ImagePanel(content.Load<Texture2D>("background"));
            background.HAligment = HorizontalAligment.Center;
            background.VAligment = VerticalAligment.Center;
            background.MinBounds = new Rectangle(0, 0, Width, Height);
            background.Load(graphics);
        }

        protected override void loadpos()
        {
            int h = 51;
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Bounds = new Rectangle((Width - 300) / 2, (Height - h - buttons.Count * h) / 2 + i * h, 300, h - 1);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            buttons.ForEach(btn => btn.Update(time, keyboard, mouse));
            background.Update(time, keyboard, mouse);
        }
    }
}
