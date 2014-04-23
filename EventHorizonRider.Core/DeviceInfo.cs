﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// Information to scale game based on hardware device it's running on in a platform agnostic way.
    /// </summary>
    internal class DeviceInfo
    {
        public static Matrix OutputScaleMatrix { get; private set; }

        public static float InputScale { get; private set; }

        public static float OutputScale { get; private set; }

        public static int LogicalWidth { get; private set; }

        public static int LogicalHeight { get; private set; }

        public static int NativeWidth { get; private set; }

        public static int NativeHeight { get; private set; }

        public static DetailLevel DetailLevel { get; private set; }

        public static void Initialize(GraphicsDevice graphics, DetailLevel detailLevel)
        {
            // Original native resolution
            const int baseHeight = 640;
            // const int baseWidth = 1136;

            DetailLevel = detailLevel;

            NativeHeight = graphics.Viewport.Height;
            NativeWidth = graphics.Viewport.Width;

            // Only scale output on Height since the game is run in portrait mode
            OutputScale = NativeHeight / (float)baseHeight;
            OutputScaleMatrix = Matrix.CreateScale(OutputScale, OutputScale, 1);

            InputScale = 1f / OutputScale;

            LogicalHeight = baseHeight;
            LogicalWidth = (int)Math.Round(NativeWidth * (baseHeight / (float)NativeHeight));
        }
    }
}
