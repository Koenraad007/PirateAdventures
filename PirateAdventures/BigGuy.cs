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
    public class BigGuy : IEnemy, ICollidable
    {
        public const int SPRITE_WIDTH = 77;
        public const int SPRITE_HEIGHT = 74;

        private Texture2D texture2D;
        private List<Animation> animations = new();
        public int EnemyState { get; set; } = 0;    // 0=idle,1=running,2=attack
        public int EnemyType { get; set; } = 0;
        public bool Passable { get; set; } = false;
        public Vector2 Position { get; set; } = new Vector2(200, 7 * 64 - SPRITE_HEIGHT);
        public Rectangle BoundingBox { get; set; }
        private SpriteEffects spriteEffects = SpriteEffects.None;

        public BigGuy(Texture2D texture)
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
            spriteBatch.Draw(texture2D, Position, animations[EnemyState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteEffects, 0);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            foreach (var item in collisionObjects)
            {
                if (item is Hero)
                {
                    var hero = item as Hero;
                    if (hero.BoundingBox.Intersects(BoundingBox))
                    {
                        EnemyState = 2;
                        if (hero.Position.X < Position.X + SPRITE_WIDTH / 2) spriteEffects = SpriteEffects.FlipHorizontally;
                        else spriteEffects = SpriteEffects.None;
                    }
                    else if (Math.Abs(hero.Position.X - Position.X) < 150)
                    {
                        EnemyState = 1;
                        int speed = 2;
                        if (hero.Position.X < Position.X)
                        {
                            speed *= -1;
                            spriteEffects = SpriteEffects.FlipHorizontally;
                        }
                        else spriteEffects = SpriteEffects.None;
                        Position = new Vector2(Position.X + speed, Position.Y);
                        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);
                    }
                    else
                    {
                        EnemyState = 0;
                    }
                }
            }


            animations[EnemyState].Update(gameTime);
            for (int i = 0; i < animations.Count; i++)
            {
                if (EnemyState != i) animations[i].ResetAnimation();
            }
        }
    }
}