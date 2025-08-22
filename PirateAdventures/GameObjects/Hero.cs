using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PirateAdventures.GameObjects
{
    public class Hero : IGameObject, ICollidable, IKillable, IMovable, IAttackable
    {
        public const int SPRITE_WIDTH = 32;
        public const int SPRITE_HEIGHT = 32;
        public const int MAX_SPEED = 3;

        private Vector2 _position = Vector2.Zero;
        public Vector2 Position
        {
            get => _position; set
            {
                _position = value;
                BoundingBox = new Rectangle((int)value.X, (int)value.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
                _futureBoundingBox = BoundingBox;
                _futurePosition = value;
            }
        }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public Vector2 Acceleration { get; set; } = new Vector2(0.1f, 0.3f);

        private SpriteEffects spriteFx = SpriteEffects.None;
        private readonly float scale = 1f;
        private HeroState currentState, prevState = HeroState.IDLE;
        private bool isGrounded = false;
        private Vector2 collision = Vector2.Zero;
        private List<IGameObject> enemies = new List<IGameObject>();
        public int Health { get; set; } = 100;

        public bool Passable { get; set; } = true;

        public Rectangle BoundingBox { get; set; }
        private Rectangle _futureBoundingBox;
        private Vector2 _futurePosition;

        private TextureAtlas textureAtlas;
        private AnimatedSprite currentAnimation;

        private bool attackAnimtionPlaying => currentState == HeroState.ATTACK && currentAnimation.CurrentFrame < currentAnimation.Animation.Frames.Count - 1;

        public event Action<Hero> OnDeath;

        public Hero(TextureAtlas ta)
        {
            textureAtlas = ta;
            Position = new Vector2(200, 200);
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
            _futureBoundingBox = BoundingBox;
            _futurePosition = Position;

            currentAnimation = textureAtlas.CreateAnimatedSprite("hero-idle");
        }

        public void Update(List<IGameObject> objects, GameTime gameTime)
        {
            enemies = objects.FindAll(obj => obj is IEnemy);

            CheckCollision(objects);   // check if next position doesn't collide

            if (Speed.Y > 0) isGrounded = false;
            Position = new Vector2(BoundingBox.X, BoundingBox.Y);

            // if the hero is not moving, the state is IDLE (0), else it's RUNNING (1)
            if (!attackAnimtionPlaying)
            {
                if (Speed.X != 0 && Math.Abs(Speed.Y) < 1) currentState = HeroState.RUNNING;
                else if (currentState != HeroState.HIT && Speed.Y <= -1)
                {
                    currentState = HeroState.JUMPING;
                }
                else if (currentState != HeroState.HIT && Speed.Y >= 1) currentState = HeroState.FALLING;
                else if (currentState != HeroState.HIT) currentState = HeroState.IDLE;
            }

            if (currentState != prevState)
            {
                string animationName = $"hero-{currentState.ToString().ToLower()}";
                currentAnimation = textureAtlas.CreateAnimatedSprite(animationName);
                if (currentState >= HeroState.HIT)
                {
                    currentAnimation.PlayOnce = true;
                }
                else
                {
                    currentAnimation.PlayOnce = false;
                }
                prevState = currentState;
            }

            currentAnimation.Update(gameTime);
        }

        public void Move(Vector2 direction)
        {
            _futureBoundingBox = BoundingBox;
            _futurePosition = Position;

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

            // if jump key is pressed
            if (direction.Y < 0 && isGrounded)
            {
                Speed = new Vector2(Speed.X, Speed.Y - 6);
                isGrounded = false;
                SoundManager.Instance.PlaySound("jump", 0.3f, false);
            }

            if (Speed.X != 0 && Math.Abs(Speed.Y) < 1f)
            {
                SoundManager.Instance.PlaySound("walk" + LevelManager.Instance.CurrentLevelNumber.ToString(), .2f, true);
            }
            else
            {
                SoundManager.Instance.StopSound("walk" + LevelManager.Instance.CurrentLevelNumber.ToString());
            }

            Speed = new Vector2(Speed.X, Speed.Y + Acceleration.Y);

            _futurePosition += Speed;
            _futureBoundingBox = new Rectangle(
                (int)_futurePosition.X,
                (int)_futurePosition.Y,
                BoundingBox.Width,
                BoundingBox.Height
            );

        }

        private void CheckCollision(List<IGameObject> objects)
        {
            foreach (var block in objects)
            {
                if (block is ICollidable collisionObj)
                {
                    if (collisionObj.Passable) continue;

                    if (collisionObj.BoundingBox.Intersects(_futureBoundingBox))
                    {
                        Rectangle intersection = Rectangle.Intersect(_futureBoundingBox, collisionObj.BoundingBox);

                        if (collisionObj is Block)
                        {
                            Block collBlock = (Block)collisionObj;

                            if (collBlock.BlockType == BlockType.DEATH) OnDeath?.Invoke(this);

                            // collision on the X axis
                            if (intersection.Width < intersection.Height)
                            {
                                if (collBlock.BlockType == BlockType.FULL)
                                {
                                    if (_futureBoundingBox.Center.X < collisionObj.BoundingBox.Center.X)
                                        _futurePosition = new Vector2(_futurePosition.X - intersection.Width, _futurePosition.Y);
                                    else
                                        _futurePosition = new Vector2(_futurePosition.X + intersection.Width, _futurePosition.Y);
                                    Speed = new Vector2(0, Speed.Y);
                                }
                            }
                            // collision on the Y axis
                            else
                            {
                                if (_futureBoundingBox.Center.Y < collisionObj.BoundingBox.Center.Y && Speed.Y > 0)
                                {
                                    _futurePosition = new Vector2(_futurePosition.X, _futurePosition.Y - intersection.Height);
                                    isGrounded = true;
                                    Speed = new Vector2(Speed.X, 0);
                                }
                                else
                                {
                                    if (collBlock.BlockType == BlockType.FULL)
                                    {
                                        _futurePosition = new Vector2(_futurePosition.X, _futurePosition.Y + intersection.Height);
                                        Speed = new Vector2(Speed.X, 0);
                                    }
                                }


                            }
                        }
                        _futureBoundingBox = new Rectangle((int)_futurePosition.X, (int)_futurePosition.Y, _futureBoundingBox.Width, _futureBoundingBox.Height);
                    }
                }
            }
            Position = _futurePosition;
            BoundingBox = _futureBoundingBox;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // face sprite in right direction
            if (Speed.X < 0) spriteFx = SpriteEffects.FlipHorizontally;
            if (Speed.X > 0) spriteFx = SpriteEffects.None;

            currentAnimation.Effects = spriteFx;
            currentAnimation.Origin = new Vector2(16 * (spriteFx == SpriteEffects.FlipHorizontally ? 1 : -1), -32);
            currentAnimation.Scale = new Vector2(scale, scale);
            currentAnimation.Draw(spriteBatch, Position + new Vector2(-24, -48));

            // draw bounding box for debugging
            //var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
            //pixel.SetData(new[] { Color.Red });
            //spriteBatch.Draw(pixel, BoundingBox, Color.Green * 0.5f);
        }

        public void TakeDamage(int damage, Vector2 hitPos)
        {
            Health -= damage;
            hitPos.Normalize();
            Speed += new Vector2(4 * hitPos.X, -4);
            isGrounded = false;
            currentState = HeroState.HIT;
            SoundManager.Instance.PlaySound("oof", 0.3f, false);
            if (Health <= 0)
            {
                OnDeath?.Invoke(this);
            }
        }

        public void Attack()
        {
            if (currentState < HeroState.HIT)
            {
                SoundManager.Instance.PlaySound("slice", 0.5f, false);
                currentState = HeroState.ATTACK;
                foreach (var enemy in enemies)
                {
                    var attackBox = new Rectangle(
                        (int)Position.X - BoundingBox.Width / 2,
                        (int)Position.Y - BoundingBox.Height / 2,
                        (int)(BoundingBox.Width * 2),
                        (int)(BoundingBox.Height * 2)
                    );
                    if (enemy is ICollidable collidable && attackBox.Intersects(collidable.BoundingBox))
                    {
                        ((IKillable)enemy).TakeDamage(50);
                    }
                }
            }
        }

    }

    enum HeroState
    {
        IDLE,
        RUNNING,
        JUMPING,
        FALLING,
        HIT,
        ATTACK
    }
}