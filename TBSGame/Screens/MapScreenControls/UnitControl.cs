using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TBSGame.Screens.MapScreenControls
{
	public class UnitControl : MapControl
    {
        public Unit Unit { get; set; }
        public int BaseIndex { get; set; } = -1;
        public bool IsAttacking => !((type == 0 && IsAnimating) && enemy == null);
        public bool IsAnimating => start_attacking == TimeSpan.Zero;
        public BallisticTrajectoryControl BallisticTrajectory { get; set; } = null;

        private Rectangle bounds;
        private Texture2D tx;
        private int type = 0, index = -1, lindex = -1, max = 0;
        private double size;
        private bool shooting = false;
        private Map map;
        private Engine engine;
        private TimeSpan start_attacking = TimeSpan.Zero;
        private GameTime time;
        private List<SoundEffect> attack = new List<SoundEffect>();
        private UnitControl enemy = null;
        private UnitInfoControl info;

        public UnitControl(Unit unit, int x, int y)
        {
            this.Unit = unit;
            this.X = x;
            this.Y = y;

            info = new UnitInfoControl(unit, x, y);
        }

        protected override void load()
        {
            if (driver[Unit.Texture] == null)
            {
                driver.LoadUnit(Unit.Texture);
            }

            info.Load(graphics, font);

            if (driver["pointer"] == null)
            {
                Texture2D pointer = sprite.Tint(content.Load<Texture2D>("pointer"), Color.Red);
                sprite.Clip(pointer, ref pointer, pointer.Bounds, SpriteEffects.FlipVertically);
                driver.LoadTexture("pointer", pointer);
            }

            max = 5;
            string path = "normal";
            if (Unit.IsRanged)
            {
                max = 3;
                path = "archer";
            }

            for (int i = 0; i < max; i++)
                attack.Add(content.Load<SoundEffect>($"sounds/attack/{path}/{i}"));

            size = driver[Unit.Texture].Height / 5;
            tx = new Texture2D(graphics.GraphicsDevice, (int)size, (int)size);

            set_dir();
            clip();
        }

        public int Attack(UnitControl enemy)
        {
            if (!IsAttacking)
            {
                if (Unit.Stamina >= Unit.StaminaPerAttack)
                {
                    List<System.Drawing.Point> range = engine.GetAttackRange(X, Y);
                    if (range.Contains(new System.Drawing.Point(enemy.X, enemy.Y)))
                    {
                        start_attacking = time.TotalGameTime;

                        this.Unit.Direction = (byte)MoveUnit.GetDirection(X - enemy.X, Y - enemy.Y);

                        this.enemy = enemy;

                        if (this.Unit.IsRanged)
                        {
                            BallisticTrajectory = new BallisticTrajectoryControl(map, engine, new Point(X, Y), new Point(enemy.X, enemy.Y), this.bounds.Height / 3, enemy.bounds.Height / 3);
                            BallisticTrajectory.Load(graphics, font);
                        }

                        Unit.Stamina -= Unit.StaminaPerAttack;
                        engine.Mobility = engine.GetMobility(X, Y);

                        return 1;
                    }
                    else
                        return -2; //jednotka nemá dostřel na nepřítele
                }
                else
                    return -1; //došla stamina
            }
            return 0; //jednotka právě útočí
        }

        private void _attack()
        {
            int hp = enemy.Unit.Health - (Unit.Attack - enemy.Unit.Armor);
            int exp = 5;
            if (hp <= 0)
            {
                exp += enemy.Unit.Experience / 5 + enemy.Unit.Price;
                map.SetUnit(enemy.X, enemy.Y, null);
            }
            else
                enemy.Unit.Health = (ushort)hp;

            int level = this.Unit.GetLevel();
            this.Unit.Experience += (ushort)exp;
            if (level < this.Unit.GetLevel())
            {

            }

            enemy = null;
            shooting = false;
        }

        public void Update(Map map, Engine engine, GameTime time)
        {
            this.time = time;
            this.map = map;
            this.engine = engine;

            info.Update(engine);

            if (shooting)
            {
                if (BallisticTrajectory == null)
                    _attack();
                else
                {
                    if (BallisticTrajectory.Update(engine, time))
                    {
                        _attack();
                        this.BallisticTrajectory = null;
                    }
                }
            }

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

            if (!IsAnimating)
            {
                clip();
                if (time.TotalGameTime - start_attacking > TimeSpan.FromMilliseconds(100))
                {
                    type++;
                    if (type > 5)
                    {
                        type = 0;
                        start_attacking = TimeSpan.Zero;
                        clip();
                    }
                    else
                    {
                        if (type == 3)
                        {
                            shooting = true;
                            Random random = new Random(DateTime.Now.Millisecond);
                            attack[random.Next(max)].Play();
                        }
                        start_attacking = time.TotalGameTime;
                    }
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

        public void DrawBallisticTrajectory()
        {
            BallisticTrajectory?.Draw();
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
            DrawBallisticTrajectory();
            info.Draw(bounds.Height);
        }
        
        public void DrawPointer(GameTime time)
        {
            Rectangle bounds = new Rectangle(this.bounds.X, this.bounds.Y + this.bounds.Height + 20 + (time.TotalGameTime.Seconds % 2 == 0 ? 10 : 5), 10, 20);
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
