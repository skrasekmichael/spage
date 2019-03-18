using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Saver;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SourcesScreenTabPanel : ScreenTabPanel
    {
        public Texture2D MapTexture { get; set; }

        private Level level;
        private Rectangle resize;
        private List<Label> labels = new List<Label>();

        private Panel switch_panel = new Panel(true);
        private Panel bar_panel1 = new Panel();
        private Panel bar_panel2 = new Panel();
        private Label source_label, research_label, total;

        private int total_income = 0;

        public SourcesScreenTabPanel(Settings settings, GameSave game, Level level, string icon) : base(settings, game, icon)
        {
            this.level = level;
        }

        public override void LoadPosition()
        {

        }

        protected override void draw()
        {
            sprite.Draw(MapTexture, resize, Color.White);
            labels.ForEach(l => l.Draw());
        }

        protected override void load()
        {
            set_map(Color.DarkGreen, 0.7f, Color.Black, 0.7f);

            float size = Panel.Bounds.Width * 0.4f;
            float coef = size / MapTexture.Width;
            resize = new Rectangle(0, 0, (int)size, (int)(coef * MapTexture.Height));
            resize = new Rectangle((Panel.Bounds.Width - resize.Width) / 2, (Panel.Bounds.Height - resize.Height) / 2, resize.Width, resize.Height);

            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                if (lm.Player == 1 && game.Info[lm.Name].RoundsToDeplete > 0)
                    total_income += lm.Sources;
                System.Drawing.Point center = lm.Center;
                Label label = new Label($"{lm.Sources.ToString()}({game.Info[lm.Name].RoundsToDeplete.ToString()})")
                {
                    Bounds = new Rectangle(resize.Left + (int)(center.X * coef) - 35, resize.Top + (int)(center.Y * coef) - 20, 80, 50),
                    Space = 0,
                    IsVisible = lm.Player == 1 && lm.Rounds > 0,
                    Foreground = Color.Lime
                };
                label.Load(graphics);
                label.Font = graphics.Small;
                labels.Add(label);
            }

            game.Income = total_income - game.Research * 3;

            switch_panel.Bounds = new Rectangle(resize.X, resize.Y + resize.Height, resize.Width, 100);
            switch_panel.Desc = false;

            bar_panel1.Border.IsVisible = false;
            bar_panel1.Fill = Color.Lime;

            bar_panel2.Border.IsVisible = false;
            bar_panel2.Fill = Color.Gold;

            total = new Label(total_income.ToString())
            {
                Bounds = new Rectangle((switch_panel.Bounds.Width - 100) / 2, (switch_panel.Bounds.Height - 50) / 2 - 5, 100, 50),
                Foreground = Color.White
            };

            Label g = new Label(Resources.GetString("gold"))
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 150, 0, 150, 50),
                Foreground = Color.Gold,
                VAligment = VerticalAligment.Bottom,
                Space = 0
            };

            source_label = new Label("0")
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 150, 50, 150, 50),
                Foreground = Color.Gold,
                VAligment = VerticalAligment.Top,
                Space = 0
            };

            MenuButton researchs = new MenuButton("+")
            {
                Bounds = new Rectangle(150, 25, 50, 50),
                MouseOverFill = Color.Lime
            };
            researchs.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (total_income - (game.Research + 1) * 3 >= 0)
                    game.Research += 1;
            });

            Label r = new Label(Resources.GetString("research"))
            {
                Bounds = new Rectangle(0, 0, 150, 50),
                Foreground = Color.Lime,
                VAligment = VerticalAligment.Bottom,
                Space = 0
            };

            research_label = new Label("0")
            {
                Bounds = new Rectangle(0, 50, 150, 50),
                Foreground = Color.Lime,
                VAligment = VerticalAligment.Top,
                Space = 0
            };

            MenuButton sources = new MenuButton("+")
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 200, 25, 50, 50),
                MouseOverFill = Color.Gold
            };
            sources.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (game.Research - 1 >= 0)
                    game.Research -= 1;
            });

            Panel.Add(switch_panel);
            switch_panel.AddRange(new Control[] { researchs, sources, r, g, source_label, research_label, total, bar_panel1, bar_panel2 });
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            total_income = 0;
            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                labels[i].IsVisible = lm.Player == 1 && game.Info[lm.Name].RoundsToDeplete > 0;
                labels[i].Update(time, keyboard, mouse);
                labels[i].Text = $"{lm.Sources.ToString()}({game.Info[lm.Name].RoundsToDeplete.ToString()})";
                if (lm.Player == 1 && game.Info[lm.Name].RoundsToDeplete > 0)
                    total_income += lm.Sources;
            }

            game.Income = total_income - game.Research * 3;
            research_label.Text = game.Research.ToString();
            source_label.Text = game.Income.ToString();
            total.Text = total_income.ToString();

            if (total_income > 0)
            {
                int space = 5;
                int w = (switch_panel.Bounds.Width - 400 - 2 * space) * game.Income / total_income;
                bar_panel1.Bounds = new Rectangle(200 + space, 75 - 8, (switch_panel.Bounds.Width - 400 - 2 * space) - w, 4);
                bar_panel2.Bounds = new Rectangle(200 + space + bar_panel1.Bounds.Width, 75 - 8, w, 4);

                bar_panel1.Fill = Color.Lime;
                bar_panel2.Fill = Color.Gold;
            }
            else
            {
                bar_panel1.Fill = Color.Silver;
                bar_panel2.Fill = Color.Silver;
            }
        }

        private void set_map(Color c1, float f1, Color c2, float f2)
        {
            Color[] colors = new Color[MapTexture.Width * MapTexture.Height];
            MapTexture.GetData(colors);

            for (int y = 0; y < MapTexture.Height; y++)
            {
                for (int x = 0; x < MapTexture.Width; x++)
                {
                    int index = x + y * MapTexture.Width;
                    
                    Color color = Color.Lerp(colors[index], c2, f2);
                    string key = level.GetMap(x, y);
                    if (key != null)
                    {
                        LevelMap lm = level[key];
                        if (lm.Player == 1)
                            color = Color.Lerp(colors[index], c1, f1);
                    }

                    colors[index] = color;
                }
            }

            MapTexture.SetData(colors);
        }

        public override void Reload()
        {
            
        }
    }
}
