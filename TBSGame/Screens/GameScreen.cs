using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using TBSGame.MessageBoxes;
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

        private Label resources, round;

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

        protected override void load()
        {
            this.level = Level.Load(path);
            Texture2D map_texture = sprite.GetTexture(level.Map);

            resources = (Label)parent.GetControl("resources");
            round = (Label)parent.GetControl("round");

            parent.GetControl("next_turn").OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                if (game.Researching == null && game.Research > 0)
                {
                    YesNoMessageBox message = new YesNoMessageBox(Resources.GetString("points_in_researching"));
                    message.OnMessageBox += new MessageBoxEventHandler((obj, result) =>
                    {
                        if (result == DialogResult.Yes)
                            next();
                    });
                    this.ShowMessage(message);
                }
                else
                    next();
            });

            MapScreenTabPanel map = new MapScreenTabPanel(this.path, Settings, game, level, "gamemap");
            map.OnPlayGame += new PlayGameEventhandle((sender, level_map, list) =>
            {
                string path = $"{Path.GetDirectoryName(this.path)}/maps/{level_map.Name}.dat";
                if (File.Exists(path))
                    this.Dispose(new MapScreen(game, Settings, Map.Load(path), level_map.Name, list));
            });
            map.MapTexture = sprite.Clone(map_texture);

            UnitsScreenTabPanel units = new UnitsScreenTabPanel(Settings, game, "barracks");

            ResearchScreenTabPanel research = new ResearchScreenTabPanel(Settings, game, "anvil");

            SourcesScreenTabPanel sources = new SourcesScreenTabPanel(Settings, game, level, "income");
            sources.MapTexture = map_texture;

            #region _saves_
            SaveScreenTabPanel save = new SaveScreenTabPanel(Settings, game, "gamesave");
            save.Saves.OnSaveGame += new SaveGameEventHandler((sender, i, name) =>
            {
                this.game.Name = name;
                string path = Settings.GameSaves + i.ToString() + ".dat";
                this.game.Save(path);
            });
            save.Saves.OnLoadGame += new LoadGameEventHandler((sender, i) =>
            {
                GameSave game = GameSave.Load(Settings.GameSaves + i.ToString() + ".dat");
                Scenario.Load("scenario\\campaign.dat", game.Scenario + "campaign\\");
                if (game != null)
                    this.Dispose(new GameScreen(game));
            });
            save.Saves.OnDeleteGame += new DeleteGameEventHandler((sender, i) =>
            {
                YesNoMessageBox message = new YesNoMessageBox(Resources.GetString("delete?"));
                message.OnMessageBox += new MessageBoxEventHandler((obj, result) =>
                {
                    if (result == DialogResult.Yes)
                        save.Saves.Delete(i);
                });
                this.ShowMessage(message);
            });
            save.Exit.OnControlClicked += new ControlClickedEventHandler(sender =>
            {
                YesNoMessageBox message = new YesNoMessageBox(Resources.GetString("exit?"));
                message.OnMessageBox += new MessageBoxEventHandler((obj, result) =>
                {
                    if (result == DialogResult.Yes)
                        this.Dispose(new MainMenuScreen());
                });
                this.ShowMessage(message);
            });
            #endregion

            tabs = new ScreenTabPanel[] { map, units, research, sources, save };

            int index = 0;
            foreach (ScreenTabPanel tab in tabs)
            {
                tab.Index = index;
                tab.OnSelectedTab += new SelectedTabEventHandler(sender => {
                    ScreenTabPanel stp = (ScreenTabPanel)sender;
                    selected = stp.Index;
                    foreach (ScreenTabPanel _tab in tabs.Where(t => t.Index != index))
                        _tab.Panel.IsVisible = false;
                });
                tab.Load(graphics, parent);
                index++;
            }
        }

        protected override void loadpos()
        {
            tabs?.ToList().ForEach(t => t.LoadPosition());
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            resources.Text = $"{Resources.GetString("gold")}: {game.Sources}";
            round.Text = $"{Resources.GetString("round")}: {game.Round}";

            tabs.ToList().ForEach(t => t.UpdateButton(time, keyboard, mouse));
            tabs[selected].Update(time, keyboard, mouse);
        }

        private void next()
        {
            if (game.Researching != null)
            {
                game.Researching.Done += game.Research;
                if (game.Researching.Done >= game.Researching.ResearchDifficulty)
                {
                    game.Researches.Add(game.GetType());
                    game.Researching = null;
                }
            }

            game.Sources += game.Income;
            game.Round++;

            foreach (Unit unit in game.Units)
            {
                if (unit.Rounds > 0)
                {
                    unit.Rounds--;
                    if (unit.Rounds == 0)
                        unit.UnitStatus = UnitStatus.InBarracks;
                }
            }

            foreach (KeyValuePair<string, MapInfo> kvp in game.Info)
            {
                if (level[kvp.Value.Name].Player == 1)
                {
                    kvp.Value.RoundsToDeplete -= 1;
                    if (kvp.Value.RoundsToDeplete < 0)
                        kvp.Value.RoundsToDeplete = 0;
                }
            }

            foreach (ScreenTabPanel tab in tabs)
                tab.Reload();
        }
    }
}
