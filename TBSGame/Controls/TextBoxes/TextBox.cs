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
    public delegate void TextBoxConfirmedEventHandler(object sender);
    public delegate void TextBoxCanceledEventHandler(object sender);

    public abstract class TextBox : Control
    {
        public event TextBoxConfirmedEventHandler OnConfirm;
        private void confirm()
        {
            IsFocused = false;
            IsLocked = true;
            OnConfirm?.Invoke(this);
        }

        public event TextBoxCanceledEventHandler OnCancel;
        private void cancel()
        {
            text = source;
            IsFocused = false;
            IsLocked = true;
            OnCancel?.Invoke(this);
        }

        public override Border Border { get; set; } = new Border() { IsVisible = true };

        public abstract Color BackColor { get; set; }
        public abstract Color PlaceHolderColor { get; set; }

        public bool IsVisibled { get; set; } = true;
        public bool IsFocused { get; protected set; } = false;
        public string PlaceHolder { get; set; } = "";
        public string Text
        {
            get { return text; }
            set
            {
                if (Font.MeasureString(value).X <= Bounds.Width)
                    text = value;
            }
        }

        protected TextInput input;

        private string text = "";
        private TimeSpan last_cursor_time = TimeSpan.Zero;
        private bool is_cursor_visible = true;
        private int pos = 0;
        private bool is_mouse_down_outside;
        private string source = "";

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
                }
            }
        }

        public TextBox()
        {
            input = new TextInput();
            input.OnKeyDown += this.OnKeyDown;
        }

        public void Focus()
        {
            IsFocused = !IsLocked;
            source = text;
            cursor_pos = 0;
        }

        private void focus(MouseState state)
        {
            bool before = IsFocused;
            Focus();
            if (IsFocused)
            {
                //pozice kutzoru v textu

                string text = Text;
                if (before)
                    text = Text.Substring(0, pos) + " " + Text.Substring(pos);

                float left = bounds.X + (Bounds.Width - Font.MeasureString(text).X) / 2;
                for (int i = 1; i <= text.Length; i++)
                {
                    float left1 = left + Font.MeasureString(text.Substring(0, i - 1)).X;
                    float left2 = left + Font.MeasureString(text.Substring(0, i)).X;

                    if (state.X <= left1)
                    {
                        cursor_pos = i - 1;
                        break;
                    }
                    else if (state.X <= left2)
                    {
                        float d1 = Math.Abs(state.X - left1);
                        float d2 = Math.Abs(state.X - left2);
                        if (Math.Min(d1, d2) == d1)
                            cursor_pos = i - 1;
                        else
                            cursor_pos = i;
                        break;
                    }
                    else
                    {
                        if (i == text.Length)
                            cursor_pos = text.Length;
                    }
                }
            }
        }

        protected override void draw()
        {
            draw_background();

            string text = Text;
            if (IsFocused)
                text = Text.Substring(0, pos) + " " + Text.Substring(pos);

            Color tc = Foreground;
            if (text.Length == 0)
            {
                text = PlaceHolder;
                tc = PlaceHolderColor;
            }

            Vector2 middle = new Vector2(bounds.X + (Bounds.Width - Font.MeasureString(text).X) / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1);
            sprite.DrawString(Font, text, middle, tc);

            if (is_cursor_visible && IsFocused)
            {
                Vector2 cur_pos = new Vector2(middle.X + Font.MeasureString(Text.Substring(0, pos)).X - 3 + Font.MeasureString(" ").X / 2, bounds.Y + (Bounds.Height - Font.LineSpacing) / 2 + 1);
                sprite.DrawString(Font, "|", cur_pos, Foreground);
            }

            draw_foreground();
        }

        protected abstract void draw_background();
        protected virtual void draw_foreground() { }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            if (!IsLocked)
            {
                if (IsFocused)
                {
                    //blikání kurzoru
                    if (last_cursor_time == TimeSpan.Zero)
                        last_cursor_time = time.TotalGameTime + TimeSpan.FromMilliseconds(500);

                    if (time.TotalGameTime - last_cursor_time > TimeSpan.FromMilliseconds(500))
                    {
                        is_cursor_visible = !is_cursor_visible;
                        last_cursor_time = time.TotalGameTime;
                    }

                    input.Update(time, keyboard);
                }

                if (this.bounds.Contains(mouse.X, mouse.Y))
                {
                    this.is_mouse_hover = true;
                    if (mouse.LeftButton != ButtonState.Pressed && is_mouse_down)
                    {
                        focus(mouse);
                        this.is_mouse_down = false;
                    }
                    else
                        this.is_mouse_down = (mouse.LeftButton == ButtonState.Pressed);
                }
                else
                {
                    //kliknutí mimo textbox
                    if (mouse.LeftButton != ButtonState.Pressed && is_mouse_down_outside)
                    {
                        if (IsFocused)
                            cancel();
                        this.is_mouse_down_outside = false;
                    }
                    else
                        this.is_mouse_down_outside = (mouse.LeftButton == ButtonState.Pressed);

                    this.is_mouse_hover = false;
                    this.is_mouse_down = false;
                }
            }
        }

        private void OnKeyDown(object sender, Keys key)
        {
            if (key == Keys.Right)
                cursor_pos++;
            else if (key == Keys.Left)
                cursor_pos--;
            else if (key == Keys.Home)
                cursor_pos = 0;
            else if (key == Keys.End)
                cursor_pos = Text.Length;
            else if (key == Keys.Back && cursor_pos > 0)
            {
                Text = Text.Substring(0, cursor_pos - 1) + Text.Substring(cursor_pos);
                cursor_pos--;
            }
            else if (key == Keys.Delete && cursor_pos < Text.Length)
                Text = Text.Substring(0, cursor_pos) + Text.Substring(cursor_pos + 1);
            else if (key == Keys.Enter)
                confirm();
            else if (key == Keys.Escape)
                cancel();
            else
            {
                char val = input.KeyToChar(key);
                if (val != '\0')
                {
                    Text = Text.Substring(0, cursor_pos) + val + Text.Substring(cursor_pos);
                    cursor_pos++;
                }
            }
        }

        public void SetText(string text) => this.text = text;
    }
}
