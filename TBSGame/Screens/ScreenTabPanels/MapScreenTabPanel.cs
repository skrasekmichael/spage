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
using TBSGame.Screens.GameScreenControls;

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

        public Texture2D MapTexture { get; set; }

        private LevelMap selected = null;

        private Label desc = new Label("");
        private MenuButton play, play_at_night, cancel, select_all;
        private List<UnitBox> units = new List<UnitBox>();

        private Panel map_panel = new Panel(true);
        private Panel units_panel = new Panel(true);

        private Level level;

        private Texture2D texture, borders, shadow, selected_map;
        private Dictionary<string, Texture2D> hover;
        private Rectangle bounds = new Rectangle(140, 140, 800, 600);
        private Rectangle resize;
        private string key = null, path = null;

        public MapScreenTabPanel(string path, Settings settings, GameSave game, Level level, string icon) : base(settings, game, icon)
        {
            this.level = level;
            this.path = path;

            hover = new Dictionary<string, Texture2D>(level.Count);
            play = new MenuButton(Resources.GetString("assault"));
            play.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                List<Unit> list = new List<Unit>();
                for (int i = 0; i < units.Count; i++)
                {
                    UnitBox unitbox = units[i];
                    if (unitbox.IsChecked)
                        list.Add(game.Units[i]);
                }

                if (list.Count > 0)
                    PlayGame(selected, list);
            });
            play_at_night = new MenuButton(Resources.GetString("assault2"));
            cancel = new MenuButton(Resources.GetString("cancel"));
            cancel.OnControlClicked += new ControlClickedEventHandler(sender => deselect());
            select_all = new MenuButton(Resources.GetString("select_all_units"));
            select_all.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                MenuButton button = (MenuButton)sender;
                bool check = (bool)button.Tag;
                for (int i = 0; i < game.Units.Count; i++)
                    units[i].IsChecked = check;
                button.Tag = !check;
                button.Text = check ? Resources.GetString("deselect_all_units") : Resources.GetString("select_all_units");
            });
        }

        protected override void draw()
        {
            sprite.Draw(MapTexture, resize, Color.White);

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
        }

        protected override void load()
        {
            texture = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            borders = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            shadow = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);
            selected_map = new Texture2D(graphics.GraphicsDevice, bounds.Width, bounds.Height);

            map_panel.Bounds = new Rectangle(10, 10, (int)(panel.Bounds.Width * 0.5f), panel.Bounds.Height - 20);
            units_panel.Bounds = new Rectangle(map_panel.Bounds.Width + 20, 10, panel.Bounds.Width - map_panel.Bounds.Width - 30, panel.Bounds.Height - 20);
            panel.AddRange(new[] { map_panel, units_panel });

            units_panel.IsVisible = false;

            load_units();

            float size = map_panel.Bounds.Width;
            resize = new Rectangle(10, 10, (int)size, (int)(size / bounds.Width * bounds.Height));

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

            desc.Bounds = new Rectangle(resize.X, resize.Y + resize.Height, resize.Width, map_panel.Bounds.Height - resize.Height - 30);
            desc.VAligment = VerticalAligment.Top;
            desc.HAligment = HorizontalAligment.Left;
            desc.Foreground = Color.White;

            play.Bounds = new Rectangle(0, units_panel.Bounds.Height - 50, 150, 50);
            play_at_night.Bounds = new Rectangle(160, units_panel.Bounds.Height - 50, 210, 50);
            cancel.Bounds = new Rectangle(380, units_panel.Bounds.Height - 50, 110, 50);

            select_all.Bounds = new Rectangle(0, 0, 350, 50);
            select_all.Tag = true;

            map_panel.Add(desc);
            units_panel.AddRange(new Control[] { play, play_at_night, cancel, select_all });

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

            if (game.Units.Count != units.Count)
                Reload();

            bool val = (units.Where(ch => ch.IsChecked).Count() == 0);
            play.IsLocked = val;
            play_at_night.IsLocked = val;

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
            set_all(false);
            selected = null;
            desc.Text = "";
        }

        private void select(LevelMap lm)
        {
            if (selected != lm)
            {
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
            foreach (UnitBox unitbox in units)
                unitbox.IsChecked = false;

            select_all.Text = Resources.GetString("select_all_units");
            select_all.Tag = true;

            units_panel.IsVisible = visibility;
        }

        public override void LoadPosition()
        {
            
        }

        public override void Reload()
        {
            load_units();
        }

        private void load_units()
        {
            int index = 0;
            foreach (Unit unit in game.Units)
            {
                UnitBox unitbox;
                if (index < units.Count)
                {
                    unitbox = units[index];
                    unitbox.Unit = unit;
                }
                else
                {
                    unitbox = new UnitBox(unit);
                    units.Add(unitbox);
                    units_panel.Add(unitbox);
                }

                unitbox.Bounds = new Rectangle(0, resize.Top + index * 40 + 60, 350, 40);
                unitbox.Checked = Color.Crimson;
                unitbox.UnChecked = new Color(60, 60, 60);
                unitbox.IsLocked = unit.Rounds > 0;

                index++;
            }
        }
    }
}
