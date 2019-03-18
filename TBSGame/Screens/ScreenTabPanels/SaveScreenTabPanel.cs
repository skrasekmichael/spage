using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.Controls.TextBoxes;
using TBSGame.MessageBoxes;
using TBSGame.Saver;

namespace TBSGame.Screens.ScreenTabPanels
{
    public class SaveScreenTabPanel : ScreenTabPanel
    {
        public GameSavePanel Saves { get; private set; }
        public MenuButton Exit { get; private set; }


        public SaveScreenTabPanel(Settings settings, GameSave game, string icon) : base(settings, game, icon)
        {
            Saves = new GameSavePanel(settings.GameSaves, GameSavePanel.Display.LoadSave);
            Exit = new MenuButton(Resources.GetString("exit"));
        }

        protected override void draw()
        {
            
        }

        protected override void load()
        {
            Panel.Add(Saves);
            Panel.Add(Exit);
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            Exit.Bounds = new Rectangle(Saves.Bounds.X + Saves.Bounds.Width - 200, Saves.Bounds.Y + Saves.Bounds.Height + 20, 200, 50);
        }

        public override void LoadPosition()
        {
            Saves.LoadPostiton();
        }

        public override void Reload()
        {
            
        }
    }
}
