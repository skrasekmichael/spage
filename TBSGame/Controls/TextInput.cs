using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TBSGame.Controls
{
    public class TextInput : Control
    {
        public string Text
        {
            get { return text; }
            set
            {
                if (Font.MeasureString(value).X <= Bounds.Width)
                    text = value;
            }
        }
        private Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);

        private string text = "", underline = "";
        private Texture2D rec;
        private TimeSpan last_cursor_time = TimeSpan.Zero;
        private bool is_cursor_visible = true;
        private Keys[] lastPressedKeys;
        private int pos = 0;
        private int dc = 0;
        private int cursor_pos
        {
            get { return pos; }
            set
            {
                if (value > Text.Length)
                    pos = Text.Length;
                else if (value < 0)
                    pos = 0;
                else
                {
                    pos = value;
                    dc = 0;
                }
            }
        }
        private bool IsShiftPressed => (lastPressedKeys.Contains(Keys.LeftShift) || lastPressedKeys.Contains(Keys.RightShift));
        private bool CapsLockState => Keyboard.GetState().CapsLock;

        private Dictionary<Keys, char> other = new Dictionary<Keys, char>()
            {
                { Keys.OemPeriod, '.' },
                { Keys.Space, ' ' },
                { Keys.OemOpenBrackets, 'ú' },
                { Keys.OemSemicolon, 'ů' }
            };

        private Dictionary<char, char> c1 = new Dictionary<char, char>()
            {
                { 'e', 'é' }, { 'u', 'ú' }, { 'i', 'í' }, { 'o', 'ó' }, { 'a', 'á' }, { 'y', 'ý' }
            };

        private Dictionary<char, char> c2 = new Dictionary<char, char>()
            {
                { 'e', 'ě' }, { 'r', 'ř' }, { 't', 'ť' }, { 's', 'š' }, { 'd', 'ď' }, { 'c', 'č' }, { 'n', 'ň' }
            };

        public TextInput()
        {
            lastPressedKeys = new Keys[0];
        }

        protected override void load()
        {
            rec = sprite.GetColorFill(Color.Silver, Bounds.Width, Bounds.Height);

            for (int i = 0; i <= (int)(Bounds.Width / Font.MeasureString("_").X); i++)
                underline += "_";
        }

        public override void Draw()
        {
            string text = Text.Substring(0, cursor_pos) + " " + Text.Substring(cursor_pos);


            Vector2 middle = new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(text).X) / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1);
            sprite.DrawString(Font, text, middle, Color.Black);

            if (is_cursor_visible)
            {
                Vector2 cur_pos = new Vector2(middle.X + Font.MeasureString(Text.Substring(0, cursor_pos)).X - 2, bounds.Y);
                sprite.DrawString(Font, "|", cur_pos, Color.Black);
            }

            sprite.DrawString(Font, underline, new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(underline).X) / 2, bounds.Y + Bounds.Height - Font.LineSpacing + 8), Color.Black);
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            TimeSpan duration = time.TotalGameTime - last_cursor_time;
            if (duration.TotalMilliseconds > 500)
            {
                is_cursor_visible = !is_cursor_visible;
                last_cursor_time = time.TotalGameTime;
            }

            KeyboardState state = keyboard.Value;

            Keys[] pressedKeys = state.GetPressedKeys();

            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                    OnKeyDown(key);
            }

            lastPressedKeys = pressedKeys;
        }

        private void OnKeyDown(Keys key)
        {
            if (key == Keys.Right)
                cursor_pos++;
            else if (key == Keys.Left)
                cursor_pos--;
            else if (key == Keys.Home)
                cursor_pos = 0;
            else if (key == Keys.End)
                cursor_pos = Text.Length;
            else if (key == Keys.OemQuestion)
            {
                if (IsShiftPressed)
                    dc = 2;
                else
                    dc = 1;
            }
            else if (key == Keys.Back && cursor_pos > 0)
            {
                Text = Text.Substring(0, cursor_pos - 1) + Text.Substring(cursor_pos);
                cursor_pos--;
            }
            else if (key == Keys.Delete && cursor_pos < Text.Length)
                Text = Text.Substring(0, cursor_pos) + Text.Substring(cursor_pos + 1);
            else
            {
                char val = to_char(key);
                if (val != '\0')
                {
                    Text = Text.Substring(0, cursor_pos) + val + Text.Substring(cursor_pos);
                    cursor_pos++;
                }
            }
        }

        private char to_char(Keys key)
        {

            if (key >= Keys.A && key <= Keys.Z)
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
            else if (key >= Keys.D0 && key <= Keys.D9)
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
            else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return char.Parse((key - Keys.NumPad0).ToString());
            else
            {
                if (other.ContainsKey(key))
                    return CapsLockState ? char.ToUpper(other[key]) : other[key];
                else
                    return '\0';
            }
        }

        private void OnKeyUp(Keys key)
        {

        }
    }
}
