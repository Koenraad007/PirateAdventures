using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using MonoGameLib.Scenes;
using PirateAdventures.GameObjects;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.Scenes
{
    public class GameScene : Scene
    {
        private TextureAtlas _heroAtlas;
        private List<IGameObject> _gameObjects;
        private List<IGameObject> _blocks;
        private TiledMap _tiledMap;
        private Vector2 cameraOffset = Vector2.Zero;
        private const int CAMERA_MARGIN_X = 600, CAMERA_MARGIN_Y = 200;
        private InputSettings _inputSettings;
        private Texture2D _tileset, _enemyTexture;
        private float _cameraZoom = 2f;
        private Texture2D _gameOverTexture, _buttonsTexture, _levelCompleteTexture;
        private Rectangle _playAgainSrcRect;
        private bool _isGameOver = false, _isLevelComplete = false;
        private SpriteFont font;
        private Color _playAgainColor = Color.White;
        private string _levelPath;

        public GameScene(string levelPath)
        {
            _levelPath = levelPath;
        }

        public override void Initialize()
        {
            base.Initialize();

            _inputSettings = SettingsManager.LoadSettings();
            _tiledMap.Initialize(_levelPath);
            _blocks = new List<IGameObject>();

            _playAgainSrcRect = new Rectangle(144, 32, 16, 16);

            InitializeGameObjects();
        }

        public override void LoadContent()
        {
            _heroAtlas = TextureAtlas.FromFile(Core.Content, "hero-atlas.xml");

            _enemyTexture = Core.Content.Load<Texture2D>("enemy_bigguy");

            _tileset = Core.Content.Load<Texture2D>("tileset64");

            _tiledMap = new TiledMap();

            _tiledMap.LoadContent(Core.Content);

            _gameOverTexture = Core.Content.Load<Texture2D>("Menu/GameOver");
            _levelCompleteTexture = Core.Content.Load<Texture2D>("Menu/LevelComplete");
            _buttonsTexture = Core.Content.Load<Texture2D>("BrownButtons");

            font = Core.Content.Load<SpriteFont>("Fonts/Pixellari");

        }

        private void InitializeGameObjects()
        {
            KeyboardInputReader kir = new KeyboardInputReader(_inputSettings);

            _gameObjects = _tiledMap.CreateGameObjects(kir);

            _blocks = _tiledMap.CollisionObjects;
            _gameObjects.AddRange(_blocks);

            foreach (var endPoint in _gameObjects.OfType<EndPoint>())
            {
                endPoint.OnReached += HandleEndPointReached;
            }
            foreach (var hero in _gameObjects.OfType<Hero>())
            {
                hero.OnDeath += HandleGameOver;
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCamera();

            if (_isGameOver || _isLevelComplete)
            {
                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;
                Vector2 centerScreen = new Vector2(screenWidth / 2f, screenHeight / 2f);

                Texture2D texture = _isGameOver ? _gameOverTexture : _levelCompleteTexture;

                float scale = Math.Min(screenWidth / (texture.Width * 2f),
                               screenHeight / (texture.Height * 2f));

                Vector2 playAgainPos = centerScreen + new Vector2(0, texture.Height * scale / 2f + 14*scale);
                Rectangle playAgainBounds = new Rectangle((int)(playAgainPos.X - _playAgainSrcRect.Width * scale / 2), (int)(playAgainPos.Y - _playAgainSrcRect.Height * scale / 2), (int)(_playAgainSrcRect.Width * scale), (int)(_playAgainSrcRect.Height * scale));
               
                if (playAgainBounds.Contains(Core.Input.Mouse.Position))
                {
                    _playAgainColor = Color.Yellow;
                }
                else
                {
                    _playAgainColor = Color.White;
                }
            }
            else
            {
                foreach (IGameObject gameObject in _gameObjects)
                {
                    gameObject.Update(_gameObjects, gameTime);
                }
            }

            base.Update(gameTime);
        }

        private void HandleEndPointReached(EndPoint endPoint)
        {
            _isLevelComplete = true;
        }

        private void HandleGameOver(Hero hero)
        {
            _isGameOver = true;
            Debug.WriteLine("Game Over! Hero died.");
        }

        private void UpdateCamera()
        {
            Hero _hero = _gameObjects.OfType<Hero>().FirstOrDefault();

            int screenWidth = Core.GraphicsDevice.Viewport.Width;
            int screenHeight = Core.GraphicsDevice.Viewport.Height;

            float viewWidth = screenWidth / _cameraZoom;
            float viewHeight = screenHeight / _cameraZoom;
            float marginX = (screenWidth * 0.5f) / _cameraZoom;
            float marginY = (screenHeight * 0.5f) / _cameraZoom;

            // calculate where the hero should be displayed on the screen
            float heroDisplayX = _hero.Position.X - cameraOffset.X;
            float heroDisplayY = _hero.Position.Y - cameraOffset.Y;

            // horizontal scrolling
            if (heroDisplayX < marginX) cameraOffset.X = _hero.Position.X - marginX;
            else if (heroDisplayX > viewWidth - marginX) cameraOffset.X = _hero.Position.X - (viewWidth - marginX);

            // vertical scrolling
            if (heroDisplayY < marginY) cameraOffset.Y = _hero.Position.Y - marginY + 32f;
            else if (heroDisplayY > viewHeight - marginY) cameraOffset.Y = _hero.Position.Y - (viewHeight - marginY) + 32f;

            cameraOffset.X = MathHelper.Clamp(cameraOffset.X, 0, _tiledMap.Width - viewWidth);
            cameraOffset.Y = MathHelper.Clamp(cameraOffset.Y, 0, _tiledMap.Height - viewHeight);
        }

        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(new Color(146, 169, 206));

            var transform = Matrix.CreateTranslation(-cameraOffset.X, -cameraOffset.Y, 0) *
                Matrix.CreateScale(_cameraZoom, _cameraZoom, 1);

            Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    transformMatrix: transform
                );

            _tiledMap.Draw(Core.SpriteBatch);

            foreach (IGameObject gameObject in _gameObjects)
            {
                if (gameObject.GetType() == typeof(Block) || gameObject.GetType() == typeof(Hero)) continue;
                gameObject.Draw(Core.SpriteBatch);
            }

            _gameObjects.OfType<Hero>().FirstOrDefault()?.Draw(Core.SpriteBatch); // Draw hero last to ensure it is on top of other objects

            Core.SpriteBatch.End();

            if (_isGameOver || _isLevelComplete)
            {
                Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp
                );

                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;

                Texture2D texture = _isGameOver ? _gameOverTexture : _levelCompleteTexture;

                float scale = Math.Min(screenWidth / (texture.Width * 2f),
                               screenHeight / (texture.Height * 2f));

                Vector2 centerScreen = new Vector2(screenWidth / 2f, screenHeight / 2f);

                // darken the background
                var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Core.SpriteBatch.Draw(pixel, new Rectangle(0,0,screenWidth, screenHeight), new Color(0, 0, 0, 128));

                Core.SpriteBatch.Draw(
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

                // draw the score
                int score = 100;
                Core.SpriteBatch.DrawString(
                    font,
                    "Score: " + score.ToString("D7"),
                    centerScreen + new Vector2(0,6f*scale),
                    Color.LightYellow,
                    0f,
                    font.MeasureString("Score: 0000000") / 2f,
                    scale / 2,
                    SpriteEffects.None,
                    0f
                );

                Vector2 playAgainPos = centerScreen + new Vector2(0, texture.Height * scale / 2f + 14*scale);
                Vector2 playAgainOrigin = new Vector2(_playAgainSrcRect.Width / 2f, _playAgainSrcRect.Height / 2f);

                Core.SpriteBatch.Draw(
                    _buttonsTexture,
                    playAgainPos,
                    _playAgainSrcRect,
                    _playAgainColor,
                    0f,
                    playAgainOrigin,
                    scale,
                    SpriteEffects.None,
                    0f
                );

                Core.SpriteBatch.End();
            }



            base.Draw(gameTime);
        }
    }
}
