using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.GameObjects
{
    public class EndPoint : IGameObject
    {
        public Vector2 Position { get; set; }
        public AnimatedSprite Texture { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);
        public bool Reached { get; set; } = false;

        public EndPoint(Vector2 position, AnimatedSprite texture)
        {
            Position = position;
            Texture = texture;
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            if (!Reached && collisionObjects.Find(collisionObjects => collisionObjects is Hero) is Hero hero)
            {
                if (hero.BoundingBox.Intersects(Bounds))
                {
                    Reached = true;
                    Core.ChangeScene(new Startscreen());
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, Position);
        }

    }
}
