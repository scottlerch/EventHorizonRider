using MonoTouch.Foundation;
using MonoTouch.UIKit;
using EventHorizonRider.Core;

namespace EventHorizonRider.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private MainGame game;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var detailLevel = DetailLevel.Full;

            // TODO: determine detail level on other devices
            switch (DeviceHardware.Version)
            {
                case HardwareType.iPad3:
                    detailLevel = DetailLevel.PixelShaderEffectsHalf | DetailLevel.CollisionDetectionHalf | DetailLevel.StaticStars;
                    break;
                case HardwareType.iPhone5CDMAGSM:
                case HardwareType.iPhone5GSM:
                    detailLevel = DetailLevel.PixelShaderEffectsFull | DetailLevel.CollisionDetectionFull | DetailLevel.StaticStars;
                    break;
            }

            game = new MainGame(detailLevel);
            game.Run();

            return true;
        }
    }
}

