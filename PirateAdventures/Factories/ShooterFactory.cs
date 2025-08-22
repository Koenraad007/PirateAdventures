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
    public class ShooterFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "shoot";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            return new Shooter(TextureManager.Instance.GetTextureAtlas("shooterAtlas"))
            {
                Position = new Vector2(position.X - Shooter.SPRITE_WIDTH / 2, position.Y - Shooter.SPRITE_HEIGHT)
            };
        }
    }
}
