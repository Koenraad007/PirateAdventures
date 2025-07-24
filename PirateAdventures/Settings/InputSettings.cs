using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Input;

namespace PirateAdventures.Input;

public class InputSettings
{
    public Keys MoveLeftKey { get; set; } = Keys.Q;
    public Keys MoveRightKey { get; set; } = Keys.D;
    public Keys JumpKey { get; set; } = Keys.Space;
}