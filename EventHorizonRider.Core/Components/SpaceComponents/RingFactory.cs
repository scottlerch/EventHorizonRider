using EventHorizonRider.Core.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingFactory
    {
        public const int SegmentsCount = 3;

        private Texture2D[] ringEdgeTextures;
        private float startRadius;
        private Vector2 viewportCenter;

        internal void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            ringEdgeTextures = new Texture2D[SegmentsCount];
            for (int i = 0; i < ringEdgeTextures.Length; i++)
            {
                ringEdgeTextures[i] = content.Load<Texture2D>(@"Images\ring_segment" + (i + 3));
            }

            // TODO: calculate from viewport
            startRadius = 700;
        }

        internal Ring Create(RingInfo info)
        {
            return new Ring
            {
                RotationalVelocity = info.RotationalVelocity,
                Textures = ringEdgeTextures,
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