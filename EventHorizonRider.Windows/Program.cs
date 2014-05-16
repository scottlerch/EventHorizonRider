using System.Runtime.InteropServices;
using EventHorizonRider.Core;
using System;
using System.Reflection;
using System.Windows.Forms;
using SharpDX.DXGI;

namespace EventHorizonRider.Windows
{
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main(string[] args)
        {
            DeviceInfo.InitializePlatform(new Platform
            {
                IsMouseVisible = true,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
                TouchEnabled = GetSystemMetrics(95) > 0,
            });

            using (var game = new MainGame())
            {
                game.SetResolution(1366, 768);

                if (args.Length > 0 && args[0].Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    var developmentToolsForm = new DevelopmentToolsForm(game);
                    var parentForm = game.Window.GetType().GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(game.Window) as IWin32Window;
                    developmentToolsForm.Show(parentForm);
                }

                game.Run();
            }
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern int GetSystemMetrics(int nIndex);
    }
}
