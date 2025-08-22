using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PirateAdventures.GameObjects
{
    public class EndPoint : IGameObject
    {
        public const int SPRITE_WIDTH = 64;
        public const int SPRITE_HEIGHT = 64;
        public Vector2 Position { get; set; }
        public AnimatedSprite Texture { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);
        public bool Reached { get; set; } = false;

        public event Action<EndPoint> OnReached;

        public EndPoint(Vector2 position, TextureAtlas atlas)
        {
            Position = position;
            Texture = atlas.CreateAnimatedSprite("endpoint");
            Texture.PlayOnce = true;
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            Hero hero = collisionObjects.Find(collisionObjects => collisionObjects is Hero) as Hero;
            var heroBoundsSmall = new Rectangle(
                hero.BoundingBox.Center.X - 1,
                hero.BoundingBox.Center.Y - 1,
                2, 2);

            if (!Reached && heroBoundsSmall.Intersects(Bounds))
            {
                Reached = true;
                Debug.WriteLine("EndPoint reached!");
                OnReached?.Invoke(this);
            }

            if (Vector2.Distance(hero.BoundingBox.Center.ToVector2(), Bounds.Center.ToVector2()) < 100f)
            {
                Texture.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, Position);
        }

    }
}
