using Microsoft.Xna.Framework;
using PirateAdventures.GameObjects;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
using PirateAdventures.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Factories
{
    public class HeroFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "hero";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            KeyboardInputReader kir = new KeyboardInputReader(SettingsManager.LoadSettings());
            return new Hero(kir, TextureManager.Instance.GetTextureAtlas("heroAtlas"))
            {
                Position = new Vector2(position.X, position.Y - Hero.SPRITE_HEIGHT)
            };
        }
    }
}
