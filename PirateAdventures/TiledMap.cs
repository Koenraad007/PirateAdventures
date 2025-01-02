using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

public class TiledMap
{
    private TmxMap _map;
    private Texture2D _tilesetTexture;

    public void LoadContent(ContentManager contentManager, string filePath)
    {
        _map = new TmxMap(filePath);
        _tilesetTexture = contentManager.Load<Texture2D>("tileset64");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var layer in _map.Layers)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    int tileId = layer.Tiles[x + y * _map.Width].Gid;
                    if (tileId > 0)
                    {
                        // Calculate source rectangle from tileset
                        Rectangle sourceRect = CalculateTileSourceRectangle(tileId);

                        // Draw tile
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
        // Logic to convert tile ID to source rectangle
        // This depends on your tileset's layout
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