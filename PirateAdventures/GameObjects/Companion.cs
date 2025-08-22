using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.GameObjects;

public class Companion : IGameObject, ICollidable
{
    private Texture2D _texture;
    public const int SPRITE_WIDTH = 32;
    public const int SPRITE_HEIGHT = 32;
    public const float MAX_SPEED = 3f;
    private Animation _animation;
    private Vector2 speed = Vector2.Zero;
    private Vector2 acceleration = new Vector2(0.1f, 0.1f);
    private SpriteEffects spriteFx = SpriteEffects.None;
    private float scale = .5f;
    private KeyboardInputReader input;

    public bool Passable { get; set; } = true;
    public Vector2 Position { get; set; }
    public Rectangle BoundingBox { get; set; }

    public Companion(KeyboardInputReader input, Texture2D texture)
    {
        this.input = input;
        _texture = texture;
        Position = new Vector2(100, 400);
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

        _animation = new Animation();
        for (int i = 0; i < 9; i++)
        {
            _animation.AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        var hero = collisionObjects.Find(obj => obj is Hero) as Hero;
        var heroPos = hero != null ? hero.BoundingBox.Center.ToVector2()+new Vector2(0f,-32f) : Vector2.Zero;

        var direction = input.ReadBirdInput();
        if (direction.Length() > 0)
        {
            Move(direction);
        }
        else if (Vector2.Distance(heroPos, Position) > 20f)
        {
            direction = heroPos - Position;
            direction.Normalize();
            Move(direction);
        }
        else
        {
            speed *= 0.5f; 
        }

        if (direction.X > 0)
        {
            spriteFx = SpriteEffects.FlipHorizontally;
        }
        else if (direction.X < 0)
        {
            spriteFx = SpriteEffects.None;
        }

        _animation.Update(gameTime);
    }

    private void Move(Vector2 direction)
    {
        if (direction.Length() > 0)
        {
            direction.Normalize();
            speed += direction * acceleration;
            if (speed.Length() > MAX_SPEED)
            {
                speed = Vector2.Normalize(speed) * MAX_SPEED;
            }
        }
       
        Position += speed;
        BoundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            BoundingBox.Width,
            BoundingBox.Height
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _animation.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), scale, spriteFx, 0);
    }
}