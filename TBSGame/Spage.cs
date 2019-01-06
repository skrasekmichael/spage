using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using TBSGame.Screens;

namespace TBSGame
{
    public class Spage : Game
    {
        GraphicsDeviceManager graphics;
        CustomSpriteBatch sprite;
        Screen screen;
        Settings settings;
        TextureDriver driver;
        Texture2D cursor;
        FPSCounter fps = new FPSCounter();

        public Spage(Settings settings)
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
                Error.Log("Resolution error. ");
                Error.Log(ex.Message);
            }

            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            driver = new TextureDriver(Content, sprite);
            screen = new MainMenuScreen();
            screen.Settings = settings;

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
            GC.Collect();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
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
            
            //deleting temp files
            string[] files = Directory.GetFiles(settings.Temp); 
            foreach (string file in files)
                File.Delete(file);

            string[] dirs = Directory.GetDirectories(settings.Temp);
            foreach (string dir in dirs)
                Directory.Delete(dir, true);

            //logging missing keys in dictionary
            List<string> missing = Resources.GetMissing();
            if (missing.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter(settings.MissingFile))
                {
                    foreach (string key in missing)
                        sw.WriteLine(key);
                }
            }
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
