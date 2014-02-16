using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    internal class Levels
    {
        private RingInfoFactory ringInfoFactory = new RingInfoFactory();

        public Level GetLevelOne()
        {
            return new Level
            {
                RingSpeed = 150,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(10),
                    ringInfoFactory.GetZigZagSequence(10),
                    ringInfoFactory.GetRandomSequence()),
            };
        }
    }
}
