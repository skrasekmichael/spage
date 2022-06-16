using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TBSGame.Controls.Buttons;
using Microsoft.Xna.Framework.Input;
using TBSGame.Saver;
using TBSGame.Controls;
using System.Reflection;
using System.IO;

namespace TBSGame.Screens.ScreenTabPanels
{
	public delegate void SelectedTabEventHandler(object sender);
    public delegate void RefreshDataEventHandlet(object sender);

    public abstract class ScreenTabPanel : Layout
    {
        public event RefreshDataEventHandlet OnRefresh;
        protected void Refresh() => OnRefresh?.Invoke(this);

        public event SelectedTabEventHandler OnSelectedTab;
        private void SelectTab()
        {
            OnSelectedTab?.Invoke(this);
            Panel.IsVisible = true;
        }

        protected ContentManager content => graphics.Content;
        protected CustomSpriteBatch sprite => graphics.Sprite;
        protected Graphics graphics { get; private set; }
        public GameButton Button { get; private set; }
        protected Settings settings;
        protected GameSave game;

        public Panel Panel = new Panel(false);

        public int Width => graphics.ScreenWidth;
        public int Height => graphics.ScreenHeight;

        public SpriteFont Font { get; set; }
        public int Index { get; set; }

        public ScreenTabPanel(Settings settings, GameSave game, string icon)
        {
            Button = new GameButton(icon, icon);
            Button.Tint = Color.Lime;
            this.game = game;
            this.settings = settings;
        }

        public void Load(Graphics graphics, Panel parent)
        {
            this.graphics = graphics;

            Panel.Fill = new Color(10, 10, 10);
            Panel.Border.IsVisible = false;
            Panel.Bounds = new Rectangle(0, 0, Width - 100, Height);
            Panel.Desc = false;
            Panel.IsVisible = false;
            ((Panel)parent.GetControl("panel")).Add(Panel);

            Button.Load(graphics);
            Button.Bounds = new Rectangle(Width - 99, 100 * Index, 99, 99);
            Button.OnControlClicked += new ControlClickedEventHandler(sender => SelectTab());

            Assembly assembly = Assembly.GetCallingAssembly();
            string res = "TBSGame.Layout.GameScreen." + this.GetType().Name + ".xml";

            Stream stream = assembly.GetManifestResourceStream(res);
            if (stream != null)
            {
                LayoutLoader layout = new LayoutLoader(settings, stream);
                layout.Load(graphics, Panel);
                base.LoadControls(Panel);
            }

            load();
        }

        protected abstract void load();

        public void UpdateButton(GameTime time, KeyboardState keyboard, MouseState mouse) => Button.Update(time, keyboard, mouse);

        public void Update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            this.Panel.IsVisible = true;
            update(time, keyboard, mouse);
        }

        protected virtual void update(GameTime time, KeyboardState keyboard, MouseState mouse) { }

        public void DrawButton() => Button.Draw();
        public void Draw() => draw();

        protected virtual void draw() { }
        public virtual void LoadPosition() { }
        public virtual void Reload() { }
        public virtual void Deselect() { }
    }
}
