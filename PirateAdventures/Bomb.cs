using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;

public class Bomb : IGameObject, ICollidable
{
    public const int SPRITE_HEIGHT = 160, SPRITE_WIDTH = 176;
    public const int BOMB_WIDTH = 32, BOMB_HEIGHT = 48;
    public bool Passable { get; set; } = true;
    public Vector2 Position { get; set; } = new Vector2(1000, 600);
    public Vector2 Center
    {
        get => Position - new Vector2(SPRITE_WIDTH / 2, SPRITE_HEIGHT / 2);
        set => Position = value + new Vector2(SPRITE_WIDTH / 2, SPRITE_HEIGHT / 2);
    }
    public Rectangle BoundingBox { get; set; }
    private Texture2D texture2D;
    private List<Animation> animations = new();
    private int BombState = 1;
    private float speed = 0, acceleration = 0.3f;

    public Bomb(Texture2D texture, Vector2 position)
    {
        texture2D = texture;
        Position = position;
        // BoundingBox = new Rectangle((int)Center.X - BOMB_WIDTH / 2, (int)Center.Y, BOMB_WIDTH, BOMB_HEIGHT);
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);

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
        spriteBatch.Draw(texture2D, Center + new Vector2(0, SPRITE_HEIGHT - 32), animations[BombState].CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        Move();
        CheckCollision(collisionObjects);
        Position = new Vector2(BoundingBox.X, BoundingBox.Y);

        animations[BombState].Update(gameTime);
        for (int i = 0; i < animations.Count; i++)
        {
            if (BombState != i) animations[i].ResetAnimation();
        }
    }

    public void Move()
    {
        speed += acceleration;

        Position += new Vector2(0, speed);
        // BoundingBox = new Rectangle((int)Center.X - BOMB_WIDTH / 2, (int)Center.Y, BOMB_WIDTH, BOMB_HEIGHT);
        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);
    }

    private void CheckCollision(List<IGameObject> objects)
    {
        // TODO: fix bomb collision

        // System.Console.WriteLine($"Length of objects: {objects.Count}");
        System.Console.WriteLine($"Bomb position: {Position}");
        System.Console.WriteLine($"Bomb bounding box: {BoundingBox}");


        foreach (var block in objects)
        {
            if (block is ICollidable)
            {
                var collisionObj = block as ICollidable;

                if (collisionObj.Passable) continue;
                // System.Console.WriteLine($"Object bounding box: {collisionObj.BoundingBox}");

                if (collisionObj.BoundingBox.Intersects(BoundingBox))
                {
                    System.Console.WriteLine("Collision");
                    Rectangle intersection = Rectangle.Intersect(BoundingBox, collisionObj.BoundingBox);

                    if (collisionObj is Block)
                    {
                        Block collBlock = (Block)collisionObj;

                        // // collision on the X axis
                        // if (intersection.Width < intersection.Height)
                        // {
                        //     if (collBlock.BlockType == BlockType.FULL)
                        //     {
                        //         if (BoundingBox.Center.X < collisionObj.BoundingBox.Center.X)
                        //             Position = new Vector2(Position.X - intersection.Width, Position.Y);
                        //         else
                        //             Position = new Vector2(Position.X + intersection.Width, Position.Y);
                        //         speed.X = 0;
                        //     }
                        // }
                        // collision on the Y axis

                        if (BoundingBox.Center.Y < collisionObj.BoundingBox.Center.Y && speed > 0)
                        {
                            Position = new Vector2(Position.X, Position.Y - intersection.Height);
                            // isGrounded = true;
                            speed = 0;
                        }
                        else
                        {
                            if (collBlock.BlockType == BlockType.FULL)
                            {
                                Position = new Vector2(Position.X, Position.Y + intersection.Height);
                                speed = 0;
                            }
                        }



                    }
                    // BoundingBox = new Rectangle((int)Center.X - BOMB_WIDTH / 2, (int)Center.Y - BOMB_HEIGHT / 2, BoundingBox.Width, BoundingBox.Height);
                    BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, BoundingBox.Width, BoundingBox.Height);
                }
            }
        }
    }
}