using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;

namespace PirateAdventures;

public class Companion : IGameObject, ICollidable
{
    private Texture2D _texture;
    public const int SPRITE_WIDTH = 32;
    public const int SPRITE_HEIGHT = 32;
    public const int MAX_SPEED = 5;
    private Animation _animation;
    private Vector2 speed = new Vector2(2, 2);
    private SpriteEffects spriteFx = SpriteEffects.None;
    private float scale = .5f;
    private KeyboardInputReader input;
    private Vector2 collision = Vector2.Zero;

    public bool Passable { get; set; } = true;
    public Vector2 Position { get; set; }
    public Rectangle BoundingBox { get; set; }

    public Companion(KeyboardInputReader input, Texture2D texture)
    {
        this.input = input;
        _texture = texture;
        this.Position = new Vector2(100, 400);
        this.BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));

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

        //Debug.WriteLine(Vector2.Distance(heroPos, Position));
        if (direction == Vector2.Zero && hero != null && Vector2.Distance(heroPos, Position) > 20f)
        {
            // move towards hero if no input is given
            direction = heroPos - Position;
            direction.Normalize();
        }

        Position += speed * direction;
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

        _animation.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _animation.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), scale, spriteFx, 0);
    }
}