﻿using System;
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
        public Color? Foreground { get; set; } = null;

        private Texture2D fill;

        public Panel(bool only = false)
        {
            OnlyArea = only;
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
            if (!OnlyArea)
            {
                Border.IsVisible = true;
                MouseOverFrame = Frame;
            }
        }

        protected override void load()
        {
            if (!OnlyArea)
                fill = sprite.GetColorFill(Fill);

            controls.ForEach(c => c.Load(graphics, content, sprite, bounds.Location.ToVector2()));
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
                control.Load(graphics, content, sprite, bounds.Location.ToVector2());
            controls.Add(control);

            if (Foreground != null)
            {
                if (control.GetType() == typeof(Button))
                    ((Button)control).TextColor = Foreground.Value;
                else if (control.GetType() == typeof(Label))
                    ((Label)control).TextColor = Foreground.Value;
            }
        }

        public void Remove(Control control) => controls.Remove(control);
        public void RemoveAll(Predicate<Control> match) => controls.RemoveAll(match);
    }
}
