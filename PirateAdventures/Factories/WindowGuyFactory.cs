using Microsoft.Xna.Framework;
using PirateAdventures.GameObjects.Enemies;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Factories
{
    public class WindowGuyFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "window";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            return new WindowGuy(TextureManager.Instance.GetTextureAtlas("windowguyAtlas"))
            {
                Position = new Vector2(position.X, position.Y - WindowGuy.SPRITE_HEIGHT)
            };
        }
    }
}
