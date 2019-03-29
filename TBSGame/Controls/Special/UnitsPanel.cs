using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls.GameScreen;

namespace TBSGame.Controls.Special
{
    public class UnitsPanel : Panel
    {
        public List<UnitBox> Units { get; } = new List<UnitBox>();

        public UnitsPanel(bool val) : base(val) { }

        public void LoadUnits(List<Unit> units)
        {
            int index = 0;
            foreach (Unit unit in units)
            {
                UnitBox unitbox;
                if (index < Units.Count)
                {
                    unitbox = Units[index];
                    unitbox.Unit = unit;
                }
                else
                {
                    unitbox = new UnitBox(unit);
                    Units.Add(unitbox);
                    this.Add(unitbox, index);
                }

                unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                {
                    Units.ForEach(u => u.IsChecked = false);
                });
                index++;
            }
        }

        public void Deselect() => Units.ForEach(u => u.IsChecked = false);
    }
}
