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
        private TextureAtlas _heroAtlas, _bombAtlas;
        private List<IGameObject> _gameObjects;
        private List<IGameObject> _blocks;
        private TiledMap _tiledMap;
        private Vector2 cameraOffset = Vector2.Zero;
        private const int CAMERA_MARGIN_X = 600, CAMERA_MARGIN_Y = 200;
        private InputSettings _inputSettings;
        private Texture2D _tileset, _enemyTexture;
        private float _cameraZoom = 2f;
        private Texture2D _gameOverTexture, _buttonsTexture, _levelCompleteTexture, _healthBarTexture, _scoreTexture, _bombTexture, _bulletTexture, _enemyHealth;
        private Rectangle _playAgainSrcRect;
        private bool _isGameOver = false, _isLevelComplete = false;
        private SpriteFont font;
        private Color _playAgainColor = Color.White;
        private string _levelPath;
        private List<IGameObject> toAdd = new List<IGameObject>();
        public int Score { get; set; } = 0;
        private Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private int skullCollected = 0;

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
            _bombAtlas = TextureAtlas.FromFile(Core.Content, "bomb-atlas.xml");

            _enemyTexture = Core.Content.Load<Texture2D>("enemy_bigguy");

            _tileset = Core.Content.Load<Texture2D>("tileset64");

            _tiledMap = new TiledMap();

            _tiledMap.LoadContent(Core.Content);

            _gameOverTexture = Core.Content.Load<Texture2D>("Menu/GameOver");
            _levelCompleteTexture = Core.Content.Load<Texture2D>("Menu/LevelComplete");
            _buttonsTexture = Core.Content.Load<Texture2D>("Menu/BrownButtons");
            _healthBarTexture = Core.Content.Load<Texture2D>("Menu/HeroHealth");
            _scoreTexture = Core.Content.Load<Texture2D>("Menu/Score");

            _bombTexture = Core.Content.Load<Texture2D>("Sprites/Bomb/Bomb");
            _bulletTexture = Core.Content.Load<Texture2D>("Sprites/Bullet/bullet");
            _enemyHealth = Core.Content.Load<Texture2D>("Menu/EnemyHealth");

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
            UpdateCamera();

            if (_isGameOver || _isLevelComplete)
            {
                SoundManager.Instance.StopAllSounds();

                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;
                Vector2 centerScreen = new Vector2(screenWidth / 2f, screenHeight / 2f);

                Texture2D texture = _isGameOver ? _gameOverTexture : _levelCompleteTexture;

                float scale = Math.Min(screenWidth / (texture.Width * 2f),
                               screenHeight / (texture.Height * 2f));

                Vector2 playAgainPos = centerScreen + new Vector2(0, texture.Height * scale / 2f + 14 * scale);
                Rectangle playAgainBounds = new Rectangle((int)(playAgainPos.X - _playAgainSrcRect.Width * scale / 2), (int)(playAgainPos.Y - _playAgainSrcRect.Height * scale / 2), (int)(_playAgainSrcRect.Width * scale), (int)(_playAgainSrcRect.Height * scale));

                if (playAgainBounds.Contains(Core.Input.Mouse.Position))
                {
                    _playAgainColor = Color.Yellow;
                    if (Core.Input.Mouse.WasButtonDown(MonoGameLib.Input.MouseButton.Left))
                    {
                        Core.ChangeScene(new GameScene(_levelPath));
                    }
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

                if (toAdd.Count > 0)
                {
                    foreach (var enemy in toAdd.OfType<IEnemy>())
                    {
                        enemy.Attack += HandleAttack;
                    }
                    _gameObjects.AddRange(toAdd);
                    toAdd.Clear();
                }

                _gameObjects.RemoveAll(obj => obj is Collectable collectable && collectable.IsCollected);
                _gameObjects.RemoveAll(obj => obj is Bomb bomb && bomb.HasExploded);
                _gameObjects.RemoveAll(obj => obj is Bullet bullet && bullet.IsHit);
                _gameObjects.RemoveAll(obj => obj is IKillable killable && killable.Health <= 0 && !(obj is Hero));
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

        private void HandleAttack(IEnemy enemy, int damage, Vector2 attackDirection)
        {
            Hero hero = _gameObjects.OfType<Hero>().FirstOrDefault();
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
                    skullCollected++;
                    break;

                default:
                    Debug.WriteLine($"Unknown collectable type: {type}");
                    break;
            }
        }

        private void HandleSpawnBomb(WindowGuy windowGuy, Vector2 position)
        {
            Bomb bomb = new Bomb(_bombTexture, position, _bombAtlas);
            toAdd.Add(bomb);
        }

        private void HandleShot(Shooter enemy, Vector2 position, Vector2 direction, float speed)
        {
            toAdd.Add(new Bullet(_bulletTexture, position, direction, speed));
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
                if (gameObject is IKillable killable && gameObject is ICollidable collidable)
                {
                    var healthBarPos = new Vector2(
                        collidable.BoundingBox.Center.X - _enemyHealth.Width/2, 
                        collidable.BoundingBox.Y - 16
                        );
                    Core.SpriteBatch.Draw(
                        _enemyHealth,
                        healthBarPos,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                    var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.Red });
                    Core.SpriteBatch.Draw(pixel, healthBarPos + new Vector2(3, 3), null, Color.White, 0f, Vector2.Zero, new Vector2((_enemyHealth.Width - 6) * killable.Health / 100, 1), SpriteEffects.None, 0);
                }
            }

            _gameObjects.OfType<Hero>().FirstOrDefault()?.Draw(Core.SpriteBatch); // Draw hero last to ensure it is on top of other objects

            Core.SpriteBatch.End();

            // ----- UI --------------

            Core.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp
                );

            // draw health bar
            Hero hero = _gameObjects.OfType<Hero>().FirstOrDefault();
            if (hero != null)
            {
                int healthBarWidth = (int)(hero.Health * 2.14f);
                Vector2 healthBarPosition = new Vector2(16, 16);
                var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.Red });

                Core.SpriteBatch.Draw(
                    _healthBarTexture,
                    healthBarPosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    2f,
                    SpriteEffects.None,
                    0f
                );

                Core.SpriteBatch.Draw(
                    pixel,
                    healthBarPosition + new Vector2(17*2, 14*2),
                    new Rectangle(0, 0, healthBarWidth, 4),
                    Color.White
                );
            }

            // draw score
            Vector2 scorePos = new Vector2(Core.GraphicsDevice.Viewport.Width - (_scoreTexture.Width * 2) - 16, 16);
            Core.SpriteBatch.Draw(
                _scoreTexture,
                new Vector2(Core.GraphicsDevice.Viewport.Width-(_scoreTexture.Width*2)-16, 16),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f
            );
            Core.SpriteBatch.DrawString(
                font,
                Score.ToString("D7"),
                scorePos + new Vector2(_scoreTexture.Width-10, 10),
                new Color(51, 50, 61),
                0f,
                Vector2.Zero,
                1.6f,
                SpriteEffects.None,
                0f
            );

            if (_isGameOver || _isLevelComplete)
            {


                int screenWidth = Core.GraphicsDevice.Viewport.Width;
                int screenHeight = Core.GraphicsDevice.Viewport.Height;

                Texture2D texture = _isGameOver ? _gameOverTexture : _levelCompleteTexture;

                float scale = Math.Min(screenWidth / (texture.Width * 2f),
                               screenHeight / (texture.Height * 2f));

                Vector2 centerScreen = new Vector2(screenWidth / 2f, screenHeight / 2f);

                // darken the background
                var pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Core.SpriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 128));

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
                Core.SpriteBatch.DrawString(
                    font,
                    "Score: " + Score.ToString("D7"),
                    centerScreen + new Vector2(0, 6f * scale),
                    Color.LightYellow,
                    0f,
                    font.MeasureString("Score: 0000000") / 2f,
                    scale / 2,
                    SpriteEffects.None,
                    0f
                );

                Vector2 playAgainPos = centerScreen + new Vector2(0, texture.Height * scale / 2f + 14 * scale);
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


            }

            Core.SpriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
