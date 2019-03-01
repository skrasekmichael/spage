using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Controls.Buttons
{
    public class MenuButton : Button
    {
        public MenuButton(string title) : base(title)
        {
            Foreground = Color.White;
            MouseOverForeground = new Color(30, 30, 30);
            Fill = new Color(30, 30, 30);
            MouseOverFill = new Color(Color.Crimson, 0.8f);
        }

        protected override void load()
        {
            base.load();
        }
    }
}
