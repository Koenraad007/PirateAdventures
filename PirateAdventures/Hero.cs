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
    public class Hero : IGameObject
    {
        public const int SPRITE_WIDTH = 58;
        public const int SPRITE_HEIGHT = 58;
        public const int MAX_SPEED = 5;


        private Texture2D heroTexture;
        private Animation idle, running;
        public Vector2 position = new Vector2(100, 100);
        private Vector2 speed = Vector2.Zero;
        private Vector2 acceleration = new Vector2(0.1f, 0.3f);
        private SpriteEffects spriteFx = SpriteEffects.None;
        private IInputReader input;
        private HeroState state;
        private bool isGrounded = false;
        private Vector2 collision = Vector2.One;
        private Rectangle boundingBox;


        public Hero(Texture2D texture, IInputReader inputReader)
        {
            heroTexture = texture;
            input = inputReader;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            idle = new Animation();
            running = new Animation();

            for (int i = 0; i < 26; i++)
            {
                idle.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            for (int i = 0; i < 14; i++)
            {
                running.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + (26 * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
        }

        public void Update(List<IGameObject> objects, GameTime gameTime)
        {
            System.Console.WriteLine($"position: {position}");

            var direction = input.ReadInput();

            Move(direction);
            Vector2 nextPos = position + speed;

            CheckCollision(nextPos, objects);

            position += speed;
            boundingBox.X = (int)position.X;
            boundingBox.Y = (int)position.Y;

            // if the hero is not moving, the state is IDLE (0), else it's RUNNING (1)
            if (speed.X != 0) state = HeroState.RUNNING;
            else state = HeroState.IDLE;

            switch (state)
            {
                case HeroState.IDLE:
                    running.ResetAnimation();
                    idle.Update(gameTime);
                    break;

                case HeroState.RUNNING:
                    running.Update(gameTime);
                    idle.ResetAnimation();
                    break;

                default: break;
            }
        }

        private void Move(Vector2 direction)
        {
            // TODO: put movement logic in IMovable interface and MovementManager class

            // if left/right keys are pressed
            if (direction.X != 0)
            {
                direction.X *= acceleration.X;

                // check if speed is below max speed
                if (Math.Abs(speed.X) < MAX_SPEED) speed.X += direction.X;
            }
            // if left/right keys aren't pressed
            else if (direction.X == 0)
            {
                // if hero is moving left
                if (speed.X < 0)
                {
                    speed.X += acceleration.X * 2;
                    if (speed.X > 0) speed.X = 0;
                }
                // if hero is moving right
                else if (speed.X > 0)
                {
                    speed.X -= acceleration.X * 2;
                    if (speed.X < 0) speed.X = 0;
                }
            }

            // if jump key is pressed
            if (direction.Y < 0 && isGrounded)
            {
                speed.Y = -10;
                isGrounded = false;
            }

            // if in the air, apply gravity/deceleration
            if (!isGrounded)
            {
                speed.Y += acceleration.Y;
            }

            // update position
            // position += speed;

            // CheckGroundCollision();

            if (position.X > 800 - SPRITE_WIDTH) position.X = 800 - SPRITE_WIDTH;
            if (position.X < 0) position.X = 0;


        }

        private void CheckCollision(Vector2 position, List<IGameObject> objects)
        {
            Rectangle bb = new Rectangle((int)position.X, (int)position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            collision = Vector2.One;
            isGrounded = false;
            foreach (var block in objects)
            {
                if (block is ICollidable)
                {
                    var collisionObj = block as ICollidable;
                    if (collisionObj.Passable) continue;

                    if (collisionObj.BoundingBox.Intersects(bb))
                    {
                        if (bb.Left < collisionObj.BoundingBox.Right)
                        {
                            position.X = collisionObj.BoundingBox.Right;
                            // speed.X = 0;
                        }
                        else if (bb.Right > collisionObj.BoundingBox.Left)
                        {
                            position.X = collisionObj.BoundingBox.Left + SPRITE_WIDTH;
                            // speed.X = 0;
                        }

                        if (bb.Bottom > collisionObj.BoundingBox.Top)
                        {
                            position.Y = collisionObj.BoundingBox.Top - SPRITE_HEIGHT;
                            speed.Y = 0;
                            isGrounded = true;
                        }

                        // if (boundingBox.Top < collisionObj.BoundingBox.Bottom)
                        // {
                        //     position.Y = collisionObj.BoundingBox.Bottom;
                        //     speed.Y = 0;
                        //     isGrounded = false;
                        // }
                    }

                    // if (boundingBox.Left < collisionObj.BoundingBox.Right || boundingBox.Right > collisionObj.BoundingBox.Left) collision.X = 0;
                }
            }
        }

        private void CheckGroundCollision()
        {
            float groundLvl = 400;

            if (position.Y >= groundLvl)
            {
                position.Y = groundLvl;
                speed.Y = 0;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // face sprite in right direction
            if (speed.X < 0) spriteFx = SpriteEffects.FlipHorizontally;
            if (speed.X > 0) spriteFx = SpriteEffects.None;

            switch (state)
            {
                case HeroState.IDLE:
                    spriteBatch.Draw(heroTexture, position, idle.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
                    break;

                case HeroState.RUNNING:
                    spriteBatch.Draw(heroTexture, position, running.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
                    break;

                default: break;
            }
        }

    }

    enum HeroState
    {
        IDLE,
        RUNNING
    }
}