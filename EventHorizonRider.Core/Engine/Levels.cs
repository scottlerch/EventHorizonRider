using System;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class Levels
    {
        private readonly RingInfoFactory ringInfoFactory;

        public Levels(RingInfoFactory ringInfoFactory)
        {
            this.ringInfoFactory = ringInfoFactory;
        }

        public Level GetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return new Level
                    {
                        RingSpeed = 125,
                        RingInterval = TimeSpan.FromSeconds(2),
                        Sequence = ringInfoFactory.GetRandomSequence(
                            iterations:10, 
                            gapSize: Range.Create(MathHelper.TwoPi / 3.5f), 
                            type: RingType.Dust),
                    };
                case 2:
                    return new Level
                    {
                        RingSpeed = 150,
                        RingInterval = TimeSpan.FromSeconds(1.5),
                        Sequence = Enumerable.Empty<RingInfo>().Concat(
                            ringInfoFactory.GetStepSequence(
                                numberOfSteps: 5, 
                                gapSize: MathHelper.TwoPi / 4f,
                                type: RingType.All,
                                typeSelection: RingTypeSelection.RoundRobin),
                            ringInfoFactory.GetStepSequence(
                                numberOfSteps: 5, 
                                gapSize: MathHelper.TwoPi / 4f, 
                                type: RingType.All, 
                                typeSelection: RingTypeSelection.RoundRobin)),
                    };
                case 3:
                    return new Level
                    {
                        RingSpeed = 175,
                        RingInterval = TimeSpan.FromSeconds(1),
                        Sequence = Enumerable.Empty<RingInfo>().Concat(
                            ringInfoFactory.GetZigZagSequence(
                                iterations:15, 
                                gapSize: MathHelper.TwoPi / 5f, 
                                type: RingType.All,
                                typeSelection: RingTypeSelection.RoundRobin)),
                    };
                case 4:
                    return new Level
                    {
                        RingSpeed = 200,
                        RingInterval = TimeSpan.FromSeconds(0.75),
                        Sequence = ringInfoFactory.GetRandomSequence(
                            iterations:20, 
                            gapSize: Range.Create(MathHelper.TwoPi / 4), 
                            type: RingType.Asteroid),
                    };
                default:
                    return new Level
                    {
                        RingSpeed = 250,
                        RingInterval = TimeSpan.FromSeconds(0.5),
                        Sequence = ringInfoFactory.GetRandomSequence(),
                    };
            }
        }
    }
}