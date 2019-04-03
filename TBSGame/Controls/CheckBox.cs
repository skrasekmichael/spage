using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls.Buttons;

namespace TBSGame.Controls
{
    public class CheckBox : MenuButton
    {
        public bool IsChecked { get; set; } = false;
        public Color Checked { get; set; } = Color.Lime;
        public Color UnChecked { get; set; } = Color.DarkGreen;
        public ControlClickedEventHandler Handler { get; set; }

        public CheckBox(string text) : base(text)
        {
            Border.Width = 0;
            Fill = Color.Transparent;
            MouseOverForeground = Color.White;
            MouseOverFill = Color.Transparent;
            Handler = sender => IsChecked = !IsChecked;

            OnControlClicked += Handler;
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Foreground = IsChecked ? Checked : UnChecked;
            base.update(time, keyboard, mouse);
        }
    }
}
