using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Managers
{
    public class SoundManager // Singleton
    {
        private static SoundManager instance;
        private SoundManager(Dictionary<string, SoundEffect> sfx, Dictionary<string, Song> songs)
        {
            soundEffects = sfx ?? new Dictionary<string, SoundEffect>();
            backgroundMusic = songs ?? new Dictionary<string, Song>();
        }

        public static SoundManager Instance
        {
            get
            {
                instance ??= new SoundManager(new Dictionary<string, SoundEffect>(), new Dictionary<string, Song>());
                return instance;
            }
        }

        private readonly Dictionary<string, SoundEffect> soundEffects;
        private readonly Dictionary<string, Song> backgroundMusic;
        private readonly Dictionary<string, SoundEffectInstance> loopedSfxInstances = new Dictionary<string, SoundEffectInstance>();

        public void AddSoundEffect(string soundName, SoundEffect soundEffect)
        {
            if (!soundEffects.ContainsKey(soundName))
            {
                soundEffects[soundName] = soundEffect;
            }
        }

        public void AddBackgroundMusic(string musicName, Song song)
        {
            if (!backgroundMusic.ContainsKey(musicName))
            {
                backgroundMusic[musicName] = song;
            }
        }

        public void PlaySound(string soundName, float volume, bool looped = false)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                if (!looped)
                {
                    var sfxInstance = soundEffects[soundName].CreateInstance();
                    sfxInstance.IsLooped = false;
                    sfxInstance.Volume = volume;
                    sfxInstance.Play();
                }
                else
                {
                    if (!loopedSfxInstances.ContainsKey(soundName))
                    {
                        var sfxInstance = soundEffects[soundName].CreateInstance();
                        sfxInstance.IsLooped = true;
                        sfxInstance.Volume = volume;
                        sfxInstance.Play();
                        loopedSfxInstances[soundName] = sfxInstance;
                    }
                }
            }
        }

        public void StopSound(string soundName)
        {
            if (soundEffects.ContainsKey(soundName) && loopedSfxInstances.ContainsKey(soundName))
            {
                loopedSfxInstances[soundName].Stop();
                loopedSfxInstances.Remove(soundName);
            }
        }

        public void StopAllSounds()
        {
            foreach (var sfx in loopedSfxInstances.Values)
            {
                sfx.Stop();
            }
            loopedSfxInstances.Clear();
        }

        public void PlayBackgroundMusic(string musicName, float volume, bool looped = true)
        {
            if (backgroundMusic.ContainsKey(musicName))
            {
                MediaPlayer.IsRepeating = looped;
                MediaPlayer.Volume = volume;
                MediaPlayer.Play(backgroundMusic[musicName]);
            }
        }

        public static void StopBackgroundMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }
    }
}
