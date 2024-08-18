using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PirateAdventures.Level
{
    public class BlockFactory
    {
        public static Block CreateBlock(char type, int posX, int posY, Texture2D tileset, int tileSize)
        {
            Block newBlock = null;


            (int x, int y) tile = (-1, -1);
            switch (type)
            {
                case '1': tile = (0, 0); break;
                case '2': tile = (0, 1); break;
                case '3': tile = (0, 2); break;
                case '4': tile = (0, 3); break;
                case '5': tile = (0, 4); break;
                case '6': tile = (0, 5); break;
                case '7': tile = (1, 0); break;
                case '8': tile = (1, 1); break;
                case '9': tile = (1, 2); break;
                case 'a': tile = (1, 3); break;
                case 'b': tile = (1, 4); break;
                case 'c': tile = (1, 5); break;
                case 'd': tile = (2, 1); break;
                case 'e': tile = (2, 2); break;
                case 'f': tile = (2, 3); break;
                case 'g': tile = (2, 4); break;
                case 'h': tile = (2, 5); break;
                default:
                    break;
            }

            if (tile != (-1, -1)) newBlock = new Block(new Vector2(posX, posY) * tileSize, tileset, new Vector2(tile.x, tile.y), tileSize);

            return newBlock;
        }
    }
}