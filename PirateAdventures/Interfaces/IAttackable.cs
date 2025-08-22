using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PirateAdventures.Interfaces
{
    public interface IAttackable : IGameObject
    {
        public void Attack();
    }
}