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
    public class EndPointFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "endpoint";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            return new EndPoint(
                new Vector2(position.X, position.Y - EndPoint.SPRITE_HEIGHT),
                TextureManager.Instance.GetTextureAtlas("endpointAtlas")
            );
        }
    }
}
