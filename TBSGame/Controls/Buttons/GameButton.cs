using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Controls.Buttons
{
    public class GameButton : Button
    {
        public Color Tint { get; set; } = Color.Red;

        private Texture2D icon, hover_icon, locked;
        private string ic;

        public GameButton(string title, string icon) : base(title)
        {
            this.TextColor = Color.Black;
            this.MouseOverFill = new Color(40, 40, 40);
            this.Fill = new Color(80, 80, 80);
            ic = icon;
        }

        protected override void draw()
        {
            sprite.Draw(IsLocked ? locked_bg : (is_mouse_hover ? background_over_fill : background_fill), bounds, Color.White);
            int padding = 8;
            sprite.Draw(IsLocked ? locked : (is_mouse_hover ? hover_icon : icon), new Rectangle(bounds.X + padding, bounds.Y + padding, Bounds.Width - 2 * padding, Bounds.Height - 2 * padding), Color.White);
        }

        protected override void load()
        {
            base.load();
            icon = content.Load<Texture2D>($"icons/{ic}");
            hover_icon = sprite.Tint(icon, Tint);
            locked = sprite.Tint(icon, Color.Silver);

            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
        }
    }
}
