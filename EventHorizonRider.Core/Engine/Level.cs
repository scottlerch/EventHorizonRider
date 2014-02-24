using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Engine
{
    internal class Level
    {
        public float RingSpeed { get; set; }

        public IEnumerable<RingInfo> Sequence { get; set; }

        public TimeSpan RingInterval { get; set; }
    }
}