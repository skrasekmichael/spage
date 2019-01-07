using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.TextBoxes;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SaveScreenTabPanel : ScreenTabPanel
    {
        public event SaveGameEventHandler OnSaveGame;
        public event LoadGameEventHandler OnLoadGame;

        private GameSavePanel saves;
        
        public SaveScreenTabPanel(Settings settings, string icon) : base(settings, icon)
        {
            saves = new GameSavePanel(settings.GameSaves, GameSavePanel.Display.LoadSave);
            saves.OnSaveGame += new SaveGameEventHandler((sender, index, name) => OnSaveGame?.Invoke(this, index, name));
            saves.OnLoadGame += new LoadGameEventHandler((sender, index) => OnLoadGame(this, index));
        }

        protected override void draw()
        {
            saves.Draw();
        }

        protected override void load()
        {
            saves.Load(graphics, content, sprite);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            saves.Update(time, keyboard, mouse);
        }

        public override void LoadPosition()
        {
            saves.LoadPostiton();
        }
    }
}
