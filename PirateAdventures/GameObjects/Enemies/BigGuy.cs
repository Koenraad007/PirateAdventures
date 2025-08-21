using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.GameObjects.Enemies
{
    public class BigGuy : IEnemy, ICollidable
    {
        public const int SPRITE_WIDTH = 77;
        public const int SPRITE_HEIGHT = 74;

        private TextureAtlas ta;
        private List<Animations.Animation> animations = new();
        private AnimatedSprite currentAnimation;
        private int prevFrame = 0;
        public int EnemyState { get; set; } = 0;    // 0=idle,1=running,2=attack
        private BigGuyState currentState, prevState = BigGuyState.Idle;
        public int EnemyType { get; set; } = 0;
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
        private float scale = .5f;
        public event Action<IEnemy, int> Attack;

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

            if (hero.BoundingBox.Intersects(BoundingBox))
            {
                EnemyState = 2;
                currentState = BigGuyState.Attacking;
                if (hero.Position.X < _pos.X + SPRITE_WIDTH*scale / 2) spriteEffects = SpriteEffects.FlipHorizontally;
                else spriteEffects = SpriteEffects.None;
                if (currentAnimation.CurrentFrame == currentAnimation.Animation.Frames.Count-1 && currentAnimation.CurrentFrame != prevFrame)
                {
                    Attack?.Invoke(this, 10);
                }
                prevFrame = currentAnimation.CurrentFrame;
            }
            else if (Math.Abs(hero.BoundingBox.Center.X - BoundingBox.Center.X) < 150 && 
                Math.Abs(hero.BoundingBox.Center.X - BoundingBox.Center.X) > 20 && 
                Math.Abs(hero.BoundingBox.Center.Y - BoundingBox.Center.Y) < 20)
            {
                EnemyState = 1;
                currentState = BigGuyState.Running;
                int speed = 2;
                if (hero.Position.X < _pos.X)
                {
                    speed *= -1;
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else spriteEffects = SpriteEffects.None;
                _pos = new Vector2(_pos.X + speed, _pos.Y);
                BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, BoundingBox.Width, BoundingBox.Height);
            }
            else
            {
                EnemyState = 0;
                currentState = BigGuyState.Idle;
            }


            if (currentState != prevState)
            {
                string animationName = $"{currentState.ToString().ToLower()}";
                currentAnimation = ta.CreateAnimatedSprite(animationName);
                prevState = currentState;
            }
            currentAnimation.Update(gameTime);
        }
    }

    public enum BigGuyState
    {
        Idle,
        Running,
        Attacking
    }
}