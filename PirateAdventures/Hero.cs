using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;

namespace PirateAdventures
{
    public class Hero : IGameObject, ICollidable
    {
        public const int SPRITE_WIDTH = 80;
        public const int SPRITE_HEIGHT = 80;
        public const int MAX_SPEED = 5;


        private Texture2D heroTexture;
        private Animations.Animation idle, running, jumping, falling;
        private Vector2 speed = Vector2.Zero;
        private Vector2 acceleration = new Vector2(0.1f, 0.3f);
        private SpriteEffects spriteFx = SpriteEffects.None;
        private float scale = 2f;
        private IInputReader input;
        private HeroState state;
        private bool isGrounded = false;
        private Vector2 collision = Vector2.Zero;

        public bool Passable { get; set; } = true;
        public Vector2 Position { get; set; }
        public Rectangle BoundingBox { get; set; }

        private TextureAtlas textureAtlas;
        private AnimatedSprite idleAS;

        public Hero(Texture2D texture, IInputReader inputReader, TextureAtlas ta)
        {
            textureAtlas = ta;
            heroTexture = texture;
            input = inputReader;
            this.Position = new Vector2(200, 200);
            this.BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

            idle = new Animations.Animation();
            running = new Animations.Animation();
            jumping = new Animations.Animation();
            falling = new Animations.Animation();

            for (int i = 0; i < 10; i++)
            {
                idle.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            for (int i = 0; i < 12; i++)
            {
                running.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + (10 * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            for (int i = 0; i < 6; i++)
            {
                jumping.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + (22 * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }
            for (int i = 0; i < 2; i++)
            {
                falling.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i + (28 * SPRITE_WIDTH), 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
            }

            idleAS = textureAtlas.CreateAnimatedSprite("idle");
            idleAS.Scale = new Vector2(scale, scale);
        }

        public void Update(List<IGameObject> objects, GameTime gameTime)
        {
            var direction = input.ReadInput();

            idleAS.Update(gameTime);

            // change speed according to direction input
            Move(direction);

            Position += speed;
            BoundingBox = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                BoundingBox.Width,
                BoundingBox.Height
            );

            CheckCollision(objects);   // check if next position doesn't collide

            if (speed.Y > 0) isGrounded = false;
            Position = new Vector2(BoundingBox.X, BoundingBox.Y);

            // Make sure the hero doesn't go out of the screen
            // if (Position.X > 800 - SPRITE_WIDTH) Position = new Vector2(800 - SPRITE_WIDTH, Position.Y);
            // if (Position.X < 0) Position = new Vector2(0, Position.Y);
            // if (Position.Y > 460 - SPRITE_HEIGHT) Position = new Vector2(Position.X, 460 - SPRITE_HEIGHT);

            // if the hero is not moving, the state is IDLE (0), else it's RUNNING (1)
            if (speed.X != 0 && Math.Abs(speed.Y) < 1) state = HeroState.RUNNING;
            else if (speed.Y <= -1) state = HeroState.JUMPING;
            else if (speed.Y >= 1) state = HeroState.FALLING;
            else state = HeroState.IDLE;

            Debug.WriteLine("Speed.Y: " + speed.Y);
            Debug.WriteLine("Abs(Speed.Y): " + Math.Abs(speed.Y));
            Debug.WriteLine("Hero State: " + state.ToString());

            switch (state)
            {
                case HeroState.IDLE:
                    running.ResetAnimation();
                    jumping.ResetAnimation();
                    idle.Update(gameTime);
                    break;

                case HeroState.RUNNING:
                    idle.ResetAnimation();
                    jumping.ResetAnimation();
                    running.Update(gameTime);
                    break;

                case HeroState.JUMPING:
                    idle.ResetAnimation();
                    running.ResetAnimation();
                    jumping.Update(gameTime);
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

            speed.Y += acceleration.Y;


        }

        private void CheckCollision(List<IGameObject> objects)
        {
            foreach (var block in objects)
            {
                if (block is ICollidable)
                {
                    var collisionObj = block as ICollidable;
                    if (collisionObj.Passable) continue;

                    if (collisionObj.BoundingBox.Intersects(BoundingBox))
                    {
                        Rectangle intersection = Rectangle.Intersect(BoundingBox, collisionObj.BoundingBox);

                        if (collisionObj is Block)
                        {
                            Block collBlock = (Block)collisionObj;

                            // collision on the X axis
                            if (intersection.Width < intersection.Height)
                            {
                                if (collBlock.BlockType == BlockType.FULL)
                                {
                                    if (BoundingBox.Center.X < collisionObj.BoundingBox.Center.X)
                                        Position = new Vector2(Position.X - intersection.Width, Position.Y);
                                    else
                                        Position = new Vector2(Position.X + intersection.Width, Position.Y);
                                    speed.X = 0;
                                }
                            }
                            // collision on the Y axis
                            else
                            {
                                if (BoundingBox.Center.Y < collisionObj.BoundingBox.Center.Y && speed.Y > 0)
                                {
                                    Position = new Vector2(Position.X, Position.Y - intersection.Height);
                                    isGrounded = true;
                                    speed.Y = 0;
                                }
                                else
                                {
                                    if (collBlock.BlockType == BlockType.FULL)
                                    {
                                        Position = new Vector2(Position.X, Position.Y + intersection.Height);
                                        speed.Y = 0;
                                    }
                                }


                            }
                        }
                        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, BoundingBox.Width, BoundingBox.Height);
                    }
                }
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
                    idleAS.Effects = spriteFx;
                    idleAS.Origin = new Vector2(16 * ((spriteFx == SpriteEffects.FlipHorizontally) ? 1 : -1), -32);
                    idleAS.Draw(spriteBatch, Position);
                    //spriteBatch.Draw(heroTexture, Position, idle.CurrentFrame.SourceRect, Color.White, 0, new Vector2(16 * ((spriteFx == SpriteEffects.FlipHorizontally) ? 1 : -1), -32), scale, spriteFx, 0);
                    break;

                case HeroState.RUNNING:
                    spriteBatch.Draw(heroTexture, Position, running.CurrentFrame.SourceRect, Color.White, 0, new Vector2(16 * ((spriteFx == SpriteEffects.FlipHorizontally) ? 1 : -1), -32), scale, spriteFx, 0);
                    break;

                case HeroState.JUMPING:
                    spriteBatch.Draw(heroTexture, Position, jumping.CurrentFrame.SourceRect, Color.White, 0, new Vector2(16 * ((spriteFx == SpriteEffects.FlipHorizontally) ? 1 : -1), -32), scale, spriteFx, 0);
                    break;

                case HeroState.FALLING:
                    spriteBatch.Draw(heroTexture, Position, falling.CurrentFrame.SourceRect, Color.White, 0, new Vector2(16 * ((spriteFx == SpriteEffects.FlipHorizontally) ? 1 : -1), -32), scale, spriteFx, 0);
                    break;

                default: break;
            }
        }

    }

    enum HeroState
    {
        IDLE,
        RUNNING,
        JUMPING,
        FALLING
    }
}