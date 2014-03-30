using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    internal interface ISpriteInfo
    {
        Vector2 Origin { get; }
        Vector2 Scale { get; }
        float Rotation { get; }
        Vector2 Position { get; }
        Texture2D Texture { get; }

        /// <summary>
        /// Just the alpha channel data of the texture used by collision detection.
        /// </summary>
        byte[] TextureAlphaData { get; }
    }
}
