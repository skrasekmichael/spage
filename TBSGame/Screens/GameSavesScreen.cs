using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Saver;

namespace TBSGame.Screens
{
    public class GameSavesScreen : Screen
    {
        private GameSavePanel saves;
        private MenuButton back;

        public GameSavesScreen()
        {
            back = new MenuButton(Resources.GetString("back"));
            back.OnButtonClicked += new ButtonClickedEventHandler(sender => this.Dispose(new PlayCampaignScreen()));
        }

        protected override void draw()
        {
            saves.Draw();
            back.Draw();
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            back.Load(graphics, content, sprite);

            saves = new GameSavePanel(Settings.GameSaves, GameSavePanel.Display.Load);
            saves.Load(graphics, content, sprite);
            saves.OnLoadGame += new LoadGameEventHandler((sender, i) =>
             {
                 GameSave game = GameSave.Load(Settings.GameSaves + i.ToString() + ".dat");
                 Scenario.Load("scenario\\campaign.dat", game.Scenario + "campaign\\");
                 if (game != null)
                     this.Dispose(new GameScreen(game));
             });
            saves.LoadPostiton();
        }

        protected override void loadpos()
        {
            saves?.LoadPostiton();
            back.Bounds = new Rectangle(Width - 210, Height - 55, 200, 50);
        }

        protected override void update(GameTime time)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            saves.Update(time, keyboard, mouse);
            back.Update(time, keyboard, mouse);
        }
    }
}
