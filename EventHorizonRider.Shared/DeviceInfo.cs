using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// Information to scale game based on hardware device it's running on in a platform agnostic way.
    /// </summary>
    public class DeviceInfo
    {
        private static bool graphicsInitialized;
        private static bool platformInitialized;

        public static Matrix OutputScaleMatrix { get; private set; }

        public static float InputScale { get; private set; }

        public static float OutputScale { get; private set; }

        public static int LogicalWidth { get; private set; }

        public static int LogicalWidthOriginal { get; private set; }

        public static int LogicalHeight { get; private set; }

        public static Vector2 LogicalCenter { get; private set; }

        public static int NativeWidth { get; private set; }

        public static int NativeHeight { get; private set; }

        /// <summary>
        /// Get platform specific settings like whether to use a mouse or the graphical detail level to use.
        /// </summary>
        public static Platform Platform { get; private set; }

        /// <summary>
        /// Initialize platform.  This must be called before the main game object is created.
        /// </summary>
        public static void InitializePlatform(Platform platform)
        {
            if (platformInitialized)
            {
                throw new InvalidOperationException("Platform already initialized");
            }

            Platform = platform;

            platformInitialized = true;
        }

        /// <summary>
        /// Initialize graphics settings.  This must be called in the game initialization logic.
        /// </summary>
        public static void InitializeGraphics(GraphicsDevice graphics)
        {
            if (!platformInitialized)
            {
                throw new InvalidOperationException("Platform must first be initialized");
            }

            if (graphicsInitialized)
            {
                throw new InvalidOperationException("Graphics already initialized");
            }

            // Original native resolution game was developed for (iPhone 4inch Retina)
            const int baseHeight = 640;
            const int baseWidth = 1136;

            NativeHeight = graphics.Viewport.Height;
            NativeWidth = graphics.Viewport.Width;

            // Only scale output on Height since the game is run in portrait mode
            OutputScale = NativeHeight / (float)baseHeight;
            OutputScaleMatrix = Matrix.CreateScale(OutputScale, OutputScale, 1);

            InputScale = 1f / OutputScale;

            LogicalHeight = baseHeight;
            LogicalWidth = (int)Math.Round(NativeWidth * (baseHeight / (float)NativeHeight));
            LogicalWidthOriginal = baseWidth;

            LogicalCenter = new Vector2(LogicalWidth / 2f, LogicalHeight / 2f);

            graphicsInitialized = true;
        }
    }
}
