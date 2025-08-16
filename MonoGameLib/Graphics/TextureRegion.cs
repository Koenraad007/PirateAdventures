using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLib.Graphics
{
    public class TextureRegion
    {
        public Texture2D Texture { get; set; }

        public Rectangle SourceRect { get; set; }

        public int Width => SourceRect.Width;
        public int Height => SourceRect.Height;

        public TextureRegion() { }

        public TextureRegion(Texture2D texture, Rectangle sourceRect)
        {
            Texture = texture;
            SourceRect = sourceRect;
        }

        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        {
            Texture = texture;
            SourceRect = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Draw(
                spriteBatch,
                position,
                color,
                rotation,
                origin,
                new Vector2(scale, scale),
                effects,
                layerDepth
            );
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(
                Texture,
                position,
                SourceRect,
                color,
                rotation,
                origin,
                scale,
                effects,
                layerDepth
            );
        }
    }
}
