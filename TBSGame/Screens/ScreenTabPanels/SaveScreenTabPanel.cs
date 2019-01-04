using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SaveScreenTabPanel : ScreenTabPanel
    {
        private Label[] save_name = new Label[10];
        private TextInput[] input = new TextInput[10];
        

        public SaveScreenTabPanel(Settings settings, string icon) : base(settings, icon)
        {

        }

        protected override void draw()
        {
            for (int i = 0; i < 10; i++)
            {
                save_name[i]?.Draw();
            }
        }

        protected override void load()
        {
            string[] files = Directory.GetFiles(settings.GameSaves);
            Array.Sort(files);
            for (int i = 0; i < 10; i++)
            {
                Label label = new Label(Resources.GetString("empty"));
                label.TextColor = Color.White;
                label.Load(graphics, content, sprite);
                label.Bounds = new Rectangle((Width - 100) / 2, (Height - 50) / 2 + i / 2 * 51, 100, 50);
                if (i < files.Length && files[i].EndsWith($"{i}.dat"))
                {
                    GameSave save = GameSave.Load(files[i]);
                    if (save != null)
                    {
                        label.Text = save.Name;
                    }
                }
                save_name[i] = label;
            }
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            for (int i = 0; i < 10; i++)
            {
                save_name[i]?.Update(time, keyboard, mouse);
                input[i]?.Update(time, keyboard, mouse);
            }
        }
    }
}
