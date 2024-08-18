using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Level
{
    public class Block : IGameObject
    {
        public Rectangle BoundingBox { get; set; }
        public Vector2 Position { get; set; }
        public bool Passable { get; set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }
        // public CollideWithEvent CollideWithEvent { get; set; }

        public Block(Vector2 position, Texture2D tileset, Vector2 tile, int tileSize)
        {
            BoundingBox = new Rectangle((int)tile.Y * tileSize, (int)tile.X * tileSize, tileSize, tileSize);
            Position = position;
            Passable = false;
            Color = Color.White;
            Texture = tileset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int scale = 1;
            spriteBatch.Draw(Texture, Position * scale, BoundingBox, Color, 0f, new Vector2(0, 0), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}