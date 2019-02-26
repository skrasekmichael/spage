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

        private Color _fill = Color.Black;
        public Color Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                fill = sprite?.GetColorFill(_fill);
            }
        }

        public override Color Frame { get; set; } = Color.Silver;
        private bool fore_is_changed = false;
        public override Color Foreground
        {
            get => _fore;
            set
            {
                fore_is_changed = true;
                _fore = value;
                controls.ForEach(c => c.Foreground = value);
            }
        }

        private Texture2D fill;

        public Panel(bool only = false)
        {
            OnlyArea = only;
            Border.IsVisible = !only;
        }

        protected override void draw()
        {
            if (!OnlyArea)
                sprite.Draw(fill, bounds, Color.White);
            controls.ForEach(c => c.Draw());
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            controls.ForEach(c => c.Update(time, keyboard, mouse, bounds.Location.ToVector2()));
        }

        protected override void load()
        {
            MouseOverFrame = Frame;

            if (!OnlyArea)
                fill = sprite.GetColorFill(Fill);

            controls.ForEach(load);
        }

        public Panel(IEnumerable<Control> controls)
        {
            this.controls.AddRange(controls);
        }

        public void AddRange(IEnumerable<Control> controls)
        {
            controls.ToList().ForEach(c => Add(c));            
        }

        public void Add(Control control, bool isloading = true)
        {
            if (isloading)
                load(control);
            controls.Add(control);
        }

        private void load(Control control)
        {
            if (fore_is_changed)
                control.Foreground = this.Foreground;
            control.Load(graphics, content, sprite, bounds.Location.ToVector2());
        }

        public void Remove(Control control) => controls.Remove(control);
        public void RemoveAll(Predicate<Control> match) => controls.RemoveAll(match);
    }
}
