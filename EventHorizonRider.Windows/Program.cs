using EventHorizonRider.Core;
using System;

namespace EventHorizonRider.Windows
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
            {
                game.DetailLevel = DetailLevel.PixelShaderEffectsNone | DetailLevel.CollisionDetectionFull | DetailLevel.StaticStars;
                game.Run();
            }
        }
    }
}
