﻿using System;
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

        private NormalTextBox[] input = new NormalTextBox[10];
        private MenuButton[] delete_buttons = new MenuButton[10];
        private MenuButton[] save_buttons;
        private MenuButton[] load_buttons;
        private Label[] dates = new Label[10];
        private Label[] scenarios = new Label[10];
        private string path;
        private Display type;

        private bool ShowLoad => type == Display.Load || type == Display.LoadSave;
        private bool ShowSave => type == Display.Save || type == Display.LoadSave;

        public GameSavePanel(string path, Display type)
        {
            this.path = path;
            this.type = type;

            if (ShowLoad)
                load_buttons = new MenuButton[10];
            if (ShowSave)
                save_buttons = new MenuButton[10];
        }

        public void Delete(int index)
        {
            File.Delete(path + index.ToString() + ".dat");
        }

        public override void Draw()
        {
            for (int i = 0; i < 10; i++)
            {
                input[i]?.Draw();
                save_buttons?[i].Draw();
                load_buttons?[i].Draw();
                delete_buttons[i].Draw();
                dates[i].Draw();
                scenarios[i].Draw();
            }
        }

        public override void Update(GameTime time, KeyboardState? keyboard, MouseState? mouse)
        {
            for (int i = 0; i < 10; i++)
            {
                input[i]?.Update(time, keyboard, mouse);
                save_buttons?[i].Update(time, keyboard, mouse);
                load_buttons?[i].Update(time, keyboard, mouse);
                delete_buttons[i].Update(time, keyboard, mouse);
                dates[i].Update(time, keyboard, mouse);
                scenarios[i].Update(time, keyboard, mouse);
            }
        }

        public void LoadPostiton()
        {
            int w = 500;
            for (int i = 0; i < 10; i++)
            {
                Rectangle source = new Rectangle((Width - w) / 2, (Height - 50) / 2 + (i - 5) * 51, w, 50);
                input[i].Bounds = source;
                delete_buttons[i].Bounds = new Rectangle(source.X - 155, source.Y, 150, 50);
                int r = source.X + source.Width + 5;
                if (ShowSave)
                {
                    save_buttons[i].Bounds = new Rectangle(r, source.Y, 150, 50);
                    r += 155;
                }
                if (ShowLoad)
                {
                    load_buttons[i].Bounds = new Rectangle(r, source.Y, 150, 50);
                    r += 155;
                }
                scenarios[i].Bounds = new Rectangle(r, source.Y, 220 + 5, 25);
                dates[i].Bounds = new Rectangle(r, source.Y + 20, 220, 25);
            }
        }

        protected override void load()
        {
            for (int i = 0; i < 10; i++)
            {
                NormalTextBox textbox = new NormalTextBox();
                textbox.Load(graphics, content, sprite);
                textbox.PlaceHolder = Resources.GetString("empty");
                textbox.IsLocked = true;
                textbox.Tag = i;
                textbox.OnConfirm += new TextBoxConfirmedEventHandler(sender => {
                    ((TextBox)sender).IsLocked = true;
                    OnSaveGame?.Invoke(this, (int)((TextBox)sender).Tag, ((TextBox)sender).Text);
                });

                MenuButton del_btn = new MenuButton(Resources.GetString("delete"));
                del_btn.Load(graphics, content, sprite);
                del_btn.Tag = i;
                del_btn.IsLocked = true;
                delete_buttons[i] = del_btn;
                del_btn.OnButtonClicked += new ButtonClickedEventHandler(sender => OnDeleteGame?.Invoke(this, (int)((Button)sender).Tag));

                Label date = new Label("");
                date.Load(graphics, content, sprite);
                date.Font = content.Load<SpriteFont>("fonts/small");
                date.TextColor = Color.Silver;
                date.HAligment = HorizontalAligment.Left;
                dates[i] = date;

                Label scenario = new Label("");
                scenario.Load(graphics, content, sprite);
                scenario.Font = content.Load<SpriteFont>("fonts/small");
                scenario.TextColor = Color.Silver;
                scenario.HAligment = HorizontalAligment.Left;
                scenarios[i] = scenario;

                if (ShowLoad)
                {
                    MenuButton load_btn = new MenuButton(Resources.GetString("load"));
                    load_btn.Load(graphics, content, sprite);
                    load_btn.Tag = i;
                    load_btn.OnButtonClicked += new ButtonClickedEventHandler(sender => OnLoadGame?.Invoke(this, (int)((MenuButton)sender).Tag));
                    load_btn.IsLocked = true;
                    load_buttons[i] = load_btn;
                }

                if (ShowSave)
                {
                    MenuButton save_btn = new MenuButton(Resources.GetString("save"));
                    save_btn.Tag = i;
                    save_btn.OnButtonClicked += new ButtonClickedEventHandler(sender =>
                    {
                        int index = (int)((Button)sender).Tag;
                        input[index].IsLocked = false;
                        input[index].Focus();
                    });
                    save_btn.Load(graphics, content, sprite);
                    save_buttons[i] = save_btn;
                }

                string file = path + i.ToString() + ".dat";
                if (File.Exists(file))
                {
                    GameSave save = GameSave.Load(file);
                    if (save != null)
                    {
                        textbox.SetText(save.Name);
                        date.Text = save.SavedAt.ToString("dd.MM.yyyy HH:mm:ss");
                        scenario.Text = save.ScenarioName;
                        delete_buttons[i].IsLocked = false;
                        if (ShowLoad)
                            load_buttons[i].IsLocked = false;
                    }
                }
                input[i] = textbox;
            }
        }
    }
}