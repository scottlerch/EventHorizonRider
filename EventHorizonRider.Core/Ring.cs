using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core
{
    internal class Ring
    {
        public List<RingGap> Gaps;

        public float Radius;
        public Texture2D Texture;
        public Vector2 Origin;

        public void Draw(SpriteBatch spriteBatch)
        {
            for (float i = -MathHelper.Pi; i < MathHelper.Pi; i += 0.03f)
            {
                if (Gaps.Any(gap => gap.IsInsideGap(i)))
                    continue;

                spriteBatch.Draw(Texture,
                    position: new Vector2(
                        Origin.X + ((float)Math.Sin(i) * Radius),
                        Origin.Y - ((float)Math.Cos(i) * Radius)),
                    origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                    rotation: i);
            }
        }

        internal bool Intersects(Ship ship)
        {
            if (IsInsideRing(ship))
            {
                return !Gaps.Any(gap => IsInsideGap(ship, gap));
            }

            return false;
        }

        private bool IsInsideRing(Ship ship)
        {
            var ringWidth = Texture.Height;

            var startRingEdge = Radius - (ringWidth / 2f);
            var endRingEdge = Radius + (ringWidth / 2f);

            var shipFrontEdge = (Origin - ship.Position).Length() + (ship.Texture.Height / 2);

            return shipFrontEdge.IsBetween(startRingEdge, endRingEdge);
        }

        private bool IsInsideGap(Ship ship, RingGap ringGap)
        {
            var gapEdges = GetGapEdges(ship, ringGap);

            return ship.Rotation.IsBetweenAngles(gapEdges.Start, gapEdges.End);
        }

        private Range<float> GetGapEdges(Ship ship, RingGap ringGap)
        {
            var textureOffset = (float)Math.Asin((Texture.Width / 2) / ship.Radius);

            var halfGap = (ringGap.GapSize / 2f) - textureOffset;

            var gapStartAngle = ringGap.GapAngle - halfGap;
            var gapEndAngle = ringGap.GapAngle + halfGap;

            return new Range<float> 
            { 
                Start = MathHelper.WrapAngle(gapStartAngle), 
                End = MathHelper.WrapAngle(gapEndAngle) 
            };
        }

        internal void ClampToNearestGapEdge(Ship ship)
        {
            if (IsInsideRing(ship))
            {
                // TODO:
            }
        }
    }
}
