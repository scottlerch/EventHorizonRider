using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings
{
    internal class RingTexturesInfo
    {
        public Texture2D[] Textures { get; set; }

        public Texture2D[] ShadowTextures { get; set; }

        public CollisionInfo[] CollisionInfos { get; set; }

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

        /// <summary>
        /// Percent jitter of angle spacing.  Set to 0 for no jitter and 1 for full.
        /// </summary>
        public float AngleJitter { get; set; }

        /// <summary>
        /// Number of objects to taper in size at edges.
        /// </summary>
        public int TaperAmount { get; set; }

        /// <summary>
        /// Amount to scale tapered edge by.
        /// </summary>
        public float TaperScale { get; set; }

        /// <summary>
        /// Determine if shadows are merged into one set.  This is useful if all objects in 
        /// ring are supposed to be merged like a dust cloud.
        /// </summary>
        public bool MergeShadows { get; set; }

        /// <summary>
        /// Offset for shadow texture.
        /// </summary>
        public Vector2 ShadowOffset { get; set; }
    }
}
