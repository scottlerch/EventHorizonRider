using System;
using System.Collections.Generic;
using System.Linq;
using EventHorizonRider.Core.Components.SpaceComponents;

namespace EventHorizonRider.Core.Engine
{
    internal class Level
    {
        private readonly IEnumerable<RingInfo> internalSequence; 

        public Level(TimeSpan ringInterval, float ringSeparation, float shipSpeed, bool infiniteSequence, IEnumerable<RingInfo> sequence)
        {
            RingInterval = ringInterval;
            ShipSpeed = shipSpeed;
            RingSeparation = ringSeparation;
            RingSpeed = ringSeparation/(float)ringInterval.TotalSeconds;
            IsInfiniteSequence = infiniteSequence;

            if (infiniteSequence)
            {
                internalSequence = sequence;
                Duration = null;
            }
            else
            {
                internalSequence = sequence.ToList();
                var list = internalSequence as List<RingInfo>;
                Duration = TimeSpan.Zero;

                for (int i = 0; i < list.Count - 1; i++)
                {
                    Duration += ringInterval;
                }
            }
        }

        public TimeSpan? Duration { get; private set; }

        public bool IsInfiniteSequence { get; private set; }

        public float RingSpeed { get; private set; }

        public IEnumerable<RingInfo> Sequence { get { return internalSequence; } }

        public TimeSpan RingInterval { get; private set; }

        public float RingSeparation { get; private set; }

        public float ShipSpeed { get; private set; }
    }
}