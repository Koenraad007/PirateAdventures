using Microsoft.Xna.Framework;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Factories
{
    public class GameObjectFactory
    {
        private readonly List<IGameObjectFactory> _factories;

        public GameObjectFactory()
        {
            _factories = new List<IGameObjectFactory>
            {
                new HeroFactory(),
                new CompanionFactory(),
                new BigGuyFactory(),
                new ShooterFactory(),
                new WindowGuyFactory(),
                new EndPointFactory(),
                new CollectableFactory()
            };
        }

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            var factory = _factories.Find(f => f.CanCreate(objectType));

            if (factory == null)
            {
                Console.WriteLine($"Warning: No factory found for object type '{objectType}'");
                return null;
            }

            return factory.CreateGameObject(objectType, position);
        }

        public void RegisterFactory(IGameObjectFactory factory)
        {
            _factories.Add(factory);
        }
    }
}
