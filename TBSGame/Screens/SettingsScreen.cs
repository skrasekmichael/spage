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
        protected override void load()
        {
            ControlClickedEventHandler handler = new ControlClickedEventHandler(sender =>
            {
                string[] resolution = ((MenuButton)sender).Text.Split('x');
                graphics.GraphicsDeviceManager.PreferredBackBufferWidth = int.Parse(resolution[0]);
                graphics.GraphicsDeviceManager.PreferredBackBufferHeight = int.Parse(resolution[1]);
                graphics.GraphicsDeviceManager.ApplyChanges();
                Reload();
            });

            for (int i = 0; i < 8; i++)
                parent.GetControl("btn_" + (i + 1).ToString()).OnControlClicked += handler;

            parent.GetControl("back").OnControlClicked += new ControlClickedEventHandler(sender => this.Dispose(new MainMenuScreen()));
        }
    }
}
