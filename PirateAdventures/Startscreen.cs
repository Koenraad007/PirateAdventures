using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Startscreen
{
    private GraphicsDevice _graphicsDevice;
    private Texture2D backgroundTexture;
    private Texture2D titlePirateTexture;
    private Texture2D titleAdventuresTexture;
    private Texture2D buttonsTexture;
    private Rectangle startButtonSrcRectangle, startButtonBounds;

    public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        backgroundTexture = content.Load<Texture2D>("start_bg");
        titlePirateTexture = content.Load<Texture2D>("PirateText");
        titleAdventuresTexture = content.Load<Texture2D>("AdventuresText");
        buttonsTexture = content.Load<Texture2D>("BrownButtons");
        startButtonSrcRectangle = new Rectangle(0, 48, 32, 16);
        startButtonBounds = new Rectangle(_graphicsDevice.Viewport.Width / 2 - 64, _graphicsDevice.Viewport.Height / 2, 128, 64);
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
        if (startButtonBounds.Contains(mouseState.Position))
        {
            startButtonSrcRectangle = new Rectangle(160, 48, 32, 16);
        }
        else
        {
            startButtonSrcRectangle = new Rectangle(0, 48, 32, 16);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Viewport viewport = _graphicsDevice.Viewport;
        float scaleX = (float)viewport.Width / (float)backgroundTexture.Width;
        float scaleY = (float)viewport.Height / (float)backgroundTexture.Height;
        float pirateScale = (float)100 / (float)titlePirateTexture.Height;
        float adventuresScale = (float)100 / (float)titleAdventuresTexture.Height;

        spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);

        spriteBatch.Draw(titlePirateTexture, new Vector2((viewport.Width / 2 - (titlePirateTexture.Width * pirateScale) / 2), 100 - (titlePirateTexture.Height * pirateScale)), null, Color.White, 0f, Vector2.Zero, pirateScale, SpriteEffects.None, 0f);
        spriteBatch.Draw(titleAdventuresTexture, new Vector2((viewport.Width / 2 - (titleAdventuresTexture.Width * adventuresScale) / 2), 150 - (titleAdventuresTexture.Height * adventuresScale)), null, Color.White, 0f, Vector2.Zero, adventuresScale, SpriteEffects.None, 0f);

        spriteBatch.Draw(buttonsTexture, startButtonBounds, startButtonSrcRectangle, Color.White);
    }
}