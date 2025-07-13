using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics;

/// <summary>
/// Information used to determine if two sprites have collided.
/// </summary>
internal class CollisionInfo(PixelData<byte> pixelData, Vector2 offset, float scale)
{
    public PixelData<byte> PixelData { get; private set; } = pixelData;

    public Rectangle Bounds { get; private set; } = new Rectangle(0, 0, pixelData.Width, pixelData.Height);

    public Vector2 Offset { get; private set; } = offset;

    public float Scale { get; private set; } = scale;
}
