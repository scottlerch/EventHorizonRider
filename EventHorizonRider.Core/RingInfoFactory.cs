using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    internal class RingInfoFactory
    {
        private Random rand = new Random();

        public IEnumerable<RingInfo> GetRandomSequence(int iterations = -1)
        {
            for (int i = 0; i < iterations || iterations == -1; i++)
            {
                yield return new RingInfo
                {
                    GapSize = MathHelper.TwoPi / 4,
                    NumberOfGaps = 1,
                    Angle = (float)rand.NextDouble() * MathHelper.TwoPi,
                };
            }
        }

        public IEnumerable<RingInfo> GetStepSequence(int numberOfSteps)
        {
            var angleStep = MathHelper.TwoPi / (float)numberOfSteps;

            for (int i = 0; i < numberOfSteps; i++)
            {
                yield return new RingInfo
                {
                    GapSize = MathHelper.TwoPi / 4,
                    NumberOfGaps = 1,
                    Angle = angleStep * i,
                };
            }
        }

        public IEnumerable<RingInfo> GetZigZagSequence(int iterations)
        {
            var baseAngle = (float)rand.NextDouble() * MathHelper.TwoPi;
            var angleStep = MathHelper.TwoPi / 4;

            for (int i = 0; i < iterations; i++)
            {
                yield return new RingInfo
                {
                    GapSize = MathHelper.TwoPi / 4,
                    NumberOfGaps = 1,
                    Angle = baseAngle + ((i % 2) * angleStep),
                };
            }
        }
    }
}
