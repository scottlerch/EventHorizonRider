using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    internal class ScreenInfo
    {
        public static Matrix ScaleMatrix { get; private set; }

        public static float InputScale { get; private set; }

        public static int LogicalWidth { get; private set; }

        public static int LogicalHeight { get; private set; }

        public static int NativeWidth { get; private set; }

        public static int NativeHeight { get; private set; }

        public static void Initialize(GraphicsDevice graphics)
        {
            // Original native resolution
            const int baseHeight = 640;
            const int baseWidth = 1136;

            NativeHeight = graphics.Viewport.Height;
            NativeWidth = graphics.Viewport.Width;

            var scale = (float)NativeHeight / (float)baseHeight;
            ScaleMatrix = Matrix.CreateScale(scale, scale, 1);

            InputScale = 1f/scale;

            LogicalHeight = baseHeight;
            LogicalWidth = (int)Math.Round(NativeWidth * ((float)baseHeight / (float)NativeHeight));
        }
    }
}
