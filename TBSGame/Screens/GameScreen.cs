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
        private MapScreenTabPanel map;
        private Level level;
        private string path;
        private bool loaded = false;
        private GameSave game;

        public GameScreen(GameSave game)
        {
            this.game = game;
            this.path = game.Level;
        }

        protected override void draw()
        {
            map.Draw();
        }

        protected override void load(GraphicsDeviceManager graphics, ContentManager content, CustomSpriteBatch sprite)
        {
            this.level = Level.Load(path);

            map = new MapScreenTabPanel(this.path, level, "gamemap", 0);
            map.OnPlayGame += new PlayGameEventhandle((sender, level_map) =>
            {
                string path = $"{Path.GetDirectoryName(this.path)}/maps/{level_map.Name}.dat";
                if (File.Exists(path))
                    this.Dispose(new MapScreen(game, Map.Load(path)));
            });

            map.Load(graphics, content, sprite);
        }

        protected override void loadpos()
        {

        }

        protected override void update(GameTime time)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            map.Update(time, keyboard, mouse);
        }
    }
}
