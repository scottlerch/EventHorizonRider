using Foundation;
using UIKit;
using EventHorizonRider.Core;

namespace EventHorizonRider.iOS;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    private MainGame game;

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        var platform = new Platform
        {
            IsMouseVisible = false,
            UseDynamicStars = false,
            PixelShaderDetail = PixelShaderDetail.Full,
            CollisionDetectionDetail = CollisionDetectionDetail.Full,
            TouchEnabled = true,
            PauseOnLoseFocus = true,
        };

        // TODO: determine detail level on other devices using DeviceHardware.Version

        DeviceInfo.InitializePlatform(platform);

        game = new MainGame();
        game.Run();

        return true;
    }
}

