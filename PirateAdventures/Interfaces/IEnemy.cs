using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateAdventures.Interfaces
{
    public interface IEnemy : IGameObject
    {
        public int EnemyState { get; set; }
        public int EnemyType { get; set; }
    }
}