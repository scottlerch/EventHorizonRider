using System;
using EventHorizonRider.Core;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using SharpDX.XInput;

namespace EventHorizonRider.WindowsStore
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var platform = new Platform
            {
                IsMouseVisible = true,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
                TouchEnabled = new Windows.Devices.Input.TouchCapabilities().TouchPresent > 0,
                PauseOnLoseFocus = true,
                TargetElapsedTime = TimeSpan.FromSeconds(1/60D),
                IsFixedTimeStep = true,
                ParticleEffectsDetails = ParticleEffectsDetails.Full,
            };

            if (false) // TODO: detect Surface RT
            {
                platform.ParticleEffectsDetails = ParticleEffectsDetails.None;
                platform.TargetElapsedTime = TimeSpan.FromSeconds(1/60D);
            }

            DeviceInfo.InitializePlatform(platform);

            var factory = new MonoGame.Framework.GameFrameworkViewSource<MainGame>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}
