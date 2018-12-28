using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;

namespace TBSGame.Screens
{
    public class SettingsScreen : Screen
    {
        private List<Button> buttons = new List<Button>();
        private int btns = 0;

        public SettingsScreen() : base()
        {
            buttons.AddRange(new Button[] 
            {
                new MenuButton("1920x1080"),
                new MenuButton("1680x1050"),
                new MenuButton("1280x1024"),
                new MenuButton("800x600")
            });

            btns = buttons.Count;

            buttons.ForEach(btn => btn.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                string[] resolution = ((MenuButton)sender).Title.Split('x');
                graphics.PreferredBackBufferWidth = int.Parse(resolution[0]);
                graphics.PreferredBackBufferHeight = int.Parse(resolution[1]);
                graphics.ApplyChanges();
                Reload();
            }));

            MenuButton back = new MenuButton(Resources.GetString("back"));
            back.OnButtonClicked += new ButtonClickedEventHandler(sender => this.Dispose(new MainMenuScreen()));
            buttons.Add(back);
        }

        protected override void draw()
        {
            buttons.ForEach(btn => btn.Draw());
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            buttons.ForEach(btn => btn.Load(graphics, content, sprite));
        }

        protected override void loadpos()
        {
            int h = 50;
            for (int i = 0; i < btns; i++)
                buttons[i].Bounds = new Rectangle(50, 50 + i * h, 200, h);

            buttons[btns].Bounds = new Rectangle(Width - 210, Height - h - 10, 200, h);
        }   

        protected override void update(GameTime time)
        {
            buttons.ForEach(btn => btn.Update(time));
        }
    }
}
