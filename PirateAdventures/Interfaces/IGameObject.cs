using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PirateAdventures.Interfaces
{
    public interface IGameObject
    {
        void Update();

        void Draw(SpriteBatch spriteBatch);
    }
}