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

namespace TBSGame.Screens
{
    public class MapScreen : Screen
    {
        private Map map;
        private Engine engine;
        private Texture2D bar;
        private Label enemy_turn;
        private GameTime time = null;
        private List<Button> buttons = new List<Button>();
        private List<AreaControl> areas;
        private int selected_index = -1, turn = 1;
        private UnitDetailControl sunit = null, hunit = null;
        private GameMapSaver saver = new GameMapSaver();
        private MoveControl move = new MoveControl();
        private SoundEffect enemysighted;
        private MapAI[] ai = new MapAI[7];

        private TextWindow start_message, win_message;
        private MenuWindow menu;
        private MiniMapWindow minimap;

        private List<MapWindow> windows = new List<MapWindow>();

        private bool active_map = true;
        private GameSave game;

        public MapScreen(GameSave game, Map map) : base()
        {
            this.game = game;
            this.map = map;

            areas = new List<AreaControl>(map.Width * map.Height);

            menu = new MenuWindow(saver, map);
            minimap = new MiniMapWindow(map);
            start_message = new TextWindow(map.Scout);
            start_message.Visible = true;
            win_message = new TextWindow(map.Win);

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

            menu.OnExit += new EventHandler((sender, args) => Dispose(new MainMenuScreen()));

            move.OnTargetInSight += new TargetInSightEventHandle((sender, source, dest) =>
            {
                enemysighted.Play();
            });

            buttons.AddRange(new GameButton[]
            {
                new GameButton("ok", "ok"),
                new GameButton("map", "map"),
                new GameButton("menu", "menu")
            });

            buttons[0].OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                engine.LoadVisibility();
                map.Units.Values.ToList().ForEach(u => u.Stamina = u.MaxStamina);

                set();

                turn++;
                ai[turn - 2].Turn(map);
            });

            buttons[1].OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                if (turn == 1)
                {
                    show_window(minimap);
                }
            });

            buttons[2].OnButtonClicked += new ButtonClickedEventHandler(sender =>
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
                sunit = new UnitDetailControl(a.UnitControl.Unit, a.X, a.Y);
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
                        if (turn == 1)
                        {
                            AreaControl area = (AreaControl)sender;
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
                                    hunit = new UnitDetailControl(area.UnitControl.Unit, area.X, area.Y);
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
            }

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
        
        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            enemysighted = content.Load<SoundEffect>("Sounds/enemysighted");

            windows.ForEach(w => w.Load(graphics, content, sprite, driver, font));
            areas.ForEach(area => area.Load(graphics, content, sprite, driver, small));

            bar = sprite.GetColorFill(Color.Gray);
            for (int i = 0; i < buttons.Count; i++)
            { 
                buttons[i].Load(graphics, content, sprite);
                buttons[i].Bounds = new Rectangle(Width - (i + 1) * 80, Height - 75, 80, 75);
            }

            enemy_turn.Load(graphics, content, sprite);
            enemy_turn.Bounds = new Rectangle(0, Height - 150, Width, 150);
            enemy_turn.Font = font;
        }

        protected override void loadpos()
        {
            engine = new Engine(map, Height / 15, Width, Height);
            engine.Center = new System.Drawing.PointF(0, 75 / 2);
        }

        private void refresh(MouseState mouse, KeyboardState keyboard)
        {
            engine.CalcPoints();

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
                    if (win_message.Visible)
                        Dispose(new GameScreen(game));
                    else
                        show_window(win_message);
                }

                buttons.ForEach(btn => btn.Update(mouse));

                minimap.Update(map, engine, mouse);
                menu.Update(time, map, mouse);
                start_message.Update(mouse);
                win_message.Update(mouse);
                
                active_map = windows.Where(w => w.Visible).Count() == 0;

                //posouvání jednotek po mapě
                move.Update(time, map, engine, areas, ref selected_index);
            }
            else
            {
                enemy_turn.Update();
            }

            for (int i = 0; i < ai.Length; i++)
                ai[i].Update(map, engine, areas, time);

            areas.ForEach(area => area.Update(map, engine, time, keyboard, mouse, active_map));
        }

        protected override void update(GameTime time)
        {
            this.time = time;
            int move = 15;

            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            //posouvání po mapě
            if (!start_message.Visible)
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
