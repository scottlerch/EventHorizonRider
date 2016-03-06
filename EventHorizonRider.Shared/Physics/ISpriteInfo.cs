using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    /// <summary>
    /// Interface for all information needed to render sprites and check for collisions between sprites.
    /// </summary>
    internal interface ISpriteInfo
    {
        Vector2 Origin { get; }

        Vector2 Scale { get; }

        float Rotation { get; }

        Vector2 Position { get; }

        Texture2D Texture { get; }

        CollisionInfo CollisionInfo { get; }
    }
}
