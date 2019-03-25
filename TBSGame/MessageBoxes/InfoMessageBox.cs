using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.MessageBoxes
{
    public class InfoMessageBox : MessageBox
    {
        private Label text;

        public InfoMessageBox(string message)
        {
            text = new Label(message);
        }

        protected override void load()
        {
            MenuButton ok = new MenuButton(Resources.GetString("ok"));
            ok.Bounds = new Rectangle((Size.X - 100) / 2, Size.Y - 60, 100, 50);

            buttons.Add(ok, DialogResult.No);

            text.Bounds = new Rectangle(0, 0, Size.X, Size.Y - 60);
            text.HAligment = HorizontalAligment.Center;
            text.VAligment = VerticalAligment.Center;
            text.Load(graphics);
            text.Foreground = Color.White;

            panel.Add(text);
        }
    }
}
