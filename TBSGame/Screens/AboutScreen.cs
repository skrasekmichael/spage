using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;

namespace TBSGame.Screens
{
    public class AboutScreen : Screen
    {
        private Label label;

        protected override void draw()
        {
            label.Draw();
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            label = new Label(Resources.GetString("about_text"));
            label.Load(graphics, content, sprite);
            label.TextColor = Color.Silver;
            label.LineHeight = 20;
            label.VAligment = VerticalAligment.Center;
            label.HAligment = HorizontalAligment.Left;
        }

        protected override void loadpos()
        {
            label.Bounds = new Rectangle((Width - 600) / 2, 0, 600, Height);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            label.Update(time);
            MouseState ms = Mouse.GetState();
            if (Keyboard.GetState().GetPressedKeys().Count() > 0 || ms.LeftButton == ButtonState.Pressed ||
                ms.MiddleButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed)
                this.Dispose(new MainMenuScreen());
        }
    }
}
