using System.Runtime.InteropServices;
using EventHorizonRider.Core;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace EventHorizonRider.Windows;

/// <summary>
/// The main class.
/// </summary>
internal static partial class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    internal static void Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        DeviceInfo.InitializePlatform(new Platform
        {
            IsMouseVisible = true,
            UseDynamicStars = false,
            PixelShaderDetail = PixelShaderDetail.Full,
            CollisionDetectionDetail = CollisionDetectionDetail.Full,
            TouchEnabled = GetSystemMetrics(95) > 0,
            PauseOnLoseFocus = false,
        });

        using var game = new MainGame();
        game.SetResolution(1366, 768);

        if (args.Length > 0 && args[0].Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            var developmentToolsForm = new DevelopmentToolsForm(game);

            var parentForm = game.Window.GetType()
                .GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game.Window) as IWin32Window;

            developmentToolsForm.Show(parentForm);
        }

        game.Run();
    }

    [LibraryImport("user32.dll")]
    private static partial int GetSystemMetrics(int nIndex);
}
