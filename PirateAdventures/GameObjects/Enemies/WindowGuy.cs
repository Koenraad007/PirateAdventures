using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.GameObjects;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public class WindowGuy : IEnemy
{
    public const int SPRITE_WIDTH = 64, SPRITE_HEIGHT = 64;
    private Texture2D texture2D, bombTexture;
    public Vector2 Position
    {
        get => _pos;
        set
        {
            _pos = new Vector2(value.X + SPRITE_WIDTH * scale, value.Y + SPRITE_HEIGHT * scale);
            BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
        }
    }
    private Vector2 _pos = Vector2.Zero;
    public Rectangle BoundingBox { get; set; }
    public int EnemyState { get; set; } = 0;
    public int EnemyType { get; set; } = 2;
    private List<PirateAdventures.Animations.Animation> animations = new();
    private double mSecondCtr = 0;
    private bool dynamiteThrown = false;
    private List<Bomb> bombs = new();
    TextureAtlas textureAtlas;
    private Sprite _staticSprite;
    private MonoGameLib.Graphics.AnimatedSprite _attackAnimation;
    private float scale = 0.5f;
    private bool hasAttacked = false;

    public event Action<WindowGuy, Vector2> SpawnBomb;
    public event Action<IEnemy, int> Attack;

    public WindowGuy(Texture2D texture, Texture2D dynamiteTexture, TextureAtlas ta)
    {
        texture2D = texture;
        bombTexture = dynamiteTexture;
        BoundingBox = new Rectangle((int)_pos.X, (int)_pos.Y, (int)(SPRITE_WIDTH * scale), (int)(SPRITE_HEIGHT * scale));
        textureAtlas = ta;
        _staticSprite = textureAtlas.CreateSprite("attack1");
        _attackAnimation = textureAtlas.CreateAnimatedSprite("attack");
        _attackAnimation.PlayOnce = false;

        animations.Add(new PirateAdventures.Animations.Animation());
        animations[0].AddFrame(new AnimationFrame(new Rectangle(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));

        animations.Add(new PirateAdventures.Animations.Animation());
        for (int i = 0; i < 35; i++)
        {
            animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * i, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
        // add more frames so animation is 2 seconds (40 frames) long
        for (int i = 0; i < 5; i++)
        {
            animations[1].AddFrame(new AnimationFrame(new Rectangle(SPRITE_WIDTH * 34, 0, SPRITE_WIDTH, SPRITE_HEIGHT)));
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (EnemyState == 0)
        {
            _staticSprite.Scale = new Vector2(.5f, .5f);
            _staticSprite.Draw(spriteBatch, Position);
        }
        else
        {
            _attackAnimation.Effects = SpriteEffects.None;
            _attackAnimation.Scale = new Vector2(.5f, .5f);
            _attackAnimation.Draw(spriteBatch, Position);
        }

        var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Red });
        spriteBatch.Draw(pixel, BoundingBox, Color.Red * 0.5f);

        foreach (var bomb in bombs)
        {
            bomb.Draw(spriteBatch);
        }
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
                _attackAnimation.Update(gameTime);
                if (_attackAnimation.CurrentFrame <= 0)
                {
                    EnemyState = 0;
                    hasAttacked = false;
                    break;
                }

                // if the attack animation is done, throw a bomb
                if (!hasAttacked && _attackAnimation.CurrentFrame >= Math.Ceiling(_attackAnimation.Animation.Frames.Count/2f))
                {
                    SpawnBomb?.Invoke(this, BoundingBox.Center.ToVector2());
                    hasAttacked = true;
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

        foreach (var bomb in bombs)
        {
            bomb.Update(collisionObjects, gameTime);
        }
    }
}