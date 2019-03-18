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
        private List<UnitBox> buy_units = new List<UnitBox>();
        private List<UnitBox> units = new List<UnitBox>();
        private UnitBuyBox buy;

        private Panel buy_unist_panel = new Panel(true);
        private Panel units_panel = new Panel(true);
        private Panel info_panel = new Panel(true);

        public UnitsScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            
        }

        public override void Reload()
        {
            load_buy();
            load_units();
        }

        public override void LoadPosition()
        {

        }

        protected override void draw()
        {
            
        }

        protected override void load()
        {
            info_panel.Description = Resources.GetString("units_info");
            info_panel.DescConstantly = true;
            units_panel.Description = Resources.GetString("units");
            buy_unist_panel.Description = Resources.GetString("available_units");

            units_panel.Bounds = new Rectangle(10, 10, (int)(Panel.Bounds.Width * 0.6f), Panel.Bounds.Height - 20);
            buy_unist_panel.Bounds = new Rectangle(units_panel.Bounds.Width + 20, 10, Panel.Bounds.Width - units_panel.Bounds.Width - 30, Panel.Bounds.Height - 430);
            info_panel.Bounds = new Rectangle(units_panel.Bounds.Width + 20, Panel.Bounds.Height - 410, buy_unist_panel.Bounds.Width, 400);
            Panel.AddRange(new[] { buy_unist_panel, units_panel, info_panel });

            load_buy();
            load_units();

            buy = new UnitBuyBox(null);
            buy.Bounds = new Rectangle(0, 0, info_panel.Bounds.Width, info_panel.Bounds.Height);
            buy.Recruit.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                Unit unit = (Unit)Activator.CreateInstance(((Unit)((Control)sender).Tag).GetType(), new object[] { (byte)1 });
                unit.Rounds = unit.Training;
                unit.UnitStatus = UnitStatus.Training;
                game.Units.Add(unit);
                game.Sources -= unit.Price;
                buy.Recruit.IsLocked = game.Sources < buy.Unit.Price;
                load_units();
            });
            buy.Cancel.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                buy_units.ForEach(u => u.IsChecked = false);
                buy.IsVisible = false;
            });

            info_panel.Add(buy);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {

        }

        private void load_buy()
        {
            int index = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(Unit)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(Unit)))
                {
                    Unit unit = (Unit)Activator.CreateInstance(type, new object[] { (byte)1 });
                    if (unit.Researches.All(game.Researches.Contains))
                    {
                        UnitBox unitbox;
                        if (index < buy_units.Count)
                        {
                            unitbox = buy_units[index];
                            unitbox.Unit = unit;
                        }
                        else
                        {
                            unitbox = new UnitBox(unit);
                            buy_units.Add(unitbox);
                            buy_unist_panel.Add(unitbox);
                        }

                        unitbox.Bounds = new Rectangle(0, index * 50, 350, 50);
                        unitbox.Checked = Color.Crimson;
                        unitbox.UnChecked = new Color(60, 60, 60);
                        unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                        {
                            buy_units.ForEach(u => u.IsChecked = false);
                            buy.Reload(((UnitBox)sender).Unit);
                            buy.Recruit.IsLocked = game.Sources < ((UnitBox)sender).Unit.Price;
                        });
                        index++;
                    }
                }
            }
        }

        private void load_units()
        {
            int index = 0;
            foreach (Unit unit in game.Units)
            {
                UnitBox unitbox;
                if (index < units.Count)
                {
                    unitbox = units[index];
                    unitbox.Unit = unit;
                }
                else
                {
                    unitbox = new UnitBox(unit);
                    units.Add(unitbox);
                    units_panel.Add(unitbox);
                }

                unitbox.Bounds = new Rectangle(0, index * 50, 350, 50);
                unitbox.Checked = Color.Crimson;
                unitbox.UnChecked = new Color(60, 60, 60);
                unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                {
                    units.ForEach(u => u.IsChecked = false);
                });
                index++;
            }
        }
    }
}
