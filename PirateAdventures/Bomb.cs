using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;

public class Bomb : IGameObject, ICollidable
{
    public const int SPRITE_HEIGHT = 176, SPRITE_WIDTH = 160;
    public bool Passable { get; set; } = true;
    public Vector2 Position { get; set; } = new Vector2(1000, 600);
    public Rectangle BoundingBox { get; set; }
    private Texture2D texture2D;
    private List<Animation> animations = new();
    private int BombState = 1;

    public Bomb(Texture2D texture)
    {
        texture2D = texture;
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);

        // Bomb off
        animations.Add(new Animation());
        animations[0].AddFrame(new AnimationFrame(new Rectangle(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));

        // Bomb on  
        animations.Add(new Animation());
        for (int i = 1; i < 11; i++)
        {
            animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }

        // Bomb explode
        animations.Add(new Animation());
        for (int i = 11; i < 20; i++)
        {
            animations[2].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture2D, Position, animations[BombState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        animations[BombState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (BombState != i) animations[i].ResetAnimation();
        }
    }
}