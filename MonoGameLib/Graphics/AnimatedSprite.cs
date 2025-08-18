using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLib.Graphics
{
    public class AnimatedSprite: Sprite
    {
        private int _currentFrame;
        private TimeSpan _elapsed;
        private Animation _animation;
        public bool PlayOnce = false;

        public Animation Animation
        {
            get => _animation;
            set
            {
                _animation = value;
                Region = _animation.Frames[0];
            }
        }

        public AnimatedSprite() { }

        public AnimatedSprite(Animation animation)
        {
            Animation = animation;
        }

        public void Update(GameTime gameTime)
        {
            if (PlayOnce && _currentFrame >= _animation.Frames.Count - 1)
            {
                return;
            }

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= _animation.Delay)
            {
                _elapsed -= _animation.Delay;
                _currentFrame++;

                if (_currentFrame >= _animation.Frames.Count)
                {
                    _currentFrame = 0;
                }

                Region = _animation.Frames[_currentFrame];
            }
        }
    }
}
