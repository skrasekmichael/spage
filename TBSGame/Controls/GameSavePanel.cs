using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls.Buttons;
using TBSGame.Controls.TextBoxes;
using TBSGame.MessageBoxes;
using TBSGame.Saver;

namespace TBSGame.Controls
{
    public delegate void SaveGameEventHandler(object sender, int index, string name);
    public delegate void LoadGameEventHandler(object sender, int index);
    public delegate void DeleteGameEventHandler(object sender, int index);

    public class GameSavePanel : Control
    {
        public event SaveGameEventHandler OnSaveGame;
        public event LoadGameEventHandler OnLoadGame;
        public event DeleteGameEventHandler OnDeleteGame;

        [Flags]
        public enum Display
        {
            Load, Save, LoadSave
        }

        private Panel panel = new Panel(true);

        private NormalTextBox[] input = new NormalTextBox[10];
        private MenuButton[] delete_buttons = new MenuButton[10];
        private MenuButton[] save_buttons = new MenuButton[10];
        private MenuButton[] load_buttons = new MenuButton[10];
        private Label[] labels = new Label[10];
        private string path;
        private Display type;

        private bool ShowLoad => type == Display.Load || type == Display.LoadSave;
        private bool ShowSave => type == Display.Save || type == Display.LoadSave;

        public GameSavePanel(string path, Display type)
        {
            this.path = path;
            this.type = type;
        }

        public void Delete(int index)
        {
            File.Delete(path + index.ToString() + ".dat");
            Reload(index);
        }

        public void Reload(int index)
        {
            string file = path + index.ToString() + ".dat";
            bool a = false;
            GameSave save = null;

            if (File.Exists(file))
            {
                save = GameSave.Load(file);
                a = (save != null);
            }

            input[index].SetText(a ? save.Name : "");
            labels[index].Text = a ? save.ScenarioName + "\n" + save.SavedAt.ToString("dd.MM.yyyy HH:mm:ss") : "";
            delete_buttons[index].IsLocked = !a;
            if (ShowLoad)
                load_buttons[index].IsLocked = !a;
        }

        protected override void draw()
        {
            panel.Draw();
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            panel.Update(time, keyboard, mouse);
        }

        public void LoadPostiton()
        {
            int width = (int)(Width * 0.5f);
            panel.Bounds = new Rectangle((Width - width) / 2, (Height - 510) / 2, width, 510);
            for (int i = 0; i < 10; i++)
            {
                delete_buttons[i].Bounds = new Rectangle(0, i * 51, 150, 50);
                int w = panel.Bounds.Width - 151;
                if (ShowLoad)
                {
                    w -= 151;
                    load_buttons[i].Bounds = new Rectangle(151 + w + 1, i * 51, 150, 50);
                }
                if (ShowSave)
                {
                    w -= 151;
                    save_buttons[i].Bounds = new Rectangle(151 + w + 1, i * 51, 150, 50);
                }

                input[i].Bounds = new Rectangle(151, i * 51, w, 50);
                labels[i].Bounds = new Rectangle(panel.Bounds.Width, i * 51, 300, 50);
            }
        }

        protected override void load()
        {
            panel.Load(graphics, content, sprite);

            for (int i = 0; i < 10; i++)
            {
                NormalTextBox textbox = new NormalTextBox();
                textbox.PlaceHolder = Resources.GetString("empty");
                textbox.IsLocked = true;
                textbox.Tag = i;
                textbox.OnConfirm += new TextBoxConfirmedEventHandler(sender =>
                {
                    ((TextBox)sender).IsLocked = true;
                    int index = (int)((TextBox)sender).Tag;
                    OnSaveGame?.Invoke(this, index, ((TextBox)sender).Text);
                    Reload(index);
                });
                panel.Add(textbox);

                MenuButton del_btn = new MenuButton(Resources.GetString("delete"));
                del_btn.Tag = i;
                del_btn.IsLocked = true;
                delete_buttons[i] = del_btn;
                del_btn.OnControlClicked += new ControlClickedEventHandler(sender => OnDeleteGame?.Invoke(this, (int)((Button)sender).Tag));
                panel.Add(del_btn);

                Label label = new Label("");
                label.Foreground = Color.Silver;
                label.HAligment = HorizontalAligment.Left;
                labels[i] = label;
                panel.Add(label);
                label.Font = content.Load<SpriteFont>("fonts/small");

                MenuButton load_btn = new MenuButton(Resources.GetString("load"));
                load_btn.Tag = i;
                load_btn.OnControlClicked += new ControlClickedEventHandler(sender => OnLoadGame?.Invoke(this, (int)((MenuButton)sender).Tag));
                load_btn.IsLocked = true;
                load_buttons[i] = load_btn;
                panel.Add(load_btn);

                if (!ShowLoad)
                    load_btn.IsVisible = false;

                MenuButton save_btn = new MenuButton(Resources.GetString("save"));
                save_btn.Tag = i;
                save_btn.OnControlClicked += new ControlClickedEventHandler(sender =>
                {
                    int index = (int)((Button)sender).Tag;
                    input[index].IsLocked = false;
                    input[index].Focus();
                });
                save_buttons[i] = save_btn;
                panel.Add(save_btn);

                if (!ShowSave)
                    save_btn.IsVisible = false;

                string file = path + i.ToString() + ".dat";
                if (File.Exists(file))
                {
                    GameSave save = GameSave.Load(file);
                    if (save != null)
                    {
                        textbox.SetText(save.Name);
                        label.Text = save.ScenarioName + "\n" + save.SavedAt.ToString("dd.MM.yyyy HH:mm:ss");
                        delete_buttons[i].IsLocked = false;
                        load_buttons[i].IsLocked = false;
                    }
                }
                input[i] = textbox;
            }
        }
    }
}
