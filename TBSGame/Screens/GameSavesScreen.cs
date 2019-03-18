using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using TBSGame.Controls;
using TBSGame.MessageBoxes;
using TBSGame.Saver;

namespace TBSGame.Screens
{
    public class GameSavesScreen : Screen
    {
        private GameSavePanel saves;

        protected override void load()
        {
            parent.GetControl("back").OnControlClicked += new ControlClickedEventHandler(sender => this.Dispose(new MainMenuScreen()));
            saves = (GameSavePanel)parent.GetControl("saves");
            saves.OnLoadGame += new LoadGameEventHandler((sender, i) =>
             {
                 GameSave game = GameSave.Load(Settings.GameSaves + i.ToString() + ".dat");
                 Scenario.Load("scenario\\campaign.dat", game.Scenario + "campaign\\");
                 if (game != null)
                 {
                     if (game.Map == null)
                         this.Dispose(new GameScreen(game));
                     else
                         this.Dispose(new MapScreen(game, Settings, game.Map, null, null));
                 }
             });
            saves.OnDeleteGame += new DeleteGameEventHandler((sender, index) =>
            {
                YesNoMessageBox message = new YesNoMessageBox(Resources.GetString("delete?"));
                message.OnMessageBox += new MessageBoxEventHandler((obj, result) =>
                {
                    if (result == DialogResult.Yes)
                        saves.Delete(index);
                });
                this.ShowMessage(message);
            });
        }

        protected override void loadpos()
        {
            saves.LoadPostiton();
        }
    }
}
