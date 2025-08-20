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
        public int CurrentFrame { get; private set; }
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
            if (PlayOnce && CurrentFrame >= _animation.Frames.Count - 1)
            {
                return;
            }

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= _animation.Delay)
            {
                _elapsed -= _animation.Delay;
                CurrentFrame++;

                if (CurrentFrame >= _animation.Frames.Count)
                {
                    CurrentFrame = 0;
                }

                Region = _animation.Frames[CurrentFrame];
            }
        }
    }
}
