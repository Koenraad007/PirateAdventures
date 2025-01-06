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
    private Texture2D texture2D;
    private List<Animation> animations = new();
    private SpriteEffects spriteEffects = SpriteEffects.None;
    private float rotation = 0;

    public Shooter(Texture2D texture)
    {
        texture2D = texture;
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

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

        EnemyState = 1;

        if (hero.Position.X < Position.X)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        else spriteEffects = SpriteEffects.None;

        animations[EnemyState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (EnemyState != i) animations[i].ResetAnimation();
        }



        this.rotation += 1;
        if (rotation == 360) rotation = 0;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture2D, Position, animations[EnemyState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteEffects, 0);


        // Arm
        var rotationOrigin = new Vector2(13, 40);
        var armPos = new Vector2(Position.X + 13, Position.Y + 40);
        if (spriteEffects == SpriteEffects.FlipHorizontally)
        {
            rotationOrigin = new Vector2(SPRITE_WIDTH - 13, 40);
            armPos = new Vector2(Position.X + SPRITE_WIDTH - 13, Position.Y + 40);
        }
        spriteBatch.Draw(texture2D, armPos, armSrcRect, Color.White, MathHelper.ToRadians(rotation), rotationOrigin, 1.5f, spriteEffects, 0);
    }
}