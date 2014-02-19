using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core
{
    internal class RingFactory
    {
        private Texture2D ringEdgeTexture;
        private Random rand = new Random();
        private GraphicsDevice graphicsDevice;
        private float startRadius;

        internal void LoadContent(GraphicsDevice graphicsDevice)
        {
            const int ringSegmentWidth = 27;
            const int ringSegmentHeight = 45;

            this.graphicsDevice = graphicsDevice;

            var ringColor = Color.DarkGray.AdjustLight(0.5f).PackedValue;

            ringEdgeTexture = new Texture2D(graphicsDevice, ringSegmentWidth, ringSegmentHeight, false, SurfaceFormat.Color);
            ringEdgeTexture.SetData(Enumerable.Range(0, ringEdgeTexture.Width * ringEdgeTexture.Height).Select(i => ringColor).ToArray());

            // TODO: calculate from viewport
            startRadius = 700;
        }

        internal Ring Create(RingInfo info)
        {
            return new Ring
            {
                RotationalVelocity = info.RotationalVelocity,
                Texture = ringEdgeTexture,
                Radius = startRadius,
                Origin = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2),
                Gaps = Enumerable.Range(0, info.NumberOfGaps).Select(i =>
                    new RingGap 
                    {
                        GapAngle = MathHelper.WrapAngle(((float)i * (MathHelper.TwoPi / (float)info.NumberOfGaps)) + info.Angle),
                        GapSize = info.GapSize,
                    }).ToList(),
            };
        }
    }
}
