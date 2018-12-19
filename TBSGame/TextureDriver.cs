using MapDriver;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame
{
    public class TextureDriver
    {
        private Dictionary<string, Texture2D> data = new Dictionary<string, Texture2D>();
        private ContentManager content;
        private CustomSpriteBatch sprite;

        public TextureDriver(ContentManager content, CustomSpriteBatch sprite)
        {
            this.content = content;
            this.sprite = sprite;
        }

        public void Load(string key, string path)
        {
            try
            {
                LoadTexture(key, LoadContent(path));
            } 
            catch (Exception ex)
            {
                Error.Log(ex.Message);
            }
        }

        public void LoadTexture(string key, Texture2D value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }

        public void LoadTexture(string key)
        {
            try
            {
                LoadTexture(key, LoadContent($"Textures/{key}"));
            }
            catch (Exception ex)
            {
                Error.Log(ex.Message);
            }
        }

        public void LoadObject(string key)
        {
            try
            {
                LoadTexture(key, LoadContent($"MapObjects/{key}"));
            }
            catch (Exception ex)
            {
                Error.Log(ex.Message);
            }
        }

        public void LoadUnit(string key)
        {
            try
            {
                LoadTexture(key, LoadContent($"Units/{key}"));
            }
            catch (Exception ex)
            {
                Error.Log(ex.Message);
            }
        }

        public Texture2D LoadContent(string path) => content.Load<Texture2D>(path);

        public Texture2D this[string key]
        {
            get
            {
                if (data.ContainsKey(key))
                    return data[key];
                else
                    return null;
            }
        }
    }
}
