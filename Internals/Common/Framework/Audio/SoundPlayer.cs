using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TanksRebirth.GameContent.ModSupport;
using TanksRebirth.GameContent.UI;
using TanksRebirth.Internals.Common.Utilities;

namespace TanksRebirth.Internals.Common.Framework.Audio
{
    public static class SoundPlayer
    {

        public static Dictionary<string, OggAudio> SavedSounds = new();
        public static TimeSpan GetLengthOfSound(string filePath)
        {
            byte[] soundData = File.ReadAllBytes(filePath);
            return SoundEffect.GetSampleDuration(soundData.Length, 44100, AudioChannels.Stereo);
        }
        private static float MusicVolume => TankGame.Settings.MusicVolume;
        private static float EffectsVolume => TankGame.Settings.EffectsVolume;
        private static float AmbientVolume => TankGame.Settings.AmbientVolume;
        public static OggAudio PlaySoundInstance(string audioPath, SoundContext context, float volume = 1f, float panOverride = 0f, float pitchOverride = 0f, bool gameplaySound = false, bool rememberMe = false)
        {
            // because ogg is the only good audio format.
            var prepend = TankGame.Instance.Content.RootDirectory + "/";
            audioPath = prepend + audioPath;

            switch (context)
            {
                case SoundContext.Music:
                    volume *= MusicVolume;
                    break;
                case SoundContext.Effect:
                    volume *= EffectsVolume;
                    if (gameplaySound && MainMenu.Active)
                        volume *= 0.25f;
                    if (SteamworksUtils.IsOverlayActive)
                        volume *= 0.5f;
                    break;
                case SoundContext.Ambient:
                    volume *= AmbientVolume;
                    break;
            }

            OggAudio sfx = null;

            // check if it exists in the cache first
            if (rememberMe) {
                bool exists = SavedSounds.ContainsKey(audioPath);

                if (!exists) {
                    sfx = new OggAudio(audioPath);
                    SavedSounds.Add(audioPath, sfx);

                    sfx.Instance.Pan = MathHelper.Clamp(panOverride, -1f, 1f);
                    sfx.Instance.Pitch = MathHelper.Clamp(pitchOverride, -1f, 1f);
                    sfx.Instance.Play();
                    sfx.Instance.Volume = MathHelper.Clamp(volume, 0f, 1f);
                }
                else {
                    sfx = SavedSounds[SavedSounds.Keys.ToList().First(x => x == audioPath)];
                    sfx.Instance.Pan = MathHelper.Clamp(panOverride, -1f, 1f);
                    sfx.Instance.Pitch = MathHelper.Clamp(pitchOverride, -1f, 1f);
                    sfx.Instance.Play();
                    sfx.Instance.Volume = MathHelper.Clamp(volume, 0f, 1f);
                }
            }
            else {
                sfx = new OggAudio(audioPath);
                sfx.Instance.Pan = MathHelper.Clamp(panOverride, -1f, 1f);
                sfx.Instance.Pitch = MathHelper.Clamp(pitchOverride, -1f, 1f);
                sfx.Instance.Play();
                sfx.Instance.Volume = MathHelper.Clamp(volume, 0f, 1f);
            }

            //GameContent.Systems.ChatSystem.SendMessage($"{nameof(exists)}: {exists}", Color.White);
            //GameContent.Systems.ChatSystem.SendMessage($"new list count: {Sounds.Count}", Color.White);

            /*if (exists)
            {
                var sound = Sounds[Sounds.FindIndex(p => p.Name == soundDef.Name)];// = soundDef;

                if (sound.Sound.IsPlaying())
                    sound.Sound.Instance.Stop();
                sound.Sound.Instance.Play();
                sound.Sound.Instance.Volume = volume;

                Sounds[Sounds.FindIndex(p => p.Name == soundDef.Name)] = soundDef;
            }
            else
            {
                soundDef.Sound.Instance.Volume = volume;
                soundDef.Sound.Instance?.Play();
                Sounds.Add(soundDef);
            }*/

            return sfx;
        }
        public static SoundEffectInstance PlaySoundInstance(SoundEffect fromSound, SoundContext context, Vector3 position, Matrix world, float volume = 1f)
        {
            switch (context)
            {
                case SoundContext.Music:
                    volume *= MusicVolume;
                    break;
                case SoundContext.Effect:
                    volume *= EffectsVolume;
                    break;
                case SoundContext.Ambient:
                    volume *= AmbientVolume;
                    break;
            }

            var pos2d = MatrixUtils.ConvertWorldToScreen(position, world, TankGame.GameView, TankGame.GameProjection);

            var lerp = MathUtils.ModifiedInverseLerp(-(WindowUtils.WindowWidth / 2), WindowUtils.WindowWidth + WindowUtils.WindowWidth / 2, pos2d.X, true);

            var sfx = fromSound.CreateInstance();
            sfx.Volume = volume;

            // System.Diagnostics.Debug.WriteLine(sfx.Pan);
            sfx?.Play();
            sfx.Pan = lerp;

            return sfx;
        }

        public static OggAudio SoundError() => PlaySoundInstance("Assets/sounds/menu/menu_error.ogg", SoundContext.Effect);
    }
    public enum SoundContext : byte
    {
        Music,
        Effect,
        Ambient
    }
}