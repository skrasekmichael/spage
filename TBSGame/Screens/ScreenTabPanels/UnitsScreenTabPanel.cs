using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class UnitsScreenTabPanel : ScreenTabPanel
    {
        public UnitsScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            button.Tint = Color.Goldenrod;
        }

        public override void LoadPosition()
        {

        }

        protected override void draw()
        {

        }

        protected override void load()
        {

        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {

        }
    }
}
