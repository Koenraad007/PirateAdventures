using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Animations;
using PirateAdventures.GameObjects;
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
    private Vector2 lockedPos = Vector2.Zero;
    private double secondCtr = 0, msCtr = 0;
    private Color laserColor = Color.Red;

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
        var heroPos = hero.BoundingBox.Center.ToVector2();

        if (heroPos.Y < Position.Y + SPRITE_HEIGHT)
            EnemyState = 1;
        else
        {
            EnemyState = 0;
            secondCtr = 0;
            msCtr = 0;
            lockedPos = heroPos;
        }

        animations[EnemyState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (EnemyState != i) animations[i].ResetAnimation();
        }

        switch (EnemyState)
        {
            case 0:
                break;
            case 1:
                secondCtr += gameTime.ElapsedGameTime.TotalSeconds;
                msCtr += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (secondCtr > 2)
                {
                    lockedPos = heroPos;
                    secondCtr = 0;
                    msCtr = 0;
                }

                // change laser color
                if (msCtr / 100 < 256)
                    laserColor = new Color(255, (int)msCtr / 10, (int)msCtr / 10);

                if (lockedPos.X < Position.X) spriteEffects = SpriteEffects.FlipHorizontally;
                else spriteEffects = SpriteEffects.None;

                // Arm
                var direction = lockedPos - Position;
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
                laserLength = Vector2.Distance(armPos, new Vector2(lockedPos.X + Hero.SPRITE_WIDTH / 2, lockedPos.Y + Hero.SPRITE_HEIGHT / 2));
                // normalize rotation
                rotation = (rotation + MathF.PI * 2) % (MathF.PI * 2);
                break;
        }

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture2D, Position, animations[EnemyState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteEffects, 0);

        if (EnemyState == 1)
        {
            spriteBatch.Draw(laser, armPos, null, laserColor, rotation + MathF.PI / 2, Vector2.Zero, new Vector2(laserLength, 3), SpriteEffects.None, 0);

            spriteBatch.Draw(texture2D, armPos, armSrcRect, Color.White, rotation, rotationOrigin, 1.5f, spriteEffects, 0);
        }
    }
}