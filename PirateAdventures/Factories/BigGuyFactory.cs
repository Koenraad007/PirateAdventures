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
    public class BigGuyFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "big";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            return new BigGuy(TextureManager.Instance.GetTextureAtlas("bigguyAtlas"))
            {
                Position = new Vector2(position.X - BigGuy.SPRITE_WIDTH / 2, position.Y - BigGuy.SPRITE_HEIGHT)
            };
        }
    }
}
