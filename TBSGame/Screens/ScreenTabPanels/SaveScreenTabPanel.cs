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
using TBSGame.Controls.Special;
using TBSGame.Controls.TextBoxes;
using TBSGame.MessageBoxes;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SaveScreenTabPanel : ScreenTabPanel
    {
        [LayoutControl("saves")] public GameSavePanel Saves;
        [LayoutControl("exit")] public MenuButton Exit;

        public SaveScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon) { }

        protected override void load() { }

        public override void LoadPosition()
        {
            Saves.LoadPostiton();
        }
    }
}
