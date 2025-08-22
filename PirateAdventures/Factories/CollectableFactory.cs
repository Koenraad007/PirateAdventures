using Microsoft.Xna.Framework;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Factories
{
    public class CollectableFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType)
        {
            var type = objectType.ToLower();
            return type == "silver" || type == "gold" || type == "skull";
        }

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            var collectableType = objectType.ToLower() switch
            {
                "silver" => CollectableType.SilverCoin,
                "gold" => CollectableType.GoldCoin,
                "skull" => CollectableType.Skull,
                _ => throw new ArgumentException($"Unknown collectable type: {objectType}")
            };

            return new Collectable(
                TextureManager.Instance.GetTextureAtlas("collectablesAtlas"),
                new Vector2(position.X, position.Y - Collectable.SPRITE_HEIGHT),
                collectableType
            );
        }
    }
}
