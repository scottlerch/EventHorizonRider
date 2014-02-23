using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core
{
    internal class RingInfoFactory
    {
        private float DefaultRotationVelocity = MathHelper.TwoPi / 32;

        private Random rand = new Random();

        public IEnumerable<RingInfo> GetRandomSequence(int iterations = -1, int numberOfGaps = -1, float gapSize = -1f)
        {
            for (int i = 0; i < iterations || iterations == -1; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize < 0f? rand.Next(4, 10) : MathHelper.TwoPi / 4f,
                    NumberOfGaps = numberOfGaps < 1? rand.Next(1, 5) : numberOfGaps,
                    Angle = (float)rand.NextDouble() * MathHelper.TwoPi,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetStepSequence(int numberOfSteps, float gapSize)
        {
            var angleStep = MathHelper.TwoPi / (float)numberOfSteps;

            for (int i = 0; i < numberOfSteps; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = angleStep * i,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetZigZagSequence(int iterations, float gapSize)
        {
            var baseAngle = (float)rand.NextDouble() * MathHelper.TwoPi;
            var angleStep = MathHelper.TwoPi / 4;

            for (int i = 0; i < iterations; i++)
            {
                yield return new RingInfo
                {
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = baseAngle + ((i % 2) * angleStep),
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }
    }
}
