using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Interfaces
{
    public interface IGameObjectFactory
    {
        IGameObject CreateGameObject(string objectType, Vector2 position);
        bool CanCreate(string objectType);
    }
}
