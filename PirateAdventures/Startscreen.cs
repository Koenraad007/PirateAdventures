using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Startscreen
{
    private Texture2D backgroundTexture;
    private Texture2D titleTexture1;
    private Texture2D titleTexture2;
    private Texture2D startButtonTexture;
    private Rectangle startButtonRectangle;

    public void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("start_bg");
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
    }
}