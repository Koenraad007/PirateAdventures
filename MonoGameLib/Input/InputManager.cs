using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLib.Input
{
    public class InputManager
    {
        public KeyboardInfo Keyboard { get; private set; }
        public MouseInfo Mouse { get; private set; }

        public InputManager(KeyboardInfo keyboardInfo, MouseInfo mouseInfo)
        {
            Keyboard = keyboardInfo;
            Mouse = mouseInfo;
        }

        public void Update()
        {
            Keyboard.Update();
            Mouse.Update();
        }
    }
}
