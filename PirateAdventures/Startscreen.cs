using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Startscreen
{
    private GraphicsDevice _graphicsDevice;
    private Texture2D backgroundTexture;
    private Texture2D titleTexture1;
    private Texture2D titleTexture2;
    private Texture2D startButtonTexture;
    private Rectangle startButtonRectangle;

    public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        backgroundTexture = content.Load<Texture2D>("start_bg");
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Viewport viewport = _graphicsDevice.Viewport;
        float scaleX = (float)viewport.Width / (float)backgroundTexture.Width;
        float scaleY = (float)viewport.Height / (float)backgroundTexture.Height;

        spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
    }
}