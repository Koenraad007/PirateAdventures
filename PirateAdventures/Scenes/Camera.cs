using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib;
using PirateAdventures.GameObjects;

namespace PirateAdventures
{
    public class Camera
    {
        private Vector2 _position;
        private readonly float _zoom;
        private readonly GraphicsDevice _graphicsDevice;

        public Camera(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _zoom = 2f;
            _position = Vector2.Zero;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(-_position.X, -_position.Y, 0) *
                   Matrix.CreateScale(_zoom, _zoom, 1);
        }

        public void Follow(Hero target, int mapWidth, int mapHeight)
        {
            if (target == null) return;

            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;

            float viewWidth = screenWidth / _zoom;
            float viewHeight = screenHeight / _zoom;
            float marginX = screenWidth * 0.5f / _zoom;
            float marginY = screenHeight * 0.5f / _zoom;

            // calculate where the hero should be displayed on the screen
            float heroDisplayX = target.Position.X - _position.X;
            float heroDisplayY = target.Position.Y - _position.Y;

            // horizontal scrolling
            if (heroDisplayX < marginX) _position.X = target.Position.X - marginX;
            else if (heroDisplayX > viewWidth - marginX) _position.X = target.Position.X - (viewWidth - marginX);

            // vertical scrolling
            if (heroDisplayY < marginY) _position.Y = target.Position.Y - marginY + 32f;
            else if (heroDisplayY > viewHeight - marginY) _position.Y = target.Position.Y - (viewHeight - marginY) + 32f;

            _position.X = MathHelper.Clamp(_position.X, 0, mapWidth - viewWidth);
            _position.Y = MathHelper.Clamp(_position.Y, 0, mapHeight - viewHeight);
        }
    }
}