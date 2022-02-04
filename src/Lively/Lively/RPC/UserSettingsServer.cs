﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Lively.Core.Display;
using Lively.Grpc.Common.Proto.Settings;
using Lively.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lively.Common;
using Lively.Models;
using System.Linq;
using System.Diagnostics;

namespace Lively.RPC
{
    internal class UserSettingsServer : SettingsService.SettingsServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IDisplayManager displayManager;
        private readonly IUserSettingsService userSettings;
        private readonly object appRulesWriteLock = new object();
        private readonly object settingsWriteLock = new object();

        public UserSettingsServer(IDisplayManager displayManager, IUserSettingsService userSettings)
        {
            this.displayManager = displayManager;
            this.userSettings = userSettings;
        }

        public override Task<AppRulesSettings> GetAppRulesSettings(Empty _, ServerCallContext context)
        {
            var resp = new AppRulesSettings();
            foreach (var app in userSettings.AppRules)
            {
                resp.AppRules.Add(new AppRulesDataModel
                {
                    AppName = app.AppName,
                    Rule = (AppRules)((int)app.Rule)
                });
            }
            return Task.FromResult(resp);
        }

        public override Task<Empty> SetAppRulesSettings(AppRulesSettings req, ServerCallContext context)
        {
            userSettings.AppRules.Clear();
            foreach (var item in req.AppRules)
            {
                userSettings.AppRules.Add(new ApplicationRulesModel(item.AppName, (AppRulesEnum)((int)item.Rule)));
            }

            try
            {
                return Task.FromResult(new Empty());
            }
            finally
            {
                lock (appRulesWriteLock)
                {
                    userSettings.Save<List<IApplicationRulesModel>>();
                }
            }
        }

        public override Task<Empty> SetSettings(SettingsDataModel req, ServerCallContext context)
        {
            userSettings.Settings.SavedURL = req.SavedUrl;
            userSettings.Settings.ProcessMonitorAlgorithm = (ProcessMonitorAlgorithm)((int)req.ProcessMonitorAlogorithm);
            userSettings.Settings.SelectedDisplay = displayManager.DisplayMonitors.FirstOrDefault(x => req.SelectedDisplay.DeviceId == x.DeviceId) ?? displayManager.PrimaryDisplayMonitor;
            userSettings.Settings.WallpaperArrangement = (WallpaperArrangement)((int)req.WallpaperArrangement);
            userSettings.Settings.AppVersion = req.AppVersion;
            userSettings.Settings.Startup = req.Startup;
            userSettings.Settings.IsFirstRun = req.IsFirstRun;
            userSettings.Settings.ControlPanelOpened = req.ControlPanelOpened;
            userSettings.Settings.AppFocusPause = (AppRulesEnum)((int)req.AppFocusPause);
            userSettings.Settings.AppFullscreenPause = (AppRulesEnum)((int)req.AppFullscreenPause);
            userSettings.Settings.BatteryPause = (AppRulesEnum)((int)req.BatteryPause);
            userSettings.Settings.VideoPlayer = (LivelyMediaPlayer)((int)req.VideoPlayer);
            userSettings.Settings.VideoPlayerHwAccel = req.VideoPlayerHwAccel;
            userSettings.Settings.WebBrowser = (LivelyWebBrowser)((int)req.WebBrowser);
            userSettings.Settings.GifPlayer = (LivelyGifPlayer)((int)req.GifPlayer);
            userSettings.Settings.PicturePlayer = (LivelyPicturePlayer)((int)req.PicturePlayer);
            userSettings.Settings.WallpaperWaitTime = req.WallpaperWaitTime;
            userSettings.Settings.ProcessTimerInterval = req.ProcessTimerInterval;
            userSettings.Settings.StreamQuality = (Common.StreamQualitySuggestion)((int)req.StreamQuality);
            userSettings.Settings.LivelyZipGenerate = req.LivelyZipGenerate;
            userSettings.Settings.ScalerVideo = (WallpaperScaler)((int)req.ScalerVideo);
            userSettings.Settings.ScalerGif = (WallpaperScaler)((int)req.ScalerGif);
            userSettings.Settings.GifCapture = req.GifCapture;
            userSettings.Settings.MultiFileAutoImport = req.MultiFileAutoImport;
            userSettings.Settings.SafeShutdown = req.SafeShutdown;
            userSettings.Settings.IsRestart = req.IsRestart;
            userSettings.Settings.InputForward = (Common.InputForwardMode)req.InputForward;
            userSettings.Settings.MouseInputMovAlways = req.MouseInputMovAlways;
            userSettings.Settings.TileSize = req.TileSize;
            userSettings.Settings.LivelyGUIRendering = (LivelyGUIState)((int)req.LivelyGuiRendering);
            userSettings.Settings.WallpaperDir = req.WallpaperDir;
            userSettings.Settings.WallpaperDirMoveExistingWallpaperNewDir = req.WallpaperDirMoveExistingWallpaperNewDir;
            userSettings.Settings.SysTrayIcon = req.SysTrayIcon;
            userSettings.Settings.WebDebugPort = req.WebDebugPort;
            userSettings.Settings.AutoDetectOnlineStreams = req.AutoDetectOnlineStreams;
            userSettings.Settings.ExtractStreamMetaData = req.ExtractStreamMetaData;
            userSettings.Settings.WallpaperBundleVersion = req.WallpaperBundleVersion;
            userSettings.Settings.AudioVolumeGlobal = req.AudioVolumeGlobal;
            userSettings.Settings.AudioOnlyOnDesktop = req.AudioOnlyOnDesktop;
            userSettings.Settings.WallpaperScaling = (WallpaperScaler)req.WallpaperScaling;
            userSettings.Settings.CefDiskCache = req.CefDiskCache;
            userSettings.Settings.DebugMenu = req.DebugMenu;
            userSettings.Settings.TestBuild = req.TestBuild;
            userSettings.Settings.ApplicationTheme = (Common.AppTheme)req.ApplicationTheme;
            userSettings.Settings.RemoteDesktopPause = (AppRulesEnum)req.RemoteDesktopPause;
            userSettings.Settings.PowerSaveModePause = (AppRulesEnum)req.PowerSaveModePause;
            userSettings.Settings.LockScreenAutoWallpaper = req.LockScreenAutoWallpaper;
            userSettings.Settings.DesktopAutoWallpaper = req.DesktopAutoWallpaper;
            userSettings.Settings.SystemTaskbarTheme = (Common.TaskbarTheme)req.SystemTaskbarTheme;
            userSettings.Settings.ScreensaverIdleWait = (Common.ScreensaverIdleTime)((int)req.ScreensaverIdleWait);
            userSettings.Settings.ScreensaverOledWarning = req.ScreensaverOledWarning;
            userSettings.Settings.ScreensaverEmptyScreenShowBlack = req.ScreensaverEmptyScreenShowBlack;
            userSettings.Settings.ScreensaverLockOnResume = req.ScreensaverLockOnResume;
            userSettings.Settings.Language = req.Language;
            userSettings.Settings.KeepAwakeUI = req.KeepAwakeUi;

            try
            {
                return Task.FromResult(new Empty());
            }
            finally
            {
                lock (settingsWriteLock)
                {
                    userSettings.Save<ISettingsModel>();
                }
            }
        }

        public override Task<SettingsDataModel> GetSettings(Empty _, ServerCallContext context)
        {
            var settings = userSettings.Settings;
            var resp = new SettingsDataModel()
            {
                SavedUrl = settings.SavedURL,
                ProcessMonitorAlogorithm = (ProcessMonitorRule)((int)settings.ProcessMonitorAlgorithm),
                WallpaperArrangement = (WallpaperArrangementRule)settings.WallpaperArrangement,
                SelectedDisplay = new GetScreensResponse()
                {
                    DeviceId = settings.SelectedDisplay.DeviceId,
                    DeviceName = settings.SelectedDisplay.DeviceName,
                    DisplayName = settings.SelectedDisplay.DisplayName,
                    HMonitor = settings.SelectedDisplay.HMonitor.ToInt32(),
                    IsPrimary = settings.SelectedDisplay.IsPrimary,
                    WorkingArea = new Rectangle()
                    {
                        X = settings.SelectedDisplay.WorkingArea.X,
                        Y = settings.SelectedDisplay.WorkingArea.Y,
                        Width = settings.SelectedDisplay.WorkingArea.Width,
                        Height = settings.SelectedDisplay.WorkingArea.Height
                    },
                    Bounds = new Rectangle()
                    {
                        X = settings.SelectedDisplay.Bounds.X,
                        Y = settings.SelectedDisplay.Bounds.Y,
                        Width = settings.SelectedDisplay.Bounds.Width,
                        Height = settings.SelectedDisplay.Bounds.Height
                    }
                },
                AppVersion = settings.AppVersion,
                Startup = settings.Startup,
                IsFirstRun = settings.IsFirstRun,
                ControlPanelOpened = settings.ControlPanelOpened,
                AppFocusPause = (AppRules)((int)settings.AppFocusPause),
                AppFullscreenPause = (AppRules)((int)settings.AppFullscreenPause),
                BatteryPause = (AppRules)((int)settings.BatteryPause),
                VideoPlayer = (MediaPlayer)((int)settings.VideoPlayer),
                VideoPlayerHwAccel = settings.VideoPlayerHwAccel,
                WebBrowser = (WebBrowser)((int)settings.WebBrowser),
                GifPlayer = (GifPlayer)((int)settings.GifPlayer),
                PicturePlayer = (PicturePlayer)(((int)settings.PicturePlayer)),
                WallpaperWaitTime = settings.WallpaperWaitTime,
                ProcessTimerInterval = settings.ProcessTimerInterval,
                StreamQuality = (Grpc.Common.Proto.Settings.StreamQualitySuggestion)((int)settings.StreamQuality),
                LivelyZipGenerate = settings.LivelyZipGenerate,
                ScalerVideo = (WallpaperScalerRule)((int)settings.ScalerVideo),
                ScalerGif = (WallpaperScalerRule)((int)settings.ScalerGif),
                GifCapture = settings.GifCapture,
                MultiFileAutoImport = settings.MultiFileAutoImport,
                SafeShutdown = settings.SafeShutdown,
                IsRestart = settings.IsRestart,
                InputForward = (Grpc.Common.Proto.Settings.InputForwardMode)((int)settings.InputForward),
                MouseInputMovAlways = settings.MouseInputMovAlways,
                TileSize = settings.TileSize,
                LivelyGuiRendering = (GuiMode)settings.LivelyGUIRendering,
                WallpaperDir = settings.WallpaperDir,
                WallpaperDirMoveExistingWallpaperNewDir = settings.WallpaperDirMoveExistingWallpaperNewDir,
                SysTrayIcon = settings.SysTrayIcon,
                WebDebugPort = settings.WebDebugPort,
                AutoDetectOnlineStreams = settings.AutoDetectOnlineStreams,
                ExtractStreamMetaData = settings.ExtractStreamMetaData,
                WallpaperBundleVersion = settings.WallpaperBundleVersion,
                AudioVolumeGlobal = settings.AudioVolumeGlobal,
                AudioOnlyOnDesktop = settings.AudioOnlyOnDesktop,
                WallpaperScaling = (WallpaperScalerRule)settings.WallpaperScaling,
                CefDiskCache = settings.CefDiskCache,
                DebugMenu = settings.DebugMenu,
                TestBuild = settings.TestBuild,
                ApplicationTheme = (Grpc.Common.Proto.Settings.AppTheme)settings.ApplicationTheme,
                RemoteDesktopPause = (AppRules)settings.RemoteDesktopPause,
                PowerSaveModePause = (AppRules)settings.PowerSaveModePause,
                LockScreenAutoWallpaper = settings.LockScreenAutoWallpaper,
                DesktopAutoWallpaper = settings.DesktopAutoWallpaper,
                SystemTaskbarTheme = (Grpc.Common.Proto.Settings.TaskbarTheme)((int)settings.SystemTaskbarTheme),
                ScreensaverIdleWait = (Grpc.Common.Proto.Settings.ScreensaverIdleTime)((uint)settings.WallpaperWaitTime),
                ScreensaverOledWarning = settings.ScreensaverOledWarning,
                ScreensaverEmptyScreenShowBlack = settings.ScreensaverEmptyScreenShowBlack,
                ScreensaverLockOnResume = settings.ScreensaverLockOnResume,
                Language = settings.Language,
                KeepAwakeUi = settings.KeepAwakeUI,
            };
            return Task.FromResult(resp);
        }
    }
}
