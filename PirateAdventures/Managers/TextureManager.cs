using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Managers
{
    public class TextureManager
    {
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, TextureAtlas> textureAtlases;

        private static TextureManager instance;
        private TextureManager() { 
            textures = new Dictionary<string, Texture2D>();
            textureAtlases = new Dictionary<string, TextureAtlas>();
        }
        public static TextureManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new TextureManager();
                }
                return instance;
            }
        }

        public void AddTexture(string name, Texture2D texture)
        {
            if (!textures.ContainsKey(name))
            {
                textures[name] = texture;
            }
            else
            {
                Debug.WriteLine($"Texture '{name}' already exists in the texture manager.");
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];
            return null;
        }

        public void AddTextureAtlas(string name, TextureAtlas textureAtlas)
        {
            if (!textureAtlases.ContainsKey(name))
            {
                textureAtlases[name] = textureAtlas;
            }
            else
            {
                Debug.WriteLine($"Texture '{name}' already exists in the texture manager.");
            }
        }

        public TextureAtlas GetTextureAtlas(string name)
        {
            if (textureAtlases.ContainsKey(name))
                return textureAtlases[name];
            return null;
        }
    }
}
