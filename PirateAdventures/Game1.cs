using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using PirateAdventures.Settings;
using MonoGameLib;


namespace PirateAdventures;

public class Game1 : Core
{
    private Startscreen startscreen;
    private GameStateManager _stateManager;
    private InputSettings inputSettings;

    private Texture2D _heroTexture, _tileset, _enemyTexture, _companionTexture;
    private Hero hero;
    private Companion companion;
    private List<IGameObject> _blocks;
    private List<IGameObject> _enemies;
    private TiledMap tiledMap;
    private Vector2 cameraOffset = Vector2.Zero;
    private const int CAMERA_MARGIN_X = 400, CAMERA_MARGIN_Y = 200;

    public Game1(): base("Pirate Adventures", 1000, 800, false)
    {
    }

    protected override void Initialize()
    {
        // Add your initialization logic here

        _stateManager = GameStateManager.Instance;
        _stateManager.ChangeState(GameState.Start);
        inputSettings = SettingsManager.LoadSettings();

        startscreen = new Startscreen();

        _blocks = new List<IGameObject>();

        base.Initialize();  // bevat de LoadContent() method, dus na deze lijn zijn de textures geladen

        // TODO: store level in different location
        tiledMap.Initialize("./../../../Content/naamloos.tmx");
        startscreen.Initialize(GraphicsDevice);
        InitializeGameObjects();


    }

    protected override void LoadContent()
    {
        startscreen.LoadContent(Content);

        // use this.Content to load your game content here
        _heroTexture = Content.Load<Texture2D>("cptclownnose20fps");
        _companionTexture = Content.Load<Texture2D>("bluebird20fps");
        _enemyTexture = Content.Load<Texture2D>("enemy_bigguy");

        _tileset = Content.Load<Texture2D>("tileset64");

        tiledMap = new TiledMap();

        tiledMap.LoadContent(Content);

    }

    private void InitializeGameObjects()
    {
        KeyboardInputReader kir = new KeyboardInputReader(inputSettings);

        hero = new Hero(_heroTexture, kir);
        companion = new Companion(kir, _companionTexture);
        //bigGuy = new BigGuy(_enemyTexture);
        _enemies = tiledMap.CreateEnemyObjects();

        _blocks = tiledMap.CollisionObjects;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            SettingsManager.SaveSettings(inputSettings);
            Exit();
        }


        switch (_stateManager.CurrentState)
        {
            case GameState.Start:
                startscreen.Update(gameTime);
                break;
            case GameState.Playing:
                hero.Update(_blocks, gameTime);
                companion.Update(_blocks, gameTime);
                foreach (IGameObject enemy in _enemies)
                {
                    var gameObjects = new List<IGameObject>() { hero };
                    gameObjects.AddRange(_blocks);
                    enemy.Update(gameObjects, gameTime);
                }
                UpdateCamera();
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    private void UpdateCamera()
    {
        int screenWidth = GraphicsDevice.Viewport.Width;
        int screenHeight = GraphicsDevice.Viewport.Height;

        // calculate where the hero should be displayed on the screen
        float heroDisplayX = hero.Position.X - cameraOffset.X;
        float heroDisplayY = hero.Position.Y - cameraOffset.Y;

        // horizontal scrolling
        if (heroDisplayX < CAMERA_MARGIN_X) cameraOffset.X = hero.Position.X - CAMERA_MARGIN_X;
        else if (heroDisplayX > screenWidth - CAMERA_MARGIN_X) cameraOffset.X = hero.Position.X - (screenWidth - CAMERA_MARGIN_X);

        // vertical scrolling
        if (heroDisplayY < CAMERA_MARGIN_Y) cameraOffset.Y = hero.Position.Y - CAMERA_MARGIN_Y;
        else if (heroDisplayY > screenHeight - CAMERA_MARGIN_Y) cameraOffset.Y = hero.Position.Y - (screenHeight - CAMERA_MARGIN_Y);

        cameraOffset.X = MathHelper.Clamp(cameraOffset.X, 0, tiledMap.Width - screenWidth);
        cameraOffset.Y = MathHelper.Clamp(cameraOffset.Y, 0, tiledMap.Height - screenHeight);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(50, 52, 67));

        // Add your drawing code here
        SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateTranslation(-cameraOffset.X, -cameraOffset.Y, 0)
            );

        switch (_stateManager.CurrentState)
        {
            case GameState.Start:
                startscreen.Draw(SpriteBatch);
                break;

            case GameState.Playing:
                tiledMap.Draw(SpriteBatch);

                foreach (IGameObject enemy in _enemies)
                {
                    enemy.Draw(SpriteBatch);
                }

                companion.Draw(SpriteBatch);
                hero.Draw(SpriteBatch);
                break;

            case GameState.GameOver:
                break;

            default:
                break;
        }

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
