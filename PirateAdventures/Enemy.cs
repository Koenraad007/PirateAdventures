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
        private List<Animation> animations = new();
        private int state = 0;  // 0=idle,1=running,2=attack
        public bool Passable { get; set; } = false;
        public Vector2 Position { get; set; } = new Vector2(200, 7 * 64 - SPRITE_HEIGHT);
        public Rectangle BoundingBox { get; set; }
        private SpriteEffects spriteEffects = SpriteEffects.None;

        public Enemy(Texture2D texture)
        {
            texture2D = texture;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            animations.Add(new Animation());
            for (int i = 0; i < 38; i++)
            {
                animations[0].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            animations.Add(new Animation());
            for (int i = 0; i < 16; i++)
            {
                animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + (38 * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            animations.Add(new Animation());
            for (int i = 0; i < 11; i++)
            {
                animations[2].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + ((38 + 16) * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, Position, animations[state].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteEffects, 0);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            System.Console.WriteLine(collisionObjects[0]);
            foreach (var item in collisionObjects)
            {
                if (item is ICollidable)
                {
                    var collisionObj = item as ICollidable;
                    if (collisionObj.BoundingBox.Intersects(BoundingBox))
                    {
                        state = 2;
                        if (collisionObj.Position.X < Position.X + SPRITE_WIDTH / 2) spriteEffects = SpriteEffects.FlipHorizontally;
                        else spriteEffects = SpriteEffects.None;
                    }
                    else
                    {
                        state = 0;
                    }
                }
            }


            animations[state].Update(gameTime);
            for (int i = 0; i < animations.Count; i++)
            {
                if (state != i) animations[i].ResetAnimation();
            }
        }
    }
}