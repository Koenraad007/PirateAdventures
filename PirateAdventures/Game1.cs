using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PirateAdventures;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;
    private Rectangle _deelRectangle;
    private int _schuifOp_X = 0;

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
        _deelRectangle = new Rectangle(_schuifOp_X,0,58,58);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // use this.Content to load your game content here
        _texture = Content.Load<Texture2D>("hero");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Add your update logic here
        _schuifOp_X += 58;
        if (_schuifOp_X > 26*58) _schuifOp_X = 0;
        _deelRectangle.X = _schuifOp_X;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Add your drawing code here
        _spriteBatch.Begin();

        _spriteBatch.Draw(_texture, new Vector2(0, 0), _deelRectangle, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
