using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class RingInfoFactory
    {
        private readonly Random rand = new Random();
        public const float DefaultRotationVelocity = MathHelper.TwoPi/32;

        public IEnumerable<RingInfo> GetRandomSequence(int iterations = -1, int numberOfGaps = -1, float gapSize = -1f)
        {
            for (var i = 0; i < iterations || iterations == -1; i++)
            {
                var currentGapSize = gapSize < 0f ? MathHelper.TwoPi / (float)rand.Next(4, 9) : gapSize;
                var currentNumberOfGaps = numberOfGaps < 0f ? rand.Next(1, (int)(MathHelper.TwoPi / currentGapSize)) : numberOfGaps;

                yield return new RingInfo
                {
                    GapSize = currentGapSize,
                    NumberOfGaps = currentNumberOfGaps,
                    Angle = (float) rand.NextDouble()*MathHelper.TwoPi,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetStepSequence(int numberOfSteps, float gapSize)
        {
            var angleStep = MathHelper.TwoPi/numberOfSteps;

            for (var i = 0; i < numberOfSteps; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = angleStep*i,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetZigZagSequence(int iterations, float gapSize)
        {
            var baseAngle = (float) rand.NextDouble()*MathHelper.TwoPi;
            const float angleStep = MathHelper.TwoPi/4;

            for (var i = 0; i < iterations; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = baseAngle + ((i%2)*angleStep),
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }
    }
}