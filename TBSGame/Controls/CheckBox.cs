using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Controls.Buttons
{
    public class CheckBox : Control
    {
        private MenuButton label;

        public bool IsChecked { get; set; } = false;
        public Color Checked { get; set; } = Color.Lime;
        public Color UnChecked { get; set; } = Color.DarkGreen;

        public CheckBox(string text)
        {
            label = new MenuButton(text);

            label.FrameWidth = 0;
            label.MouseOverTextColor = Color.White;
            label.Fill = Color.Transparent;
            label.MouseOverFill = Color.Transparent;

            label.OnButtonClicked += new ButtonClickedEventHandler(sendr =>
            {
                IsChecked = !IsChecked;
            });
        }

        protected override void draw()
        {
            label.Draw();
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            label.TextColor = IsChecked ? Checked : UnChecked;
            label.Update(time, keyboard, mouse);
        }

        protected override void load()
        {
            label.Load(graphics, content, sprite);
            label.Bounds = this.Bounds;
        }
    }
}
