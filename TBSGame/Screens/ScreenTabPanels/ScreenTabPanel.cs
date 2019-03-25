using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public abstract class ScreenTabPanel
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
        protected GameButton button;
        protected Settings settings;
        protected GameSave game;

        protected Panel UnitsPanel;
        public Panel Panel = new Panel(true);

        public int Width => graphics.ScreenWidth;
        public int Height => graphics.ScreenHeight;

        public SpriteFont Font { get; set; }
        public int Index { get; set; }

        public ScreenTabPanel(Settings settings, GameSave game, string icon)
        {
            button = new GameButton(icon, icon);
            button.Tint = Color.Lime;
            this.game = game;
            this.settings = settings;
        }

        public void Load(Graphics graphics, Panel parent)
        {
            this.graphics = graphics;

            Panel.Bounds = new Rectangle(0, 0, Width - 100, Height);
            Panel.Desc = false;
            Panel.IsVisible = false;
            ((Panel)parent.GetControl("panel")).Add(Panel);

            button.Load(graphics);
            button.Bounds = new Rectangle(Width - 100, 100 * Index, 100, 100);
            button.OnControlClicked += new ControlClickedEventHandler(sender => SelectTab());

            Assembly assembly = Assembly.GetCallingAssembly();
            string res = "TBSGame.Layout.GameScreen." + this.GetType().Name + ".xml";

            Stream stream = assembly.GetManifestResourceStream(res);
            if (stream != null)
            {
                Layout layout = new Layout(settings, stream);
                layout.Load(graphics, Panel);
            }

            load();
        }

        protected abstract void load();

        public virtual void LoadPosition() { }

        public void UpdateButton(GameTime time, KeyboardState keyboard, MouseState mouse) => button.Update(time, keyboard, mouse);

        public void Update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            this.Panel.IsVisible = true;
            if (UnitsPanel != null)
                UnitsPanel.Controls.First().Bounds = new Rectangle(0, 0, UnitsPanel.Bounds.Width, UnitsPanel.Bounds.Height); 
            update(time, keyboard, mouse);
        }

        protected virtual void update(GameTime time, KeyboardState keyboard, MouseState mouse) { }

        public void DrawButton() => button.Draw();

        public void Draw()
        {
            draw();
        }

        protected virtual void draw() { }

        public virtual void Reload() { }

        public void LoadUnitsPanel(Panel units)
        {
            UnitsPanel = (Panel)Panel.GetControl("units_area");
            UnitsPanel.Add(units);
            units.Bounds = new Rectangle(0, 0, UnitsPanel.Bounds.Width, UnitsPanel.Bounds.Height);
        }
    }
}
