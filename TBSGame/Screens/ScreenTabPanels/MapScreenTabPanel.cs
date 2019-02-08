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
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public delegate void PlayGameEventhandle(object sender, LevelMap map, List<Unit> list);

    public class MapScreenTabPanel : ScreenTabPanel
    {
        public event PlayGameEventhandle OnPlayGame;
        private void PlayGame(LevelMap map, List<Unit> list)
        {
            OnPlayGame?.Invoke(this, map, list);
        }

        private LevelMap selected = null;

        private Label desc = new Label("");
        private MenuButton play, play_at_night, cancel, select_all;
        private List<CheckBox> units = new List<CheckBox>();

        private List<Control> controls = new List<Control>();

        private Level level;
        private Texture2D map, texture, borders, shadow, selected_map;
        private Dictionary<string, Texture2D> hover;
        private Rectangle bounds = new Rectangle(140, 140, 800, 600);
        private Rectangle resize;
        private string key = null, path = null;

        public MapScreenTabPanel(string path, Settings settings, GameSave game, Level level, string icon) : base(settings, game, icon)
        {
            this.level = level;
            this.path = path;

            button.Tint = Color.Lime;

            hover = new Dictionary<string, Texture2D>(level.Count);
            play = new MenuButton(Resources.GetString("play")) { IsVisible = false };
            play.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                List<Unit> list = new List<Unit>();
                for (int i = 0; i < units.Count; i++)
                {
                    CheckBox checkbox = units[i];
                    if (checkbox.IsChecked)
                        list.Add(game.Units[i]);
                }

                if (list.Count > 0)
                    PlayGame(selected, list);
            });
            cancel = new MenuButton(Resources.GetString("cancel")) { IsVisible = false };
            cancel.OnButtonClicked += new ButtonClickedEventHandler(sender => deselect());
            select_all = new MenuButton(Resources.GetString("select_all_units"));
            select_all.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                MenuButton button = (MenuButton)sender;
                bool check = (bool)button.Tag;
                for (int i = 0; i < game.Units.Count; i++)
                    units[i].IsChecked = check;
                button.Tag = !check;
                button.Title = check ? Resources.GetString("deselect_all_units") : Resources.GetString("select_all_units");
            });
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

            controls.ForEach(c => c.Draw());
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

            for (int i = 0; i < game.Units.Count; i++)
            {
                CheckBox checkbox = new CheckBox(Resources.GetString(game.Units[i].GetType().Name));
                checkbox.IsVisible = false;
                checkbox.Bounds = new Rectangle(resize.X + resize.Width + 10, resize.Top + i * 40 + 60, 200, 40);
                checkbox.Load(graphics, content, sprite);
                checkbox.Checked = Color.Crimson;
                checkbox.UnChecked = new Color(60, 60, 60);
                units.Add(checkbox);
            }

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

            select_all.Load(graphics, content, sprite);
            select_all.Bounds = new Rectangle(resize.X + resize.Width + 10, resize.Top, 350, 50);
            select_all.IsVisible = false;
            select_all.Tag = true;

            controls.AddRange(units);
            controls.AddRange(new Control[] { play, cancel, select_all, desc });

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

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            key = null;

            controls.ForEach(c => c.Update(time, keyboard, mouse));

            bool val = (units.Where(ch => ch.IsChecked).Count() == 0);
            play.IsLocked = val;

            if (resize.Contains(mouse.Position))
            {
                Point p = new Point(mouse.Position.X - resize.Left, mouse.Position.Y - resize.Top);
                double coef = (double)resize.Width / bounds.Width;
                string k = level.GetMap((int)(p.X / coef), (int)(p.Y / coef));
                if (k != null)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (level[k].Player == 2 && level[k].IsVisibled)
                            select(level[k]);
                    }

                    if (level[k].IsVisibled)
                        key = k;
                }
            }
        }
        
        private void deselect()
        {
            select_all.IsVisible = false;
            set_all(false);
            selected = null;
            desc.Text = "";
            play.IsVisible = false;
            cancel.IsVisible = false;
        }

        private void select(LevelMap lm)
        {
            if (selected != lm)
            {
                select_all.IsVisible = true;

                set_all(true);
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

        private void set_all(bool visibility)
        {
            foreach (CheckBox checkbox in units)
            {
                checkbox.IsVisible = visibility;
                checkbox.IsChecked = false;
            }

            play.IsVisible = true;
            cancel.IsVisible = true;
            select_all.Title = Resources.GetString("select_all_units");
            select_all.Tag = true;
        }

        public override void LoadPosition()
        {
            
        }
    }
}
