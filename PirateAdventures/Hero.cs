using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Interfaces;

namespace PirateAdventures
{
    public class Hero: IGameObject
    {
        Texture2D heroTexture;

        public Hero(Texture2D texture)
        {
            heroTexture = texture;
        }

        public void Update() {

        }

        public void Draw() {

        }

    }
}