using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Engine
{
    internal class Level
    {
        public Level(TimeSpan ringInterval, float ringSeparation, float shipSpeed, IEnumerable<RingInfo> sequence)
        {
            RingInterval = ringInterval;
            ShipSpeed = shipSpeed;
            Sequence = sequence;
            RingSeparation = ringSeparation;
            RingSpeed = ringSeparation/(float)ringInterval.TotalSeconds;
        }

        public float RingSpeed { get; private set; }

        public IEnumerable<RingInfo> Sequence { get; private set; }

        public TimeSpan RingInterval { get; private set; }

        public float RingSeparation { get; private set; }

        public float ShipSpeed { get; private set; }
    }
}