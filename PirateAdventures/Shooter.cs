using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PirateAdventures;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;

class Shooter : IEnemy, ICollidable
{
    public const int SPRITE_HEIGHT = 67, SPRITE_WIDTH = 63;
    public int EnemyState { get; set; } = 0;
    public int EnemyType { get; set; } = 1;
    public bool Passable { get; set; } = true;
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Rectangle BoundingBox { get; set; }
    private Rectangle armSrcRect;
    private Texture2D texture2D, laser;
    private List<Animation> animations = new();
    private SpriteEffects spriteEffects = SpriteEffects.None;
    private float rotation = 0, laserLength = 0;
    private Vector2 rotationOrigin = new Vector2(13, 40);
    private Vector2 armPos = Vector2.Zero;

    public Shooter(Texture2D texture)
    {
        texture2D = texture;
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);
        laser = new Texture2D(texture2D.GraphicsDevice, 1, 1);
        laser.SetData(new[] { Color.White });

        animations.Add(new Animation());
        for (int i = 0; i < 34; i++)
        {
            animations[0].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
        animations.Add(new Animation());
        animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * 88, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));

        armSrcRect = new Rectangle(SPRITE_WIDTH * 89, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        var hero = collisionObjects.OfType<Hero>().First();

        if (hero.Position.Y < Position.Y + SPRITE_HEIGHT)
            EnemyState = 1;
        else EnemyState = 0;

        if (hero.Position.X < Position.X && EnemyState == 1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        else spriteEffects = SpriteEffects.None;

        animations[EnemyState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (EnemyState != i) animations[i].ResetAnimation();
        }

        // Arm
        var direction = hero.Position - Position;
        rotation = MathF.Atan2(direction.Y, direction.X);
        // Adjust rotation based on sprite flip
        if (spriteEffects == SpriteEffects.FlipHorizontally)
        {
            rotationOrigin = new Vector2(SPRITE_WIDTH - 13, 40);
            rotation -= MathF.PI / 2;
        }
        else if (spriteEffects == SpriteEffects.None)
        {
            rotationOrigin = new Vector2(13, 40);
            rotation += 3 * MathF.PI / 2;
        }
        armPos = Position + rotationOrigin;
        laserLength = Vector2.Distance(armPos, hero.BoundingBox.Center.ToVector2());
        // normalize rotation
        rotation = (rotation + MathF.PI * 2) % (MathF.PI * 2);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture2D, Position, animations[EnemyState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteEffects, 0);

        // System.Console.WriteLine($"Laser length: {laserLength}, Laser origin: {armPos}, Rotation: {rotation}");

        if (EnemyState == 1)
        {
            spriteBatch.Draw(laser, armPos, null, Color.Red, rotation + MathF.PI / 2, Vector2.Zero, new Vector2(laserLength, 3), SpriteEffects.None, 0);

            spriteBatch.Draw(texture2D, armPos, armSrcRect, Color.White, rotation, rotationOrigin, 1.5f, spriteEffects, 0);
        }
    }
}