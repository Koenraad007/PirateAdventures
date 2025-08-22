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
                if (instance == null)
                {
                    instance = new SoundManager(new Dictionary<string, SoundEffect>(), new Dictionary<string, Song>());
                }
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
            else
            {
                Debug.WriteLine($"Sound '{soundName}' already exists in the sound manager.");
            }
        }

        public void AddBackgroundMusic(string musicName, Song song)
        {
            if (!backgroundMusic.ContainsKey(musicName))
            {
                backgroundMusic[musicName] = song;
            }
            else
            {
                Debug.WriteLine($"Background music '{musicName}' already exists in the sound manager.");
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
                    else
                    {
                        Debug.WriteLine($"Sound '{soundName}' is already playing in loop.");
                    }
                }
            }
            else
            {
                Debug.WriteLine($"Sound '{soundName}' not found in the sound manager.");
            }
        }

        public void StopSound(string soundName)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                if (loopedSfxInstances.ContainsKey(soundName))
                {
                    loopedSfxInstances[soundName].Stop();
                    loopedSfxInstances.Remove(soundName);
                }
                else
                {
                    Debug.WriteLine($"Sound '{soundName}' is not playing in loop.");
                }
            }
            else
            {
                Debug.WriteLine($"Sound '{soundName}' not found in the sound manager.");
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
            else
            {
                Debug.WriteLine($"Background music '{musicName}' not found in the sound manager.");
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
