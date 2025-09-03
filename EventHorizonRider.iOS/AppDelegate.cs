using EventHorizonRider.Core;
using Foundation;
using UIKit;

namespace EventHorizonRider.iOS;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    private MainGame _game;

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

        _game = new MainGame();
        _game.Run();

        return true;
    }
}

