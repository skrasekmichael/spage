using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Saver;
using TBSGame.Screens.ScreenTabPanels;

namespace TBSGame.Screens
{
    public class GameScreen : Screen
    {
        private ScreenTabPanel[] tabs;
        private Level level;
        private string path;
        private int selected = 0;
        private GameSave game;

        public GameScreen(GameSave game)
        {
            this.game = game;
            this.path = game.Level;
        }

        protected override void draw()
        {
            tabs.ToList().ForEach(t => t.DrawButton());
            tabs[selected].Draw();
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            this.level = Level.Load(path);

            MapScreenTabPanel map = new MapScreenTabPanel(this.path, Settings, level, "gamemap");
            map.OnPlayGame += new PlayGameEventhandle((sender, level_map) =>
            {
                string path = $"{Path.GetDirectoryName(this.path)}/maps/{level_map.Name}.dat";
                if (File.Exists(path))
                    this.Dispose(new MapScreen(game, Settings, Map.Load(path)));
            });

            SaveScreenTabPanel save = new SaveScreenTabPanel(Settings, "gamesave");
            save.OnSaveGame += new SaveGameEventHandler((sender, i, name) =>
            {
                this.game.Name = name;
                string path = Settings.GameSaves + i.ToString() + ".dat";
                this.game.Save(path);
            });
            save.OnLoadGame += new LoadGameEventHandler((sender, i) =>
            {
                GameSave game = GameSave.Load(Settings.GameSaves + i.ToString() + ".dat");
                Scenario.Load("scenario\\campaign.dat", game.Scenario + "campaign\\");
                if (game != null)
                    this.Dispose(new GameScreen(game));
            });

            tabs = new ScreenTabPanel[] { map, save };

            int index = 0;
            foreach (ScreenTabPanel tab in tabs)
            {
                tab.Index = index;
                tab.OnSelectedTab += new SelectedTabEventHandler(sender => selected = ((ScreenTabPanel)sender).Index);
                tab.Load(graphics, content, sprite);
                tab.LoadPosition();
                index++;
            }
        }

        protected override void loadpos()
        {
            tabs?.ToList().ForEach(t => t.LoadPosition());
        }

        protected override void update(GameTime time)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            tabs.ToList().ForEach(t => t.UpdateButton(time, keyboard, mouse));
            tabs[selected].Update(time, keyboard, mouse);
        }
    }
}
