using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Controls
{
    public class Panel : Control
    {
        private List<Control> controls = new List<Control>();

        public bool OnlyArea { get; private set; } = false;
        public Color Fill { get; set; } = Color.Black;
        public Color Border { get; set; } = Color.Silver;

        private Rectangle bounds => new Rectangle(Bounds.X + (int)start.X, Bounds.Y + (int)start.Y, Bounds.Width, Bounds.Height);
        private Texture2D fill, border;

        public Panel(bool only = false)
        {
            OnlyArea = only;
        }

        protected override void draw()
        {
            if (!OnlyArea)
            {
                sprite.Draw(border, bounds, Color.White);
                sprite.Draw(fill, new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2), Color.White);
            }
            controls.ForEach(c => c.Draw());
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            controls.ForEach(c => c.Update(time, bounds.Location.ToVector2(), keyboard, mouse));
        }

        protected override void load()
        {
            if (!OnlyArea)
            {
                fill = sprite.GetColorFill(Fill);
                border = sprite.GetColorFill(Border);
            }
            controls.ForEach(c => c.Load(graphics, content, sprite));
        }

        public Panel(IEnumerable<Control> controls)
        {
            this.controls.AddRange(controls);
        }

        public void AddRange(IEnumerable<Control> controls)
        {
            controls.ToList().ForEach(c => c.Load(graphics, content, sprite));
            this.controls.AddRange(controls);
        }

        public void Add(Control control, bool isloading = true)
        {
            if (!isloading)
                control.Load(graphics, content, sprite);
            controls.Add(control);
        }

        public void Remove(Control control) => controls.Remove(control);
        public void RemoveAll(Predicate<Control> match) => controls.RemoveAll(match);
    }
}
