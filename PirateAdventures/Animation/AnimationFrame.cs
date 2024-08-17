using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PirateAdventures.Animation
{
    public class AnimationFrame
    {
        public Rectangle SourceRect { get; set; }

        public AnimationFrame(Rectangle rect)
        {
            SourceRect = rect;
        }
    }
}