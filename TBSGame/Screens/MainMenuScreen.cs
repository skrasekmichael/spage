using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TBSGame.Controls;
using TBSGame.Saver;

namespace TBSGame.Screens
{
	public class MainMenuScreen : Screen
    {
        private ImagePanel background;

        protected override void draw_background()
        {
            background.Draw();
        }

        protected override void load()
        {
            background = new ImagePanel(content.Load<Texture2D>("background"));
            background.HAligment = HorizontalAligment.Center;
            background.VAligment = VerticalAligment.Center;
            background.MinBounds = new Rectangle(0, 0, Width, Height);
            background.Load(graphics);

            string[] buttons = { "campaign", "custom", "loadgame", "about", "settings", "exit" };
            ControlClickedEventHandler[] handlers = new ControlClickedEventHandler[] {
                sender => {
                    Scenario scenario = Scenario.Load("Resources\\Scenario\\campaign.dat", Settings.Scenario + "campaign\\");
                    GameSave game = new GameSave(scenario, Settings);
                    Dispose(new GameScreen(game));
                },
                sender => Dispose(null),
                sender => Dispose(new GameSavesScreen()),
                sender => Dispose(new AboutScreen()),
                sender => Dispose(new SettingsScreen()),
                sender => Dispose(null)
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                parent.GetControl("btn_" + buttons[i]).OnControlClicked += new ControlClickedEventHandler(handlers[i]);
            }
        }

        protected override void update(GameTime time, KeyboardState keyboard, MouseState mouse)
        {
            background.Update(time, keyboard, mouse);
        }
    }
}
