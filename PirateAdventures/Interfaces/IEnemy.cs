using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateAdventures.Interfaces
{
    public interface IEnemy : IGameObject
    {
        public event Action<IEnemy, int, Vector2> Attack;
    }
}