using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame
{
    public delegate void KeyDownEventHandler(object sender, Keys key);
    public delegate void KeyUpEventHandler(object sender, Keys key);

    public class TextInput
    {
        public event KeyDownEventHandler OnKeyDown;
        public event KeyUpEventHandler OnKeyUp;

        private KeyboardState state;
        private Keys[] last_pressed_keys;
        private Dictionary<Keys, KeyPress> press_time = new Dictionary<Keys, KeyPress>();
        private int dc = 0;

        private bool IsShiftPressed => (last_pressed_keys.Contains(Keys.LeftShift) || last_pressed_keys.Contains(Keys.RightShift));
        private bool CapsLockState => state.CapsLock;

        public class KeyPress
        {
            public TimeSpan Time { get; set; }
            public TimeSpan Click { get; set; }
        }

        public TextInput()
        {
            last_pressed_keys = new Keys[0];
        }

        private Dictionary<Keys, char> other = new Dictionary<Keys, char>() //speciální znaky
            {
                { Keys.OemPeriod, '.' },
                { Keys.Space, ' ' },
                { Keys.OemOpenBrackets, 'ú' },
                { Keys.OemSemicolon, 'ů' },
                { Keys.Divide, '/' },
                { Keys.Multiply, '*' },
                { Keys.OemMinus, '-' }
            };

        private Dictionary<char, char> c1 = new Dictionary<char, char>() //čárky
            {
                { 'e', 'é' }, { 'u', 'ú' }, { 'i', 'í' }, { 'o', 'ó' }, { 'a', 'á' }, { 'y', 'ý' }
            };

        private Dictionary<char, char> c2 = new Dictionary<char, char>() //háčky
            {
                { 'e', 'ě' }, { 'r', 'ř' }, { 't', 'ť' }, { 's', 'š' }, { 'd', 'ď' }, { 'c', 'č' }, { 'n', 'ň' }
            };

        public void Update(GameTime time, KeyboardState keyboard)
        {
            this.state = keyboard;

            Keys[] pressed_keys = state.GetPressedKeys();

            if (OnKeyUp != null)
            {
                foreach (Keys key in last_pressed_keys)
                {
                    if (!pressed_keys.Contains(key))
                        OnKeyUp(this, key);
                }
            }

            foreach (Keys key in pressed_keys)
            {
                if (!last_pressed_keys.Contains(key)) //stisknutí klávesy
                {
                    if (press_time.ContainsKey(key))
                        press_time[key].Click = time.TotalGameTime;
                    else
                        press_time.Add(key, new KeyPress() { Click = time.TotalGameTime, Time = time.TotalGameTime });
                    OnKeyDown?.Invoke(this, key);
                }
                else //držení klávesy
                {
                    //začátek držení klávesy > 250ms
                    if (time.TotalGameTime - press_time[key].Click >= TimeSpan.FromMilliseconds(250)) 
                    {
                        //mezičas mezi kliknutím 50ms
                        if (time.TotalGameTime - press_time[key].Time >= TimeSpan.FromMilliseconds(50))
                        {
                            press_time[key].Time = time.TotalGameTime;
                            OnKeyDown?.Invoke(this, key);
                        }
                    }
                }
            }

            last_pressed_keys = pressed_keys;
        }

        public char KeyToChar(Keys key)
        {
            if (key == Keys.OemQuestion) //diakritika
            {
                if (IsShiftPressed) //háčky
                    dc = 2;
                else //čárky
                    dc = 1;
                return '\0';
            }
            else if (key >= Keys.A && key <= Keys.Z) //písmena
            {
                char a = char.Parse(key.ToString().ToLower());
                if (dc > 0)
                {
                    Dictionary<char, char>[] p = { c1, c2 };
                    if (p[dc - 1].ContainsKey(a))
                        a = p[dc - 1][a];
                }
                return (IsShiftPressed ^ CapsLockState) ? char.ToUpper(a) : a;
            }
            else if (key >= Keys.D0 && key <= Keys.D9) //0 - 9
            {
                if (IsShiftPressed)
                    return char.Parse((key - Keys.D0).ToString());
                else
                {
                    char[] dia = { 'é', '\0', 'ě', 'š', 'č', 'ř', 'ž', 'ý', 'á', 'í' };
                    char chr = dia[key - Keys.D0];
                    return CapsLockState ? char.ToUpper(chr) : chr;
                }
            }
            else if (key >= Keys.NumPad0 && key <= Keys.NumPad9) //numerická klávesnice 0 - 9
                return char.Parse((key - Keys.NumPad0).ToString());
            else
            {
                if (other.ContainsKey(key)) //ostatní znaky
                    return CapsLockState ? char.ToUpper(other[key]) : other[key];
                else
                    return '\0';
            }
        }
    }
}
