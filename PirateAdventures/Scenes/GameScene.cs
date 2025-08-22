using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using MonoGameLib.Graphics;
using MonoGameLib.Scenes;
using PirateAdventures.GameObjects;
using PirateAdventures.GameObjects.Enemies;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Settings;
using PirateAdventures.Managers;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.Scenes
{
    public class GameScene : Scene
    {
        private List<IGameObject> _gameObjects;
        private List<IGameObject> _blocks;
        private TiledMap _tiledMap;
        private InputSettings _inputSettings;
        private bool _isGameOver = false, _isLevelComplete = false;
        private SpriteFont font;
        public int Score { get; set; } = 0;
        private int skullCollected = 0;
        private Camera _camera;
        private GameObjectManager _gameObjectManager;
        private UIManager _uiManager;
        private GameObjectInputManager _inputManager;

        public GameScene()
        { }

        public override void Initialize()
        {
            base.Initialize();

            _inputSettings = SettingsManager.LoadSettings();
            _tiledMap.Initialize(LevelManager.Instance.GetCurrentLevel());
            _blocks = new List<IGameObject>();

            _camera = new Camera(Core.GraphicsDevice);

            InitializeGameObjects();
            _gameObjectManager = new GameObjectManager(_gameObjects);

            _uiManager = new UIManager(font);

            _inputManager = new GameObjectInputManager();
            var inputSettings = SettingsManager.LoadSettings();
            var hero = _gameObjects.OfType<Hero>().FirstOrDefault();
            var companion = _gameObjects.OfType<Companion>().FirstOrDefault();
            _inputManager.AddInputReader(hero, new HeroInputReader(inputSettings));
            _inputManager.AddInputReader(companion, new CompanionInputReader(inputSettings));
        }

        public override void LoadContent()
        {
            _tiledMap = new TiledMap();

            _tiledMap.LoadContent(Core.Content);

            font = Core.Content.Load<SpriteFont>("Fonts/Pixellari");
        }

        private void InitializeGameObjects()
        {
            _gameObjects = _tiledMap.CreateGameObjects();

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
            foreach (var enemy in _gameObjects.OfType<IEnemy>())
            {
                enemy.Attack += HandleAttack;
            }
            foreach (var collectable in _gameObjects.OfType<Collectable>())
            {
                collectable.OnPickup += HandleCollectablePickup;
            }
            foreach (var windowGuy in _gameObjects.OfType<WindowGuy>())
            {
                windowGuy.SpawnBomb += HandleSpawnBomb;
            }
            foreach (var shooter in _gameObjects.OfType<Shooter>())
            {
                shooter.Shoot += HandleShot;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update the camera
            Hero hero = _gameObjects.OfType<Hero>().FirstOrDefault();
            if (hero != null) _camera.Follow(hero, _tiledMap.Width, _tiledMap.Height);

            if (_isGameOver || _isLevelComplete)
            {
                SoundManager.Instance.StopAllSounds();
            }
            else
            {
                _gameObjectManager.Update(gameTime);
            }

            _uiManager.Update(_isGameOver, _isLevelComplete);

            _inputManager.Update(gameTime);

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

        private void HandleAttack(IEnemy enemy, int damage, Vector2 attackDirection)
        {
            Hero hero = _gameObjectManager.GetGameObjects<Hero>().FirstOrDefault();
            if (hero != null)
            {
                Debug.WriteLine($"Hero attacked by {enemy.GetType().Name} for {damage} damage.");
                hero.TakeDamage(damage, attackDirection);
            }
        }

        private void HandleCollectablePickup(Collectable collectable, CollectableType type)
        {
            switch (type)
            {
                case CollectableType.SilverCoin:
                    Score += 10;
                    break;
                case CollectableType.GoldCoin:
                    Score += 20;
                    break;

                case CollectableType.Skull:
                    Score += 50;
                    skullCollected++;
                    break;

                default:
                    Debug.WriteLine($"Unknown collectable type: {type}");
                    break;
            }
        }

        private void HandleSpawnBomb(WindowGuy windowGuy, Vector2 position)
        {
            Bomb bomb = new Bomb(position);
            _gameObjectManager.Add(bomb);
        }

        private void HandleShot(Shooter enemy, Vector2 position, Vector2 direction, float speed)
        {
            _gameObjectManager.Add(new Bullet(position, direction, speed));
        }

        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(new Color(146, 169, 206));

            var transform = _camera.GetViewMatrix();

            // ----- Game Objects --------------

            Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    transformMatrix: transform
                );

            _tiledMap.Draw(Core.SpriteBatch);

            _gameObjectManager.Draw(Core.SpriteBatch);

            Core.SpriteBatch.End();

            // ----- UI --------------

            Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp
                );

            Hero hero = _gameObjects.OfType<Hero>().FirstOrDefault();
            _uiManager.Draw(Core.SpriteBatch, hero, Score, _isGameOver, _isLevelComplete);

            Core.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
