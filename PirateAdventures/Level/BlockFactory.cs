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
            bool passable = true;

            (int x, int y) tile = (-1, -1);
            switch (type)
            {
                case '1': tile = (0, 0); passable = false; break;
                case '2': tile = (0, 1); passable = false; break;
                case '3': tile = (0, 2); passable = false; break;
                case '4': tile = (0, 3); break;
                case '5': tile = (0, 4); break;
                case '6': tile = (0, 5); break;
                case '7': tile = (1, 0); passable = false; break;
                case '8': tile = (1, 1); passable = false; break;
                case '9': tile = (1, 2); passable = false; break;
                case 'a': tile = (1, 3); break;
                case 'b': tile = (1, 4); break;
                case 'c': tile = (1, 5); break;
                case 'd': tile = (2, 0); passable = false; break;
                case 'e': tile = (2, 1); passable = false; break;
                case 'f': tile = (2, 2); passable = false; break;
                case 'g': tile = (2, 3); break;
                case 'h': tile = (2, 4); break;
                case 'i': tile = (2, 5); break;
                case 'j': tile = (3, 0); break;
                case 'k': tile = (3, 1); break;
                case 'l': tile = (3, 2); break;
                case 'm': tile = (3, 3); break;
                case 'n': tile = (3, 4); break;
                case 'o': tile = (3, 5); break;
                case 'p': tile = (4, 0); break;
                case 'q': tile = (4, 1); break;
                case 'r': tile = (4, 2); break;
                case 's': tile = (4, 3); break;
                case 't': tile = (4, 4); break;
                case 'u': tile = (4, 5); break;
                default:
                    break;
            }

            if (tile != (-1, -1)) newBlock = new Block(new Vector2(posX, posY) * tileSize, tileset, new Vector2(tile.x, tile.y), tileSize, passable);

            return newBlock;
        }
    }
}