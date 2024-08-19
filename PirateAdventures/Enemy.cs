using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;

namespace PirateAdventures
{
    public class Enemy : IGameObject, ICollidable
    {
        public const int SPRITE_WIDTH = 77;
        public const int SPRITE_HEIGHT = 74;

        private Texture2D texture2D;
        private Animation idle;
        public bool Passable { get; set; } = false;
        public Vector2 Position { get; set; } = new Vector2(200, 7 * 64 - SPRITE_HEIGHT);
        public Rectangle BoundingBox { get; set; }

        public Enemy(Texture2D texture)
        {
            texture2D = texture;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            idle = new Animation();
            for (int i = 0; i < 38; i++)
            {
                idle.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, Position, idle.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            idle.Update(gameTime);
        }
    }
}