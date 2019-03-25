using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Controls.TextBoxes;
using TBSGame.MessageBoxes;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SaveScreenTabPanel : ScreenTabPanel
    {
        public GameSavePanel Saves { get; private set; }
        public MenuButton Exit { get; private set; }


        public SaveScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {

        }

        protected override void load()
        {
            Saves = (GameSavePanel)Panel.GetControl("saves");
            Exit = (MenuButton)Panel.GetControl("exit");
        }

        public override void LoadPosition()
        {
            Saves.LoadPostiton();
        }
    }
}
