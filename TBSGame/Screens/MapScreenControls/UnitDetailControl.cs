using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class UnitDetailControl : MapControl
    {
        public Unit Unit { get; set; }
        private Texture2D bg, experience;

        public UnitDetailControl(Unit unit, int x, int y)
        {
            this.Unit = unit;

            this.X = x; 
            this.Y = y;
        }

        protected override void load()
        {
            if (driver[Unit.Texture] == null)
                driver.LoadUnit(Unit.Texture);

            if (driver["attack"] == null)
            {
                driver.LoadTexture("staminashadow", sprite.Shadow(driver["stamina"], Color.Black, 0.4f));
                driver.LoadTexture("healthshadow", sprite.Shadow(driver["health"], Color.Black, 0.4f));

                driver.LoadTexture("star", sprite.Tint(driver.LoadContent("icons/unitdetail/star"), Color.Gold));
                driver.LoadTexture("starshadow", sprite.Shadow(driver["star"], Color.Black, 0.5f));

                driver.Load("attack", "icons/unitdetail/attack");
                driver.Load("armor", "icons/unitdetail/armor");
                driver.Load("piecearmor", "icons/unitdetail/piecearmor");
                driver.Load("range", "icons/unitdetail/range");
            }

            bg = sprite.GetColorFill(Color.Black);
            experience = sprite.GetColorFill(Color.Gold);
        }

        public void Draw(Texture2D fill, Point p)
        {
            int padding = 10, space = 5, width = 275, height = 120;

            sprite.Draw(fill, new Rectangle(p.X, p.Y, width, height), Color.White);

            //pozadí ta jednotkou
            sprite.Draw(bg, new Rectangle(p.X + padding, p.Y + padding, height - 2 * padding, height - 2 * padding), Color.White);
            //náhled jednotky
            sprite.Draw(
                texture: driver[Unit.Texture],
                destinationRectangle: new Rectangle(p.X + padding + 5, p.Y + padding + 5, height - 2 * padding - 10, height - 2 * padding - 10),
                sourceRectangle: new Rectangle(0, 4 * driver[Unit.Texture].Height / 5, driver[Unit.Texture].Height / 5, driver[Unit.Texture].Height / 5),
                color: Color.White
                );

            int max = width - height - padding - space;
            //vypotřebovaná stamina
            sprite.Draw(driver["staminashadow"], new Rectangle(p.X + height, p.Y + padding + 2 * (15 + space), max, 15), Color.White);
            //stracené životy
            sprite.Draw(driver["healthshadow"], new Rectangle(p.X + height, p.Y + padding + 15 + space, max, 15), Color.White);

            //název jednotky
            sprite.DrawMultiLineText(font, new string[] { Resources.GetString(Unit.GetType().Name) }, new Rectangle(p.X + height, p.Y + padding, max, 15), HorizontalAligment.Left, VerticalAligment.Center, 2, Color.Black);

            //aktuální stamina
            double percent = (100 * Unit.Stamina) / Unit.MaxStamina;
            sprite.Draw(driver["stamina"], new Rectangle(p.X + height, p.Y + padding + 2 * (15 + space), (int)((max * percent) / 100), 15), Color.White);

            //aktuální počet životů
            percent = (100 * Unit.Health) / Unit.MaxHealth;
            sprite.Draw(driver["health"], new Rectangle(p.X + height, p.Y + padding + 15 + space, (int)((max * percent) / 100), 15), Color.White);

            //počet hvězd
            int sw = (height - 2 * padding) / 6;
            int level = Unit.GetLevel();
            for (int i = 0; i < 6; i++)
            {
                Rectangle des = new Rectangle(p.X + padding + i * sw + (int)(sw * 0.2), p.Y + height - padding - sw, (int)(sw * 0.8), (int)(sw * 0.8));
                sprite.Draw(driver[i < level ? "star" : "starshadow"], des, Color.White);
            }

            //zkušenosti na další úroveň
            double per_level = Unit.ExperiencePerLevel[level + 1] - Unit.ExperiencePerLevel[level];
            double epercent = (100 * (Unit.Experience - Unit.ExperiencePerLevel[level])) / per_level;
            int epl = (height - 2 * padding) - 2;

            sprite.Draw(bg, new Rectangle(p.X + padding, p.Y + height - 9, epl + 2, 7), Color.White);
            sprite.Draw(this.experience, new Rectangle(p.X + padding + 1, p.Y + height - 8, (int)((epercent * epl) / 100), 5), Color.White);

            //vlastnoti jednotky
            string attack = $"{Unit.Attack - Unit.UpgradeBonuses[0]}+{Unit.UpgradeBonuses[0]}";
            string piecearmor = $"{Unit.PieceArmor - Unit.UpgradeBonuses[2]}+{Unit.UpgradeBonuses[2]}";
            string armor = $"{Unit.Armor - Unit.UpgradeBonuses[1]}+{Unit.UpgradeBonuses[1]}";
            string range = $"{Unit.MinRange}-{Unit.Range}";

            sprite.Draw(driver["attack"], new Rectangle(p.X + height, p.Y + padding + 3 * (15 + space), 25, 25), Color.White);
            sprite.DrawMultiLineText(font, new string[] { attack }, new Rectangle(p.X + height + 25, p.Y + padding + 3 * (15 + space), 50, 25), HorizontalAligment.Left, VerticalAligment.Center, 0, Color.Black);

            sprite.Draw(driver["piecearmor"], new Rectangle(p.X + height, p.Y + padding + 4 * (15 + space) + 4, 25, 25), Color.White);
            sprite.DrawMultiLineText(font, new string[] { piecearmor }, new Rectangle(p.X + height + 25, p.Y + padding + 4 * (15 + space) + 4, 50, 25), HorizontalAligment.Left, VerticalAligment.Center, 0, Color.Black);

            sprite.Draw(driver["armor"], new Rectangle(p.X + height + max / 2, p.Y + padding + 3 * (15 + space), 25, 25), Color.White);
            sprite.DrawMultiLineText(font, new string[] { armor }, new Rectangle(p.X + height + max / 2 + 25, p.Y + padding + 3 * (15 + space), 50, 25), HorizontalAligment.Left, VerticalAligment.Center, 0, Color.Black);

            sprite.Draw(driver["range"], new Rectangle(p.X + height + max / 2, p.Y + padding + 4 * (15 + space) + 4, 25, 25), Color.White);
            sprite.DrawMultiLineText(font, new string[] { range }, new Rectangle(p.X + height + max / 2 + 25, p.Y + padding + 4 * (15 + space) + 4, 50, 25), HorizontalAligment.Left, VerticalAligment.Center, 0, Color.Black);

            //počet útoků
            int amax = (int)Math.Floor((double)Unit.MaxStamina / Unit.StaminaPerAttack);
            int acount = (int)Math.Floor((double)Unit.Stamina / Unit.StaminaPerAttack);
            for (int i = 0; i < amax; i++)
            {
                Rectangle des = new Rectangle(p.X + padding + 3, p.Y + padding + 3 + i * 10, 9, 9);
                sprite.Draw(driver[i >= acount ? "healthshadow" : "health"], des, Color.White);
            }
        }
    }
}
