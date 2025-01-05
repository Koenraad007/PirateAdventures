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


namespace PirateAdventures;

public class Game1 : Game
{
    private Startscreen startscreen;
    private GraphicsDeviceManager _graphics;
    private GameStateManager _stateManager;
    private SpriteBatch _spriteBatch;

    private Texture2D _heroTexture, _tileset, _enemyTexture;
    private Hero hero;
    private BigGuy bigGuy;
    private List<IGameObject> _blocks;
    private TiledMap tiledMap;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;  // zorgt ervoor dat we hoger resolutie sprites kunnen gebruiken
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Add your initialization logic here

        _stateManager = GameStateManager.Instance;
        _stateManager.ChangeState(GameState.Start);

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

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // use this.Content to load your game content here
        _heroTexture = Content.Load<Texture2D>("hero");
        _enemyTexture = Content.Load<Texture2D>("enemy_bigguy");

        _tileset = Content.Load<Texture2D>("tileset64");

        tiledMap = new TiledMap();

        tiledMap.LoadContent(Content);

    }

    private void InitializeGameObjects()
    {
        hero = new Hero(_heroTexture, new KeyboardInputReader());
        bigGuy = new BigGuy(_enemyTexture);

        _blocks = tiledMap.CollisionObjects;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        switch (_stateManager.CurrentState)
        {
            case GameState.Start:
                startscreen.Update(gameTime);
                break;
            case GameState.Playing:
                hero.Update(_blocks, gameTime);
                bigGuy.Update(new List<IGameObject>() { hero }, gameTime);
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(50, 52, 67));

        // Add your drawing code here
        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp
            );

        switch (_stateManager.CurrentState)
        {
            case GameState.Start:
                startscreen.Draw(_spriteBatch);
                break;

            case GameState.Playing:
                tiledMap.Draw(_spriteBatch);

                bigGuy.Draw(_spriteBatch);

                hero.Draw(_spriteBatch);
                break;

            case GameState.GameOver:
                break;

            default:
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
