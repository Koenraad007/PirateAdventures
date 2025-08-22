using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Managers;

public class Bomb : IEnemy, ICollidable, IMovable
{
    public const int SPRITE_HEIGHT = 160, SPRITE_WIDTH = 176;
    public const int BOMB_WIDTH = 32, BOMB_HEIGHT = 64;
    public bool Passable { get; set; } = true;
    private Vector2 _pos = Vector2.Zero;
    public Vector2 Position
    {
        get => _pos;
        set
        {
            _pos = new Vector2(value.X - SPRITE_WIDTH * scale / 2, value.Y - SPRITE_HEIGHT * scale / 2);
            BoundingBox = new Rectangle((int)(_pos.X + (SPRITE_WIDTH - BOMB_WIDTH) * scale / 2), (int)(_pos.Y + (SPRITE_HEIGHT - BOMB_HEIGHT) * scale / 2), (int)(BOMB_WIDTH * scale), (int)(BOMB_HEIGHT * scale));
        }
    }
    public Vector2 Center
    {
        get => Position - new Vector2(SPRITE_WIDTH / 2, SPRITE_HEIGHT / 2);
        set => Position = value + new Vector2(SPRITE_WIDTH / 2, SPRITE_HEIGHT / 2);
    }
    public Rectangle BoundingBox { get; set; }
    public int EnemyState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int EnemyType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private int currentState = 1, prevState = 1;
    public Vector2 Speed { get; set; } = Vector2.Zero;
    public Vector2 Acceleration { get; set; } = new Vector2(0, .3f);
    private float scale = 0.5f;
    private TextureAtlas _textureAtlas;
    private AnimatedSprite _currentAnimation;
    private bool isGrounded = false, _damageDealt = false;
    public bool HasExploded = false;
    private double timer = 0;

    public event Action<IEnemy, int, Vector2> Attack;

    public Bomb(Vector2 position)
    {
        _textureAtlas = TextureManager.Instance.GetTextureAtlas("bombAtlas");
        _currentAnimation = _textureAtlas.CreateAnimatedSprite("on");
        Position = position;

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentAnimation.Effects = SpriteEffects.None;
        _currentAnimation.Scale = new Vector2(scale, scale);
        _currentAnimation.Draw(spriteBatch, Position);

        // var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        // pixel.SetData(new[] { Color.White });
        // spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);

        // var explosionBox = new Rectangle(
        //        (int)(BoundingBox.X - 32),
        //        (int)(BoundingBox.Y - 16),
        //        (int)(BoundingBox.Width + 32 * 2),
        //        (int)(BoundingBox.Height + 16 * 2));
        // spriteBatch.Draw(pixel, explosionBox, Color.Yellow * 0.5f);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        _currentAnimation.Update(gameTime);

        Move(new Vector2(0, 1));
        CheckCollision(collisionObjects);

        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer >= 3) // bomb explodes after 3 seconds
        {
            currentState = 2;
            _currentAnimation = _textureAtlas.CreateAnimatedSprite("explode");
            _currentAnimation.PlayOnce = true;
            timer = 0;
        }

        if (currentState == 2)
        {
            if (currentState != prevState)
            {
                SoundManager.Instance.PlaySound("explosion", .2f, false);
                prevState = currentState;
            }

            if (_currentAnimation.CurrentFrame >= _currentAnimation.Animation.Frames.Count / 2 && !_damageDealt)
            {
                var hero = collisionObjects.OfType<Hero>().FirstOrDefault();
                var explosionBox = new Rectangle(
                    (int)(BoundingBox.X - 16),
                    (int)(BoundingBox.Y - 8),
                    (int)(BoundingBox.Width + 16 * 2),
                    (int)(BoundingBox.Height + 8 * 2));
                if (hero != null && explosionBox.Intersects(hero.BoundingBox))
                {
                    var attackDirection = hero.BoundingBox.Center.ToVector2() - BoundingBox.Center.ToVector2();
                    Debug.WriteLine($"Bomb explosion at {BoundingBox.Center} with direction {attackDirection}");
                    Attack?.Invoke(this, 20, attackDirection);
                    _damageDealt = true;
                }
            }

            if (_currentAnimation.CurrentFrame >= _currentAnimation.Animation.Frames.Count - 1)
            {
                HasExploded = true;
            }
        }
    }

    public void Move(Vector2 direction)
    {
        if (!isGrounded)
        {
            Speed += Acceleration;
            _pos.Y += Speed.Y;
        }

        BoundingBox = new Rectangle(
            (int)Math.Ceiling(_pos.X + (SPRITE_WIDTH - BOMB_WIDTH) * scale / 2),
            (int)Math.Ceiling(_pos.Y + (SPRITE_HEIGHT - BOMB_HEIGHT) * scale / 2),
            (int)(BOMB_WIDTH * scale),
            (int)(BOMB_HEIGHT * scale));
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
                    System.Console.WriteLine("Collision");
                    Rectangle intersection = Rectangle.Intersect(BoundingBox, collisionObj.BoundingBox);

                    if (collisionObj is Block)
                    {
                        Block collBlock = (Block)collisionObj;

                        if (BoundingBox.Center.Y < collisionObj.BoundingBox.Center.Y && Speed.Y >= 0)
                        {
                            _pos.Y -= intersection.Height;
                            Speed = Vector2.Zero;
                            isGrounded = true;
                        }
                        else
                        {
                            isGrounded = false;
                        }
                    }
                    BoundingBox = new Rectangle((int)(_pos.X + (SPRITE_WIDTH - BOMB_WIDTH) * scale / 2f), (int)(_pos.Y + (SPRITE_HEIGHT - BOMB_HEIGHT) * scale / 2f), (int)(BOMB_WIDTH * scale), (int)(BOMB_HEIGHT * scale));
                }
            }
        }
    }
}