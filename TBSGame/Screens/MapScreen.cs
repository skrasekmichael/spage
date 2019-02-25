using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TBSGame.AI;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Saver;
using TBSGame.Screens.MapScreenControls;
using TBSGame.Screens.MapScreenControls.MapWindows;
using TBSGame.MessageBoxes;

namespace TBSGame.Screens
{
    public class MapScreen : Screen
    {
        private const int START = 0;
        private const int GAME = 1;

        private Map map;
        private MapInfo info;
        private Engine engine;
        private GameMapSaver saver;
        private GameSave game;

        private Texture2D bar;
        private SoundEffect enemysighted;

        private Label enemy_turn;
        private UnitDetailControl sunit = null, hunit = null;

        private GameTime time = null;

        private bool active_map = true;
        private int selected_index = -1, turn = 1, phase = START;

        private MoveControl move = new MoveControl();
        private MapAI[] ai = new MapAI[7];

        private TextWindow start_message, win_message;
        private MenuWindow menu;
        private MiniMapWindow minimap;

        private List<Button> buttons = new List<Button>();
        private List<MapWindow> windows = new List<MapWindow>();
        private List<AreaControl> areas;
        private List<Unit> selected_units;

        public MapScreen(GameSave game, Settings settings, Map map, string name, List<Unit> selected_units) : base()
        {
            this.game = game;
            this.map = map;

            if (name != null)
            {
                this.info = game.Info[name];
                this.selected_units = selected_units;
            }

            saver = new GameMapSaver(settings);
            areas = new List<AreaControl>(map.Width * map.Height);

            menu = new MenuWindow(saver, map);
            minimap = new MiniMapWindow(map);
            start_message = new TextWindow(map.Scout);
            start_message.Visible = true;
            win_message = new TextWindow(map.Win);

            //načíst
            menu.OnLoadMapSaveEventHandler += new LoadMapSaveEventHandler((sender, loaded_map) =>
            {
                MapScreen map_screen = new MapScreen(game, settings, loaded_map, name, selected_units);
                map_screen.phase = GAME;
                map_screen.start_message.Visible = false;
                this.Dispose(map_screen);
            });

            windows.AddRange(new MapWindow[] { menu, minimap, start_message, win_message });

            for (int i = 0; i < ai.Length; i++)
            {
                ai[i] = new MapAI(map, i + 2);
                ai[i].OnEndTurn += new EndTurnEnventHandler(sender =>
                {
                    turn = ((MapAI)sender).Player + 1;
                    if (turn > ai.Length)
                    {
                        turn = 1;
                        set();
                    }
                    else
                        ai[turn - 2].Turn(map);
                });
            }

            enemy_turn = new Label("Enemy turn");

            //odejít
            menu.OnExit += new EventHandler((sender, args) => Dispose(new MainMenuScreen()));

            //ustoupit
            menu.OnRetreat += new EventHandler((sender, args) =>
            {
                Visibility[,] visibilities = new Visibility[map.Width, map.Height];
                HashSet<Unit> death = new HashSet<Unit>();

                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        visibilities[x, y] = engine.GetVisibility(x, y);
                        Unit unit = map.GetUnit(x, y);
                        MapObject obj = map.GetMapObject(x, y);

                        if (unit != null && unit.Player == 1)
                        {
                            if (!(obj != null && obj.GetType() == typeof(StartPoint)))
                                death.Add(unit);
                        }
                    }
                }

                YesNoMessageBox message = new YesNoMessageBox(Resources.GetString("retreat?", new[] { death.Count.ToString() }));
                message.OnMessageBox += new MessageBoxEventHandler((obj, result) =>
                {
                    if (result == DialogResult.Yes)
                    {
                        info.MapVisibilities = visibilities;
                        game.Units = game.Units.Where(u => !death.Contains(u)).ToList();
                        this.Dispose(new GameScreen(game));
                    }
                });

                this.ShowMessage(message);
            });

            move.OnTargetInSight += new TargetInSightEventHandle((sender, source, dest) =>
            {
                enemysighted.Play();
            });

            buttons.AddRange(new GameButton[]
            {
                new GameButton("ok", "ok") { Tag = true },
                new GameButton("map", "map") { Tag = true },
                new GameButton("menu", "menu") { Tag = false }
            });

            buttons[0].OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (phase == START)
                    phase = GAME;
                else
                {
                    engine.LoadVisibility();
                    map.Units.Values.ToList().ForEach(u => u.Stamina = u.MaxStamina);

                    set();

                    turn++;
                    ai[turn - 2].Turn(map);
                }
            });
            buttons[0].IsLocked = true;

            buttons[1].OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (turn == 1)
                {
                    show_window(minimap);
                }
            });

            buttons[2].OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (turn == 1)
                {
                    show_window(menu);
                }
            });

            load();
        }

        private void deselect()
        {
            selected_index = -1;
            sunit = null;
            engine.Mobility = null;
        }

        private void set()
        {
            if (selected_index != -1)
            {
                if (areas[selected_index].UnitControl != null)
                    select(areas[selected_index]);
                else
                    deselect();
            }
        }

        private void show_window(MapWindow window)
        {
            windows.ForEach(w => w.Visible = false);
            window.Visible = true;
        }

        private void select(AreaControl a)
        {
            selected_index = a.Index;
            if (a.UnitControl != null)
            {
                sunit = new UnitDetailControl(a.UnitControl.Unit);
                sunit.Load(graphics, content, sprite, driver, small);

                var mob = engine.GetMobility(a.X, a.Y);
                engine.Mobility = mob;

                var arange = engine.GetAttackRange(a.X, a.Y);
                engine.AttackRange = arange;
            }
            else
                sunit = null;
        }

        private void load()
        {
            int index = 0;
            for (int x = map.Width - 1; x >= 0; x--)
            {
                for (int y = map.Height - 1; y >= 0; y--)
                {
                    AreaControl a = new AreaControl(map.GetTerrain(x, y), x, y);
                    a.Index = index;
                    a.AreaClicked += new EventHandler((sender, e) =>
                    {
                        AreaControl area = (AreaControl)sender;
                        if (phase == START)
                        {
                            if (selected_units.Count > 0)
                            {
                                MapObject obj = map.GetMapObject(area.X, area.Y);
                                if (obj?.GetType() == typeof(StartPoint))
                                {
                                    Unit unit = map.GetUnit(area.X, area.Y);
                                    if (unit != null)
                                    {
                                        selected_units.Add(unit);
                                        map.SetUnit(area.X, area.Y, null);
                                        buttons[0].IsLocked = true;
                                    }
                                    else
                                    {
                                        map.SetUnit(area.X, area.Y, selected_units[0]);
                                        selected_units.RemoveAt(0);

                                        if (selected_units.Count > 0)
                                        {
                                            sunit = new UnitDetailControl(selected_units[0]);
                                            sunit.Load(graphics, content, sprite, driver, small);
                                            buttons[0].IsLocked = true;
                                        }
                                        else
                                            buttons[0].IsLocked = false;
                                    }
                                }
                            }
                        }
                        else if (phase == GAME)
                        {
                            if (turn == 1)
                            {
                                if (area.UnitControl != null)
                                {
                                    if (area.UnitControl.Unit.Player == 1)
                                    {
                                        select(area);
                                    }
                                    else
                                    {
                                        if (selected_index != -1)
                                        {
                                            areas[selected_index].UnitControl.Attack(area.UnitControl);
                                        }
                                    }
                                }
                                else
                                {
                                    if (selected_index != -1)
                                    {
                                        var key = new System.Drawing.Point(area.X, area.Y);
                                        if (engine.Mobility.ContainsKey(key))
                                        {
                                            move.Move(engine, engine.Mobility, areas[selected_index].UnitControl, new Point(area.X, area.Y));
                                        }
                                    }
                                }
                            }
                        }
                    });
                    a.HoverArea += new EventHandler((sender, e) =>
                    {
                        if (turn == 1)
                        {
                            AreaControl area = (AreaControl)sender;
                            if (area.UnitControl != null)
                            {
                                if (hunit == null || (hunit.X != area.X && hunit.Y != area.Y))
                                {
                                    hunit = new UnitDetailControl(area.UnitControl.Unit);
                                    hunit.Load(graphics, content, sprite, driver, small);
                                }
                            }
                        }
                    });
                    a.LeaveArea += new EventHandler((sender, e) => hunit = null);
                    areas.Add(a);
                    index++;
                }
            }
        }

        protected override void draw()
        {
            for (int i = 0; i < areas.Count; i++)
            {
                int x = areas[i].X;
                int y = areas[i].Y;
                if (engine.GetVisibility(x, y) != Visibility.Gone)
                    areas[i].Draw();
                else
                {
                    //vykreslení šípu
                    if (areas[i].UnitControl?.IsAnimating == true)
                        areas[i].UnitControl?.DrawBallisticTrajectory();
                }
            }

            if (phase == START)
            {
                sprite.Draw(bar, new Rectangle(0, Height - 75, Width, 75), Color.White);
                sunit?.Draw(bar, new Point(0, Height - 125));
                hunit?.Draw(bar, new Point(295, Height - 125));

                buttons.Where(btn => (bool)btn.Tag).ToList().ForEach(btn => btn.Draw());

                start_message.Draw();
                minimap.Draw();
            }
            else if (phase == GAME)
            {
                if (turn == 1)
                {
                    if (selected_index != -1)
                        areas[selected_index].UnitControl.DrawPointer(time);

                    windows.ForEach(w => w.Draw());

                    sprite.Draw(bar, new Rectangle(0, Height - 75, Width, 75), Color.White);
                    sunit?.Draw(bar, new Point(0, Height - 125));
                    hunit?.Draw(bar, new Point(295, Height - 125));
                    buttons.ForEach(btn => btn.Draw());
                }
                else
                {
                    sprite.Draw(bar, new Rectangle(0, Height - 150, Width, 150), Color.White);
                    enemy_turn.Draw();
                }
            }
        }
        
        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            enemysighted = content.Load<SoundEffect>("Sounds/enemysighted");

            windows.ForEach(w => w.Load(graphics, content, sprite, driver, font));
            areas.ForEach(area => area.Load(graphics, content, sprite, driver, small));

            bar = sprite.GetColorFill(new Color(90, 90, 90));
            for (int i = 0; i < buttons.Count; i++)
            { 
                buttons[i].Load(graphics, content, sprite);
                buttons[i].Bounds = new Rectangle(Width - (i + 1) * 80, Height - 75, 80, 75);
            }

            enemy_turn.Load(graphics, content, sprite);
            enemy_turn.Bounds = new Rectangle(0, Height - 150, Width, 150);
            enemy_turn.Font = font;

            sunit = new UnitDetailControl(selected_units[0]);
            sunit.Load(graphics, content, sprite, driver, small);
        }

        protected override void loadpos()
        {
            engine = new Engine(map, Height / 15, Width, Height);
            engine.ScreenWidth = Width;
            engine.ScreenHeight = Height;
            engine.Center = new System.Drawing.PointF(0, 75 / 2);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (info.MapVisibilities == null)
                    {
                        MapObject obj = map.GetMapObject(x, y);
                        if (obj != null && obj.GetType() == typeof(StartPoint))
                        {
                            engine.SetVisibility(x, y, Visibility.Visible);
                        }
                    }
                    else
                        engine.SetVisibility(x, y, info.MapVisibilities[x, y]);
                }
            }  
        }

        private void refresh(MouseState mouse, KeyboardState keyboard)
        {
            engine.CalcPoints(phase);

            if (phase == START)
            {
                buttons.Where(btn => (bool)btn.Tag).ToList().ForEach(btn => btn.Update(time, keyboard, mouse));
                minimap.Update(map, engine, mouse);

                start_message.Update(mouse);
            }
            else if (phase == GAME)
            {
                if (turn == 1)
                {
                    //pokud všechny jednotky umřou
                    if (map.Units.Where(kvp => kvp.Value.Player == 1).Count() == 0)
                    {
                        Dispose(new GameScreen(game));
                    }

                    //pokud budou splněny všechny úkoly
                    List<Quest> task = map.QuestList.ToList();
                    if (task.Where(t => t.Check(map, engine)).Count() == task.Count)
                    {
                        if (!win_message.Visible)
                            Dispose(new GameScreen(game));
                        show_window(win_message);
                    }

                    buttons.ForEach(btn => btn.Update(time, keyboard, mouse));

                    minimap.Update(map, engine, mouse);
                    menu.Update(time, map, keyboard, mouse);
                    win_message.Update(mouse);

                    active_map = windows.Where(w => w.Visible).Count() == 0;

                    //posouvání jednotek po mapě
                    move.Update(time, map, engine, areas, ref selected_index);
                }
                else
                {
                    enemy_turn.Update(time, keyboard, mouse);
                }

                for (int i = 0; i < ai.Length; i++)
                    ai[i].Update(map, engine, areas, time);
            }

            areas.ForEach(area => area.Update(map, engine, time, keyboard, mouse, active_map));
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            this.time = time;
            int move = 15;

            //posouvání po mapě
            if (!start_message.Visible && turn == 1)
            {
                if (mouse.X > Width - 5)
                    engine.View = new System.Drawing.PointF(engine.View.X - move, engine.View.Y);
                else if (mouse.X < 5)
                    engine.View = new System.Drawing.PointF(engine.View.X + move, engine.View.Y);

                if (mouse.Y < 25)
                    engine.View = new System.Drawing.PointF(engine.View.X, engine.View.Y - move);
                else if (mouse.Y > Height - 5)
                    engine.View = new System.Drawing.PointF(engine.View.X, engine.View.Y + move);
            }

            refresh(mouse, keyboard);
        }
    }
}
