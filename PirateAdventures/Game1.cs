using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Level;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using PirateAdventures.Settings;
using MonoGameLib;
using MonoGameLib.Graphics;
using Microsoft.Xna.Framework.Media;

namespace PirateAdventures;

public class Game1 : Core
{
    Song backgroundMusic;

    public Game1(): base("Pirate Adventures", 1200, 600, false) { }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new Startscreen());
    }

    protected override void LoadContent()
    {
        backgroundMusic = Content.Load<Song>("Music/Background/backgroundMusic");
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }
        MediaPlayer.Volume = SettingsManager.Volume;
        MediaPlayer.Play(backgroundMusic);
        MediaPlayer.IsRepeating = true;
    }
}
