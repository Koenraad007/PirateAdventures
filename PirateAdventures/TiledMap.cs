using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using TiledSharp;

public class TiledMap
{
    private TmxMap _map;
    private Texture2D _tilesetTexture;
    public List<IGameObject> CollisionObjects { get; private set; } = new List<IGameObject>();

    public void LoadContent(ContentManager contentManager, string filePath)
    {
        _map = new TmxMap(filePath);
        _tilesetTexture = contentManager.Load<Texture2D>("tileset64");

        CreateCollisionObjects();
    }

    private void CreateCollisionObjects()
    {
        var collisionLayer = _map.Layers.FirstOrDefault(l => l.Name.ToLower().Contains("collision"));

        if (collisionLayer != null)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    int tileId = collisionLayer.Tiles[x + y * _map.Width].Gid;

                    if (tileId > 0)
                    {
                        var srcRect = CalculateTileSourceRectangle(tileId);
                        var block = new Block(
                            new Vector2(x * _map.TileWidth, y * _map.TileHeight),
                            _tilesetTexture,
                            new Vector2(srcRect.X / 64, srcRect.Y / 64),
                            64,
                            false
                            );

                        CollisionObjects.Add(block);
                    }
                }
            }
        }
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