using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib;
using PirateAdventures.Interfaces;
using PirateAdventures.Settings;

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

            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.MoveLeft]))
                direction.X = -1;
            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.MoveRight]))
                direction.X = 1;
            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.Jump]))
                direction.Y = -1;

            return direction;
        }

        public Vector2 ReadBirdInput()
        {
            var direction = Vector2.Zero;

            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.BirdLeft]))
                direction.X = -1;
            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.BirdRight]))
                direction.X = 1;
            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.BirdUp]))
                direction.Y = -1;
            if (Core.Input.Keyboard.IsKeyDown(inputSettings.KeyBindings[EGameAction.BirdDown]))
                direction.Y = 1;

            return direction;
        }
    }
}