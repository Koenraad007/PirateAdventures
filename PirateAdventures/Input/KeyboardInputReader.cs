using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Input
{
    public class KeyboardInputReader : IInputReader
    {
        public Vector2 ReadInput()
        {
            var direction = Vector2.Zero;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
                direction = new Vector2(-1, 0);
            if (state.IsKeyDown(Keys.Right))
                direction = new Vector2(1, 0);

            return direction;
        }
    }
}