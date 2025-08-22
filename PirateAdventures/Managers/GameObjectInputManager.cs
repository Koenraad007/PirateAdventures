using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PirateAdventures.Commands;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Managers
{
    public class GameObjectInputManager
    {
        private readonly List<(IGameObject gameObject, IInputReader inputReader)> _inputReaders;
        private readonly Queue<ICommand> _commandQueue;


        public GameObjectInputManager()
        {
            _inputReaders = new List<(IGameObject, IInputReader)>();
            _commandQueue = new Queue<ICommand>();
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
                    _commandQueue.Enqueue(new MoveCommand(movable, direction));
                }

                if (inputReader.IsAttackPressed && gameObject is IAttackable attackable)
                {
                    _commandQueue.Enqueue(new AttackCommand(attackable));
                }
            }

            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                command.Execute();
            }
        }
    }
}