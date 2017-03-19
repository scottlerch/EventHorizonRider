using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using EventHorizonRider.Core;
using System;
using Android.Content.Res;

namespace EventHorizonRider.Android
{
    [Activity(
        Label = "Event Horizon Rider", 
        MainLauncher = true,
        Icon = "@drawable/icon",
        //Theme = "@style/Theme.Splash",
        Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        private View view;

        public MainActivity()
        {

        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var platform = new Platform
            {
                IsFullScreen = true,
                IsMouseVisible = false,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
                TouchEnabled = true,
                PauseOnLoseFocus = true,
                TargetElapsedTime = TimeSpan.FromSeconds(1 / 60D),
                IsFixedTimeStep = true,
                ParticleEffectsDetails = ParticleEffectsDetails.Full,
            };

            DeviceInfo.InitializePlatform(platform);

            var mainGame = new MainGame();

            view = (View)mainGame.Services.GetService(typeof(View));

            SetImmersive();
            SetContentView(view);

            mainGame.Run();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                SetImmersive();
            }
        }

        private void SetImmersive()
        {
            view.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.LayoutStable | 
                SystemUiFlags.LayoutHideNavigation | 
                SystemUiFlags.LayoutFullscreen | 
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen | 
                SystemUiFlags.ImmersiveSticky);
        }
    }
}

