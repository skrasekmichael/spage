using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Saver;

namespace TBSGame.Screens.MapScreenControls.MapWindows
{
    public class MenuWindow : MapWindow
    {
        public event EventHandler OnExit;

        private void Exit()
        {
            OnExit?.Invoke(this, new EventArgs());
        }

        private TabPanel tab = new TabPanel();
        private GameMapSaver saver;
        private Panel system, tasks;
        private Label save_counter;
        private Map map;
        public Rectangle Bounds { get; set; }
        private Label[] tasks_labels;
        private int last_count = 0, h = 75, w = 165;

        public MenuWindow(GameMapSaver saver, Map map)
        {
            this.saver = saver;
            this.map = map;
        }

        protected override void load()
        {
            Bounds = new Rectangle((Width - 800) / 2, (Height - 600) / 2, 800, 600);

            #region system_panel
            system = new Panel();

            save_counter = new Label("");
            save_counter.TextColor = Color.Red;
            save_counter.Bounds = new Rectangle(0, Bounds.Height - h, h, h);

            MenuButton save = new MenuButton(Resources.GetString("save"));
            save.Bounds = new Rectangle(h, Bounds.Height - h, w, h);
            save.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                if (saver.Saves < 4 && saver.Saves >= 0)
                    saver.Save(map);
            });

            MenuButton back = new MenuButton(Resources.GetString("go_back"));
            back.Bounds = new Rectangle(h + w, Bounds.Height - h, w, h);
            back.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                
            });

            MenuButton exit = new MenuButton(Resources.GetString("exit"));
            exit.Bounds = new Rectangle(h + 2 * w, Bounds.Height - h, w, h);
            exit.OnButtonClicked += new ButtonClickedEventHandler(sender =>
            {
                Exit();
            });

            system.Add(save);
            system.Add(exit);
            system.Add(back);
            system.Add(save_counter);

            TabPanelButton btn_saves = new TabPanelButton(Resources.GetString("system"));
            tab.Add("system", system, btn_saves);
            #endregion
            #region tasks_panel
            tasks = new Panel();

            Quest[] task_list = map.QuestList;
            tasks_labels = new Label[task_list.Length];
            for (int i = 0; i < task_list.Length; i++)
            {
                Label label = new Label(Resources.GetString(task_list[i].Name));
                label.Bounds = new Rectangle(5, 5 + i * (h + 2), Bounds.Width - w - 10, h);
                label.TextColor = Color.Red;
                tasks_labels[i] = label;
                tasks.Add(label);
            }

            TabPanelButton btn_tasks = new TabPanelButton(Resources.GetString("tasks"));
            tab.Add("tasks", tasks, btn_tasks);
            #endregion

            Panel[] panels = new Panel[] { system, tasks };
            TabPanelButton[] btns = new TabPanelButton[] { btn_saves, btn_tasks };

            tab.Load(graphics, content, sprite);
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Bounds = new Rectangle(Bounds.X + w - 1, Bounds.Y, Bounds.Width - w, Bounds.Height); 
                btns[i].Bounds = new Rectangle(Bounds.X, Bounds.Y + i * (h + 2) + 15, w, h);
                btns[i].BorderRight = false;
            }
        }

        public void Update(GameTime time, Map map, MouseState mouse)
        {
            if (Visible)
            {
                this.map = map;
                tab.Update(time);

                if (mouse.RightButton == ButtonState.Pressed)
                {
                    Visible = false;
                    return;
                }

                Quest[] task_list = map.QuestList;
                for (int i = 0; i < task_list.Length; i++)
                {
                    if (task_list[i].Done)
                        tasks_labels[i].TextColor = Color.Lime;
                }

                save_counter.Text = (4 - saver.Saves).ToString();

                if (last_count != saver.Saves)
                {
                    int i = saver.Saves;
                    Panel save_row = new Panel();
                    Label name = new Label($"{i}. save");
                    name.TextColor = Color.White;
                    MenuButton load = new MenuButton(Resources.GetString("load"));
                    load.OnButtonClicked += new ButtonClickedEventHandler(sender =>
                    {

                    });

                    int row = Bounds.Width - w - 2 * 5;
                    save_row.Bounds = new Rectangle(5, (i - 1) * (h + 2) + 5, row, h);
                    name.Bounds = new Rectangle(0, 0, row - w, h);
                    load.Bounds = new Rectangle(row - w, 0, w, h);

                    save_row.Add(name);
                    save_row.Add(load);
                    system.Add(save_row, false);
                    tab.Update(time);
                }

                last_count = saver.Saves;
            }
        }

        protected override void draw() => tab.Draw();        
    }
}
