using System;
using System.Collections.Generic;
using System.Linq;
using EventHorizonRider.Core.Components.SpaceComponents;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class Level
    {
        private readonly IEnumerable<RingInfo> internalSequence; 

        public Level(TimeSpan ringInterval, float ringSeparation, float shipSpeed, Color color, bool infiniteSequence, IEnumerable<RingInfo> sequence)
        {
            Color = color;
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

                for (int i = 0; i < list.Count; i++)
                {
                    Duration += ringInterval + TimeSpan.FromSeconds(list[i].SpiralRadius / RingSpeed);

                    if (i == 0)
                    {
                        Duration += TimeSpan.FromSeconds(RingFactory.StartRadius/RingSpeed);
                    }
                }
            }
        }

        public Color Color { get; private set; }

        public TimeSpan? Duration { get; private set; }

        public bool IsInfiniteSequence { get; private set; }

        public float RingSpeed { get; private set; }

        public IEnumerable<RingInfo> Sequence { get { return internalSequence; } }

        public TimeSpan RingInterval { get; private set; }

        public float RingSeparation { get; private set; }

        public float ShipSpeed { get; private set; }
    }
}