using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Screens
{
    public class AboutScreen : Screen
    {
        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            MouseState ms = Mouse.GetState();
            if (Keyboard.GetState().GetPressedKeys().Count() > 0 || ms.LeftButton == ButtonState.Pressed ||
                ms.MiddleButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed)
                this.Dispose(new MainMenuScreen());
        }
    }
}
