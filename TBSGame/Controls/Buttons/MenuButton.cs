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
            this.TextColor = Color.White;
            this.MouseOverTextColor = new Color(30, 30, 30);
            this.Fill = new Color(30, 30, 30);
            this.Frame = Color.Transparent;
            this.MouseOverFrame = Color.Transparent;
            this.MouseOverFill = new Color(Color.Crimson, 0.8f);
        }

        protected override void load()
        {
            base.load();
        }
    }
}
