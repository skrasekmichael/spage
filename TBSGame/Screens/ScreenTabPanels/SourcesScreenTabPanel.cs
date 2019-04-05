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
        private ImagePanel image;
        private List<Label> labels = new List<Label>();

        [LayoutControl("switch")] private Panel switch_panel;
        [LayoutControl("image")] private Panel image_panel; 
        [LayoutControl("bar1")] private Panel bar_panel1;
        [LayoutControl("bar2")] private Panel bar_panel2;
        [LayoutControl("income_label")] private Label source_label;
        [LayoutControl("research_label")] private Label research_label;
        [LayoutControl] private Label total;

        private int total_income = 0;

        public SourcesScreenTabPanel(Settings settings, GameSave game, Level level, string icon) : base(settings, game, icon)
        {
            this.level = level;
        }

        protected override void load()
        {
            set_map(Color.DarkGreen, 0.7f, Color.Black, 0.7f);

            image = new ImagePanel(MapTexture);
            image_panel.Add(image);

            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                if (lm.Player == 1 && game.Info[lm.Name].RoundsToDeplete > 0)
                    total_income += lm.Sources;

                System.Drawing.Point center = lm.Center;
                Label label = new Label($"{lm.Sources.ToString()}({game.Info[lm.Name].RoundsToDeplete.ToString()})")
                {
                    Bounds = new Rectangle((int)(center.X * image.Coef) - 35, (int)(center.Y * image.Coef) - 20, 80, 50),
                    IsVisible = lm.Player == 1 && lm.Rounds > 0
                };                
                labels.Add(label);
                image_panel.Add(label);
            }

            game.Income = total_income - game.Research * 3;

            MenuButton researchs = (MenuButton)Panel.GetControl("btn_research");
            researchs.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (total_income - (game.Research + 1) * 3 >= 0)
                    game.Research += 1;
            });

            MenuButton sources = (MenuButton)Panel.GetControl("btn_income");
            sources.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (game.Research - 1 >= 0)
                    game.Research -= 1;
            });
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            total_income = 0;
            for (int i = 0; i < level.Maps.Count; i++)
            {
                LevelMap lm = level.Maps[i];
                labels[i].IsVisible = lm.Player == 1 && game.Info[lm.Name].RoundsToDeplete > 0;
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
    }
}
