using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.GameObjects.Enemies
{
    public class Bullet : IGameObject, ICollidable, IEnemy
    {
        public const int SPRITE_HEIGHT = 10, SPRITE_WIDTH = 19;
        public bool Passable { get; set; } = true;
        public Vector2 Position { get; set; }
        public Rectangle BoundingBox { get; set; }
        public bool IsHit { get; set; } = false;

        private Texture2D texture;
        private Vector2 direction;
        private float speed;
        private float scale = .5f;
        public event Action<IEnemy, int, Vector2> Attack;

        public Bullet(Vector2 position, Vector2 direction, float speed)
        {
            this.texture = TextureManager.Instance.GetTexture("bullet");
            Position = position;
            BoundingBox = new Rectangle((int)position.X, (int)position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
            this.direction = direction;
            this.speed = speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var rotation = MathF.Atan2(direction.Y, direction.X);
            Core.SpriteBatch.Draw(texture, Position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            direction.Normalize();
            Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

            foreach (var obj in collisionObjects)
            {
                if (obj is ICollidable collidable && BoundingBox.Intersects(collidable.BoundingBox) && !(obj is IEnemy))
                {
                    IsHit = true;
                    if (obj is Hero hero) Attack?.Invoke(this, 5, hero.BoundingBox.Center.ToVector2() - BoundingBox.Center.ToVector2());
                    return;
                }
            }
        }
    }
}
