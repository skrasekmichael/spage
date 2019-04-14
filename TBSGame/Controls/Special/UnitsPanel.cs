using MapDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls.GameScreen;

namespace TBSGame.Controls.Special
{
    public delegate void SelectedEventHandler(object sender, UnitBox target);

    public class UnitsPanel : Panel
    {
        public List<UnitBox> Units { get; } = new List<UnitBox>();
        public event SelectedEventHandler OnSelected;
        public bool MultiSelect { get; set; } = false;

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

                if (!MultiSelect)
                {
                    unitbox.Check.CallHandler = false;
                    unitbox.OnControlClicked += new ControlClickedEventHandler(sender =>
                    {
                        UnitBox box = (UnitBox)sender;
                        Units.ForEach(u => u.IsChecked = false);
                        box.IsChecked = true;
                        OnSelected?.Invoke(this, box);
                    });
                }

                index++;
            }
        }

        public void Deselect() => Units.ForEach(u => u.IsChecked = false);
    }
}
