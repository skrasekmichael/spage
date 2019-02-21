using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls.Buttons;

namespace TBSGame.Controls
{
    public class TabPanel : Control
    {
        private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();
        private Dictionary<string, TabPanelButton> buttons = new Dictionary<string, TabPanelButton>();
        private string selected = null;

        public void Add(string key, Panel panel, TabPanelButton button)
        {
            if (!panels.ContainsKey(key))
            {
                panels.Add(key, panel);
                button.Tag = key;
                button.OnControlClicked += new ControlClickedEventHandler((obj) =>
                {
                    TabPanelButton sender = (TabPanelButton)obj;
                    Select((string)sender.Tag);
                });
                buttons.Add(key, button);
            }

            if (selected == null)
                Select(key);
        }

        private void deselect()
        {
            if (selected != null)
            {
                panels[selected].IsVisible = false;
                buttons[selected].TextColor = Color.White;
            }
        }

        public void Select(string key)
        {
            if (key != null && panels.ContainsKey(key))
            {
                deselect();
                selected = key;
                panels[selected].IsVisible = true;
                buttons[selected].TextColor = Color.Orange;
            }
        }

        protected override void load()
        {
            buttons.Values.ToList().ForEach(btn => btn.Load(graphics, content, sprite));
            panels.Values.ToList().ForEach(p => p.Load(graphics, content, sprite));
        }

        protected override void draw()
        {
            panels[selected].Draw();
            buttons.Values.ToList().ForEach(btn => btn.Draw());
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            buttons.Values.ToList().ForEach(btn => btn.Update(time, keyboard, mouse));
            panels[selected].Update(time, keyboard, mouse);
        }
    }
}
