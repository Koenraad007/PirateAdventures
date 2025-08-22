using Microsoft.Xna.Framework;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly IMovable _movable;
        private readonly Vector2 _direction;

        public MoveCommand(IMovable movable, Vector2 direction)
        {
            _movable = movable;
            _direction = direction;
        }

        public void Execute()
        {
            _movable.Move(_direction);
        }
    }
}