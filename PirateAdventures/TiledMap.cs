using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
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
        private Texture2D _heroTexture, _companionTexture, _tilesetTexture, _bigGuyTexture, _shooterTexture, _windowGuyTexture, _bombTexture;
        private TextureAtlas _heroAtlas;

        public List<IGameObject> CollisionObjects { get; private set; } = new List<IGameObject>();
        public int Width { get; private set; }
        public int Height { get; private set; }
        private float scale = 2f;
        public const int TileSize = 32;

        public void Initialize(string filePath)
        {
            _map = new TmxMap(filePath);
            Width = _map.Width * _map.TileWidth;
            Height = _map.Height * _map.TileHeight;
            CreateCollisionObjects();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _heroTexture = contentManager.Load<Texture2D>("Sprites/Hero/cptclownnose20fps");
            _companionTexture = contentManager.Load<Texture2D>("bluebird20fps");
            _heroAtlas = TextureAtlas.FromFile(contentManager, "hero-atlas.xml");
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
                                    new Vector2(srcRect.X / TileSize, srcRect.Y / TileSize),
                                    TileSize,
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
                                    new Vector2(srcRect.X / TileSize, srcRect.Y / TileSize),
                                    TileSize,
                                    BlockType.PLATFORM
                                    );

                                CollisionObjects.Add(block);
                            }
                        }
                    }
                }
            }
        }

        public List<IGameObject> CreateGameObjects(KeyboardInputReader kir)
        {
            var gameObjects = new List<IGameObject>();

            var gameObjectLayer = _map.ObjectGroups.FirstOrDefault(l => l.Name.ToLower().Contains("gameobjects"));

            if (gameObjectLayer != null)
            {
                foreach (var gameObject in gameObjectLayer.Objects)
                {
                    Console.WriteLine($"Enemy Name: {gameObject.Name}");

                    switch (gameObject.Name.ToLower())
                    {
                        case "hero":
                            var hero = new Hero(
                                    _heroTexture,
                                    kir,
                                    _heroAtlas
                                    )
                            {
                                Position = new Vector2((float)gameObject.X, (float)gameObject.Y - Hero.SPRITE_HEIGHT),
                            };
                            gameObjects.Add(hero);
                            break;

                        case "companion":
                            var companion = new Companion(
                                    kir,
                                    _companionTexture
                                    )
                            {
                                Position = new Vector2((float)gameObject.X, (float)gameObject.Y - Companion.SPRITE_HEIGHT),
                            };
                            gameObjects.Add(companion);
                            break;

                        case "big":
                            var bigGuy = new BigGuy(
                                    _bigGuyTexture
                                    )
                            {
                                Position = new Vector2((float)gameObject.X, (float)gameObject.Y - BigGuy.SPRITE_HEIGHT),
                            };
                            gameObjects.Add(bigGuy);
                            break;

                        case "shoot":
                            var shooter = new Shooter(
                                _shooterTexture
                            )
                            {
                                Position = new Vector2((float)gameObject.X, (float)gameObject.Y - Shooter.SPRITE_HEIGHT)
                            };
                            gameObjects.Add(shooter);
                            break;

                        case "window":
                            var windowGuy = new WindowGuy(_windowGuyTexture, _bombTexture)
                            {
                                Position = new Vector2((float)gameObject.X, (float)gameObject.Y - WindowGuy.SPRITE_HEIGHT)
                            };
                            gameObjects.Add(windowGuy);
                            break;

                        default:
                            break;
                    }
                }
            }

            return gameObjects;
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