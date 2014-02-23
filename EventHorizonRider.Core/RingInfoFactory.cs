using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core
{
    internal class RingInfoFactory
    {
        private readonly Random rand = new Random();
        private const float DefaultRotationVelocity = MathHelper.TwoPi/32;

        public IEnumerable<RingInfo> GetRandomSequence(int iterations = -1, int numberOfGaps = -1, float gapSize = -1f)
        {
            for (var i = 0; i < iterations || iterations == -1; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize < 0f ? rand.Next(4, 10) : MathHelper.TwoPi/4f,
                    NumberOfGaps = numberOfGaps < 1 ? rand.Next(1, 5) : numberOfGaps,
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