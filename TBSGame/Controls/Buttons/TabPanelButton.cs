using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Controls.Buttons
{
    public class TabPanelButton : Button
    {
        public TabPanelButton(string title) : base(title)
        {
            Frame = Color.Silver;
            MouseOverFrame = Color.Orange;
            Fill = Color.Black;
            MouseOverFill = Color.Black;
            TextColor = Color.White;
            MouseOverTextColor = Color.Orange;
        }

        protected override void load()
        {
            base.load();
        }
    }
}
