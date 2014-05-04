using EventHorizonRider.Core;
using System;
using System.Reflection;
using System.Windows.Forms;

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
            using (var game = new MainGame())
            {
                game.DetailLevel = DetailLevel.PixelShaderEffectsFull | DetailLevel.CollisionDetectionFull | DetailLevel.StaticStars;
                game.IsMouseVisible = true;

                if (args.Length > 0 && args[0].Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    var developmentToolsForm = new DevelopmentToolsForm(game);
                    var parentForm = game.Window.GetType().GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(game.Window) as IWin32Window;
                    developmentToolsForm.Show(parentForm);
                }

                game.Run();
            }
        }
    }
}
