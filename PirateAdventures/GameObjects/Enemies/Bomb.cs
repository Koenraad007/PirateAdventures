using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;

public class Bomb : IEnemy, ICollidable
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
            _pos = new Vector2(value.X - SPRITE_WIDTH*scale/2, value.Y - SPRITE_HEIGHT*scale/2);
            BoundingBox = new Rectangle((int)(_pos.X + (SPRITE_WIDTH-BOMB_WIDTH)*scale/2), (int)(_pos.Y + (SPRITE_HEIGHT - BOMB_HEIGHT)*scale / 2), (int)(BOMB_WIDTH * scale), (int)(BOMB_HEIGHT * scale));
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

    private Texture2D texture2D;
    private int BombState = 1;
    private float speed = 0, acceleration = 0.3f;
    private float scale = 0.5f;
    private TextureAtlas _textureAtlas;
    private AnimatedSprite _currentAnimation;
    private bool isGrounded = false;
    public bool HasExploded = false;
    private double timer = 0;

    public event Action<IEnemy, int> Attack;

    public Bomb(Texture2D texture, Vector2 position, TextureAtlas ta)
    {
        texture2D = texture;
        _textureAtlas = ta;
        _currentAnimation = _textureAtlas.CreateAnimatedSprite("on");
        Position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentAnimation.Effects = SpriteEffects.None;
        _currentAnimation.Scale = new Vector2(scale, scale);
        _currentAnimation.Draw(spriteBatch, Position);

        var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);

        var explosionBox = new Rectangle(
               (int)(BoundingBox.X - 32),
               (int)(BoundingBox.Y - 16),
               (int)(BoundingBox.Width + 32*2),
               (int)(BoundingBox.Height + 16 * 2));
        spriteBatch.Draw(pixel, explosionBox, Color.Yellow * 0.5f);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        _currentAnimation.Update(gameTime);

        Move();
        CheckCollision(collisionObjects);

        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer >= 3) // bomb explodes after 3 seconds
        {
            BombState = 2;
            _currentAnimation = _textureAtlas.CreateAnimatedSprite("explode");
            _currentAnimation.PlayOnce = true;
            timer = 0;
        }

        if (BombState == 2 && _currentAnimation.CurrentFrame >= _currentAnimation.Animation.Frames.Count-1)
        {
            var hero = collisionObjects.OfType<Hero>().FirstOrDefault();
            var explosionBox = new Rectangle(
                (int)(BoundingBox.X - 32),
                (int)(BoundingBox.Y - 16),
                (int)(BoundingBox.Width + 32 * 2),
                (int)(BoundingBox.Height + 16 * 2));
            if (hero != null && explosionBox.Intersects(hero.BoundingBox))
            {
                Attack?.Invoke(this, 20);
            }
            HasExploded = true;
        }
    }

    public void Move()
    {
        if (!isGrounded) { 
            speed += acceleration;
            _pos.Y += speed;
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

                        if (BoundingBox.Center.Y < collisionObj.BoundingBox.Center.Y && speed >= 0)
                        {
                            _pos.Y -= intersection.Height;
                            speed = 0;
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