using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Controls.Buttons
{
    public class TextButton : Button
    {
        public TextButton(string title) : base(title)
        {
            this.TextColor = Color.White;
            this.MouseOverTextColor = Color.Orange;
            this.Fill = Color.Black;
            this.Frame = Color.Transparent;
            this.MouseOverFill = Color.Black;
        }
    }
}
