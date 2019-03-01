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
            Foreground = Color.White;
            MouseOverForeground = Color.Orange;
            Fill = Color.Black;
            MouseOverFill = Color.Black;
        }
    }
}
