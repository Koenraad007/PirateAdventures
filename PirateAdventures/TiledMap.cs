using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Factories;
using PirateAdventures.GameObjects;
using PirateAdventures.GameObjects.Enemies;
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
        private Texture2D _companionTexture, _tilesetTexture, _windowGuyTexture;
        private TextureAtlas _heroAtlas, _endpointAtlas, _bigguyAtlas, _collectableAtlas, _windowguyAtlas, _shooterAtlas;

        public List<IGameObject> CollisionObjects { get; private set; } = new List<IGameObject>();
        public int Width { get; private set; }
        public int Height { get; private set; }
        private float scale = 2f;
        public const int TileSize = 32;

        private GameObjectFactory _factory;

        public void Initialize(string filePath)
        {
            _map = new TmxMap(filePath);
            Width = _map.Width * _map.TileWidth;
            Height = _map.Height * _map.TileHeight;
            CreateCollisionObjects();

            _factory = new GameObjectFactory();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _companionTexture = contentManager.Load<Texture2D>("Sprites/Companion/bluebird20fps");
            _heroAtlas = TextureAtlas.FromFile(contentManager, "hero-atlas.xml");
            _bigguyAtlas = TextureAtlas.FromFile(contentManager, "bigguy-atlas.xml");
            _shooterAtlas = TextureAtlas.FromFile(contentManager, "shooter-atlas.xml");
            _collectableAtlas = TextureAtlas.FromFile(contentManager, "collectables-atlas.xml");
            _tilesetTexture = contentManager.Load<Texture2D>("Tileset32");
            _windowGuyTexture = contentManager.Load<Texture2D>("Sprites/Windowguy/WindowGuy");
            _endpointAtlas = TextureAtlas.FromFile(contentManager, "endpoint-atlas.xml");
            _windowguyAtlas = TextureAtlas.FromFile(contentManager, "windowguy-atlas.xml");
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

                            if (tileId == firstGid + 2)
                            {
                                var srcRect = CalculateTileSourceRectangle(tileId);
                                var block = new Block(
                                    new Vector2(x * _map.TileWidth, y * _map.TileHeight),
                                    _tilesetTexture,
                                    new Vector2(srcRect.X / TileSize, srcRect.Y / TileSize),
                                    TileSize,
                                    BlockType.DEATH
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
                    Console.WriteLine($"Creating object: {gameObject.Name}");

                    var pos = new Vector2((float)gameObject.X, (float)gameObject.Y);
                    var createdObj = _factory.CreateGameObject(gameObject.Name, pos);

                    if (createdObj != null) gameObjects.Add(createdObj);
                }
            }

            return gameObjects;
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