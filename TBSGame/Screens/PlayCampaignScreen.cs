using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Saver;

namespace TBSGame.Screens
{
    public class PlayCampaignScreen : Screen
    {
        private List<Button> buttons = new List<Button>();

        public PlayCampaignScreen() : base()
        {
            buttons.AddRange(new Button[] {
                new MenuButton(Resources.GetString("newgame")),
                new MenuButton(Resources.GetString("loadgame")),
                new MenuButton(Resources.GetString("back"))
            });

            ButtonClickedEventHandler[] handlers = new ButtonClickedEventHandler[] {
                sender => {
                    Scenario scenario = Scenario.Load("scenario\\campaign.dat", Settings.Scenario + "campaign\\");
                    GameSave game = new GameSave(scenario, Settings);
                    Dispose(new GameScreen(game));
                },
                sender => Dispose(new GameSavesScreen()),
                sender => Dispose(new MainMenuScreen())
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
