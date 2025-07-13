using EventHorizonRider.Core.Components.SpaceComponents.Rings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Engine;

internal class Level
{
    public const float DefaultRotationalVelocity = MathHelper.TwoPi/32f;

    private readonly IEnumerable<RingInfo> internalSequence; 

    public Level(
        TimeSpan ringInterval, 
        float ringSeparation, 
        float shipSpeed, 
        float rotationVelocity, 
        Color color, 
        bool infiniteSequence, 
        IEnumerable<RingInfo> sequence)
    {
        Color = color;
        RingInterval = ringInterval;
        ShipSpeed = shipSpeed;
        RotationalVelocity = rotationVelocity;
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
            internalSequence = [.. sequence];
            Duration = TimeSpan.Zero;

            bool isFirst = true;

            foreach (var ring in internalSequence)
            {
                Duration += ringInterval + TimeSpan.FromSeconds(Math.Abs(ring.SpiralRadius) / RingSpeed);

                if (isFirst)
                {
                    Duration += TimeSpan.FromSeconds(RingFactory.StartRadius / RingSpeed);
                    isFirst = false;
                }
            }
        }
    }

    public Color Color { get; set; }

    public TimeSpan? Duration { get; set; }

    public bool IsInfiniteSequence { get; private set; }

    public float RingSpeed { get; set; }

    public IEnumerable<RingInfo> Sequence { get { return internalSequence; } }

    public TimeSpan RingInterval { get; set; }

    public float RingSeparation { get; set; }

    public float ShipSpeed { get; set; }

    public float RotationalVelocity { get; set; }
}