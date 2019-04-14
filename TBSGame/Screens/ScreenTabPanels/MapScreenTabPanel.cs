using System;
using System.Collections.Generic;
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
using TBSGame.Controls.GameScreen;
using TBSGame.Controls.Special;
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

        public Texture2D MapTexture { get; set; }

        private Level level;
        private LevelMap selected = null;

        [LayoutControl] protected Label description;
        [LayoutControl] private MenuButton play, play_at_night, cancel, select_all;
        [LayoutControl] private Panel map_panel;
        [LayoutControl] private UnitsPanel units_panel;

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

            play.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                List<Unit> list = new List<Unit>();
                for (int i = 0; i < units_panel.Units.Count; i++)
                {
                    UnitBox unitbox = units_panel.Units[i];
                    if (unitbox.IsChecked)
                        list.Add(game.Units[i]);
                }

                if (list.Count > 0)
                    PlayGame(selected, list);
            });
            cancel.OnControlClicked += new ControlClickedEventHandler(sender => deselect());
            select_all.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                MenuButton button = (MenuButton)sender;
                bool check = (bool)button.Tag;
                for (int i = 0; i < game.Units.Count; i++)
                {
                    if (!units_panel.Units[i].IsLocked)
                        units_panel.Units[i].IsChecked = check;
                }
                button.Tag = !check;
                button.Text = check ? Resources.GetString("deselect_all_units") : Resources.GetString("select_all_units");
            });

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

            description.Bounds = new Rectangle(description.Bounds.X, resize.Height + 11, description.Bounds.Width, Panel.Bounds.Height - resize.Height);
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

            if (game.Units.Count != units_panel.Units.Count)
                Reload();

            bool val = (units_panel.Units.Where(ch => ch.IsChecked).Count() == 0);
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
            description.Text = Resources.GetString("map_desc");
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
                    description.Text = map.Description;
                }
                else
                    description.Text = Resources.GetString("missing_map");
            }
        }

        private void set_all(bool visibility)
        {
            foreach (UnitBox unitbox in units_panel.Units)
            {
                unitbox.IsChecked = false;
                unitbox.IsLocked = unitbox.Unit.UnitStatus != UnitStatus.InBarracks;
            }

            select_all.Text = Resources.GetString("select_all_units");
            select_all.Tag = true;

            units_panel.IsVisible = visibility;
        }

        public override void Reload() => load_units();
        public override void Deselect() => deselect();

        private void load_units()
        {
            units_panel.LoadUnits(game.Units);
            foreach (UnitBox unitbox in units_panel.Units)
                unitbox.IsLocked = unitbox.Unit.UnitStatus != UnitStatus.InBarracks;
        }
    }
}
