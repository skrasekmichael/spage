using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using TBSGame.Screens;

namespace TBSGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        CustomSpriteBatch sprite;
        Screen screen;
        Settings settings;
        TextureDriver driver;
        Texture2D cursor;
        FPSCounter fps = new FPSCounter();

        public Game1(Settings settings)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.settings = settings;
        }

        protected override void Initialize()
        {
            try
            {
                graphics.PreferredBackBufferWidth = settings.ResolutionWidth;
                graphics.PreferredBackBufferHeight = settings.ResolutionHeight;
            }
            catch (Exception ex)
            {
                Error.Log("Chyba rozlišení, soubor s nastavením je pravděpodobně poškozen.");
                Error.Log(ex.Message);
            }

            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            /*
            var ms = System.Windows.Forms.Screen.AllScreens[0];
            form.Left = ms.WorkingArea.Left;
            form.Top = ms.WorkingArea.Top;
            */
            driver = new TextureDriver(Content, sprite);
            screen = new MainMenuScreen();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            sprite = new CustomSpriteBatch(graphics);
            fps.Load(Content);

            cursor = Content.Load<Texture2D>("cursor");
            screen.Load(graphics, Content, sprite, driver);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            GC.Collect();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                screen = screen.Update(gameTime);
                if (screen == null)
                    Exit();

                base.Update(gameTime);
            }
            Error.Update(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            string[] files = Directory.GetFiles($"{Environment.CurrentDirectory}\\temp"); 
            foreach (string file in files)
                File.Delete(file);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (this.IsActive)
            {
                GraphicsDevice.Clear(Color.Black);
                sprite.Begin();

                screen.Draw();

                fps.Update(gameTime);
                fps.Draw(sprite);

                MouseState state = Mouse.GetState();
                sprite.Draw(cursor, new Rectangle(state.X, state.Y, 20, 20), Color.White);

                sprite.End();

                base.Draw(gameTime);
            }
        }
    }
}
