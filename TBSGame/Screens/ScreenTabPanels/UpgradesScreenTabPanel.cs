using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Controls.GameScreen;
using TBSGame.Controls.Special;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class UpgradesScreenTabPanel : ScreenTabPanel
    {
        [LayoutControl("units")]
        private UnitsPanel units_panel;

        [LayoutControl] private UnitUpgrades unit_upgrades;
        [LayoutControl] private Panel upgrades;

        public UpgradesScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon) { }

        public override void Reload()
        {
            this.Deselect();
        }

        public override void Deselect()
        {
            units_panel.LoadUnits(game.Units);
            unit_upgrades.Reload();
            unit_upgrades.IsVisible = false;
            upgrades.Controls.Clear();
            units_panel.Deselect();
        }

        protected override void load()
        {
            units_panel.LoadUnits(game.Units);
            units_panel.OnSelected += new SelectedEventHandler((sender, unitbox) =>
            {
                unit_upgrades.SetUnit(unitbox.Unit);
                load_upgrades(unitbox.Unit.Type);
            });

            unit_upgrades.UpgradeButton.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                foreach (UnitUpgrade upgrade in unit_upgrades.GetUpgrades())
                    upgrade.Apply(unit_upgrades.Unit);

                unit_upgrades.Unit.Rounds += unit_upgrades.Rounds;
                unit_upgrades.Unit.UnitStatus = UnitStatus.Upgrading;
                game.Sources -= unit_upgrades.Total;
            });
        }

        private void load_upgrades(UnitType unittype)
        {
            upgrades.Controls.Clear();

            int index = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(UnitUpgrade)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(UnitUpgrade)))
                {
                    UnitUpgrade upgrade = (UnitUpgrade)Activator.CreateInstance(type);
                    if (upgrade.Group.Contains(unittype) && game.Researches.Intersect(upgrade.Researches).Any())
                    {
                        CheckBox btn = new CheckBox(Resources.GetString(upgrade.ToString()));
                        btn.Tag = upgrade;
                        btn.OnControlClicked += new ControlClickedEventHandler(sender =>
                        {
                            CheckBox obj = (CheckBox)sender;
                            bool val = obj.IsChecked;

                            foreach (CheckBox checkbox in upgrades.Controls.Where(control => control.GetType() == typeof(CheckBox)))
                            {
                                if (((UnitUpgrade)checkbox.Tag).Type == ((UnitUpgrade)obj.Tag).Type)
                                    checkbox.IsChecked = false;
                            }

                            obj.IsChecked = val;
                            unit_upgrades.SetUpgrade((UnitUpgrade)obj.Tag, val);
                        });

                        upgrades.Add(btn, index);
                        index++;
                    }
                }
            }
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            base.update(time, keyboard, mouse);
            unit_upgrades.UpgradeButton.IsLocked = game.Sources < unit_upgrades.Total;
        }
    }
}
