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
using PirateAdventures.Scenes;

namespace PirateAdventures;

public class Game1 : Core
{
    public Game1() : base("Pirate Adventures", 1200, 600, false) { }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new Startscreen());
        SoundManager.Instance.PlayBackgroundMusic("backgroundMusic", .1f, true);
    }

    protected override void LoadContent()
    {
        LoadSounds();
        LoadTextures();
    }

    private void LoadSounds()
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

    private void LoadTextures()
    {
        TextureManager.Instance.AddTexture("start_bg", Core.Content.Load<Texture2D>("Menu/start_bg"));
        TextureManager.Instance.AddTexture("titlePirateText", Core.Content.Load<Texture2D>("Menu/PirateText"));
        TextureManager.Instance.AddTexture("titleAdventuresText", Core.Content.Load<Texture2D>("Menu/AdventuresText"));
        TextureManager.Instance.AddTexture("buttons", Core.Content.Load<Texture2D>("Menu/BrownButtons"));
        TextureManager.Instance.AddTexture("gameOver", Core.Content.Load<Texture2D>("Menu/GameOver"));
        TextureManager.Instance.AddTexture("levelComplete", Core.Content.Load<Texture2D>("Menu/LevelComplete"));
        TextureManager.Instance.AddTexture("heroHealth", Core.Content.Load<Texture2D>("Menu/HeroHealth"));
        TextureManager.Instance.AddTexture("enemyHealth", Core.Content.Load<Texture2D>("Menu/EnemyHealth"));
        TextureManager.Instance.AddTexture("score", Core.Content.Load<Texture2D>("Menu/Score"));
        TextureManager.Instance.AddTexture("bomb", Core.Content.Load<Texture2D>("Sprites/Bomb/Bomb"));
        TextureManager.Instance.AddTexture("bullet", Core.Content.Load<Texture2D>("Sprites/Bullet/bullet"));
        TextureManager.Instance.AddTexture("companion", Core.Content.Load<Texture2D>("Sprites/Companion/bluebird20fps"));
        TextureManager.Instance.AddTexture("tileset", Core.Content.Load<Texture2D>("Tileset32"));

        TextureManager.Instance.AddTextureAtlas("heroAtlas", TextureAtlas.FromFile(Core.Content, "hero-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("bigguyAtlas", TextureAtlas.FromFile(Core.Content, "bigguy-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("shooterAtlas", TextureAtlas.FromFile(Core.Content, "shooter-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("collectablesAtlas", TextureAtlas.FromFile(Core.Content, "collectables-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("bombAtlas", TextureAtlas.FromFile(Core.Content, "bomb-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("endpointAtlas", TextureAtlas.FromFile(Core.Content, "endpoint-atlas.xml"));
        TextureManager.Instance.AddTextureAtlas("windowguyAtlas", TextureAtlas.FromFile(Core.Content, "windowguy-atlas.xml"));
    }
}
