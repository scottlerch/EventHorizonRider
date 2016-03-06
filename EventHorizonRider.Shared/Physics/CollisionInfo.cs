using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    /// <summary>
    /// Information used to determine if two sprites have collided.
    /// </summary>
    internal class CollisionInfo
    {
        public PixelData<byte> PixelData { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Vector2 Offset { get; private set; }

        public float Scale { get; private set; }

        public CollisionInfo(PixelData<byte> pixelData, Vector2 offset, float scale)
        {
            PixelData = pixelData;
            Offset = offset;
            Scale = scale;
            Bounds = new Rectangle(0, 0, pixelData.Width, pixelData.Height);
        }
    }
}
