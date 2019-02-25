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

        private Panel switch_panel = new Panel(false);
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
            set_map(Color.DarkGreen, 0.7f, Color.Black, 0.9f);

            float size = panel.Bounds.Width * 0.4f;
            float coef = size / MapTexture.Width;
            resize = new Rectangle(0, 0, (int)size, (int)(coef * MapTexture.Height));
            resize = new Rectangle((panel.Bounds.Width - resize.Width) / 2, (panel.Bounds.Height - resize.Height) / 2, resize.Width, resize.Height);

            SpriteFont font = content.Load<SpriteFont>("fonts/small");
            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                if (lm.Player == 1 && lm.Rounds > 0)
                    total_income += lm.Sources;
                System.Drawing.Point center = lm.Center;
                Label label = new Label($"{lm.Sources.ToString()} ({game.Info[lm.Name].RoundsToDeplete.ToString()})")
                {
                    Bounds = new Rectangle(resize.Left + (int)(center.X * coef) - 35, resize.Top + (int)(center.Y * coef) - 20, 80, 50),
                    Space = 0,
                    IsVisible = lm.Player == 1 && lm.Rounds > 0,
                    TextColor = Color.Lime
                };
                label.Load(graphics, content, sprite);
                label.Font = font;
                labels.Add(label);
            }

            switch_panel.Bounds = new Rectangle(resize.X, resize.Y + resize.Height, resize.Width, 100);

            game.Income = total_income - game.Research * 3;

            total = new Label(total_income.ToString())
            {
                Bounds = new Rectangle((switch_panel.Bounds.Width - 100) / 2, (switch_panel.Bounds.Height - 50) / 2, 100, 50),
                TextColor = Color.Lime
            };

            Label g = new Label(Resources.GetString("gold"))
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 150, 0, 150, 50),
                TextColor = Color.Lime,
                VAligment = VerticalAligment.Bottom,
                Space = 0
            };

            source_label = new Label("0")
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 150, 50, 150, 50),
                TextColor = Color.Lime,
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
                TextColor = Color.Lime,
                VAligment = VerticalAligment.Bottom,
                Space = 0
            };

            research_label = new Label("0")
            {
                Bounds = new Rectangle(0, 50, 150, 50),
                TextColor = Color.Lime,
                VAligment = VerticalAligment.Top,
                Space = 0
            };

            MenuButton sources = new MenuButton("+")
            {
                Bounds = new Rectangle(switch_panel.Bounds.Width - 200, 25, 50, 50),
                MouseOverFill = Color.Lime
            };
            sources.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (game.Research - 1 >= 0)
                    game.Research -= 1;
            });

            panel.Add(switch_panel);
            switch_panel.AddRange(new Control[] { researchs, sources, r, g, source_label, research_label, total });
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            total_income = 0;
            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                labels[i].IsVisible = lm.Player == 1 && lm.Rounds > 0;
                labels[i].Update(time, keyboard, mouse);
                if (lm.Player == 1 && lm.Rounds > 0)
                    total_income += lm.Sources;
            }

            game.Income = total_income - game.Research * 3;
            research_label.Text = game.Research.ToString();
            source_label.Text = game.Income.ToString();
            total.Text = total_income.ToString();
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
    }
}
