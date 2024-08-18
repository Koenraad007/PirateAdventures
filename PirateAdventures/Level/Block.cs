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
        public bool Passable { get; set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }
        // public CollideWithEvent CollideWithEvent { get; set; }

        public Block(int x, int y, GraphicsDevice graphicsDevice)
        {
            BoundingBox = new Rectangle(x, y, 64, 64);
            Passable = false;
            Color = Color.White;
            Texture = new Texture2D(graphicsDevice, 1, 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, BoundingBox, Color);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}