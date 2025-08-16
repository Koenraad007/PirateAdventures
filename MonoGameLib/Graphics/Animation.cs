using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLib.Graphics
{
    public class Animation
    {
        public List<TextureRegion> Frames { get; set; }

        /// <summary>
        /// Delay between each frame.
        /// </summary>
        public TimeSpan Delay { get; set; }

        public Animation()
        {
            Frames = new List<TextureRegion>();
            Delay = TimeSpan.FromMilliseconds(100);
        }

        public Animation(List<TextureRegion> frames, TimeSpan delay)
        {
            Frames = frames;
            Delay = delay;
        }
    }
}
