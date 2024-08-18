using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PirateAdventures.Animations
{
    public class Animation
    {
        public AnimationFrame CurrentFrame { get; set; }
        private List<AnimationFrame> frames;
        private int ctr;
        private double secondCtr = 0;

        public Animation()
        {
            frames = new List<AnimationFrame>();
        }

        public void AddFrame(AnimationFrame frame)
        {
            frames.Add(frame);
            CurrentFrame = frames[0];
        }

        public void ResetAnimation()
        {
            CurrentFrame = frames[0];
        }

        public void Update(GameTime gameTime)
        {
            CurrentFrame = frames[ctr];

            secondCtr += gameTime.ElapsedGameTime.TotalSeconds;
            int fps = 20;

            if (secondCtr >= 1d / fps)
            {
                ctr++;
                secondCtr = 0;
            }



            if (ctr >= frames.Count) ctr = 0;
        }
    }
}