using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.GameObjects;

public class Companion : IGameObject, ICollidable, IMovable
{
    public const int SPRITE_WIDTH = 32;
    public const int SPRITE_HEIGHT = 32;
    public const float MAX_SPEED = 3f;
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; } = Vector2.Zero;
    public Vector2 Acceleration { get; set; } = new Vector2(0.1f, 0.1f);
    private SpriteEffects spriteFx = SpriteEffects.None;
    private readonly float scale = .5f;
    public bool Passable { get; set; } = true;
    private Vector2 heroPos;
    private readonly AnimatedSprite currentAnimation;

    public Rectangle BoundingBox { get; set; }

    public Companion()
    {
        Position = new Vector2(100, 400);
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
        var ta = TextureManager.Instance.GetTextureAtlas("companionAtlas");
        currentAnimation = ta.CreateAnimatedSprite("fly");
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        var hero = collisionObjects.Find(obj => obj is Hero) as Hero;
        heroPos = hero != null ? hero.BoundingBox.Center.ToVector2() + new Vector2(0f, -32f) : Vector2.Zero;

        currentAnimation.Update(gameTime);
    }

    public void Move(Vector2 direction)
    {
        if (direction.Length() > 0)
        {
            direction.Normalize();
            Speed += direction * Acceleration;
            if (Speed.Length() > MAX_SPEED)
            {
                Speed = Vector2.Normalize(Speed) * MAX_SPEED;
            }
        }
        else if (Vector2.Distance(heroPos, Position) > 20f)
        {
            direction = heroPos - Position;
            direction.Normalize();
            Speed += direction * Acceleration;
            if (Speed.Length() > MAX_SPEED)
            {
                Speed = Vector2.Normalize(Speed) * MAX_SPEED;
            }
        }
        else
        {
            Speed *= 0.5f;
        }

        Position += Speed;
        BoundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            BoundingBox.Width,
            BoundingBox.Height
        );

        if (direction.X > 0)
        {
            spriteFx = SpriteEffects.FlipHorizontally;
        }
        else if (direction.X < 0)
        {
            spriteFx = SpriteEffects.None;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        currentAnimation.Effects = spriteFx;
        currentAnimation.Scale = new Vector2(scale, scale);
        currentAnimation.Draw(spriteBatch, Position);
    }
}