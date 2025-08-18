using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledSharp;

namespace PirateAdventures
{
    public class TiledMap
    {
        private TmxMap _map;
        private Texture2D _tilesetTexture, _bigGuyTexture, _shooterTexture, _windowGuyTexture, _bombTexture, _heroTexture;

        public List<IGameObject> CollisionObjects { get; private set; } = new List<IGameObject>();
        public int Width { get; private set; }
        public int Height { get; private set; }
        private float scale = 2f;

        public void Initialize(string filePath)
        {
            _map = new TmxMap(filePath);
            Width = _map.Width * _map.TileWidth;
            Height = _map.Height * _map.TileHeight;
            CreateCollisionObjects();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _tilesetTexture = contentManager.Load<Texture2D>("Tileset32");
            _bigGuyTexture = contentManager.Load<Texture2D>("enemy_bigguy");
            _shooterTexture = contentManager.Load<Texture2D>("enemy_shooter");
            _windowGuyTexture = contentManager.Load<Texture2D>("enemy_windowguy");
            _bombTexture = contentManager.Load<Texture2D>("Bomb");

        }

        private void CreateCollisionObjects()
        {
            var collisionLayer = _map.Layers.FirstOrDefault(l => l.Name.ToLower().Contains("collision"));
            var firstGid = 19;

            if (collisionLayer != null)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    for (int y = 0; y < _map.Height; y++)
                    {
                        int tileId = collisionLayer.Tiles[x + y * _map.Width].Gid;

                        if (tileId > 0)
                        {
                            // full block
                            if (tileId == firstGid)
                            {
                                var srcRect = CalculateTileSourceRectangle(tileId);
                                var block = new Block(
                                    new Vector2(x * _map.TileWidth, y * _map.TileHeight),
                                    _tilesetTexture,
                                    new Vector2(srcRect.X / 64, srcRect.Y / 64),
                                    64,
                                    BlockType.FULL
                                    );

                                CollisionObjects.Add(block);
                            }

                            // platform, only collision from top
                            if (tileId == firstGid + 1)
                            {
                                var srcRect = CalculateTileSourceRectangle(tileId);
                                var block = new Block(
                                    new Vector2(x * _map.TileWidth, y * _map.TileHeight),
                                    _tilesetTexture,
                                    new Vector2(srcRect.X / 64, srcRect.Y / 64),
                                    64,
                                    BlockType.PLATFORM
                                    );

                                CollisionObjects.Add(block);
                            }
                        }
                    }
                }
            }
        }

        public List<IGameObject> CreateEnemyObjects()
        {
            var enemyObjects = new List<IGameObject>();

            var enemyLayer = _map.ObjectGroups.FirstOrDefault(l => l.Name.ToLower().Contains("gameobjects"));

            if (enemyLayer != null)
            {
                foreach (var enemy in enemyLayer.Objects)
                {
                    Console.WriteLine($"Enemy Name: {enemy.Name}");

                    switch (enemy.Name.ToLower())
                    {
                        case "big":
                            var bigGuy = new BigGuy(
                                    _bigGuyTexture
                                    )
                            {
                                Position = new Vector2((float)enemy.X, (float)enemy.Y - BigGuy.SPRITE_HEIGHT),
                            };
                            enemyObjects.Add(bigGuy);
                            break;

                        case "shoot":
                            var shooter = new Shooter(
                                _shooterTexture
                            )
                            {
                                Position = new Vector2((float)enemy.X, (float)enemy.Y - Shooter.SPRITE_HEIGHT)
                            };
                            enemyObjects.Add(shooter);
                            break;

                        case "window":
                            var windowGuy = new WindowGuy(_windowGuyTexture, _bombTexture)
                            {
                                Position = new Vector2((float)enemy.X, (float)enemy.Y - WindowGuy.SPRITE_HEIGHT)
                            };
                            enemyObjects.Add(windowGuy);
                            break;

                        default:
                            break;
                    }
                }
            }

            return enemyObjects;
        }

        public Hero CreateHero(Texture2D texture, KeyboardInputReader kir, TextureAtlas ta)
        {
            var gameObjectsLayer = _map.ObjectGroups.FirstOrDefault(l => l.Name.ToLower().Contains("gameobjects"));

            if (gameObjectsLayer != null)
            {
                var heroObject = gameObjectsLayer.Objects.FirstOrDefault(o => o.Name.ToLower().Contains("hero"));
                if (heroObject != null)
                {
                    return new Hero(
                        texture,
                        kir,
                        ta
                        )
                    {
                        Position = new Vector2((float)heroObject.X, (float)heroObject.Y - Hero.SPRITE_HEIGHT),
                    };
                }
            }

            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in _map.Layers)
            {
                if (layer.Name.ToLower().Contains("collision")) continue;

                for (int x = 0; x < _map.Width; x++)
                {
                    for (int y = 0; y < _map.Height; y++)
                    {
                        int tileId = layer.Tiles[x + y * _map.Width].Gid;
                        if (tileId > 0)
                        {
                            Rectangle sourceRect = CalculateTileSourceRectangle(tileId);

                            spriteBatch.Draw(
                                _tilesetTexture,
                                new Vector2(x * _map.TileWidth, y * _map.TileHeight),
                                sourceRect,
                                Color.White
                            );
                        }
                    }
                }
            }

        }

        private Rectangle CalculateTileSourceRectangle(int tileId)
        {
            int tilesPerRow = _tilesetTexture.Width / _map.TileWidth;
            int tileX = (tileId - 1) % tilesPerRow;
            int tileY = (tileId - 1) / tilesPerRow;

            return new Rectangle(
                tileX * _map.TileWidth,
                tileY * _map.TileHeight,
                _map.TileWidth,
                _map.TileHeight
            );
        }
    }
}