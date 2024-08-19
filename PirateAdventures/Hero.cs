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
    public class Hero : IGameObject, ICollidable
    {
        public const int SPRITE_WIDTH = 58;
        public const int SPRITE_HEIGHT = 58;
        public const int MAX_SPEED = 5;


        private Texture2D heroTexture;
        private Animation idle, running;
        private Vector2 speed = Vector2.Zero;
        private Vector2 acceleration = new Vector2(0.1f, 0.3f);
        private SpriteEffects spriteFx = SpriteEffects.None;
        private IInputReader input;
        private HeroState state;
        private bool isGrounded = false;
        private Vector2 collision = Vector2.Zero;

        public bool Passable { get; set; } = false;
        public Vector2 Position { get; set; }
        public Rectangle BoundingBox { get; set; }

        public Hero(Texture2D texture, IInputReader inputReader)
        {
            heroTexture = texture;
            input = inputReader;
            this.Position = new Vector2(100, 100);
            this.BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

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


            var direction = input.ReadInput();

            // change speed according to direction input
            Move(direction);

            Vector2 nextPos = Position + speed;
            CheckCollision(nextPos, objects);   // check if next position doesn't collide

            Position += speed;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            if (Position.X > 800 - SPRITE_WIDTH) Position = new Vector2(800 - SPRITE_WIDTH, Position.Y);
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y > 460 - SPRITE_HEIGHT) Position = new Vector2(Position.X, 460 - SPRITE_HEIGHT);

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

        }

        private void CheckCollision(Vector2 nextPos, List<IGameObject> objects)
        {
            Rectangle bb = new Rectangle((int)nextPos.X, (int)nextPos.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            // System.Console.WriteLine($"collision:\t{collision};\tpos:\t{position};\tspeed:\t{speed}");

            Vector2 newCollision = Vector2.Zero;
            foreach (var block in objects)
            {
                if (block is ICollidable)
                {
                    var collisionObj = block as ICollidable;
                    if (collisionObj.Passable) continue;

                    if (collisionObj.BoundingBox.Intersects(bb))
                    {
                        if (bb.Left < collisionObj.BoundingBox.Right && speed.X < 0)
                        {
                            Position = new Vector2(collisionObj.BoundingBox.Right, Position.Y);
                            speed.X = 0;
                            newCollision.X = -1;
                            continue;
                            // break;
                        }
                        else if (bb.Right > collisionObj.BoundingBox.Left && speed.X > 0)
                        {
                            Position = new Vector2(collisionObj.BoundingBox.Left - SPRITE_WIDTH + 1, Position.Y);
                            speed.X = 0;
                            newCollision.X = 1;
                            continue;
                            // break;
                        }

                        if (bb.Bottom > collisionObj.BoundingBox.Top && speed.Y > 0)
                        {
                            Position = new Vector2(Position.X, collisionObj.BoundingBox.Top - SPRITE_HEIGHT);
                            speed.Y = 0;
                            isGrounded = true;
                            newCollision.Y = 1;
                            continue;
                            // break;
                        }
                        else if (bb.Top < collisionObj.BoundingBox.Bottom && speed.Y < 0)
                        {
                            Position = new Vector2(Position.X, collisionObj.BoundingBox.Bottom);
                            speed.Y = 0;
                            newCollision.Y = -1;
                            continue;
                            // break;
                        }
                    }
                }
            }

            collision = newCollision;

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // face sprite in right direction
            if (speed.X < 0) spriteFx = SpriteEffects.FlipHorizontally;
            if (speed.X > 0) spriteFx = SpriteEffects.None;

            switch (state)
            {
                case HeroState.IDLE:
                    spriteBatch.Draw(heroTexture, Position, idle.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
                    break;

                case HeroState.RUNNING:
                    spriteBatch.Draw(heroTexture, Position, running.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
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