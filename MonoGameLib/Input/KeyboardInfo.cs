using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLib.Input
{
    public class KeyboardInfo
    {
        // 2 states to check the difference in input between frames
        public KeyboardState PreviousState { get; private set; }
        public KeyboardState CurrentState { get; private set; }

        public KeyboardInfo()
        {
            PreviousState = Keyboard.GetState();
            CurrentState = PreviousState;
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        public bool WasKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        public bool WasKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }
    }
}
