﻿using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    internal class CollisionInfo
    {
        public PixelData<byte> PixelData { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Vector2 Offset { get; private set; }

        public CollisionInfo(PixelData<byte> pixelData, Vector2 offset)
        {
            PixelData = pixelData;
            Offset = offset;
            Bounds = new Rectangle(0, 0, pixelData.Width, pixelData.Height);
        }
    }
}
