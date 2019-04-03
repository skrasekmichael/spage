using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using TBSGame.Controls.GameScreen;
using TBSGame.Controls.Special;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class UpgradesScreenTabPanel : ScreenTabPanel
    {
        private UnitsPanel units_panel;

        public UpgradesScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {

        }

        public override void Reload()
        {
            units_panel.LoadUnits(game.Units);
        }

        protected override void load()
        {
            units_panel = (UnitsPanel)Panel.GetControl("units");

            units_panel.LoadUnits(game.Units);
        }
    }
}
