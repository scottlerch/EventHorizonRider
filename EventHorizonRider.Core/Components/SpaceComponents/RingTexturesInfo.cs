using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingTexturesInfo
    {
        public Texture2D[] Textures { get; set; }
        public byte[][] TexturesAlphaData { get; set; }

        /// <summary>
        /// Color to apply to texture when rendering.  To new [] { Color.White } for default.
        /// </summary>
        public Color[] TextureColors { get; set; }

        /// <summary>
        /// Range of number of textures per ring.
        /// </summary>
        public Range<int> DensityRange { get; set; }

        /// <summary>
        /// Range of scales per texture.  Set to range to 1,1 for default.
        /// </summary>
        public Range<float> ScaleRange { get; set; }

        /// <summary>
        /// Random jitter from ring radius.
        /// </summary>
        public float RadiusOffsetJitter { get; set; }
    }
}
