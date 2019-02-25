using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rect = System.Drawing.RectangleF;

namespace TBSGame.Screens.MapScreenControls
{
    public class UnitInfoControl : MapControl
    {
        public Unit Unit { get; set; }
        private Vector2 loc;
        private Texture2D bg;

        public UnitInfoControl(Unit unit, int x, int y)
        {
            this.Unit = unit;

            this.X = x;
            this.Y = y;
        }

        protected override void load()
        {
            bg = sprite.GetColorFill(new Color(0, 0, 0, 0.5f));
            if (driver["health"] == null)
            {
                driver.LoadTexture("health", sprite.GetColorFill(Color.Red));
                driver.LoadTexture("health2", sprite.GetColorFill(Color.GreenYellow));
                driver.LoadTexture("stamina", sprite.GetColorFill(Color.CornflowerBlue));
            }
        }

        public void Update(Engine engine)
        {
            System.Drawing.PointF loc = engine.GetCenter(X, Y);
            this.loc = new Vector2(loc.X, loc.Y);
        }

        public void Draw(double height)
        {
            float w = 70, h = 30, space = 2;
            float x = (float)(loc.X - w / 2);
            float y = (float)(loc.Y + height + 5);

            draw(bg, new Rect(x, y, w, h));

            //aktuální počet životů
            double hp = (100 * Unit.Health) / Unit.MaxHealth;

            if (Unit.Player == 1)
            {
                double stamina = (100 * Unit.Stamina) / Unit.MaxStamina;

                draw(driver["stamina"], new Rect(x + space, y - space, (float)(((w - 2 * space) * stamina) / 100), 5));
                draw(driver["health2"], new Rect(x + space, y - 2 * space - 5, (float)(((w - 2 * space) * hp) / 100), 5));

                //počet útoků
                int acount = (int)Math.Floor((double)Unit.Stamina / Unit.StaminaPerAttack);
                for (int i = 0; i < acount; i++)
                {
                    Rect des = new Rect(x + space + i * (space + 3), y - 3 * space - 10, 3, 3);
                    draw(driver["health"], des);
                }
            }   
            else
            {
                draw(driver["health"], new Rect(x + space, y - space - 15 + 5, (float)(((w - 2 * space) * hp) / 100), 5));
            }
        }

        private void draw(Texture2D texture, Rect bounds)
        {
            VertexPositionTexture[] vertex = new VertexPositionTexture[5]
            {
                new VertexPositionTexture(new Vector3(bounds.X, bounds.Y, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y - bounds.Height, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(bounds.X, bounds.Y - bounds.Height, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(bounds.X, bounds.Y, 0), new Vector2(1, 0))
            };
            sprite.FillArea(vertex, texture);
        }
    }
}
