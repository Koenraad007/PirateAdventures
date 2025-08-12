using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Interfaces;

namespace PirateAdventures.Input
{
    public class KeyboardInputReader : IInputReader
    {
        private readonly InputSettings inputSettings;

        public KeyboardInputReader(InputSettings inputSettings)
        {
            this.inputSettings = inputSettings;
        }

        public Vector2 ReadInput()
        {
            var direction = Vector2.Zero;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(inputSettings.MoveLeftKey))
                direction.X = -1;
            if (state.IsKeyDown(inputSettings.MoveRightKey))
                direction.X = 1;
            if (state.IsKeyDown(inputSettings.JumpKey))
                direction.Y = -1;

            return direction;
        }

        public Vector2 ReadBirdInput()
        {
            var direction = Vector2.Zero;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(inputSettings.BirdLeftKey))
                direction.X = -1;
            if (state.IsKeyDown(inputSettings.BirdRightKey))
                direction.X = 1;
            if (state.IsKeyDown(inputSettings.BirdUpKey))
                direction.Y = -1;
            if (state.IsKeyDown(inputSettings.BirdDownKey))
                direction.Y = 1;

            return direction;
        }
    }
}