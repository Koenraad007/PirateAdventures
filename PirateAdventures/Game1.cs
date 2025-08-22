using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLib;
using MonoGameLib.Graphics;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using PirateAdventures.Settings;
using PirateAdventures.Managers;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.IO;

namespace PirateAdventures;

public class Game1 : Core
{
    public Game1(): base("Pirate Adventures", 1200, 600, false) { }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new Startscreen());
        SoundManager.Instance.PlayBackgroundMusic("backgroundMusic", .1f, true);
    }

    protected override void LoadContent()
    {
        SoundManager.Instance.AddBackgroundMusic("backgroundMusic", Content.Load<Song>("Music/Background/backgroundMusic"));

        SoundManager.Instance.AddSoundEffect("jump", Core.Content.Load<SoundEffect>("Music/SoundFx/jumpSound"));
        SoundManager.Instance.AddSoundEffect("walk1", Core.Content.Load<SoundEffect>("Music/SoundFx/walkingGrass"));
        SoundManager.Instance.AddSoundEffect("walk2", Core.Content.Load<SoundEffect>("Music/SoundFx/walking"));
        SoundManager.Instance.AddSoundEffect("oof", Core.Content.Load<SoundEffect>("Music/SoundFx/oof"));
        SoundManager.Instance.AddSoundEffect("slice", Core.Content.Load<SoundEffect>("Music/SoundFx/slice"));
        SoundManager.Instance.AddSoundEffect("coin", Core.Content.Load<SoundEffect>("Music/SoundFx/coin"));
        SoundManager.Instance.AddSoundEffect("explosion", Core.Content.Load<SoundEffect>("Music/SoundFx/explosion"));
    }
}
