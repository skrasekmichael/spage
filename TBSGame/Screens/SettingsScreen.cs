using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens
{
    public class SettingsScreen : Screen
    {
        private Panel panel = new Panel();
        private Panel wide_screen_panel = new Panel();
        private Panel screen_panel = new Panel();

        protected override void draw()
        {
            panel.Draw();
        }

        protected override void load()
        {
            panel.Load(graphics);

            panel.Bounds = new Rectangle(10, 10, Width - 20, Height - 20);
            wide_screen_panel.Bounds = new Rectangle(0, 0, 220, 4 * 51 + 20);
            screen_panel.Bounds = new Rectangle(240, 0, 220, 4 * 51 + 20);

            panel.Add(screen_panel);
            panel.Add(wide_screen_panel);

            ControlClickedEventHandler handler = new ControlClickedEventHandler(sender =>
            {
                string[] resolution = ((MenuButton)sender).Text.Split('x');
                graphics.GraphicsDeviceManager.PreferredBackBufferWidth = int.Parse(resolution[0]);
                graphics.GraphicsDeviceManager.PreferredBackBufferHeight = int.Parse(resolution[1]);
                graphics.GraphicsDeviceManager.ApplyChanges();
                Reload();
            });

            MenuButton[] wide = new MenuButton[]
            {
                new MenuButton("1920x1080"),
                new MenuButton("1600x900"),
                new MenuButton("1280x720"),
                new MenuButton("1024x576")
            };

            MenuButton[] _43 = new MenuButton[] {
                new MenuButton("1280x960"),
                new MenuButton("1024x768"),
                new MenuButton("960x720"),
                new MenuButton("800x600")
            };

            for (int i = 0; i < 4; i++)
            {
                wide[i].Bounds = new Rectangle(10, 10 + i * 51, 200, 50);
                wide[i].OnControlClicked += handler;
                wide_screen_panel.Add(wide[i]);

                _43[i].Bounds = new Rectangle(10, 10 + i * 51, 200, 50);
                _43[i].OnControlClicked += handler;
                screen_panel.Add(_43[i]);
            }

            MenuButton back = new MenuButton(Resources.GetString("back"));
            back.OnControlClicked += new ControlClickedEventHandler(sender => this.Dispose(new MainMenuScreen()));
            back.Bounds = new Rectangle(panel.Bounds.Width - 210, panel.Bounds.Height - 60, 200, 50);
            panel.Add(back);
        }

        protected override void loadpos()
        {

        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
        }
    }
}
