using EventHorizonRider.Core;
using Foundation;
using Microsoft.Xna.Framework;
using UIKit;

namespace EventHorizonRider.iOS;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    private MainGame _game;
    private static UIWindow _keyWindow;

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
            SafeAreaInsetsProvider = GetSafeAreaInsetsInPixels,
        };

        // TODO: determine detail level on other devices using DeviceHardware.Version

        DeviceInfo.InitializePlatform(platform);

        _game = new MainGame();
        _game.Run();

        return true;
    }

    // Safe-area insets (notch / Dynamic Island / rounded corners / home indicator) in native pixels
    // as (Left, Top, Right, Bottom). UIKit reports them in points, so scale to pixels to match the
    // game's native viewport. Returns zero until the window is laid out.
    private static Vector4 GetSafeAreaInsetsInPixels()
    {
        // Cache the key window once found (foreach over scenes is allocation-free; called per frame).
        var window = _keyWindow ??= FindKeyWindow();
        if (window is null)
        {
            return Vector4.Zero;
        }

        var insets = window.SafeAreaInsets;
        var scale = (float)UIScreen.MainScreen.Scale;
        return new Vector4(
            (float)insets.Left * scale,
            (float)insets.Top * scale,
            (float)insets.Right * scale,
            (float)insets.Bottom * scale);
    }

    private static UIWindow FindKeyWindow()
    {
        foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is UIWindowScene windowScene)
            {
                foreach (var window in windowScene.Windows)
                {
                    if (window.IsKeyWindow)
                    {
                        return window;
                    }
                }
            }
        }

        return null;
    }
}

