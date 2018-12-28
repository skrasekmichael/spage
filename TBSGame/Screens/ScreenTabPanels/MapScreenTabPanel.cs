using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens.ScreenTabPanels
{
    public delegate void PlayGameEventhandle(object sender, LevelMap map);

    public class MapScreenTabPanel : ScreenTabPanel
    {
        public event PlayGameEventhandle OnPlayGame;
        private void PlayGame(LevelMap map)
        {
            OnPlayGame?.Invoke(this, map);
        }

        private LevelMap selected = null;
        private Label desc = new Label("");
        private MenuButton play, play_at_night, cancel;
        private Level level;
        private Texture2D map, texture, borders, shadow, selected_map;
        private Dictionary<string, Texture2D> hover;
        private Rectangle bounds = new Rectangle(140, 140, 800, 600);
        private Rectangle resize;
        private string key = null, path = null;

        public MapScreenTabPanel(string path, Level level, string icon, int index) : base(icon, index)
        {
            this.level = level;
            this.path = path;

            hover = new Dictionary<string, Texture2D>(level.Count);
            play = new MenuButton(Resources.GetString("play"));
            play.OnButtonClicked += new ButtonClickedEventHandler(sender => PlayGame(selected));
            cancel = new MenuButton(Resources.GetString("cancel"));
            cancel.OnButtonClicked += new ButtonClickedEventHandler(sener => deselect());
        }

        protected override void draw()
        {
            sprite.Draw(map, resize, Color.White);

            sprite.Draw(borders, resize, Color.White);

            if (key != null)
                sprite.Draw(hover[key], resize, Color.White);

            if (selected != null)
            {
                sprite.Draw(shadow, resize, Color.White);
                sprite.Draw(selected_map, resize, Color.White);
            }
            else
                sprite.Draw(texture, resize, Color.White);

            desc.Draw();
            play.Draw();
            cancel.Draw();
        }

        protected override void load()
        {
            map = sprite.GetTexture(level.Map);
            texture = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            borders = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            shadow = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            selected_map = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);

            float size = (Height * 0.6f) - 15;
            resize = new Rectangle(bounds.X, bounds.Y, (int)(size / bounds.Height * bounds.Width), (int)size);

            for (int i = 0; i < level.Count; i++)
            {
                LevelMap lm = level.GetByIndex(i);
                if (lm.Player == 1 || lm.Neighbors.Where(s => level[s].Player == 1).Count() > 0)
                    lm.IsVisibled = true;
                else
                    lm.IsVisibled = false;

                Texture2D t = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
                set_borders(ref t, Color.Red, Color.Red, lm);
                hover.Add(lm.Name, t);
            }

            desc.Load(graphics, content, sprite);
            desc.Bounds = new Rectangle(resize.X, resize.Y + resize.Height + 10, resize.Width, resize.Height);
            desc.VAligment = VerticalAligment.Top;
            desc.HAligment = HorizontalAligment.Left;
            desc.TextColor = Color.White;

            play.Load(graphics, content, sprite);
            play.Bounds = new Rectangle(resize.Width + resize.Left + 10, Height - 70, 110, 50);

            cancel.Load(graphics, content, sprite);
            cancel.Bounds = new Rectangle(resize.Width + resize.Left + 2 * 10 + 110, Height - 70, 110, 50);

            SetColors();
        }

        private void SetColors()
        {
            set_maps(ref texture, Color.Red, Color.Transparent);
            set_maps(ref shadow, Color.Gray, Color.Transparent);
            set_borders(ref borders, Color.Black, Color.Black);
        }

        private void set_maps(ref Texture2D texture, Color sct, Color scf, LevelMap des = null)
        {
            Color[] colors = new Color[texture.Width * texture.Height];

            int space = 6;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int index = x + y * texture.Width;

                    Color color = Color.Transparent;
                    Color cf = scf, ct = sct;

                    if (scf == Color.Transparent)
                        cf = color;
                    else if (sct == Color.Transparent)
                        ct = color;

                    string key = level.GetMap(x, y);
                    if (key != null && (x % space == y % space || (x + 1) % space == y % space))
                    {
                        LevelMap lm = level[key];
                        if (lm.IsVisibled)
                        {
                            if (des != null)
                            {
                                if (des.Name == lm.Name)
                                    color = cf;
                            }
                            else
                            {
                                if (lm.Player == 1)
                                    color = cf;
                                else
                                    color = ct;
                            }
                        }
                    }

                    colors[index] = color;
                }
            }

            texture.SetData(colors);
        }

        private void set_borders(ref Texture2D texture, Color sct, Color scf, LevelMap des = null)
        {
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int index = x + y * texture.Width;

                    Color color = Color.Transparent;
                    Color cf = scf, ct = sct;

                    if (scf == Color.Transparent)
                        cf = color;
                    else if (sct == Color.Transparent)
                        ct = color;

                    bool border = level.GetBorders(x, y);
                    string mkey = level.GetMap(x, y);
                    if (border && mkey == null)
                    {
                        bool visibled = false;
                        HashSet<int> players = new HashSet<int>();
                        for (int i = 0; i < level.Count; i++)
                        {
                            LevelMap lm = level.GetByIndex(i);
                            if (lm.GetBorders(x + y * texture.Width))
                            {
                                if (lm.IsVisibled)
                                    visibled = true;
                                players.Add(lm.Player);
                            }
                        }

                        if (visibled)
                        {
                            if (des != null)
                            {
                                if (des.GetBorders(x + y * texture.Width)) 
                                    color = cf;
                            }
                            else
                            {
                                if (players.Contains(1))
                                    color = cf;
                                else
                                    color = ct;
                            }
                        }
                    }

                    colors[index] = color;
                }
            }

            texture.SetData(colors);
        }

        protected override void update(GameTime time, KeyboardState keybord, MouseState mouse)
        {
            key = null;
            desc.Update();

            play.Update(mouse);
            cancel.Update(mouse);

            if (resize.Contains(mouse.Position))
            {
                Point p = new Point(mouse.Position.X - resize.Left, mouse.Position.Y - resize.Top);
                double coef = (double)resize.Width / bounds.Width;
                string k = level.GetMap((int)(p.X / coef), (int)(p.Y / coef));
                if (k != null)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (level[k].Player == 2)
                            select(level[k]);
                    }

                    if (level[k].IsVisibled)
                        key = k;
                }
            }
        }
        
        private void deselect()
        {
            selected = null;
            desc.Text = "";
        }

        private void select(LevelMap lm)
        {
            if (selected != lm)
            {
                selected = lm;
                set_maps(ref selected_map, Color.Red, Color.Red, lm);
                string path = $"{Path.GetDirectoryName(this.path)}/maps/{lm.Name}.dat";
                if (File.Exists(path))
                {
                    Map map = Map.Load(path);
                    desc.Text = map.Description;
                }
                else
                {
                    desc.Text = Resources.GetString("missing_map");
                }
            }
        }
    }
}
