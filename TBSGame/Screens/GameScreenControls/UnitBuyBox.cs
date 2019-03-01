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
    public class UnitBuyBox : Control
    {
        public Unit Unit { get; private set; }

        private Panel panel = new Panel();
        private List<Label> labels = new List<Label>();
        public MenuButton Recruit { get; private set; }

        public UnitBuyBox(Unit unit)
        {
            this.Unit = unit;
            Recruit = new MenuButton(Resources.GetString("recruit"));
        }

        protected override void draw()
        {
            panel.Draw();
        }

        protected override void load()
        {
            for (int i = 0; i < 6; i++)
            {
                Label l = new Label("");
                l.Bounds = new Rectangle(0, i * 30, 300, 30);
                l.HAligment = HorizontalAligment.Left;
                labels.Add(l);
            }

            Recruit.Bounds = new Rectangle(10, bounds.Height - 60, 200, 50);

            Reload(Unit);

            panel.Foreground = Color.White;
            panel.Bounds = this.bounds;
            panel.Load(graphics, content, sprite);
            panel.AddRange(labels);
            panel.Add(Recruit);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
        }

        public void Reload(Unit unit)
        {
            if (unit != null)
            {
                IsVisible = true;
                Unit = unit;
                Recruit.Tag = Unit;

                string[] vals =
                {
                    $"{Resources.GetString("attack")}: {Unit.Attack}",
                    $"{Resources.GetString("armor")}: {Unit.Armor}", 
                    $"{Resources.GetString("piecearmor")}: {Unit.PieceArmor}",
                    $"{Resources.GetString("range")}: {Unit.MinRange}-{Unit.Range}",
                    $"{Resources.GetString("price")}: {Unit.Price}",
                    $"{Resources.GetString("unittype")}: {Resources.GetString(Unit.Type.ToString().Trim().ToLower())}"
                };

                for (int i = 0; i < labels.Count; i++)
                    labels[i].Text = vals[i];
            }
            else
                IsVisible = false;
        }
    }
}
