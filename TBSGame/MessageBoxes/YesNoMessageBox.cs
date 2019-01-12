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
    public class YesNoMessageBox : MessageBox
    {
        private Label text;

        public YesNoMessageBox(string message)
        {
            text = new Label(message);
        }

        protected override void load()
        {
            MenuButton yes = new MenuButton(Resources.GetString("yes"));
            yes.Bounds = new Rectangle((Size.X - (2 * 100 + 5)) / 2, Size.Y - 60, 100, 50);
            MenuButton no = new MenuButton(Resources.GetString("no"));
            no.Bounds = new Rectangle(yes.Bounds.X + 105, Size.Y - 60, 100, 50);

            buttons.Add(yes, DialogResult.Yes);
            buttons.Add(no, DialogResult.No);

            text.Bounds = new Rectangle(0, 0, Size.X, Size.Y - 60);
            text.HAligment = HorizontalAligment.Center;
            text.VAligment = VerticalAligment.Center;
            text.Load(graphics, content, sprite);
            text.TextColor = Color.White;

            panel.Add(text);
        }
    }
}
