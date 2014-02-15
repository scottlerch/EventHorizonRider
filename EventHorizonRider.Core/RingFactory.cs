using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class RingFactory
    {
        private Texture2D ringEdgeTexture;
        private Random rand = new Random();
        private GraphicsDevice graphicsDevice;

        internal void LoadContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            var ringColor = Color.DarkGray.AdjustLight(0.5f).PackedValue;

            ringEdgeTexture = new Texture2D(graphicsDevice, 16, 25, false, SurfaceFormat.Color);
            ringEdgeTexture.SetData(Enumerable.Range(0, ringEdgeTexture.Width * ringEdgeTexture.Height).Select(i => ringColor).ToArray());
        }

        internal Ring Create(RingInfo info)
        {
            return new Ring
            {
                Texture = ringEdgeTexture,
                Radius = 768,
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
