using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Interfaces
{
    public interface IKillable
    {
        public int Health { get; set; }
        public bool isDead => Health <= 0;
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
}
