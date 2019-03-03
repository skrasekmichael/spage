using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.MessageBoxes;
using MessageBox = TBSGame.MessageBoxes.MessageBox;

namespace TBSGame.Screens
{
    public abstract class Screen
    { 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsMouseVisible { get; set; } = true;
        public Settings Settings { get; set; }

        protected bool starting = true, ending = false;
        protected float end_coef = 0f, start_coef = 1f, display_coef = 0f;
        protected TextureDriver driver;
        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected CustomSpriteBatch sprite;
        protected Texture2D black, anim;
        protected SpriteFont font, small;

        private Task loading;
        private TimeSpan start_ending = TimeSpan.Zero, start_starting = TimeSpan.Zero, start_animating = TimeSpan.Zero, 
            start_loading = TimeSpan.Zero, start_display = TimeSpan.Zero;
        private Screen next_screen = null;
        private bool is_loading = false;
        private double move = -60;
        private Point[] loc = new Point[] { new Point(-1, -1), new Point(-1, 1), new Point(1, 1), new Point(1, -1) };
        private int dir = 0;
        private MessageBox message = null;

        protected abstract void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite);
        protected abstract void update(GameTime time, KeyboardState keyboard, MouseState mouse);
        protected abstract void draw();
        protected abstract void loadpos();

        public void Load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite, TextureDriver driver)
        {
            this.graphics = graphics;
            this.content = content;
            this.sprite = sprite;
            this.driver = driver;

            Width = graphics.PreferredBackBufferWidth;
            Height = graphics.PreferredBackBufferHeight;
            sprite.Load();

            black = sprite.GetColorFill(Color.Black, Width, Height);
            anim = sprite.GetColorFill(Color.Lime);

            font = content.Load<SpriteFont>("fonts/text");
            small = content.Load<SpriteFont>("fonts/small");

            load(graphics, content, sprite);
            loadpos();
        }

        protected void Reload()
        {
            Width = graphics.PreferredBackBufferWidth;
            Height = graphics.PreferredBackBufferHeight;

            sprite.Load();
            loadpos();
        }

        public Screen Update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            //pokud se načítájí data 
            if (is_loading) //animace načítání
            {
                if (start_loading == TimeSpan.Zero)
                    start_loading = time.TotalGameTime;

                if (time.TotalGameTime - start_loading > TimeSpan.FromMilliseconds(300))
                {
                    if (start_animating == TimeSpan.Zero)
                    {
                        start_animating = time.TotalGameTime;
                        start_display = time.TotalGameTime;
                    }

                    if (time.TotalGameTime - start_animating >= TimeSpan.FromMilliseconds(450))
                    {
                        dir++;
                        if (dir > 3)
                            dir = 0;

                        start_animating = time.TotalGameTime;
                    }

                    move = (double)(time.TotalGameTime - start_animating).TotalMilliseconds * 120 / 450 - 60;
                    if (display_coef != 1)
                        display_coef = (float)(time.TotalGameTime - start_display).TotalMilliseconds / 300f;
                }

                return this;
            }
            else //normální režim
            {
                if (loading != null)
                    return next_screen;

                if (end_coef == 1f)
                {
                    ending = false;
                    end();
                }
                //animace rozvicování                
                else if (starting)
                {
                    if (start_starting == TimeSpan.Zero)
                        start_starting = time.TotalGameTime;

                    TimeSpan duration = time.TotalGameTime - start_starting;
                    start_coef = 1f - (float)(duration.TotalMilliseconds / 500f);
                    if (start_coef < 0f)
                    {
                        start_coef = 0f;
                        starting = false;
                    }
                }
                //animace zhasínání
                else if (ending)
                {
                    if (start_ending == TimeSpan.Zero)
                        start_ending = time.TotalGameTime;

                    TimeSpan duration = time.TotalGameTime - start_ending;
                    end_coef = (float)(duration.TotalMilliseconds / 400f);
                    if (end_coef > 1f)
                        end_coef = 1f;
                }

                if (message == null)
                    update(time, keyboard, mouse);
                else
                {
                    if (!message.IsVisible)
                        message.IsVisible = true;
                    message.Update(time, keyboard, mouse);
                    if (!message.IsVisible)
                        message = null;
                }
                return this;
            }
        }

        private void end()
        {
            //asynchroní načítání dat
            is_loading = true;
            loading = new Task(() =>
            {
                set();
                is_loading = false;
            });

            loading.Start();
        }

        private void set()
        {
            if (next_screen != null)
                next_screen.Load(graphics, content, sprite, driver);
        }

        public void Draw()
        {
            if (is_loading)
            {
                //animace načítání
                if (start_animating != TimeSpan.Zero)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int index = i + dir;
                        if (index > 3)
                            index -= 4;

                        double mx = 60, my = 60;

                        //pokud se jedná o právě animovaný čtverec
                        if (index == dir)
                        {
                            //nastvení která souřadnice se animuje
                            switch (index)
                            {
                                case 0:
                                    mx = -move;
                                    break;
                                case 1:
                                    my = -move;
                                    break;
                                case 2:
                                    mx = -move;
                                    break;
                                case 3:
                                    my = -move;
                                    break;
                            }
                        }

                        sprite.Draw(anim, new Rectangle((int)((Width - 100) / 2 + mx * loc[index].X), (int)((Height - 100) / 2 + my * loc[index].Y), 100, 100), Color.White * display_coef);
                    }
                }
            }
            else
            {
                draw();
                if (message != null && message.IsVisible)
                    message.Draw();

                sprite.Draw(black, new Rectangle(0, 0, Width, Height), Color.White * end_coef);
                sprite.Draw(black, new Rectangle(0, 0, Width, Height), Color.White * start_coef);
            }
        }

        public void ShowMessage(MessageBox message)
        {
            this.message = message;
            message.Load(graphics, content, sprite);
        }

        public void Dispose(Screen next)
        {
            ending = true;
            next_screen = next;
            if (next != null)
                next_screen.Settings = this.Settings;
        }
    }
}
