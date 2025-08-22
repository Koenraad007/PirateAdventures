using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.GameObjects.Enemies
{
    public class BigGuy : IEnemy, ICollidable, IKillable, IMovable
    {
        public const int SPRITE_WIDTH = 77;
        public const int SPRITE_HEIGHT = 74;
        private const float MAX_SPEED = 2f;

        private readonly TextureAtlas ta;
        private AnimatedSprite currentAnimation;
        private int prevFrame = 0;
        private BigGuyState currentState, prevState = BigGuyState.Idle;
        public bool Passable { get; set; } = false;
        private Vector2 _pos = Vector2.Zero;
        public Vector2 Position
        {
            get => _pos;
            set
            {
                _pos = new Vector2(value.X + SPRITE_WIDTH * scale, value.Y + SPRITE_HEIGHT * scale);
                BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
            }
        }
        public Rectangle BoundingBox { get; set; }
        private SpriteEffects spriteEffects = SpriteEffects.None;
        private readonly float scale = .5f;
        public Vector2 Speed { get; set; } = new Vector2(2, 5);
        public Vector2 Acceleration { get; set; } = new Vector2(0.1f, 0.3f);
        public event Action<IEnemy, int, Vector2> Attack;
        public int Health { get; set; } = 100;


        public BigGuy(TextureAtlas ta)
        {
            this.ta = ta;
            BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

            currentAnimation = this.ta.CreateAnimatedSprite("idle");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentAnimation.Effects = spriteEffects;
            currentAnimation.Scale = new Vector2(scale, scale);
            currentAnimation.Draw(spriteBatch, Position);

            //var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
            //pixel.SetData(new[] { Color.Red });
            //spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            var hero = collisionObjects.OfType<Hero>().FirstOrDefault();

            var direction = Vector2.Zero;

            if (hero.BoundingBox.Intersects(BoundingBox))
            {
                currentState = BigGuyState.Attacking;
                if (hero.Position.X < _pos.X + SPRITE_WIDTH * scale / 2) spriteEffects = SpriteEffects.FlipHorizontally;
                else spriteEffects = SpriteEffects.None;
            }
            else if (Math.Abs(hero.BoundingBox.Center.X - BoundingBox.Center.X) < 150 &&
                Math.Abs(hero.BoundingBox.Center.X - BoundingBox.Center.X) > 20 &&
                Math.Abs(hero.BoundingBox.Center.Y - BoundingBox.Center.Y) < 20)
            {
                currentState = BigGuyState.Running;

                direction = (hero.BoundingBox.Center - BoundingBox.Center).ToVector2();
                direction.Normalize();
                if (hero.Position.X < _pos.X)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else spriteEffects = SpriteEffects.None;
            }
            else
            {
                currentState = BigGuyState.Idle;
            }

            Move(direction);
            CheckCollision(collisionObjects);
            _pos = new Vector2(BoundingBox.X, BoundingBox.Y);


            if (currentState != prevState)
            {
                string animationName = $"{currentState.ToString().ToLower()}";
                currentAnimation = ta.CreateAnimatedSprite(animationName);
                prevState = currentState;
            }
            if (currentState == BigGuyState.Attacking)
            {
                if (currentAnimation.CurrentFrame == currentAnimation.Animation.Frames.Count / 2 && currentAnimation.CurrentFrame != prevFrame)
                {
                    var attackDirection = hero.BoundingBox.Center - BoundingBox.Center;
                    Attack?.Invoke(this, 10, attackDirection.ToVector2());
                }
                prevFrame = currentAnimation.CurrentFrame;
            }

            currentAnimation.Update(gameTime);
        }

        public void Move(Vector2 direction)
        {
            // if left/right keys are pressed
            if (direction.X != 0)
            {
                direction.X *= Acceleration.X;

                // check if speed is below max speed
                if (Math.Abs(Speed.X) < MAX_SPEED) Speed = new Vector2(Speed.X + direction.X, Speed.Y);
            }
            // if left/right keys aren't pressed
            else if (direction.X == 0)
            {
                // if hero is moving left
                if (Speed.X < 0)
                {
                    Speed = new Vector2(Speed.X + Acceleration.X * 2, Speed.Y);
                    if (Speed.X > 0) Speed = new Vector2(0, Speed.Y);
                }
                // if hero is moving right
                else if (Speed.X > 0)
                {
                    Speed = new Vector2(Speed.X - Acceleration.X * 2, Speed.Y);
                    if (Speed.X < 0) Speed = new Vector2(0, Speed.Y);
                }
            }

            Speed = new Vector2(Speed.X, Speed.Y + Acceleration.Y);

            _pos += Speed;
            BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, BoundingBox.Width, BoundingBox.Height);
        }

        private void CheckCollision(List<IGameObject> objects)
        {
            var blocks = objects.OfType<Block>().ToList();

            foreach (var block in blocks)
            {
                if (block.BlockType != BlockType.FULL) continue;

                if (block.BoundingBox.Intersects(BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(BoundingBox, block.BoundingBox);


                    // collision on the X axis
                    if (intersection.Width < intersection.Height)
                    {
                        if (block.BlockType == BlockType.FULL)
                        {
                            if (BoundingBox.Center.X < block.BoundingBox.Center.X)
                                _pos = new Vector2(_pos.X - intersection.Width, _pos.Y);
                            else
                                _pos = new Vector2(_pos.X + intersection.Width, _pos.Y);
                            Speed = new Vector2(0, Speed.Y);
                        }
                    }
                    // collision on the Y axis
                    else
                    {
                        if (BoundingBox.Center.Y < block.BoundingBox.Center.Y && Speed.Y > 0)
                        {
                            _pos = new Vector2(_pos.X, _pos.Y - intersection.Height);
                            Speed = new Vector2(Speed.X, 0);
                        }
                        else
                        {
                            if (block.BlockType == BlockType.FULL)
                            {
                                _pos = new Vector2(_pos.X, _pos.Y + intersection.Height);
                                Speed = new Vector2(Speed.X, 0);
                            }
                        }



                    }
                    BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, BoundingBox.Width, BoundingBox.Height);
                }
            }
        }
    }

    public enum BigGuyState
    {
        Idle,
        Running,
        Attacking
    }
}