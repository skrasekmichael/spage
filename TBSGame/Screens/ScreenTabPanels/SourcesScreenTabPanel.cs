using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SourcesScreenTabPanel : ScreenTabPanel
    {
        public SourcesScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            button.Tint = Color.Orange;
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
