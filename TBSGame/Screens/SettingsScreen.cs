using TBSGame.Controls;
using TBSGame.Controls.Buttons;
using System.IO;

namespace TBSGame.Screens
{
	public class SettingsScreen : Screen
    {
        [LayoutControl] private Panel lang;

        protected override void load()
        {
            ControlClickedEventHandler handler = new ControlClickedEventHandler(sender =>
            {
                string[] resolution = ((MenuButton)sender).Text.Split('x');
                graphics.GraphicsDeviceManager.PreferredBackBufferWidth = int.Parse(resolution[0]);
                graphics.GraphicsDeviceManager.PreferredBackBufferHeight = int.Parse(resolution[1]);
                graphics.GraphicsDeviceManager.ApplyChanges();
                Reload();
            });

            for (int i = 0; i < 8; i++)
                parent.GetControl("btn_" + (i + 1).ToString()).OnControlClicked += handler;

            parent.GetControl("back").OnControlClicked += new ControlClickedEventHandler(sender => this.Dispose(new MainMenuScreen()));

            string[] langs = Directory.GetFiles("Resources/Lang");
            for (int i = 0; i < langs.Length; i++)
            {
                MenuButton button = new MenuButton(langs[i]);
                button.OnControlClicked += new ControlClickedEventHandler(sender =>
                {
                    Resources.Load(((MenuButton)sender).Text);
                    this.Dispose(new SettingsScreen());
                });
                lang.Add(button, i);
            }
        }
    }
}
