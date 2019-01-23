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
            this.TextColor = Color.CornflowerBlue;
            this.MouseOverTextColor = Color.Black;
            this.Fill = Color.Black;
            this.Frame = Color.CornflowerBlue;
            this.MouseOverFill = Color.CornflowerBlue;
        }

        protected override void load()
        {
            base.load();
        }
    }
}
