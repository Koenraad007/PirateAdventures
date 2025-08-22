using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using PirateAdventures.GameObjects;
using System;
using System.Diagnostics;

namespace PirateAdventures.Managers
{
    public class UIManager
    {
        private readonly SpriteFont _font;
        private Color _playAgainColor = Color.White, _nextLevelColor = Color.White;
        private Rectangle _playAgainSrcRect, _nextLevelSrcRect;
        private Vector2 playAgainPos = Vector2.Zero, nextLevelPos = Vector2.Zero;
        private float scale = 1f;

        public UIManager(SpriteFont font)
        {
            _font = font;
            _playAgainSrcRect = new Rectangle(144, 32, 16, 16);
            _nextLevelSrcRect = new Rectangle(48, 80, 32, 16);
        }

        public void Update(bool isGameOver, bool isLevelComplete)
        {
            if (isGameOver || isLevelComplete)
            {
                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;
                Vector2 centerScreen = new Vector2(screenWidth / 2f, screenHeight / 2f);

                Texture2D texture = isGameOver ? TextureManager.Instance.GetTexture("gameOver") : TextureManager.Instance.GetTexture("levelComplete");

                scale = Math.Min(screenWidth / (texture.Width * 2f), screenHeight / (texture.Height * 2f));

                playAgainPos = centerScreen + new Vector2(0, texture.Height * scale / 2f + 14 * scale);
                Rectangle playAgainBounds = new Rectangle((int)(playAgainPos.X - _playAgainSrcRect.Width * scale / 2), (int)(playAgainPos.Y - _playAgainSrcRect.Height * scale / 2), (int)(_playAgainSrcRect.Width * scale), (int)(_playAgainSrcRect.Height * scale));
                var nextLevelBounds = new Rectangle();
                if (isLevelComplete)
                {
                    playAgainPos += new Vector2(-_playAgainSrcRect.Width * scale, 0);
                    nextLevelPos = centerScreen + new Vector2(_playAgainSrcRect.Width * scale, texture.Height * scale / 2f + 14 * scale);
                    playAgainBounds = new Rectangle((int)(playAgainPos.X - _playAgainSrcRect.Width * scale / 2), (int)(playAgainPos.Y - _playAgainSrcRect.Height * scale / 2), (int)(_playAgainSrcRect.Width * scale), (int)(_playAgainSrcRect.Height * scale));
                    nextLevelBounds = new Rectangle((int)(nextLevelPos.X - _nextLevelSrcRect.Width * scale / 2), (int)(nextLevelPos.Y - _nextLevelSrcRect.Height * scale / 2), (int)(_nextLevelSrcRect.Width * scale), (int)(_nextLevelSrcRect.Height * scale));
                }

                _playAgainColor = playAgainBounds.Contains(Core.Input.Mouse.Position) ? Color.Yellow : Color.White;
                _nextLevelColor = isLevelComplete && nextLevelBounds.Contains(Core.Input.Mouse.Position) ? Color.Yellow : Color.White;

                if (_playAgainColor == Color.Yellow && Core.Input.Mouse.IsButtonDown(MonoGameLib.Input.MouseButton.Left))
                {
                    HandlePlayAgainButton();
                }

                if (_nextLevelColor == Color.Yellow && Core.Input.Mouse.IsButtonDown(MonoGameLib.Input.MouseButton.Left))
                {
                    HandleNextLevelButton();
                }

            }
        }

        public void HandlePlayAgainButton()
        {
            if (_playAgainSrcRect.X == 144)
            {
                _playAgainSrcRect.X += 160;
            }
            else
            {
                Core.ChangeScene(new Scenes.GameScene());
            }
        }

        public void HandleNextLevelButton()
        {
            if (_playAgainSrcRect.X == 48)
            {
                _playAgainSrcRect.X += 160;
            }
            else
            {
                if (LevelManager.Instance.HasNextLevel())
                {
                    LevelManager.Instance.GetNextLevel();
                    Core.ChangeScene(new Scenes.GameScene());
                }
                else
                {
                    Core.ChangeScene(new Scenes.Startscreen());
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Hero hero, int score, bool isGameOver, bool isLevelComplete)
        {
            // Draw HUD
            DrawHealthBar(spriteBatch, hero);
            DrawScore(spriteBatch, score);

            // Draw game over/level complete screen
            if (isGameOver || isLevelComplete)
            {
                DrawGameOverUI(spriteBatch, isGameOver, score);
            }
        }

        private static void DrawHealthBar(SpriteBatch spriteBatch, Hero hero)
        {
            var healthBarTexture = TextureManager.Instance.GetTexture("heroHealth");
            int healthBarWidth = (int)(hero.Health * 2.14f);
            Vector2 healthBarPosition = new Vector2(16, 16);
            var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Red });

            spriteBatch.Draw(
                healthBarTexture,
                healthBarPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                    pixel,
                    healthBarPosition + new Vector2(17 * 2, 14 * 2),
                    new Rectangle(0, 0, healthBarWidth, 4),
                    Color.White
                );
        }

        private void DrawScore(SpriteBatch spriteBatch, int score)
        {
            var scoreTexture = TextureManager.Instance.GetTexture("score");
            Vector2 scorePos = new Vector2(Core.GraphicsDevice.Viewport.Width - (scoreTexture.Width * 2) - 16, 16);
            spriteBatch.Draw(
                scoreTexture,
                new Vector2(Core.GraphicsDevice.Viewport.Width - (scoreTexture.Width * 2) - 16, 16),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f
            );
            Core.SpriteBatch.DrawString(
                _font,
                score.ToString("D7"),
                scorePos + new Vector2(scoreTexture.Width - 10, 10),
                new Color(51, 50, 61),
                0f,
                Vector2.Zero,
                1.6f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawGameOverUI(SpriteBatch spriteBatch, bool isGameOver, int score)
        {
            // darken screen background
            var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, new Rectangle(0, 0, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 128));

            // draw game over/level complete screen background
            var texture = TextureManager.Instance.GetTexture(isGameOver ? "gameOver" : "levelComplete");
            Vector2 centerScreen = new Vector2(Core.GraphicsDevice.Viewport.Width / 2f, Core.GraphicsDevice.Viewport.Height / 2f);
            spriteBatch.Draw(
                    texture,
                    centerScreen,
                    null,
                    Color.White,
                    0f,
                    new Vector2(texture.Width / 2f, texture.Height / 2f),
                    scale,
                    SpriteEffects.None,
                    0f
                );

            // draw score   
            spriteBatch.DrawString(
                    _font,
                    "Score: " + score.ToString("D7"),
                    centerScreen + new Vector2(0, 6f * scale),
                    Color.LightYellow,
                    0f,
                    _font.MeasureString("Score: 0000000") / 2f,
                    scale / 2,
                    SpriteEffects.None,
                    0f
                );

            // draw playAgain button
            Vector2 playAgainOrigin = new Vector2(_playAgainSrcRect.Width / 2f, _playAgainSrcRect.Height / 2f);
            var buttonsTexture = TextureManager.Instance.GetTexture("buttons");
            spriteBatch.Draw(
                buttonsTexture,
                playAgainPos,
                _playAgainSrcRect,
                _playAgainColor,
                0f,
                playAgainOrigin,
                scale,
                SpriteEffects.None,
                0f
            );

            // draw next level button
            Vector2 nextLevelOrigin = new Vector2(_nextLevelSrcRect.Width / 2f, _nextLevelSrcRect.Height / 2f);
            spriteBatch.Draw(
                buttonsTexture,
                nextLevelPos,
                _nextLevelSrcRect,
                _nextLevelColor,
                0f,
                nextLevelOrigin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}