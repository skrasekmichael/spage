using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TBSGame
{
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    public class KeyboardChecker
    {
        public Keys[] LastPressedKeys { get; private set; }
        public event KeyEventHandler KeyUp, KeyDown;
        private void OnKeyUp(KeyEventArgs e) => KeyUp?.Invoke(this, e);
        private void OnKeyDown(KeyEventArgs e) => KeyDown?.Invoke(this, e);

        public KeyboardChecker()
        {
            LastPressedKeys = new Keys[0];
        }

        public void Update(GameTime time)
        {
            KeyboardState state = Keyboard.GetState();
            Keys[] pressed_keys = state.GetPressedKeys();

            foreach (Keys key in LastPressedKeys)
            {
                if (!pressed_keys.Contains(key))
                    OnKeyUp(new KeyEventArgs(key));
            }

            foreach (Keys key in pressed_keys)
            {
                if (!LastPressedKeys.Contains(key))
                    OnKeyDown(new KeyEventArgs(key));
            }

            LastPressedKeys = pressed_keys;
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public Keys Key { get; private set; }

        public KeyEventArgs(Keys key)
        {
            Key = key;
        }
    }
}
