﻿using System;
using System.Collections.Generic;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class LevelCollection
    {
        private readonly float maxRingRadius = DeviceInfo.LogicalWidth / 2f;
        private readonly List<Level> levels;

        public IEnumerable<Level> Levels { get { return levels; } }
 
        public LevelCollection(RingInfoFactory ringInfoFactory)
        {
            levels = new List<Level>
            {
                new Level(
                    shipSpeed: MathHelper.TwoPi*0.8f,
                    ringSeparation: maxRingRadius/1.5f,
                    ringInterval: TimeSpan.FromSeconds(2),
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetRandomSequence(
                            iterations: 4,
                            numberOfGaps: Range.Create(2),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.Dust,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.Dust),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 5,
                            numberOfGaps: Range.Create(2),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.DustWithAsteroid),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.Dust),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 5,
                            numberOfGaps: Range.Create(2),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.Asteroid),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 5,
                            numberOfGaps: Range.Create(1),
                            gapSize: Range.Create(MathHelper.TwoPi/5),
                            type: RingType.Asteroid))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*0.9f,
                    ringSeparation: maxRingRadius/2f,
                    ringInterval: TimeSpan.FromSeconds(1.5),
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetStepSequence(
                            numberOfSteps: 4,
                            gapSize: MathHelper.TwoPi/4f,
                            type: RingType.Dust | RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.DustWithAsteroid),
                        ringInfoFactory.GetStepSequence(
                            numberOfSteps: 4,
                            gapSize: MathHelper.TwoPi/4f,
                            type: RingType.Dust | RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.1f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(1),
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 15,
                            gapSize: MathHelper.TwoPi/4f,
                            type: RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.12f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(0.75),
                    infiniteSequence: false,
                    sequence: ringInfoFactory.GetRandomSequence(
                        iterations: 20,
                        gapSize: Range.Create(MathHelper.TwoPi/4),
                        type: RingType.Asteroid)),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.14f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(0.5),
                    infiniteSequence: true,
                    sequence: ringInfoFactory.GetRandomSequence(
                        gapSize: Range.Create(MathHelper.TwoPi/3f, MathHelper.TwoPi/4f)))
            };
        }

        public static int NumberOfLevels { get { return 5; } }

        public Level GetLevel(int level)
        {
            return levels[level - 1];
        }
    }
}