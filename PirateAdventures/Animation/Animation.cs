using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateAdventures.Animation
{
    public class Animation
    {
        public AnimationFrame CurrentFrame { get; set; }
        private List<AnimationFrame> frames;
        private int ctr;

        public Animation()
        {
            frames = new List<AnimationFrame>();
        }

        public void AddFrame(AnimationFrame frame)
        {
            frames.Add(frame);
            CurrentFrame = frames[0];
        }

        public void Update()
        {
            CurrentFrame = frames[ctr];
            ctr++;

            if (ctr >= frames.Count) ctr = 0;
        }
    }
}