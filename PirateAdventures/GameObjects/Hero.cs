using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PirateAdventures.GameObjects
{
    public class Hero : IGameObject, ICollidable
    {
        public const int SPRITE_WIDTH = 32;
        public const int SPRITE_HEIGHT = 32;
        public const int MAX_SPEED = 3;


        private Texture2D heroTexture;
        private Vector2 speed = Vector2.Zero;
        private Vector2 acceleration = new Vector2(0.1f, 0.3f);
        private SpriteEffects spriteFx = SpriteEffects.None;
        private float scale = 1f;
        private IInputReader input;
        private HeroState currentState, prevState = HeroState.IDLE;
        private bool isGrounded = false;
        private Vector2 collision = Vector2.Zero;
        public int Health { get; set; } = 100;

        public bool Passable { get; set; } = true;
        public Vector2 Position { get; set; }
        public Rectangle BoundingBox { get; set; }

        private TextureAtlas textureAtlas;
        private AnimatedSprite currentAnimation;

        public event Action<Hero> OnDeath;

        public Hero(Texture2D texture, IInputReader inputReader, TextureAtlas ta)
        {
            textureAtlas = ta;
            heroTexture = texture;
            input = inputReader;
            Position = new Vector2(200, 200);
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

            currentAnimation = textureAtlas.CreateAnimatedSprite("hero-idle");
        }

        public void Update(List<IGameObject> objects, GameTime gameTime)
        {
            var direction = input.ReadInput();

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
            if (speed.X != 0 && Math.Abs(speed.Y) < 1) currentState = HeroState.RUNNING;
            else if (speed.Y <= -1) currentState = HeroState.JUMPING;
            else if (speed.Y >= 1) currentState = HeroState.FALLING;
            else currentState = HeroState.IDLE;

            if (currentState != prevState)
            {
                string animationName = $"hero-{currentState.ToString().ToLower()}";
                currentAnimation = textureAtlas.CreateAnimatedSprite(animationName);
                prevState = currentState;
            }

            currentAnimation.Update(gameTime);
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
                speed.Y = -6;
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

                            if (collBlock.BlockType == BlockType.DEATH) OnDeath?.Invoke(this);

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

            currentAnimation.Effects = spriteFx;
            currentAnimation.Origin = new Vector2(16 * (spriteFx == SpriteEffects.FlipHorizontally ? 1 : -1), -32);
            currentAnimation.Scale = new Vector2(scale, scale);
            currentAnimation.Draw(spriteBatch, Position + new Vector2(-24,-48));

            // draw bounding box for debugging
            //var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
            //pixel.SetData(new[] { Color.Red });
            //spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);
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