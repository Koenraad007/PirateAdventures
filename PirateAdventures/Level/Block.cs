using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Level
{
    public class Block : IGameObject, ICollidable
    {
        public Rectangle BoundingBox { get; set; }
        public Rectangle SourceRect { get; set; }
        public Vector2 Position { get; set; }
        public bool Passable { get; set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }
        // public CollideWithEvent CollideWithEvent { get; set; }

        public Block(Vector2 position, Texture2D tileset, Vector2 tile, int tileSize, bool passable)
        {
            SourceRect = new Rectangle((int)tile.Y * tileSize, (int)tile.X * tileSize, tileSize, tileSize);
            BoundingBox = new Rectangle((int)position.X, (int)position.Y, tileSize, tileSize);
            Position = position;
            Passable = passable;
            Color = Color.White;
            Texture = tileset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int scale = 1;
            spriteBatch.Draw(Texture, Position * scale, SourceRect, Color, 0f, new Vector2(0, 0), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        public void Update(List<IGameObject> collidables, GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}