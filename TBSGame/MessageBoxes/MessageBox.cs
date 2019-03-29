using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;

namespace TBSGame.MessageBoxes
{
    [Flags]
    public enum DialogResult
    {
        OK, Cancel, Yes, No, Apply, Save
    }

    public delegate void MessageBoxEventHandler(object sender, DialogResult result);

    public abstract class MessageBox
    {
        public event MessageBoxEventHandler OnMessageBox;
        private void Close(DialogResult result)
        {
            if (IsVisible)
            {
                OnMessageBox?.Invoke(this, result);
                IsVisible = false;
            }
        }

        public bool IsVisible { get; set; } = false;
        public Point Size
        {
            get => panel.Bounds.Size;
            set => panel.Bounds = new Rectangle((Width - value.X) / 2, (Height - value.Y) / 2, value.X, value.Y);
        }

        protected int Width => graphics.ScreenWidth;
        protected int Height => graphics.ScreenHeight;
        protected Dictionary<Button, DialogResult> buttons = new Dictionary<Button, DialogResult>();
        protected Graphics graphics;
        protected Panel panel;

        public void Load(Graphics graphics)
        {
            this.graphics = graphics;

            panel = new Panel();
            panel.Load(graphics);
            panel.Fill = new Color(50, 50, 50);
            panel.Border.IsVisible = false;
            panel.Opacity = 0.95f;

            if (Size == new Point(0, 0))
                Size = new Point(500, 300);

            load();

            foreach (KeyValuePair<Button, DialogResult> kvp in buttons)
            {
                kvp.Key.Load(graphics);
                kvp.Key.OnControlClicked += new ControlClickedEventHandler(sender => Close(kvp.Value));
                panel.Add(kvp.Key);
            }
        }

        protected abstract void load();

        public void Update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
        }

        public void Draw()
        {
            panel.Draw();
        }
    }
}
