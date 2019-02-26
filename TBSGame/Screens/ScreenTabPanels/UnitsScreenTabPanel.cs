using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Saver;
using TBSGame.Screens.GameScreenControls;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class UnitsScreenTabPanel : ScreenTabPanel
    {
        private List<UnitBox> units = new List<UnitBox>();
        private UnitBuyBox buy;

        private Panel buy_unist_panel = new Panel();
        private Panel units_panel = new Panel();
        private Panel info_panel = new Panel();

        public UnitsScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            
        }

        public override void LoadPosition()
        {

        }

        protected override void draw()
        {
            
        }

        protected override void load()
        {
            units_panel.Bounds = new Rectangle(10, 10, (int)(panel.Bounds.Width * 0.6f), panel.Bounds.Height - 20);
            buy_unist_panel.Bounds = new Rectangle(units_panel.Bounds.Width + 20, 10, panel.Bounds.Width - units_panel.Bounds.Width - 30, panel.Bounds.Height - 430);
            info_panel.Bounds = new Rectangle(units_panel.Bounds.Width + 20, panel.Bounds.Height - 410, buy_unist_panel.Bounds.Width, 400);
            panel.AddRange(new[] { buy_unist_panel, units_panel, info_panel });

            int index = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(Unit)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(Unit)))
                {
                    Unit unit = (Unit)Activator.CreateInstance(type, new object[] { (byte)1 });
                    if (true || unit.Researches.All(game.Researches.Contains))
                    {
                        UnitBox unitbox = new UnitBox(unit);
                        unitbox.Bounds = new Rectangle(0, index * 50, 300, 50);
                        unitbox.Checked = Color.Crimson;
                        unitbox.UnChecked = new Color(60, 60, 60);                        
                        unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                        {
                            units.ForEach(u => u.IsChecked = false);
                            buy.Reload(((UnitBox)sender).Unit);
                        });
                        units.Add(unitbox);
                        index++;
                    }
                }
            }

            buy = new UnitBuyBox(null);
            buy.Bounds = new Rectangle(0, 0, info_panel.Bounds.Width, info_panel.Bounds.Height);

            buy_unist_panel.AddRange(units);
            info_panel.Add(buy);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            
        }
    }
}
