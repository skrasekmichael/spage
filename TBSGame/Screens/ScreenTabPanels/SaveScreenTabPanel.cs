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
        public GameSavePanel Saves { get; private set; }
        
        public SaveScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            Saves = new GameSavePanel(settings.GameSaves, GameSavePanel.Display.LoadSave);
        }

        protected override void draw()
        {
            Saves.Draw();
        }

        protected override void load()
        {
            Saves.Load(graphics, content, sprite);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Saves.Update(time, keyboard, mouse);
        }

        public override void LoadPosition()
        {
            Saves.LoadPostiton();
        }
    }
}
