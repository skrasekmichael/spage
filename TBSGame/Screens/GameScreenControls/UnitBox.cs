using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens.GameScreenControls
{
    public class UnitBox : Control
    {
        public Unit Unit { get; private set; }

        private Panel panel = new Panel(true);
        private CheckBox check;

        public bool IsChecked
        {
            get => check.IsChecked;
            set => check.IsChecked = value;
        }

        public Color Checked
        {
            get => check.Checked;
            set => check.Checked = value;
        }

        public Color UnChecked
        {
            get => check.UnChecked;
            set => check.UnChecked = value;
        }

        public UnitBox(Unit unit)
        {
            this.Unit = unit;
            check = new CheckBox(Resources.GetString(Unit.ToString()));
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
        }

        protected override void draw()
        {
            panel.Draw();
        }

        protected override void load()
        {
            check.Bounds = new Rectangle(0, 0, this.bounds.Width - 120, this.bounds.Height);
            check.IsChecked = false;

            int index = Unit.GetLevel();
            int exp = Unit.Experience - Unit.ExperiencePerLevel[index];
            double level = 6;
            if (index < 5)
                level = index + exp / Math.Abs((double)Unit.ExperiencePerLevel[index] - (double)Unit.ExperiencePerLevel[index + 1]);

            level = Math.Floor(level * 100) / 100;

            Label label = new Label(level.ToString() + "/6*");
            label.Bounds = new Rectangle(check.Bounds.Width, 0, 120, this.bounds.Height);
            label.TextColor = Color.Silver;
            label.HAligment = HorizontalAligment.Left;

            panel.Bounds = this.bounds;
            panel.Load(graphics, content, sprite);
            panel.AddRange(new Control[] { check, label });
        }
    }
}
