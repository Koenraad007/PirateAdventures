using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using MonoGameLib.Scenes;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Scenes
{
    public class GameScene: Scene
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

        public override void Initialize()
        {
            base.Initialize();

            _inputSettings = SettingsManager.LoadSettings();
            _tiledMap.Initialize("./../../../Content/Level1.tmx");
            _blocks = new List<IGameObject>();

            InitializeGameObjects();
        }

        public override void LoadContent()
        {
            _heroAtlas = TextureAtlas.FromFile(Core.Content, "hero-atlas.xml");

            _enemyTexture = Core.Content.Load<Texture2D>("enemy_bigguy");

            _tileset = Core.Content.Load<Texture2D>("tileset64");

            _tiledMap = new TiledMap();

            _tiledMap.LoadContent(Core.Content);

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
            foreach (IGameObject gameObject in _gameObjects)
            {
                gameObject.Update(_gameObjects, gameTime);
            }
            UpdateCamera();
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
            float marginY = (screenHeight*0.5f) / _cameraZoom;

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

            _gameObjects.OfType<Hero>().FirstOrDefault()?.Draw(Core.SpriteBatch);

            Core.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
