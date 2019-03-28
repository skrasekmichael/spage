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
using TBSGame.Controls.Buttons;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public delegate void CancelResearchingEventHandler(object sender);

    public class ResearchScreenTabPanel : ScreenTabPanel
    {
        public CancelResearchingEventHandler OnCancelResearching;

        private List<MenuButton> researches = new List<MenuButton>();
        private Label name, points, percent;
        private Panel bar, frame, researches_panel;
        private MenuButton cancel;

        public ResearchScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon) { }

        protected override void load()
        {
            name = (Label)Panel.GetControl("name");
            points = (Label)Panel.GetControl("points");
            percent = (Label)Panel.GetControl("percent");
            bar = (Panel)Panel.GetControl("bar_fill");
            frame = (Panel)Panel.GetControl("bar_frame");
            researches_panel = (Panel)Panel.GetControl("researches");
            cancel = (MenuButton)Panel.GetControl("cancel");
            cancel.OnControlClicked += new ControlClickedEventHandler(sender => OnCancelResearching?.Invoke(this));

            load_researching();
            load_reasearchs();
        }

        private void load_reasearchs()
        {
            int index = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(Research)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(Research)) && 
                    !type.IsSubclassOf(typeof(Technology)) && 
                    !type.IsSubclassOf(typeof(UnitUpgrade)) && 
                    !type.IsAbstract)
                {
                    Research res = (Research)Activator.CreateInstance(type);
                    if (res.Researches.All(game.Researches.Contains) && 
                        !game.Researches.Contains(res.GetType()) && 
                        res.GetType() != game.Researching?.GetType()) 
                    {
                        MenuButton research;
                        if (index < researches.Count)
                            research = researches[index];
                        else
                        {
                            research = new MenuButton("");
                            researches.Add(research);
                            researches_panel.Add(research, index);
                        }

                        research.Tag = res.GetType();
                        research.Text = Resources.GetString(res.GetType().Name);
                        research.OnControlClicked += new ControlClickedEventHandler(sender =>
                        {
                            game.Researching = (Research)Activator.CreateInstance((Type)((Control)sender).Tag);
                            this.Refresh();
                        });
                        index++;
                    }
                }
            }

            if (index < researches.Count)
            {
                foreach (MenuButton btn in researches.GetRange(index, researches.Count - index))
                {
                    researches_panel.Controls.Remove(btn);
                    researches.Remove(btn);
                }
            }

            researches_panel.DescConstantly = researches.Count == 0;
        }

        private void load_researching()
        {
            if (game.Researching != null)
            {
                double percent = (double)game.Researching.Done / game.Researching.ResearchDifficulty;

                name.Text = Resources.GetString(game.Researching.GetType().Name);
                points.Text = $"{game.Researching.Done.ToString()}/{game.Researching.ResearchDifficulty.ToString()}";
                this.percent.Text = (percent * 100).ToString() + "%";
                bar.Bounds = new Rectangle(bar.Bounds.X, bar.Bounds.Y, (int)(frame.Bounds.Width * percent), bar.Bounds.Height);
                cancel.IsVisible = true;
            }
            else
            {
                name.Text = Resources.GetString("none_researching");
                points.Text = "";
                percent.Text = "";
                bar.Bounds = new Rectangle(bar.Bounds.X, bar.Bounds.Y, 0, bar.Bounds.Height);
                cancel.IsVisible = false;
            }
        }

        public override void Reload()
        {
            load_researching();
            load_reasearchs();
        }
    }
}
