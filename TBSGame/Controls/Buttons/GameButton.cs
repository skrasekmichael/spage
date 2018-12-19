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
        private Texture2D icon, hover_icon;
        private string ic;

        public GameButton(string title, string icon) : base(title)
        {
            this.TextColor = Color.Black;
            this.MouseOverTextColor = Color.YellowGreen;
            this.Fill = Color.CornflowerBlue;
            ic = icon;
        }

        protected override void draw()
        {
            sprite.Draw(is_mouse_hover ? background_over_fill : background_fill, bounds, Color.White);
        }

        public override void Draw()
        {
            draw();
            int padding = 8;
            sprite.Draw(is_mouse_hover ? hover_icon : icon, new Rectangle(bounds.X + padding, bounds.Y + padding, Bounds.Width - 2 * padding, Bounds.Height - 2 * padding), Color.White);
        }

        protected override void load()
        {
            base.load();
            icon = content.Load<Texture2D>($"icons/{ic}");
            hover_icon = sprite.Tint(icon, Color.Blue);

            background_fill = sprite.GetColorFill(Fill);
            background_over_fill = sprite.GetColorFill(MouseOverFill);
        }
    }
}
