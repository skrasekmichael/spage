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
        public List<Control> Controls { get; private set; } = new List<Control>();

        public bool OnlyArea { get; private set; } = false;
        public bool Desc { get; set; } = false;
        public string Description { get; set; } = "";
        public bool DescConstantly { get; set; } = false;

        private Color _fill = Color.Black;
        public Color Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                graphics?.Sprite.SetColorFill(ref fill, _fill);
            }
        }

        private bool fore_is_changed = false;
        public override Color Foreground
        {
            get => _fore;
            set
            {
                fore_is_changed = true;
                _fore = value;
                Controls.ForEach(c => c.Foreground = value);
            }
        }

        private Texture2D fill, bg_fill;

        public Panel(bool only = false)
        {
            OnlyArea = only;
            Border.IsVisible = !only;
            Border.Color = Color.Silver;
            Desc = only;
        }

        public override void Draw()
        {
            if (Desc)
                sprite.Draw(bg_fill, bounds, Color.White * 0.4f);

            if (DescConstantly || !IsVisible)
            {
                if (Desc)
                    sprite.DrawString(Font, Description, new Vector2(bounds.X + 20, bounds.Y + 20), Color.White * 0.4f);
            }

            if (IsVisible)
            {
                draw();
                draw_border();
            }
        }

        protected override void draw()
        {
            if (!OnlyArea)
                sprite.Draw(fill, bounds, Color.White);

            Controls.ForEach(c => c.Draw());
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Controls.ForEach(control => control.Update(time, keyboard, mouse, bounds.Location.ToVector2()));
        }

        protected override void load()
        {
            Border.MouseOverColor = Border.Color;

            bg_fill = sprite.GetColorFill(new Color(50, 50, 50));
            fill = sprite.GetColorFill(Fill);

            Controls.ForEach(load);
        }

        public void AddRange(IEnumerable<Control> controls)
        {
            controls.ToList().ForEach(c => Add(c));            
        }

        public void Add(Control control, bool isloading = true)
        {
            if (isloading)
                load(control);
            Controls.Add(control);
        }

        private void load(Control control)
        {
            if (fore_is_changed)
                control.Foreground = this.Foreground;

            control.Opacity *= this.Opacity;
            control.Load(graphics, bounds.Location.ToVector2());
        }

        public Control GetControl(string name)
        {
            Queue<Control> controls = new Queue<Control>(this.Controls); 
            while (controls.Count > 0)
            {
                Control control = controls.Dequeue();
                if (control.Name == name)
                    return control;

                if (control.GetType() == typeof(Panel))
                    ((Panel)control).Controls.ForEach(c => controls.Enqueue(c));
            }

            return null;
        }
    }
}
