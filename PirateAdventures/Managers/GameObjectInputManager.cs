using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Managers
{
    public class GameObjectInputManager
    {
        private readonly List<(IGameObject gameObject, IInputReader inputReader)> _inputReaders;

        public GameObjectInputManager()
        {
            _inputReaders = new List<(IGameObject, IInputReader)>();
        }

        public void AddInputReader(IGameObject gameObject, IInputReader inputReader)
        {
            _inputReaders.Add((gameObject, inputReader));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var (gameObject, inputReader) in _inputReaders)
            {
                var direction = inputReader.ReadInput();
                if (gameObject is IMovable movable)
                {
                    movable.Move(direction);
                }

                if (inputReader.IsAttackPressed && gameObject is IAttackable attackable)
                {
                    attackable.Attack();
                }
            }
        }
    }
}