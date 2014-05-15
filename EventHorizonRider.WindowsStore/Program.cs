using EventHorizonRider.Core;

namespace EventHorizonRider.WindowsStore
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            DeviceInfo.InitializePlatform(new Platform
            {
                IsMouseVisible = true,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
            });

            var factory = new MonoGame.Framework.GameFrameworkViewSource<MainGame>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}
