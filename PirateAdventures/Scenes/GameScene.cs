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
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
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
        private Texture2D _gameOverTexture, _playAgainTexture;
        private Rectangle _playAgainSrcRect;
        private bool _isGameOver = false;
        private SpriteFont font;

        public override void Initialize()
        {
            base.Initialize();

            _inputSettings = SettingsManager.LoadSettings();
            _tiledMap.Initialize("./../../../Content/Level1.tmx");
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
            _playAgainTexture = Core.Content.Load<Texture2D>("BrownButtons");

            font = Core.Content.Load<SpriteFont>("Fonts/Pixellari");

        }

        private void InitializeGameObjects()
        {
            KeyboardInputReader kir = new KeyboardInputReader(_inputSettings);

            _gameObjects = _tiledMap.CreateGameObjects(kir);

            _blocks = _tiledMap.CollisionObjects;
            _gameObjects.AddRange(_blocks);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCamera();

            if (_isGameOver || _gameObjects.OfType<EndPoint>().Last()?.Reached == true)
            {
                _isGameOver = true;
                return;
            }

            foreach (IGameObject gameObject in _gameObjects)
            {
                gameObject.Update(_gameObjects, gameTime);
            }
            
            base.Update(gameTime);
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

            // Center the game over menu on the screen
            if (_isGameOver)
            {
                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;

                // Calculate center position in world coordinates
                float viewWidth = screenWidth / _cameraZoom;
                float viewHeight = screenHeight / _cameraZoom;
                Vector2 centerWorld = new Vector2(cameraOffset.X + viewWidth / 2f, cameraOffset.Y + viewHeight / 2f);

                // center the texture
                Vector2 gameOverOrigin = new Vector2(_gameOverTexture.Width / 2f, _gameOverTexture.Height / 2f);

                // make scale accordin to the camera size
                float scale = Math.Min(viewWidth / (_gameOverTexture.Width*2), viewHeight / (_gameOverTexture.Height*2));

                // darken the background
                var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Core.SpriteBatch.Draw(pixel, new Rectangle((int)cameraOffset.X, (int)cameraOffset.Y, (int)viewWidth, (int)viewHeight), new Color(0, 0, 0, 128)); // R,G,B,Alpha (128/255 = ~50% opacity));

                Core.SpriteBatch.Draw(
                    _gameOverTexture,
                    centerWorld,
                    null,
                    Color.White,
                    0f,
                    gameOverOrigin,
                    scale,
                    SpriteEffects.None,
                    0f
                );

                // Draw the score
                int score = 100;
                Core.SpriteBatch.DrawString(
                    font,
                    "Score: "+score.ToString("D7"),
                    centerWorld + new Vector2(-10*scale, _gameOverTexture.Height * scale / 4f - 14f*scale),
                    Color.LightYellow,
                    0f,
                    font.MeasureString("Game Over") / 2f,
                    scale/2,
                    SpriteEffects.None,
                    0f
                );

                // draw the play again button below the game over menu
                Vector2 playAgainOrigin = new Vector2(_playAgainTexture.Width / 2f, _playAgainTexture.Height / 2f);
                Vector2 playAgainPos = centerWorld + new Vector2(_gameOverTexture.Width*scale, _gameOverTexture.Height * scale / 2f + 20f*scale);
                Core.SpriteBatch.Draw(
                    _playAgainTexture,
                    playAgainPos,
                    _playAgainSrcRect,
                    Color.White,
                    0f,
                    playAgainOrigin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }


            Core.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
