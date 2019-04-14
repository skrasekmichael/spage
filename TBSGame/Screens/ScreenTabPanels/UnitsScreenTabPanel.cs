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
using TBSGame.Controls.GameScreen;
using TBSGame.Saver;
using TBSGame.Controls.Special;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class UnitsScreenTabPanel : ScreenTabPanel
    {
        private List<UnitBox> buy_units = new List<UnitBox>();
        [LayoutControl] private UnitBuyBox buy;
        [LayoutControl] private Panel available, info;
        [LayoutControl] private UnitsPanel units;

        public UnitsScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon) { }

        public override void Reload()
        {
            units.LoadUnits(game.Units);
            load_buy();
            if (buy.IsVisible)
                buy.Recruit.IsLocked = game.Sources < buy.Unit.Price;
        }

        public override void Deselect()
        {
            cancel_buying();
        }

        protected override void load()
        {
            load_buy();
            units.LoadUnits(game.Units);

            buy.Recruit.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                Unit unit = (Unit)Activator.CreateInstance(((Unit)((Control)sender).Tag).GetType(), new object[] { (byte)1 });
                unit.Rounds = unit.Training;
                unit.UnitStatus = UnitStatus.Training;
                unit.Stamina = unit.MaxStamina;
                game.Units.Add(unit);
                game.Sources -= unit.Price;
                buy.Recruit.IsLocked = game.Sources < buy.Unit.Price;
                Refresh();
            });
            buy.Cancel.OnControlClicked += new ControlClickedEventHandler(sender => cancel_buying());
        }

        private void cancel_buying()
        {
            deselect_buy_units();
            buy.IsVisible = false;
        }

        private void deselect_buy_units() => buy_units.ForEach(u => u.IsChecked = false);

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
                            available.Add(unitbox, index);
                        }

                        unitbox.Check.CallHandler = false;
                        unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                        {
                            UnitBox obj = (UnitBox)sender;
                            buy_units.ForEach(ub => ub.IsChecked = false);
                            obj.IsChecked = true;

                            units.Deselect();
                            buy.Reload(obj.Unit);
                            buy.Recruit.IsLocked = game.Sources < obj.Unit.Price;
                        });
                        index++;
                    }
                }
            }
        }
    }
}
