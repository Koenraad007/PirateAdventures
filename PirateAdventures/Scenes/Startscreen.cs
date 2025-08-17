using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib;
using MonoGameLib.Scenes;

public class Startscreen: Scene
{
    private Texture2D backgroundTexture;
    private Texture2D titlePirateTexture;
    private Texture2D titleAdventuresTexture;
    private Texture2D buttonsTexture;
    private Rectangle startButtonSrcRectangle, startButtonBounds;
    private Color startBtnColor = Color.White;
    private bool startBtnPressed = false;

    private SpriteFont font;

    public override void Initialize()
    {
        base.Initialize();

        startButtonSrcRectangle = new Rectangle(0, 48, 32, 16);
        startButtonBounds = new Rectangle(Core.GraphicsDevice.Viewport.Width / 2 - 64, Core.GraphicsDevice.Viewport.Height / 2, 128, 64);
    }

    public override void LoadContent()
    {
        backgroundTexture = Core.Content.Load<Texture2D>("start_bg");
        titlePirateTexture = Core.Content.Load<Texture2D>("PirateText");
        titleAdventuresTexture = Core.Content.Load<Texture2D>("AdventuresText");
        buttonsTexture = Core.Content.Load<Texture2D>("BrownButtons");

        font = Core.Content.Load<SpriteFont>("Fonts/Pixellari");
    }

    public override void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
        if (startButtonBounds.Contains(mouseState.Position))
        {
            startBtnColor = Color.Yellow;

            if (startBtnPressed)
            {
                // Start game
                GameStateManager.Instance.ChangeState(GameState.Playing);
                startBtnPressed = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                startButtonSrcRectangle = new Rectangle(160, 48, 32, 16);
                startBtnPressed = true;
            }

        }
        else
        {
            startBtnColor = Color.White;
            startButtonSrcRectangle = new Rectangle(0, 48, 32, 16);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Viewport viewport = Core.GraphicsDevice.Viewport;
        float scaleX = (float)viewport.Width / (float)backgroundTexture.Width;
        float scaleY = (float)viewport.Height / (float)backgroundTexture.Height;
        float pirateScale = (float)100 / (float)titlePirateTexture.Height;
        float adventuresScale = (float)100 / (float)titleAdventuresTexture.Height;

        Core.GraphicsDevice.Clear(new Color(50, 52, 67));

        Core.SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp
            );

        Core.SpriteBatch.Draw(backgroundTexture, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);

        Core.SpriteBatch.Draw(titlePirateTexture, new Vector2((viewport.Width / 2 - (titlePirateTexture.Width * pirateScale) / 2), 100 - (titlePirateTexture.Height * pirateScale)), null, Color.White, 0f, Vector2.Zero, pirateScale, SpriteEffects.None, 0f);
        Core.SpriteBatch.Draw(titleAdventuresTexture, new Vector2((viewport.Width / 2 - (titleAdventuresTexture.Width * adventuresScale) / 2), 150 - (titleAdventuresTexture.Height * adventuresScale)), null, Color.White, 0f, Vector2.Zero, adventuresScale, SpriteEffects.None, 0f);

        startButtonBounds = new Rectangle(Core.GraphicsDevice.Viewport.Width / 2 - 64, Core.GraphicsDevice.Viewport.Height / 2, 128, 64);
        Core.SpriteBatch.Draw(buttonsTexture, startButtonBounds, startButtonSrcRectangle, startBtnColor);

        //Core.SpriteBatch.DrawString(font, "Pirate\nAdventures", new Vector2(viewport.Width / 2 - 100, 40), Color.Brown, 0f, Vector2.Zero, new Vector2(4.1f, 3.1f), SpriteEffects.None, 0f);

        Core.SpriteBatch.End();
    }
}