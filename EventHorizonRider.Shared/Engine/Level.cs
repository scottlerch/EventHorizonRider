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
            internalSequence = sequence.ToList();
            var list = internalSequence as List<RingInfo>;
            Duration = TimeSpan.Zero;

            // ReSharper disable once PossibleNullReferenceException
            for (int i = 0; i < list.Count; i++)
            {
                Duration += ringInterval + TimeSpan.FromSeconds(Math.Abs(list[i].SpiralRadius) / RingSpeed);

                if (i == 0)
                {
                    Duration += TimeSpan.FromSeconds(RingFactory.StartRadius/RingSpeed);
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