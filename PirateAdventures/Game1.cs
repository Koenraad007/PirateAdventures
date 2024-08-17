using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PirateAdventures;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _heroTexture;
    private Hero hero;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Add your initialization logic here

        base.Initialize();  // bevat de LoadContent() method, dus na deze lijn zijn de textures geladen

        InitializeGameObjects();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // use this.Content to load your game content here
        _heroTexture = Content.Load<Texture2D>("hero");
    }

    private void InitializeGameObjects()
    {
        hero = new Hero(_heroTexture);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Add your update logic here
        hero.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Add your drawing code here
        _spriteBatch.Begin();

        hero.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
