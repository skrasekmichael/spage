using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Controls.GameScreen
{
    public class UnitBox : Panel
    {
        public Unit Unit { get; set; }
        public bool DisplayLevel { get; set; } = true;
        public bool DisplayHP { get; set; } = true;
        public bool DisplayIcon { get; set; } = true;

        public CheckBox Check;
        private Label label, rounds;
        private ImagePanel icon;

        private List<Texture2D> textures = new List<Texture2D>();

        public bool IsChecked
        {
            get => Check.IsChecked;
            set => Check.IsChecked = value;
        }

        public Color Checked
        {
            get => Check.Checked;
            set => Check.Checked = value;
        }

        public Color UnChecked
        {
            get => Check.UnChecked;
            set => Check.UnChecked = value;
        }

        public UnitBox(Unit unit)
        {
            Unit = unit;
            Check = new CheckBox(Resources.GetString(Unit.ToString()));
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            icon.Image = textures[(int)Unit.UnitStatus];
            Check.Text = Resources.GetString(Unit.ToString());
            label.Text = get_level();
            rounds.Text = Unit.Rounds == 0 ? "" : Unit.Rounds.ToString();

            icon.IsVisible = DisplayIcon;
            label.IsVisible = DisplayLevel;

            base.update(time, keyboard, mouse);
            if (this.IsMouseOver)
                this.DeepForeground = Color.White;
        }

        protected override void load()
        {
            Border.IsVisible = false;
            Checked = Color.Crimson;
            UnChecked = new Color(100, 100, 100);
            this.Fill = new Color(40, 40, 40);

            load_icons();

            Check.Bounds = new Rectangle(0, 0, this.bounds.Width, this.bounds.Height);
            Check.IsChecked = false;
            Check.HAligment = HorizontalAligment.Left;

            label = new Label(get_level());
            label.Bounds = new Rectangle(this.bounds.Width - 130 - this.bounds.Height, 0, 90, this.bounds.Height);
            label.Foreground = Color.Silver;
            label.HAligment = HorizontalAligment.Left;

            icon = new ImagePanel(textures[(int)Unit.UnitStatus]);
            icon.Bounds = new Rectangle(this.bounds.Width - this.bounds.Height, 0, this.bounds.Height, this.bounds.Height);
            icon.VAligment = VerticalAligment.Center;
            icon.HAligment = HorizontalAligment.Center;

            rounds = new Label(Unit.Rounds == 0 ? "" : Unit.Rounds.ToString());
            rounds.Bounds = new Rectangle(icon.Bounds.Left - 40, 0, 40, this.bounds.Height);
            rounds.Foreground = Color.White;
            
            this.AddRange(new Control[] { Check, label, icon, rounds });
            base.load();
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
                    textures.Add(new Texture2D(graphics.GraphicsDevice, 1, 1));
                else
                    textures.Add(sprite.Tint(graphics.Content.Load<Texture2D>("icons/" + name), Color.Crimson));
            }
        }
    }
}
