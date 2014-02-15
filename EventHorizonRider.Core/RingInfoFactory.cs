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
        public IEnumerable<RingInfo> GetRandomSequence()
        {
            var rand = new Random();

            while (true)
            {
                yield return new RingInfo
                {
                    GapSize = MathHelper.TwoPi / 3,
                    NumberOfGaps = 1,
                    Angle = (float)rand.NextDouble() * MathHelper.TwoPi,
                };
            }
        }
    }
}
