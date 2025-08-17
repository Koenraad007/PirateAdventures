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
        private TextureAtlas _textureAtlas;
        private Hero _hero;
        private Companion _companion;
        private List<IGameObject> _enemies;
        private List<IGameObject> _blocks;
        private TiledMap _tiledMap;
        private Vector2 cameraOffset = Vector2.Zero;
        private const int CAMERA_MARGIN_X = 400, CAMERA_MARGIN_Y = 200;
        private InputSettings _inputSettings;
        private Texture2D _heroTexture, _tileset, _enemyTexture, _companionTexture;

        public override void Initialize()
        {
            base.Initialize();

            _inputSettings = SettingsManager.LoadSettings();
            _tiledMap.Initialize("./../../../Content/naamloos.tmx");
            _blocks = new List<IGameObject>();

            InitializeGameObjects();
        }

        public override void LoadContent()
        {
            _heroTexture = Core.Content.Load<Texture2D>("Sprites/Hero/cptclownnose20fps");
            _textureAtlas = TextureAtlas.FromFile(Core.Content, "Sprites/Hero/hero-atlas.xml");

            _companionTexture = Core.Content.Load<Texture2D>("bluebird20fps");
            _enemyTexture = Core.Content.Load<Texture2D>("enemy_bigguy");

            _tileset = Core.Content.Load<Texture2D>("tileset64");

            _tiledMap = new TiledMap();

            _tiledMap.LoadContent(Core.Content);

        }

        private void InitializeGameObjects()
        {
            KeyboardInputReader kir = new KeyboardInputReader(_inputSettings);

            _hero = _tiledMap.CreateHero(_heroTexture, kir, _textureAtlas);
            _companion = new Companion(kir, _companionTexture);
            //bigGuy = new BigGuy(_enemyTexture);
            _enemies = _tiledMap.CreateEnemyObjects();

            _blocks = _tiledMap.CollisionObjects;
        }

        public override void Update(GameTime gameTime)
        {
            _hero.Update(_blocks, gameTime);
            _companion.Update(new List<IGameObject>() { _hero }, gameTime);
            foreach (IGameObject enemy in _enemies)
            {
                var gameObjects = new List<IGameObject>() { _hero };
                gameObjects.AddRange(_blocks);
                enemy.Update(gameObjects, gameTime);
            }
            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            int screenWidth = Core.GraphicsDevice.Viewport.Width;
            int screenHeight = Core.GraphicsDevice.Viewport.Height;

            // calculate where the hero should be displayed on the screen
            float heroDisplayX = _hero.Position.X - cameraOffset.X;
            float heroDisplayY = _hero.Position.Y - cameraOffset.Y;

            // horizontal scrolling
            if (heroDisplayX < CAMERA_MARGIN_X) cameraOffset.X = _hero.Position.X - CAMERA_MARGIN_X;
            else if (heroDisplayX > screenWidth - CAMERA_MARGIN_X) cameraOffset.X = _hero.Position.X - (screenWidth - CAMERA_MARGIN_X);

            // vertical scrolling
            if (heroDisplayY < CAMERA_MARGIN_Y) cameraOffset.Y = _hero.Position.Y - CAMERA_MARGIN_Y;
            else if (heroDisplayY > screenHeight - CAMERA_MARGIN_Y) cameraOffset.Y = _hero.Position.Y - (screenHeight - CAMERA_MARGIN_Y);

            cameraOffset.X = MathHelper.Clamp(cameraOffset.X, 0, _tiledMap.Width - screenWidth);
            cameraOffset.Y = MathHelper.Clamp(cameraOffset.Y, 0, _tiledMap.Height - screenHeight);
        }

        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(new Color(146, 169, 206));

            Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    transformMatrix: Matrix.CreateTranslation(-cameraOffset.X, -cameraOffset.Y, 0)
                );

            _tiledMap.Draw(Core.SpriteBatch);

            foreach (IGameObject enemy in _enemies)
            {
                enemy.Draw(Core.SpriteBatch);
            }

            _companion.Draw(Core.SpriteBatch);
            _hero.Draw(Core.SpriteBatch);

            Core.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
