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
        public MainActivity()
        {

        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            var newUiOptions = (int)uiOptions;

            newUiOptions |= (int)SystemUiFlags.LowProfile;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.Immersive;
            newUiOptions &= ~(int)SystemUiFlags.Visible;

            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            var platform = new Platform
            {
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
            var view = (View)mainGame.Services.GetService(typeof(View));
            SetContentView(view);
            mainGame.Run();
        }
    }
}

