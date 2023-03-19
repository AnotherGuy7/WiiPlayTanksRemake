﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TanksRebirth.Internals.Common.GameUI;
using TanksRebirth.Internals.Common.Utilities;
using TanksRebirth.GameContent.Systems;
using TanksRebirth.Internals.Common;
using System;
using TanksRebirth.Internals.UI;
using TanksRebirth.Internals.Common.Framework.Input;
using FontStashSharp;
using TanksRebirth.Internals.Common.Framework.Audio;
using TanksRebirth.Net;
using TanksRebirth.GameContent.Properties;

namespace TanksRebirth.GameContent.UI
{
    public static class GameUI
    {
        public static bool InOptions { get; set; }

        public static Keybind Pause = new("Pause", Keys.Escape);

        public static UIPanel MissionInfoBar;

        public static UITextButton ResumeButton;

        public static UITextButton RestartButton;

        public static UITextButton QuitButton;

        public static UITextButton OptionsButton;

        public static UITextButton VolumeButton;

        public static UITextButton GraphicsButton;

        public static UITextButton ControlsButton;

        public static UITextButton BackButton;

        // public static UIDropdown testdropdwn;

        //public static UITextButton testbuttondrop1;
        //public static UITextButton testbuttondrop2;
        //public static UITextButton testbuttondrop3;

        public static UIElement[] menuElements;
        public static UIElement[] graphicsElements;

        public static bool Paused { get; set; } = false;

        private static int _delay;

        private static float _gpuSettingsOffset = 0f;

        // TODO: make rect scissor work -> get powerups to be pickupable
        private static bool _initialized;

        internal static Vector2 QuitButtonSize = new Vector2(500, 150);
        internal static Vector2 OptionsButtonSize = new Vector2(500, 150);

        internal static Vector2 QuitButtonPos = new Vector2(700, 850);
        internal static Vector2 OptionsButtonPos = new Vector2(700, 600);

        internal static void Initialize()
        {
            if (_initialized) {
                foreach (var field in typeof(GameUI).GetFields()) {
                    if (field.GetValue(null) is UIElement) {
                        ((UIElement)field.GetValue(null)).Remove();
                        field.SetValue(null, null);
                    }
                }
            }
            _initialized = true;
            var ttColor = Color.LightGray;
            SpriteFontBase font = TankGame.TextFont;

            Vector2 drawOrigin = font.MeasureString("Unknown") / 2f;
            MissionInfoBar = new((uiPanel, spriteBatch) => spriteBatch.DrawString(font, "Unknown", uiPanel.Hitbox.Center.ToVector2(), Color.White, new Vector2(1.5f), 0f, drawOrigin));
            MissionInfoBar.BackgroundColor = Color.Red;
            MissionInfoBar.SetDimensions(() => new Vector2(WindowUtils.WindowWidth / 2 - 250.ToResolutionX(), WindowUtils.WindowHeight - 75.ToResolutionY()), () => new Vector2(500, 50).ToResolution());

            ResumeButton = new(TankGame.GameLanguage.Resume, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            ResumeButton.SetDimensions(() => new Vector2(700, 100).ToResolution(), () => new Vector2(500, 150).ToResolution());
            ResumeButton.OnLeftClick = (uiElement) => Pause.Fire();

            RestartButton = new(TankGame.GameLanguage.StartOver, font, Color.WhiteSmoke)
            {
                IsVisible = false,
            };
            RestartButton.SetDimensions(() => new Vector2(700, 350).ToResolution(), () => new Vector2(500, 150).ToResolution());

            OptionsButton = new(TankGame.GameLanguage.Options, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            OptionsButton.SetDimensions(() => OptionsButtonPos.ToResolution(), () => OptionsButtonSize.ToResolution());
            OptionsButton.OnLeftClick = (uiElement) =>
            {
                _delay = 1;
                InOptions = true;
                ResumeButton.IsVisible = false;
                RestartButton.IsVisible = false;
                QuitButton.IsVisible = false;
                OptionsButton.IsVisible = false;
                VolumeButton.IsVisible = true;
                GraphicsButton.IsVisible = true;
                ControlsButton.IsVisible = true;
                BackButton.IsVisible = true;

                MainMenu.MenuState = MainMenu.State.Options;

                BackButton.Size.Y = 150;

                if (MainMenu.Active)
                {
                    MainMenu.PlayButton.IsVisible = false;
                }
            };

            VolumeButton = new(TankGame.GameLanguage.Volume, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            VolumeButton.SetDimensions(() => new Vector2(700, 100).ToResolution(), () => new Vector2(500, 150).ToResolution());
            VolumeButton.OnLeftClick = (uiElement) =>
            {
                VolumeUI.BatchVisible = true;
                VolumeUI.ShowAll();
                VolumeUI.MusicVolume.IgnoreMouseInteractions = true;
                _delay = 1;
                VolumeButton.IsVisible = false;
                GraphicsButton.IsVisible = false;
                ControlsButton.IsVisible = false;
            };

            GraphicsButton = new(TankGame.GameLanguage.Graphics, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            GraphicsButton.SetDimensions(() => new Vector2(700, 350).ToResolution(), () => new Vector2(500, 150).ToResolution());
            GraphicsButton.OnLeftClick = (uiElement) =>
            {
                GraphicsUI.BatchVisible = true;
                GraphicsUI.ShowAll();
                GraphicsUI.VsyncButton.IgnoreMouseInteractions = true;
                _delay = 1;
                VolumeButton.IsVisible = false;
                GraphicsButton.IsVisible = false;
                ControlsButton.IsVisible = false;
            };

            ControlsButton = new(TankGame.GameLanguage.Controls, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            ControlsButton.SetDimensions(() => new Vector2(700, 600).ToResolution(), () => new Vector2(500, 150).ToResolution());
            ControlsButton.OnLeftClick = (uiElement) =>
            {
                ControlsUI.BatchVisible = true;
                ControlsUI.ShowAll();
                VolumeButton.IsVisible = false;
                GraphicsButton.IsVisible = false;
                ControlsButton.IsVisible = false;
            };

            QuitButton = new(TankGame.GameLanguage.Quit, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            QuitButton.SetDimensions(() => QuitButtonPos.ToResolution(), () => QuitButtonSize.ToResolution());
            QuitButton.OnLeftClick = (ui) =>
            {
                if (!MainMenu.Active)
                {
                    foreach (var elem in MainMenu.campaignNames)
                        elem?.Remove();
                    MainMenu.campaignNames.Clear();
                    MainMenu.Open();
                    if (LevelEditor.Active) {
                        LevelEditor.Close(true);
                        LevelEditor.Editing = false;
                    }

                    Client.SendQuit();
                }
                else {
                    TankGame.Quit();
                }
            };

            BackButton = new(TankGame.GameLanguage.Back, font, Color.WhiteSmoke)
            {
                IsVisible = false
            };
            BackButton.SetDimensions(() => new Vector2(700, 850).ToResolution(), () => new Vector2(500, 150).ToResolution());
            BackButton.OnLeftClick = (uiElement) => HandleBackButton();

            // MainMenu.Initialize();

            GraphicsUI.Initialize();
            ControlsUI.Initialize();
            VolumeUI.Initialize();
            PostInitialize();
        }

        private static void PostInitialize()
        {
            Pause.KeybindPressAction = (p) =>
            {
                if (CampaignCompleteUI.IsViewingResults)
                    return;
                if (InOptions)
                {
                    HandleBackButton();
                    return;
                }
                else if (!MainMenu.Active)
                {
                    Paused = !Paused;
                    if (Paused)
                        TankMusicSystem.PauseAll();
                    else
                        TankMusicSystem.ResumeAll();
                }

                ResumeButton.IsVisible = Paused;
                RestartButton.IsVisible = Paused;
                QuitButton.IsVisible = Paused;
                OptionsButton.IsVisible = Paused;
            };

            menuElements = new UIElement[]
            {
                ResumeButton,
                RestartButton,
                QuitButton,
                OptionsButton,
                VolumeButton,
                GraphicsButton,
                ControlsButton,
                BackButton,
                GraphicsUI.VsyncButton,
                GraphicsUI.PerPixelLightingButton,
                GraphicsUI.FullScreenButton,
                GraphicsUI.ResolutionButton,
                VolumeUI.MusicVolume,
                VolumeUI.EffectsVolume,
                VolumeUI.AmbientVolume
            };
            graphicsElements = new UIElement[]
            {
                GraphicsUI.VsyncButton,
                GraphicsUI.VsyncToggle,
                GraphicsUI.PerPixelLightingButton,
                GraphicsUI.PerPixelLightingToggle,
                GraphicsUI.FullScreenButton,
                GraphicsUI.FullScreenToggle,
                GraphicsUI.ResolutionButton
            };
            foreach (UIElement button in graphicsElements)
            {
                button.HasScissor = true;
                button.Scissor = () => new(0, (int)(WindowUtils.WindowHeight * 0.05f), WindowUtils.WindowWidth, (int)(WindowUtils.WindowHeight * 0.7f));
                button.OnMouseOver = (uiElement) => { SoundPlayer.PlaySoundInstance("Assets/sounds/menu/menu_tick.ogg", SoundContext.Effect); };
            }
            foreach (var e in menuElements)
                e.OnMouseOver = (uiElement) => { SoundPlayer.PlaySoundInstance("Assets/sounds/menu/menu_tick.ogg", SoundContext.Effect); };

            // UIElement.ResizeAndRelocate();
        }

        private static void HandleBackButton()
        {
            if (!_initialized)
                return;
            if (MainMenu.MenuState == MainMenu.State.Cosmetics)
                MainMenu.MenuState = MainMenu.State.PlayList;
            if (MainMenu.MenuState == MainMenu.State.StatsMenu)
                MainMenu.MenuState = MainMenu.State.PrimaryMenu;

            // We are on the main menu and in the settings menu.
            if (MainMenu.MenuState == MainMenu.State.Options && MainMenu.Active && VolumeButton.IsVisible) {
                // Set to main menu, we are going back to it after all.
                MainMenu.MenuState = MainMenu.State.PrimaryMenu;
                
                // Hide Options buttons and load MMenu buttons.
                MainMenu.PlayButton.IsVisible = true;
                OptionsButton.IsVisible = true;
                QuitButton.IsVisible = true;
                
                ResumeButton.IsVisible = false;
                RestartButton.IsVisible = false;
                BackButton.IsVisible = false;
                ControlsButton.IsVisible = false;
                GraphicsButton.IsVisible = false;
                VolumeButton.IsVisible = false;
            }
            

            if (VolumeButton.IsVisible && !MainMenu.Active)
            {
                ResumeButton.IsVisible = true;
                OptionsButton.IsVisible = true;
                RestartButton.IsVisible = true;
                QuitButton.IsVisible = true;

                BackButton.IsVisible = false;
                ControlsButton.IsVisible = false;
                GraphicsButton.IsVisible = false;
                VolumeButton.IsVisible = false;
            }
            else if (VolumeUI.BatchVisible)
            {
                VolumeUI.BatchVisible = false;
                VolumeUI.HideAll();
                VolumeButton.IsVisible = true;
                GraphicsButton.IsVisible = true;
                ControlsButton.IsVisible = true;
            }
            else if (GraphicsUI.BatchVisible)
            {
                GraphicsUI.BatchVisible = false;
                GraphicsUI.HideAll();
                VolumeButton.IsVisible = true;
                GraphicsButton.IsVisible = true;
                ControlsButton.IsVisible = true;

                TankGame.Settings.ResWidth = GraphicsUI.CurrentRes.Key;
                TankGame.Settings.ResHeight = GraphicsUI.CurrentRes.Value;


                TankGame.Instance.Graphics.PreferredBackBufferWidth = TankGame.Settings.ResWidth;
                TankGame.Instance.Graphics.PreferredBackBufferHeight = TankGame.Settings.ResHeight;

                TankGame.Instance.Graphics.ApplyChanges();

                // FIXME: acts weird
                // TankGame.Instance.CalculateProjection();
            }
            else if (ControlsUI.BatchVisible)
            {
                ControlsUI.BatchVisible = false;
                ControlsUI.HideAll();
                VolumeButton.IsVisible = true;
                GraphicsButton.IsVisible = true;
                ControlsButton.IsVisible = true;
            }
            else
            {
                if (MainMenu.Active)
                {
                    if (MainMenu.PlayButton.IsVisible)
                        return;
                    if (MainMenu.campaignNames.Count > 0)
                    {
                        foreach (var elem in MainMenu.campaignNames)
                            elem.Remove();
                        MainMenu.MenuState = MainMenu.State.PlayList;

                        MainMenu.campaignNames.Clear();
                    }
                    else if (MainMenu.PlayButton_SinglePlayer.IsVisible)
                    {
                        MainMenu.PlayButton.IsVisible = true;
                        OptionsButton.IsVisible = true;
                        QuitButton.IsVisible = true;

                        BackButton.IsVisible = false;

                        MainMenu.MenuState = MainMenu.State.PrimaryMenu;
                    }

                    else if (GraphicsButton.IsVisible)
                    {
                        BackButton.IsVisible = false;
                        VolumeButton.IsVisible = false;
                        GraphicsButton.IsVisible = false;
                        ControlsButton.IsVisible = false;
                        OptionsButton.IsVisible = true;
                        QuitButton.IsVisible = true;
                        GraphicsUI.BatchVisible = false;
                        VolumeUI.BatchVisible = false;

                        MainMenu.MenuState = MainMenu.State.PrimaryMenu;    
                    }
                    else if (MainMenu.ConnectToServerButton.IsVisible || MainMenu.DisconnectButton.IsVisible)
                    {
                        MainMenu.MenuState = MainMenu.State.PlayList;
                    }
                    if (MainMenu.TanksAreCalculators.IsVisible)
                    {
                        MainMenu.MenuState = MainMenu.State.PlayList;
                    }
                }
                else
                {
                    InOptions = false;
                    VolumeUI.HideAll();
                    BackButton.IsVisible = false;
                    VolumeButton.IsVisible = false;
                    GraphicsButton.IsVisible = false;
                    ControlsButton.IsVisible = false;
                }
            }

            // UIElement.ResizeAndRelocate();
        }

        private static int _newScroll;
        private static int _oldScroll;

        public static void UpdateButtons()
        {
            if (!Paused) {
                _newScroll = 0;
                _oldScroll = 0;
            }    
            if (!_initialized)
                return;
            _newScroll = InputUtils.CurrentMouseSnapshot.ScrollWheelValue;

            if (_newScroll != _oldScroll)
            {
                _gpuSettingsOffset = _newScroll - _oldScroll;
                foreach (var b in graphicsElements)
                {
                    b.Position = new(b.Position.X, b.Position.Y + _gpuSettingsOffset);
                    b.MouseHovering = false;
                }
                // ChatSystem.SendMessage(_gpuSettingsOffset, Color.White, "<Debug>");
            }
            var text = /*$"{TankGame.GameLanguage.Mission} 1        x{AITank.CountAll()}";*/
                $"{GameProperties.LoadedCampaign.CurrentMission.Name ?? $"{TankGame.GameLanguage.Mission}"} x{AITank.CountAll()}";
            Vector2 drawOrigin = TankGame.TextFont.MeasureString(text) / 2f;
            MissionInfoBar.UniqueDraw =
                (uiPanel, spriteBatch) => spriteBatch.DrawString(TankGame.TextFont, text, uiPanel.Hitbox.Center.ToVector2(), Color.White, new Vector2(1.5f).ToResolution(), 0, drawOrigin);

            //TankGame.Settings.MusicVolume = VolumeUI.MusicVolume.Value;
            //TankGame.Settings.EffectsVolume = VolumeUI.EffectsVolume.Value;
            //TankGame.Settings.AmbientVolume = VolumeUI.AmbientVolume.Value;
            VolumeUI.MusicVolume.Value = TankGame.Settings.MusicVolume;
            VolumeUI.EffectsVolume.Value = TankGame.Settings.EffectsVolume;
            VolumeUI.AmbientVolume.Value = TankGame.Settings.AmbientVolume;
            // remove this once i somehow fix these damn sliders ^

            if (VolumeUI.MusicVolume.Value <= 0.01f)
                VolumeUI.MusicVolume.Value = 0f;

            if (VolumeUI.EffectsVolume.Value <= 0.01f)
                VolumeUI.EffectsVolume.Value = 0f;

            if (VolumeUI.AmbientVolume.Value <= 0.01f)
                VolumeUI.AmbientVolume.Value = 0f;

            if (!MainMenu.Active)
                TankMusicSystem.UpdateVolume();

            if (_delay > 0 && !InputUtils.MouseLeft)
                _delay--;
            if (_delay <= 0)
            {
                VolumeUI.MusicVolume.IgnoreMouseInteractions = false;
                GraphicsUI.VsyncButton.IgnoreMouseInteractions = false;
            }
            VolumeUI.MusicVolume.Tooltip = $"{Math.Round(TankGame.Settings.MusicVolume * 100, 1)}%";
            VolumeUI.EffectsVolume.Tooltip = $"{Math.Round(TankGame.Settings.EffectsVolume * 100, 1)}%";
            VolumeUI.AmbientVolume.Tooltip = $"{Math.Round(TankGame.Settings.AmbientVolume * 100, 1)}%";

            _oldScroll = _newScroll;
        }
    }
}