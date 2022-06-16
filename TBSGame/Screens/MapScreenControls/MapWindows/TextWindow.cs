using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TBSGame.Screens.MapScreenControls.MapWindows
{
	public delegate void HideWindowEventHandler(object sender);

    public class TextWindow : MapWindow
    {
        private Rectangle bounds;
        private string text;

        public event HideWindowEventHandler OnHideWindow;
        public VerticalAligment VerticalAligment { get; set; } = VerticalAligment.Center;
        public HorizontalAligment HorizontalAligment { get; set; } = HorizontalAligment.Left;

        public TextWindow(string text)
        {
            this.text = text;
        }

        protected override void draw()
        {
            sprite.DrawMultiLineText(font, new string[] { text }, bounds, HorizontalAligment, VerticalAligment, 5, Color.White);
        }

        protected override void load()
        {
            bounds = new Rectangle((Width - 800) / 2, (Height - 600) / 2, 800, 600);
        }

        public void Update(MouseState mouse)
        {
            if (Visible)
            {
                if (mouse.RightButton == ButtonState.Pressed || mouse.LeftButton == ButtonState.Pressed)
                {
                    Visible = false;
                    OnHideWindow?.Invoke(this);
                    return;
                }
            }
        }
    }
}
