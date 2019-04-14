using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls.Buttons;

namespace TBSGame.Controls.GameScreen
{
    public class UnitUpgrades : Panel
    {
        public Unit Unit { get; private set; } = null;
        public MenuButton UpgradeButton { get; private set; }

        private Label[] labels;
        private Label price, rounds;
        private MenuButton cancel;
        private Dictionary<UpgradeType, UnitUpgrade> upgrades = new Dictionary<UpgradeType, UnitUpgrade>();

        public int Total { get; set; }
        public int Rounds { get; set; }

        public UnitUpgrades() : base(true)
        {
            this.Description = Resources.GetString("unit_upgrades");
            this.IsVisible = false;
        }

        public void SetUnit(Unit val)
        {
            Unit = val;
            load_upgrades();
            this.IsVisible = true;
        }

        public ICollection<UnitUpgrade> GetUpgrades() => upgrades.Values;

        protected override void load()
        {
            base.load();

            price = new Label($"{Resources.GetString("price")}: 0");
            price.HAligment = HorizontalAligment.Left;
            price.Bounds = new Rectangle(0, bounds.Height - 100, 250, 50);
            this.Add(price);

            rounds = new Label($"{Resources.GetString("rounds")}: 0");
            rounds.HAligment = HorizontalAligment.Left;
            rounds.Bounds = new Rectangle(250, bounds.Height - 100, 250, 50);
            this.Add(rounds);

            UpgradeButton = new MenuButton(Resources.GetString("upgrade"));
            UpgradeButton.Bounds = new Rectangle(0, bounds.Height - 50, 250, 50);
            this.Add(UpgradeButton);

            cancel = new MenuButton(Resources.GetString("cancel"));
            cancel.OnControlClicked += new ControlClickedEventHandler(sender => this.IsVisible = false);
            cancel.Bounds = new Rectangle(251, bounds.Height - 50, 250, 50);
            this.Add(cancel);

            UpgradeType[] enums = (UpgradeType[])Enum.GetValues(typeof(UpgradeType));
            labels = new Label[enums.Length];
            for (int i = 0; i < enums.Length; i++)
            {
                Label title = new Label(Resources.GetString(enums[i].ToString()) + ": ");
                title.Bounds = new Rectangle(0, i * 51, 150, 50);
                title.HAligment = HorizontalAligment.Left;
                this.Add(title);

                labels[i] = new Label(get_research(enums[i]));
                labels[i].Bounds = new Rectangle(150, i * 51, 300, 50);
                labels[i].HAligment = HorizontalAligment.Left;
                this.Add(labels[i]);
            }

            this.DeepForeground = Color.White;
        }

        public void SetUpgrade(UnitUpgrade upgrade, bool val)
        {
            int index = (int)upgrade.Type;
            if (val && (!Unit.Upgrades.ContainsKey(upgrade.Type) || Unit.Upgrades[upgrade.Type] != upgrade))
            {
                if (upgrades.ContainsKey(upgrade.Type))
                    upgrades[upgrade.Type] = upgrade;
                else
                    upgrades.Add(upgrade.Type, upgrade);

                labels[index].Text = Resources.GetString(upgrade.ToString());
                labels[index].Foreground = Color.Orange;
            }
            else
            {
                upgrades.Remove(upgrade.Type);
                set_upgrade(labels[index], upgrade.Type);
            }

            Total = upgrades.Values.Sum(e => e.Price);
            Rounds = upgrades.Values.Sum(e => e.ResearchDifficulty);

            price.Text = $"{Resources.GetString("price")}: {Total}";
            rounds.Text = $"{Resources.GetString("rounds")}: {Rounds}";
        }

        public void Reload()
        {
            load_upgrades();
        }

        private void set_upgrade(Label label, UpgradeType type)
        {
            label.Text = get_research(type);
            label.Foreground = Color.White;
        }

        private void load_upgrades()
        {
            UpgradeType[] enums = (UpgradeType[])Enum.GetValues(typeof(UpgradeType));
            for (int i = 0; i < enums.Length; i++)
                set_upgrade(labels[i], enums[i]);
        }

        private string get_research(UpgradeType type)
        {
            if (Unit == null)
                return Resources.GetString("default");

            if (Unit.Upgrades.ContainsKey(type))
                return Resources.GetString(Unit.Upgrades[type].ToString());

            return Resources.GetString("default");
        }
    }
}
