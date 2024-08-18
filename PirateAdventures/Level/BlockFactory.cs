using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PirateAdventures.Level
{
    public class BlockFactory
    {
        public static Block CreateBlock(string type, int x, int y, GraphicsDevice graphicsDevice)
        {
            Block newBlock = null;
            type = type.ToUpper();
            switch (type)
            {
                default: break;
            }

            return newBlock;
        }
    }
}