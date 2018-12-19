using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class UnitControl : MapControl
    {
        public Unit Unit { get; set; }
        public int BaseIndex { get; set; } = -1;
        private Rectangle bounds;
        private Texture2D tx;
        private int type = 0, index = -1, lindex = -1;
        private double size;
        private Map map;
        private Engine engine;
        private TimeSpan start_attacking = TimeSpan.Zero;
        private GameTime time;
        private SoundEffect attack;

        public BallisticTrajectoryControl BallisticTrajectory { get; set; } = null;

        public UnitControl(Unit unit, int x, int y)
        {
            this.Unit = unit;
            this.X = x;
            this.Y = y;
        }

        protected override void load()
        {
            if (driver[Unit.Texture] == null)
            {
                driver.LoadUnit(Unit.Texture);
            }

            if (driver["pointer"] == null)
            {
                Texture2D pointer = sprite.Tint(content.Load<Texture2D>("pointer"), Color.Red);
                sprite.Clip(pointer, ref pointer, pointer.Bounds, SpriteEffects.FlipVertically);
                driver.LoadTexture("pointer", pointer);
            }

            attack = content.Load<SoundEffect>("sounds/fight");
            size = driver[Unit.Texture].Height / 5;
            tx = new Texture2D(graphics.GraphicsDevice, (int)size, (int)size);

            set_dir();
            clip();
        }

        public int Attack(UnitControl enemy)
        {
            if (start_attacking == TimeSpan.Zero)
            {
                if (Unit.Stamina >= Unit.StaminaPerAttack)
                {
                    List<System.Drawing.Point> range = engine.GetAttackRange(X, Y);
                    if (range.Contains(new System.Drawing.Point(enemy.X, enemy.Y)))
                    {
                        start_attacking = time.TotalGameTime;
                        this.Unit.Direction = (byte)MoveUnit.GetDirection(X - enemy.X, Y - enemy.Y);
                        attack.Play();

                        BallisticTrajectory = new BallisticTrajectoryControl(engine, new Point(X, Y), new Point(enemy.X, enemy.Y));
                        BallisticTrajectory.Load(graphics, content, sprite, driver, font);

                        enemy.Unit.Health -= (ushort)(Unit.Attack - enemy.Unit.Armor);
                        if (enemy.Unit.Health <= 0)
                            map.SetUnit(enemy.X, enemy.Y, null);

                        Unit.Stamina -= Unit.StaminaPerAttack;

                        var mob = engine.GetMobility(X, Y);
                        engine.Mobility = mob;

                        return 1;
                    }
                }
                else
                    return -1;
            }
            return 0;
        }

        public void Update(Map map, Engine engine, GameTime time)
        {
            this.time = time;
            this.map = map;
            this.engine = engine;

            BallisticTrajectory?.Update(time);

            Rectangle get_unit_bounds(Rectangle bounds)
            {
                float scale = 0.7f;
                double width = 2 * engine.Width * scale;
                double height = width / bounds.Width * bounds.Height;
                Vector2 c = to_vector(engine.GetCenter(bounds.X, bounds.Y));
                c = (new Vector2(c.X, c.Y - (float)(engine.Height) / 2));
                return new Rectangle((int)c.X, (int)c.Y, (int)(width / 2), (int)height);
            }

            bounds = get_unit_bounds(new Rectangle(X, Y, (int)size, (int)size));

            set_dir();

            if (lindex != index)
                clip();

            if (start_attacking != TimeSpan.Zero)
            {
                clip();
                if (time.TotalGameTime - start_attacking > TimeSpan.FromMilliseconds(150))
                {
                    type++;
                    if (type > 5)
                    {
                        type = 0;
                        start_attacking = TimeSpan.Zero;
                        clip();
                    }
                    else
                        start_attacking = time.TotalGameTime;
                }
            }
        }

        private void clip() => sprite.Clip(driver[Unit.Texture], ref tx, new Rectangle((int)(type * size), (int)(index * size), (int)size, (int)size), Unit.Direction == index ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

        private void set_dir()
        {
            lindex = index;
            index = Unit.Direction;
            if (index > 4)
                index = Unit.Direction - 4;
        }

        public void Draw()
        {
            VertexPositionTexture[] vertex = new VertexPositionTexture[5]
            {
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(1, 0))
            };

            sprite.FillArea(vertex, tx);
            BallisticTrajectory?.Draw();
        }
        
        public void DrawPointer(GameTime time)
        {
            Rectangle bounds = new Rectangle(this.bounds.X, this.bounds.Y + this.bounds.Height + (time.TotalGameTime.Seconds % 2 == 0 ? 10 : 5), 10, 20);
            VertexPositionTexture[] vertex = new VertexPositionTexture[5]
            {
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(bounds.X - bounds.Width, bounds.Y + bounds.Height, 0), new Vector2(1, 0))
            };

            sprite.FillArea(vertex, driver["pointer"]);
        }
    }
}
