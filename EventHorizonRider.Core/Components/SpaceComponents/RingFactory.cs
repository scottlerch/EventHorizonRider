using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingFactory
    {
        private Texture2D ringEdgeTexture;
        private float startRadius;
        private Vector2 viewportCenter;

        internal void LoadContent(GraphicsDevice graphics)
        {
            const int ringSegmentWidth = 27;
            const int ringSegmentHeight = 45;

            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            var ringColor = Color.DarkGray.AdjustLight(0.5f).PackedValue;

            ringEdgeTexture = new Texture2D(graphics, ringSegmentWidth, ringSegmentHeight, false,
                SurfaceFormat.Color);
            ringEdgeTexture.SetData(
                Enumerable.Range(0, ringEdgeTexture.Width*ringEdgeTexture.Height).Select(i => ringColor).ToArray());

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
                Origin = viewportCenter,
                Gaps = Enumerable.Range(0, info.NumberOfGaps).Select(i =>
                    new RingGap
                    {
                        GapAngle = MathHelper.WrapAngle((i*(MathHelper.TwoPi/info.NumberOfGaps)) + info.Angle),
                        GapSize = info.GapSize,
                    }).ToList(),
            };
        }
    }
}