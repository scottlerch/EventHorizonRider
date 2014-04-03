﻿using System;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class Levels
    {
        private readonly RingInfoFactory ringInfoFactory;
        private const float MaxRingRadius = 1136f / 2f;

        public Levels(RingInfoFactory ringInfoFactory)
        {
            this.ringInfoFactory = ringInfoFactory;
        }

        public Level GetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return new Level(
                        shipSpeed:MathHelper.TwoPi * 0.9f,
                        ringSeparation: MaxRingRadius / 2.5f,
                        ringInterval: TimeSpan.FromSeconds(2),
                        sequence:  ringInfoFactory.GetRandomSequence(
                            iterations:10, 
                            gapSize: Range.Create(MathHelper.TwoPi / 3.5f), 
                            type: RingType.Dust));
                case 2:
                    return new Level(
                        shipSpeed: MathHelper.TwoPi * 1f,
                        ringSeparation: MaxRingRadius / 3,
                        ringInterval: TimeSpan.FromSeconds(1.5),
                        sequence:  Enumerable.Empty<RingInfo>().Concat(
                            ringInfoFactory.GetStepSequence(
                                numberOfSteps: 4,
                                gapSize: MathHelper.TwoPi/4f,
                                type: RingType.Dust | RingType.DustWithAsteroid,
                                typeSelection: RingTypeSelection.RoundRobin),
                            ringInfoFactory.GetStepSequence(
                                numberOfSteps: 4,
                                gapSize: MathHelper.TwoPi/4f,
                                type: RingType.Dust | RingType.DustWithAsteroid,
                                typeSelection: RingTypeSelection.RoundRobin)));
                case 3:
                    return new Level(
                        shipSpeed: MathHelper.TwoPi * 1.1f,
                        ringSeparation: MaxRingRadius / 3,
                        ringInterval: TimeSpan.FromSeconds(1),
                        sequence:  Enumerable.Empty<RingInfo>().Concat(
                            ringInfoFactory.GetZigZagSequence(
                                iterations:15, 
                                gapSize: MathHelper.TwoPi / 4f,
                                type: RingType.DustWithAsteroid,
                                typeSelection: RingTypeSelection.RoundRobin)));
                case 4:
                    return new Level(
                        shipSpeed: MathHelper.TwoPi * 1.12f,
                        ringSeparation: MaxRingRadius / 3,
                        ringInterval: TimeSpan.FromSeconds(0.75),
                        sequence:  ringInfoFactory.GetRandomSequence(
                            iterations: 20,
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.Asteroid));
                default:
                    return new Level(
                        shipSpeed: MathHelper.TwoPi * 1.14f,
                        ringSeparation: MaxRingRadius / 3,
                        ringInterval: TimeSpan.FromSeconds(0.5),
                        sequence:  ringInfoFactory.GetRandomSequence(
                            gapSize: Range.Create(MathHelper.TwoPi / 3f, MathHelper.TwoPi / 4f)));
            }
        }
    }
}