using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Settings;

namespace PirateAdventures.Input;

public class InputSettings
{
    public Dictionary<EGameAction, Keys> KeyBindings { get; set; }

    public InputSettings()
    {
        KeyBindings = new Dictionary<EGameAction, Keys>
        {
            { EGameAction.MoveLeft, Keys.Q },
            { EGameAction.MoveRight, Keys.D },
            { EGameAction.Jump, Keys.Space },
            { EGameAction.Attack, Keys.Z },
            { EGameAction.BirdUp, Keys.I },
            { EGameAction.BirdDown, Keys.K },
            { EGameAction.BirdLeft, Keys.J },
            { EGameAction.BirdRight, Keys.L }
        };
    }
}