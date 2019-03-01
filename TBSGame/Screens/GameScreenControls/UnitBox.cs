using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens.GameScreenControls
{
    public class UnitBox : Control
    {
        public Unit Unit { get; set; }

        private Panel panel = new Panel(true);
        private CheckBox check;
        private Label label, rounds;
        private ImagePanel icon;

        private List<Texture2D> textures = new List<Texture2D>();

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
            Unit = unit;
            check = new CheckBox(Resources.GetString(Unit.ToString()));
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            icon.Image = textures[(int)Unit.UnitStatus];
            check.Text = Resources.GetString(Unit.ToString());
            label.Text = get_level();
            rounds.Text = Unit.Rounds == 0 ? "" : Unit.Rounds.ToString();

            check.Bounds = new Rectangle(0, 0, this.bounds.Width - 130 - this.bounds.Height, this.bounds.Height);
            label.Bounds = new Rectangle(check.Bounds.Width, 0, 90, this.bounds.Height);
            icon.Bounds = new Rectangle(this.bounds.Width - this.bounds.Height, 0, this.bounds.Height, this.bounds.Height);
            rounds.Bounds = new Rectangle(icon.Bounds.Left - 40, 0, 40, this.bounds.Height);
            panel.Bounds = this.bounds;

            panel.Update(time, keyboard, mouse);
        }

        protected override void draw()
        {
            panel.Draw();
        }

        protected override void load()
        {
            load_icons();

            check.Bounds = new Rectangle(0, 0, this.bounds.Width - 130 - this.bounds.Height, this.bounds.Height);
            check.IsChecked = false;
            check.Label.HAligment = HorizontalAligment.Left;

            label = new Label(get_level());
            label.Bounds = new Rectangle(check.Bounds.Width, 0, 90, this.bounds.Height);
            label.Foreground = Color.Silver;
            label.HAligment = HorizontalAligment.Left;

            icon = new ImagePanel(textures[(int)Unit.UnitStatus]);
            icon.Bounds = new Rectangle(this.bounds.Width - this.bounds.Height, 0, this.bounds.Height, this.bounds.Height);
            icon.VAligment = VerticalAligment.Center;
            icon.HAligment = HorizontalAligment.Center;

            rounds = new Label(Unit.Rounds == 0 ? "" : Unit.Rounds.ToString());
            rounds.Bounds = new Rectangle(icon.Bounds.Left - 40, 0, 40, this.bounds.Height);
            rounds.Foreground = Color.White;

            panel.Bounds = this.bounds;
            panel.Load(graphics, content, sprite);
            panel.AddRange(new Control[] { check, label, icon, rounds });
           /* panel.Controls.ForEach(c =>
            {
                c.Border.IsVisible = true;
                c.Border.Color = Color.Yellow;
            });*/
        }

        private string get_level()
        {
            int index = Unit.GetLevel();
            int exp = Unit.Experience - Unit.ExperiencePerLevel[index];
            double level = 6;
            if (index < 5)
                level = index + exp / Math.Abs((double)Unit.ExperiencePerLevel[index] - (double)Unit.ExperiencePerLevel[index + 1]);
            return level.ToString("0.0") + "/6";
        }

        private void load_icons()
        {
            string icon_name(UnitStatus status)
            {
                switch (status)
                {
                    case UnitStatus.Healing:
                        return "heal";
                    case UnitStatus.OnWay:
                        return "foot";
                    case UnitStatus.Training:
                        return "unitdetail/attack";
                    case UnitStatus.Upgrading:
                        return "upgrade";
                    default:
                        return null;
                }
            }

            foreach (UnitStatus val in Enum.GetValues(typeof(UnitStatus)).Cast<UnitStatus>().OrderBy(s => (int)s))
            {
                string name = icon_name(val);
                if (name == null)
                    textures.Add(new Texture2D(graphics.GraphicsDevice, 10, 10));
                else
                    textures.Add(sprite.Tint(content.Load<Texture2D>("icons/" + name), Color.Crimson));
            }
        }
    }
}
