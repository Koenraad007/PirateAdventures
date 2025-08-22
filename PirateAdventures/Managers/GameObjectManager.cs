using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.GameObjects;
using PirateAdventures.GameObjects.Enemies;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PirateAdventures.Managers
{
    public class GameObjectManager
    {
        private readonly List<IGameObject> _gameObjects;
        private readonly List<IGameObject> _toAdd = new List<IGameObject>();

        public IEnumerable<IGameObject> GameObjects => _gameObjects;

        public GameObjectManager(List<IGameObject> gameObjects)
        {
            _gameObjects = gameObjects;
        }

        public void Add(IGameObject gameObject)
        {
            if (gameObject is IEnemy enemy)
            {
                enemy.Attack += HandleAttack;
            }
            _toAdd.Add(gameObject);
        }

        public void AddRange(IEnumerable<IGameObject> gameObjects)
        {
            foreach (var enemy in gameObjects.OfType<IEnemy>())
            {
                enemy.Attack += HandleAttack;
            }
            _toAdd.AddRange(gameObjects);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.Update(_gameObjects, gameTime);
            }

            if (_toAdd.Count > 0)
            {
                _gameObjects.AddRange(_toAdd);
                _toAdd.Clear();
            }

            _gameObjects.RemoveAll(obj => obj is Collectable collectable && collectable.IsCollected);
            _gameObjects.RemoveAll(obj => obj is Bomb bomb && bomb.HasExploded);
            _gameObjects.RemoveAll(obj => obj is Bullet bullet && bullet.IsHit);
            _gameObjects.RemoveAll(obj => obj is IKillable killable && killable.isDead && !(obj is Hero));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IGameObject gameObject in _gameObjects)
            {
                if (gameObject.GetType() == typeof(Block) || gameObject.GetType() == typeof(Hero)) continue;
                gameObject.Draw(spriteBatch);
                if (gameObject is IKillable killable && gameObject is ICollidable collidable)
                {
                    var healthBar = TextureManager.Instance.GetTexture("enemyHealth");
                    var healthBarPos = new Vector2(
                        collidable.BoundingBox.Center.X - healthBar.Width / 2,
                        collidable.BoundingBox.Y - 16
                        );
                    spriteBatch.Draw(
                        healthBar,
                        healthBarPos,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                    var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.Red });
                    spriteBatch.Draw(pixel, healthBarPos + new Vector2(3, 3), null, Color.White, 0f, Vector2.Zero, new Vector2((healthBar.Width - 6) * killable.Health / 100, 1), SpriteEffects.None, 0);
                }
            }

            _gameObjects.OfType<Hero>().FirstOrDefault()?.Draw(spriteBatch); // Draw hero last
        }

        public IEnumerable<T> GetGameObjects<T>() where T : IGameObject
        {
            return _gameObjects.OfType<T>();
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
    }
}