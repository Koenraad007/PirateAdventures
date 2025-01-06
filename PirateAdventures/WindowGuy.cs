using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;

public class WindowGuy : IEnemy, ICollidable
{
    public const int SPRITE_WIDTH = 64, SPRITE_HEIGHT = 64;
    private Texture2D texture2D;
    public bool Passable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Rectangle BoundingBox { get; set; }
    public int EnemyState { get; set; } = 0;
    public int EnemyType { get; set; } = 2;
    private List<Animation> animations = new();
    private double mSecondCtr = 0;
    private bool dynamiteThrown = false;

    public WindowGuy(Texture2D texture)
    {
        texture2D = texture;
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

        animations.Add(new Animation());
        animations[0].AddFrame(new AnimationFrame(new Rectangle(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));

        animations.Add(new Animation());
        for (int i = 0; i < 35; i++)
        {
            animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
        // add more frames so animation is 2 seconds (40 frames) long
        for (int i = 0; i < 7; i++)
        {
            animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * 34, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture2D, Position, animations[EnemyState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        var hero = collisionObjects.OfType<Hero>().First();
        if (hero.Position.X > Position.X && hero.Position.X < Position.X + SPRITE_WIDTH)
        {
            System.Console.WriteLine("Same X coords");
            EnemyState = 1;
        }

        switch (EnemyState)
        {
            case 0:
                break;
            case 1:
                mSecondCtr += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (mSecondCtr > 1000 && !dynamiteThrown)
                {
                    System.Console.WriteLine("Throw dynamite");
                    dynamiteThrown = true;
                }
                else if (mSecondCtr > 2000)
                {
                    EnemyState = 0;
                    mSecondCtr = 0;
                    dynamiteThrown = false;
                }
                break;
            default:
                break;
        }

        animations[EnemyState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (EnemyState != i) animations[i].ResetAnimation();
        }
    }
}