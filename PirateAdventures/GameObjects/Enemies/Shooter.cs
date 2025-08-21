using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

class Shooter : IEnemy, ICollidable, IKillable
{
    public const int SPRITE_HEIGHT = 67, SPRITE_WIDTH = 63;
    public int EnemyState { get; set; } = 0;
    public int EnemyType { get; set; } = 1;
    public bool Passable { get; set; } = true;
    private float scale = .5f;
    private Vector2 _pos = Vector2.Zero;
    public Vector2 Position
    {
        get => _pos;
        set
        {
            _pos = new Vector2(value.X + SPRITE_WIDTH * scale, value.Y + SPRITE_HEIGHT * scale);
            BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)Math.Ceiling(SPRITE_WIDTH * scale), (int)Math.Ceiling(SPRITE_HEIGHT * scale));
        }
    }
    public Rectangle BoundingBox { get; set; }
    public int Health { get; set; } = 100;

    private Texture2D texture2D, laser;
    private List<PirateAdventures.Animations.Animation> animations = new();
    private SpriteEffects spriteEffects = SpriteEffects.None;
    private float rotation = 0, laserLength = 300;
    private Vector2 rotationOrigin = new Vector2(13, 40);
    private Vector2 armPos = Vector2.Zero;
    private Vector2 lockedPos = Vector2.Zero;
    private double secondCtr = 0, msCtr = 0;
    private Color laserColor = Color.Red;
    private TextureAtlas textureAtlas;
    private AnimatedSprite currentAnimation;
    private Sprite attackBody, attackArm;

    public event Action<IEnemy, int, Vector2> Attack;
    public event Action<Shooter, Vector2, Vector2, float> Shoot;

    public Shooter(TextureAtlas ta)
    {
        textureAtlas = ta;
        currentAnimation = textureAtlas.CreateAnimatedSprite("idle");
        attackBody = textureAtlas.CreateSprite("attack-body");
        attackArm = textureAtlas.CreateSprite("attack-arm");
        BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)Math.Ceiling(SPRITE_WIDTH * scale), (int)Math.Ceiling(SPRITE_HEIGHT * scale));
        armPos = BoundingBox.Center.ToVector2();
        laser = new Texture2D(Core.GraphicsDevice, 1, 1);
        laser.SetData(new[] { Color.White });
    }

    public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
    {
        var hero = collisionObjects.OfType<Hero>().First();
        var heroPos = new Vector2(hero.BoundingBox.X, hero.BoundingBox.Y);

        if (DoesLaserHitHero(hero, collisionObjects))
        {
            EnemyState = 1;
        }
        else
        {
            EnemyState = 0;
            secondCtr = 0;
            msCtr = 0;
            lockedPos = heroPos;
        }

        switch (EnemyState)
        {
            case 0:
                break;
            case 1:
                secondCtr += gameTime.ElapsedGameTime.TotalSeconds;
                msCtr += gameTime.ElapsedGameTime.TotalMilliseconds;


                // change laser color
                if (msCtr / 200 < 256)
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
                armPos = Position + rotationOrigin * scale;
                //laserLength = SPRITE_WIDTH * scale;
                laserLength = Vector2.Distance(armPos, new Vector2(lockedPos.X + Hero.SPRITE_WIDTH / 2, lockedPos.Y + Hero.SPRITE_HEIGHT / 2)) + 16;
                // normalize rotation
                rotation = (rotation + MathF.PI * 2) % (MathF.PI * 2);

                lockedPos = heroPos;
                if (secondCtr > 5)
                {
                    secondCtr = 0;
                    msCtr = 0;
                    Shoot?.Invoke(this, armPos, direction, 100f);
                }
                break;
        }

        currentAnimation.Update(gameTime);

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (EnemyState == 0)
        {
            currentAnimation.Effects = spriteEffects;
            currentAnimation.Scale = new Vector2(scale, scale);
            currentAnimation.Draw(spriteBatch, Position);
        }
        if (EnemyState == 1)
        {
            attackBody.Effects = spriteEffects;
            attackBody.Scale = new Vector2(scale, scale);
            attackBody.Draw(spriteBatch, Position);

            spriteBatch.Draw(laser, armPos, null, laserColor, rotation + MathF.PI / 2, Vector2.Zero, new Vector2(SPRITE_WIDTH, 2), SpriteEffects.None, 0);

            attackArm.Effects = spriteEffects;
            attackArm.Scale = new Vector2(scale, scale);
            attackArm.Rotation = rotation;
            attackArm.Origin = rotationOrigin;
            attackArm.Draw(spriteBatch, armPos);
        }

        //var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        //pixel.SetData(new[] { Color.Red });
        //spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);
    }

    private bool DoesLaserHitHero(Hero hero, List<IGameObject> collisionObjects)
    {
        Vector2 laserStart = BoundingBox.Center.ToVector2();
        Vector2 laserDirection = new Vector2(hero.BoundingBox.X, hero.BoundingBox.Y) - Position;

        var hitResult = Raycast(laserStart, laserDirection, 300, collisionObjects);

        return hitResult != null && hitResult == hero;
    }

    // Raycast method that returns the first object hit
    private IGameObject Raycast(Vector2 start, Vector2 direction, float maxDistance, List<IGameObject> objects)
    {
        Vector2 end = start + direction * maxDistance;
        float closestDistance = float.MaxValue;
        IGameObject closestObject = null;

        foreach (var obj in objects)
        {
            if (!(obj is Hero) && (!(obj is ICollidable collidable) || collidable.Passable)) continue;

            collidable = obj as ICollidable;

            // Check if laser intersects with this object
            if (LineIntersectsRectangle(start, end, collidable.BoundingBox))
            {
                // Calculate distance to this object
                Vector2 objCenter = new Vector2(
                    collidable.BoundingBox.X + collidable.BoundingBox.Width / 2f,
                    collidable.BoundingBox.Y + collidable.BoundingBox.Height / 2f
                );

                float distance = Vector2.Distance(start, objCenter);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }

    // Line-Rectangle intersection helper method
    private bool LineIntersectsRectangle(Vector2 lineStart, Vector2 lineEnd, Rectangle rect)
    {
        // Convert rectangle to four line segments
        Vector2 topLeft = new Vector2(rect.Left, rect.Top);
        Vector2 topRight = new Vector2(rect.Right, rect.Top);
        Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
        Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

        // Check intersection with each edge of the rectangle
        return LineIntersectsLine(lineStart, lineEnd, topLeft, topRight) ||      // Top edge
               LineIntersectsLine(lineStart, lineEnd, topRight, bottomRight) ||  // Right edge
               LineIntersectsLine(lineStart, lineEnd, bottomRight, bottomLeft) || // Bottom edge
               LineIntersectsLine(lineStart, lineEnd, bottomLeft, topLeft) ||     // Left edge
               rect.Contains(lineStart.ToPoint()) ||                              // Start point inside rect
               rect.Contains(lineEnd.ToPoint());                                  // End point inside rect
    }

    // Line-Line intersection helper method
    private bool LineIntersectsLine(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
    {
        float denom = (line1End.X - line1Start.X) * (line2End.Y - line2Start.Y) -
                      (line1End.Y - line1Start.Y) * (line2End.X - line2Start.X);

        if (Math.Abs(denom) < 1e-10f) return false; // Lines are parallel

        float t = ((line2Start.X - line1Start.X) * (line2End.Y - line2Start.Y) -
                   (line2Start.Y - line1Start.Y) * (line2End.X - line2Start.X)) / denom;

        float u = -((line1Start.X - line1End.X) * (line1Start.Y - line2Start.Y) -
                    (line1Start.Y - line1End.Y) * (line1Start.X - line2Start.X)) / denom;

        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }
}