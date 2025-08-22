using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PirateAdventures.GameObjects.Enemies
{
    public class WindowGuy : IEnemy
    {
        public const int SPRITE_WIDTH = 64, SPRITE_HEIGHT = 64;
        public Vector2 Position
        {
            get => _pos;
            set
            {
                _pos = new Vector2(value.X + SPRITE_WIDTH * scale, value.Y + SPRITE_HEIGHT * scale);
                BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
            }
        }
        private Vector2 _pos = Vector2.Zero;
        public Rectangle BoundingBox { get; set; }
        private WindowState state = WindowState.Idle;
        private readonly Sprite _staticSprite;
        private readonly AnimatedSprite _attackAnimation;
        private readonly float scale = 0.5f;
        private bool hasAttacked = false;

        public event Action<WindowGuy, Vector2> SpawnBomb;
        public event Action<IEnemy, int, Vector2> Attack;

        public WindowGuy(TextureAtlas ta)
        {
            BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
            var textureAtlas = ta;
            _staticSprite = textureAtlas.CreateSprite("attack1");
            _attackAnimation = textureAtlas.CreateAnimatedSprite("attack");
            _attackAnimation.PlayOnce = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (state == WindowState.Idle)
            {
                _staticSprite.Scale = new Vector2(.5f, .5f);
                _staticSprite.Draw(spriteBatch, Position);
            }
            else
            {
                _attackAnimation.Effects = SpriteEffects.None;
                _attackAnimation.Scale = new Vector2(.5f, .5f);
                _attackAnimation.Draw(spriteBatch, Position);
            }
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            var hero = collisionObjects.OfType<Hero>().First();
            if (hero.BoundingBox.Center.X > Position.X && hero.BoundingBox.Center.X < Position.X + SPRITE_WIDTH * scale)
            {
                System.Console.WriteLine("Same X coords");
                state = WindowState.Attacking;
            }

            switch (state)
            {
                case WindowState.Idle:
                    break;
                case WindowState.Attacking:
                    _attackAnimation.Update(gameTime);
                    if (_attackAnimation.CurrentFrame <= 0)
                    {
                        state = WindowState.Idle;
                        hasAttacked = false;
                        break;
                    }

                    // if the attack animation is done, throw a bomb
                    if (!hasAttacked && _attackAnimation.CurrentFrame >= Math.Ceiling(_attackAnimation.Animation.Frames.Count / 2f))
                    {
                        SpawnBomb?.Invoke(this, BoundingBox.Center.ToVector2());
                        hasAttacked = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public enum WindowState
    {
        Idle,
        Attacking
    }
}