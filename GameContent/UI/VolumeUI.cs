using Microsoft.Xna.Framework;
using System;
using TanksRebirth.GameContent.Globals;
using TanksRebirth.Internals.Common.Framework.Input;
using TanksRebirth.Internals.Common.GameUI;
using TanksRebirth.Internals.Common.Utilities;
using TanksRebirth.Internals.UI;

namespace TanksRebirth.GameContent.UI
{
    public static class VolumeUI
    {
        public static UISlider MusicVolume;

        public static UISlider EffectsVolume;

        public static UISlider AmbientVolume;

        public static UIText MusicText;

        public static UIText EffectsText;

        public static UIText AmbientText;

        public static bool BatchVisible { get; set; }

        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) {
                foreach (var field in typeof(VolumeUI).GetFields()) {
                    if (field.GetValue(null) is UIElement) {
                        ((UIElement)field.GetValue(null)).Remove();
                        field.SetValue(null, null);
                    }
                }
            }
            _initialized = true;
            //Music
            MusicVolume = new()
            {
                IsVisible = false,
                FallThroughInputs = true
            };
            MusicVolume.SetDimensions(() => new Vector2(700, 100).ToResolution(), () => new Vector2(500, 150).ToResolution());
            MusicVolume.Tooltip = $"{Math.Round(TankGame.Settings.MusicVolume * 100, 1)}%";
            MusicVolume.Initialize();
            MusicVolume.Value = TankGame.Settings.MusicVolume;
            MusicVolume.BarWidth = 10;
            MusicVolume.SliderColor = Color.WhiteSmoke;

            MusicText = new(TankGame.GameLanguage.MusicVolume, FontGlobals.RebirthFont, Color.Black)
            {
                // IgnoreMouseInteractions = true,
                IsVisible = false,
                FallThroughInputs = true
            };
            MusicText.SetDimensions(() => new Vector2(950, 175).ToResolution(), () => new Vector2(500, 150).ToResolution());

            //Effects
            EffectsVolume = new()
            {
                IsVisible = false
            };
            EffectsVolume.SetDimensions(() => new Vector2(700, 350).ToResolution(), () => new Vector2(500, 150).ToResolution());
            EffectsVolume.Tooltip = $"{Math.Round(TankGame.Settings.EffectsVolume * 100, 1)}%";
            EffectsVolume.Initialize();
            EffectsVolume.Value = TankGame.Settings.EffectsVolume;
            EffectsVolume.BarWidth = 10;
            EffectsVolume.SliderColor = Color.WhiteSmoke;

            EffectsText = new(TankGame.GameLanguage.EffectsVolume, FontGlobals.RebirthFont, Color.Black)
            {
                //IgnoreMouseInteractions = true,
                IsVisible = false
            };
            EffectsText.SetDimensions(() => new Vector2(950, 425).ToResolution(), () => new Vector2(500, 150).ToResolution());

            //Ambient
            AmbientVolume = new()
            {
                IsVisible = false
            };
            AmbientVolume.SetDimensions(() => new Vector2(700, 600).ToResolution(), () => new Vector2(500, 150).ToResolution());
            AmbientVolume.Tooltip = $"{Math.Round(TankGame.Settings.AmbientVolume * 100, 1)}%";
            AmbientVolume.Initialize();
            AmbientVolume.Value = TankGame.Settings.AmbientVolume;
            AmbientVolume.BarWidth = 10;
            AmbientVolume.SliderColor = Color.WhiteSmoke;

            AmbientText = new(TankGame.GameLanguage.AmbientVolume, FontGlobals.RebirthFont, Color.Black)
            {
                //IgnoreMouseInteractions = true,
                IsVisible = false
            };
            AmbientText.SetDimensions(() => new Vector2(950, 675).ToResolution(), () => new Vector2(500, 150).ToResolution());
        }

        public static void HideAll()
        {
            MusicVolume.IsVisible = false;
            EffectsVolume.IsVisible = false;
            AmbientVolume.IsVisible = false;
            MusicText.IsVisible = false;
            EffectsText.IsVisible = false;
            AmbientText.IsVisible = false;
        }

        public static void ShowAll()
        {
            MusicVolume.IsVisible = true;
            EffectsVolume.IsVisible = true;
            AmbientVolume.IsVisible = true;
            MusicText.IsVisible = true;
            EffectsText.IsVisible = true;
            AmbientText.IsVisible = true;
        }
    }
}