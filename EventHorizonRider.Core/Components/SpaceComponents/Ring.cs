using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Ring : ComponentBase
    {
        public List<RingGap> Gaps;

        public Vector2 Origin;

        private float radius;
        private float maxRadius;

        public float Radius
        {
            get { return radius; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (radius == 0)
                {
                    maxRadius = value;
                }

                radius = value;
            }
        }

        public float RotationalVelocity;
        public Texture2D[] Textures;

        private bool isStopped;

        private float rotationalOffset;
        public bool ConsumedByBlackhole { get; set; }

        private static readonly Color[] SegmentColors =
        {
            Color.DarkGray,
            Color.LightGray,
            Color.DarkGray.AdjustLight(0.7f)
        };

        private static readonly int[] RandomColorIndex;
        private static readonly int[] RandomTextureIndex;

        static Ring()
        {
            // Precompute random but deterministic indices for performance

            const int maxCount = 200;
            var random = new Random();

            RandomColorIndex = new int[maxCount];
            RandomTextureIndex = new int[maxCount];

            for (int i = 0; i < maxCount; i++)
            {
                RandomColorIndex[i] = random.Next(0, SegmentColors.Length);
                RandomTextureIndex[i] = random.Next(0, RingFactory.SegmentsCount);
            }
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            var scale = 1f - (Radius/maxRadius);

            const float low = 0.2f;
            const float high = 0.5f;
            const float diff = high - low;

            var randomIndex = 0;

            for (var i = -MathHelper.Pi; i < MathHelper.Pi; i += 0.04f)
            {
                var angle = i + rotationalOffset;

                if (Gaps.Any(gap => gap.IsInsideGap(i)))
                    continue;

                var currentColor = SegmentColors[RandomColorIndex[randomIndex]].AdjustLight((scale * diff) + low);

                var texture = Textures[RandomTextureIndex[randomIndex]];
                spriteBatch.Draw(texture, new Vector2(
                    Origin.X + ((float)Math.Sin(angle) * Radius),
                    Origin.Y - ((float)Math.Cos(angle) * Radius)),
                    origin: new Vector2(texture.Width / 2f, texture.Height / 2f),
                    color: currentColor,
                    rotation: angle,
                    depth: Depth);

                randomIndex++;
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (!isStopped)
            {
                rotationalOffset += RotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        internal bool Intersects(Ship ship)
        {
            var intersects = false;

            if (IsInsideRing(ship))
            {
                intersects = !Gaps.Any(gap => IsInsideGap(ship, gap));
            }

            return intersects;
        }

        private bool IsInsideRing(Ship ship)
        {
            var ringWidth = Textures[0].Height;

            var startRingEdge = Radius - (ringWidth / 2f);
            var endRingEdge = Radius + (ringWidth / 2f);

            var shipFrontEdge = (Origin - ship.Position).Length() + (ship.Texture.Height / 2f);

            return shipFrontEdge.IsBetween(startRingEdge, endRingEdge);
        }

        private bool IsInsideGap(Ship ship, RingGap ringGap)
        {
            var gapEdges = GetGapEdges(ship, ringGap);

            return ship.Rotation.IsBetweenAngles(gapEdges.Start, gapEdges.End);
        }

        private Range<float> GetGapEdges(Ship ship, RingGap ringGap)
        {
            var textureOffset = (float)Math.Asin((Textures[0].Width / 2f) / ship.Radius);

            var halfGap = (ringGap.GapSize / 2f) - textureOffset;

            var gapStartAngle = (ringGap.GapAngle + rotationalOffset) - halfGap;
            var gapEndAngle = (ringGap.GapAngle + rotationalOffset) + halfGap;

            return new Range<float>
            {
                Start = MathHelper.WrapAngle(gapStartAngle),
                End = MathHelper.WrapAngle(gapEndAngle)
            };
        }

        internal void Stop()
        {
            isStopped = true;
        }
    }
}