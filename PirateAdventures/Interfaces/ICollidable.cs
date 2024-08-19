using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PirateAdventures.Interfaces
{
    public interface ICollidable
    {
        public bool Passable { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle BoundingBox { get; set; }
    }
}