using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    /// <summary>
    /// Keeps track of individual ring object states within Ring class.
    /// This could be an individual asteroid, dust particle, etc.
    /// </summary>
    internal class RingObject : ISpriteInfo
    {
        public float RelativeDepth { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D ShadowTexture { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Position { get; private set; }
        public Color Color { get; set; }
        public float ColorLightness { get; set; }

        public float Angle { get; set; }
        public CollisionInfo CollisionInfo { get; set; }

        public float RotationRate { get; set; }

        public float RadiusOffset { get; set; }

        public void UpdatePosition(Vector2 ringOrigin, float ringRadius)
        {
            Position = new Vector2(
                ringOrigin.X + ((float)Math.Sin(Angle) * (ringRadius + RadiusOffset)),
                ringOrigin.Y - ((float)Math.Cos(Angle) * (ringRadius + RadiusOffset)));
        }
    }
}
