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
    public class CheckBox : Control
    {
        public override event ControlClickedEventHandler OnControlClicked;

        public MenuButton Label;

        public bool IsChecked { get; set; } = false;
        public Color Checked { get; set; } = Color.Lime;
        public Color UnChecked { get; set; } = Color.DarkGreen;

        public string Text
        {
            get => Label.Title;
            set => Label.Title = value;
        }

        public CheckBox(string text)
        {
            Label = new MenuButton(text);
            
            Label.FrameWidth = 0;
            Label.MouseOverTextColor = Color.White;
            Label.Fill = Color.Transparent;
            Label.MouseOverFill = Color.Transparent;

            Label.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                IsChecked = !IsChecked;
                OnControlClicked?.Invoke(this);
            });
        }

        protected override void draw()
        {
            Label.Draw();
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Label.Bounds = this.bounds;
            Label.TextColor = IsChecked ? Checked : UnChecked;
            Label.Update(time, keyboard, mouse);
        }

        protected override void load()
        {
            Label.Load(graphics, content, sprite);
            Label.Bounds = this.bounds;
        }
    }
}
