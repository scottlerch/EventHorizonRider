using EventHorizonRider.Core.Engine.States;
using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Engine
{
    internal class LevelCollection
    {
        private readonly float maxRingRadius = DeviceInfo.LogicalWidth / 2f;
        private readonly List<Level> levels;

        public LevelCollection(RingInfoFactory ringInfoFactory)
        {
            CurrentLevelNumber = 1;

            levels = new List<Level>
            {
                new Level(
                    shipSpeed: MathHelper.TwoPi*0.8f,
                    rotationVelocity: MathHelper.TwoPi / 32f,
                    ringSeparation: maxRingRadius/1.5f,
                    ringInterval: TimeSpan.FromSeconds(2),
                    color: Color.White,
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetRandomSequence(
                            iterations: 2,
                            numberOfGaps: Range.Create(2),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.Dust,
                            typeSelection: RingTypeSelection.RoundRobin),
                       ringInfoFactory.GetRandomSequence(
                            iterations: 2,
                            numberOfGaps: Range.Create(2),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                       ringInfoFactory.GetRandomSequence(
                            iterations: 3,
                            numberOfGaps: Range.Create(1),
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
                            gapSize: Range.Create(MathHelper.TwoPi/4.5f),
                            type: RingType.Asteroid))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*0.9f,
                    rotationVelocity: MathHelper.TwoPi / 28f,
                    ringSeparation: maxRingRadius/2f,
                    ringInterval: TimeSpan.FromSeconds(1.5),
                    color: Color.DarkSalmon,
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetStepSequence(
                            numberOfSteps: 4,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Dust | RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi*0.2f,
                            angleStep: MathHelper.TwoPi*0.5f,
                            type: RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 4,
                            numberOfGaps: Range.Create(3),
                            gapSize: Range.Create(MathHelper.TwoPi*0.2f),
                            type: RingType.Asteroid),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.DustWithAsteroid),
                        ringInfoFactory.GetStepSequence(
                            numberOfSteps: 4,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Dust | RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 2,
                            numberOfGaps: Range.Create(3),
                            gapSize: Range.Create(MathHelper.TwoPi*0.19f),
                            type: RingType.Asteroid),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Asteroid | RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.Asteroid),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Asteroid),
                        ringInfoFactory.GetStepSequence(
                            numberOfSteps: 4,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Asteroid,
                            typeSelection: RingTypeSelection.RoundRobin))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.1f,
                    rotationVelocity: MathHelper.TwoPi / 24f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(1),
                    infiniteSequence: false,
                    color: Color.Green,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 7,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.DustWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 7,
                            gapSize: MathHelper.TwoPi*0.25f,
                            type: RingType.Asteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi*0.2f,
                            angleStep: MathHelper.TwoPi*0.5f,
                            type: RingType.DustWithAsteroid | RingType.Asteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetSpirals(
                            iterations: 1,
                            spiralRadius: 500f,
                            type: RingType.DustWithAsteroid),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 3,
                            numberOfGaps: Range.Create(1),
                            gapSize: Range.Create(MathHelper.TwoPi/4),
                            type: RingType.DustWithAsteroid),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 4,
                            numberOfGaps: Range.Create(1, 3),
                            gapSize: Range.Create(MathHelper.TwoPi*0.2f),
                            type: RingType.Asteroid),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 3,
                            numberOfGaps: Range.Create(1),
                            gapSize: Range.Create(MathHelper.TwoPi*0.28f),
                            type: RingType.IceCrystalsWithAsteroid),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 3,
                            numberOfGaps: Range.Create(1),
                            gapSize: Range.Create(MathHelper.TwoPi*0.2f),
                            type: RingType.IceCrystals))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.12f,
                    rotationVelocity: MathHelper.TwoPi / 20f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(0.75),
                    color: Color.Blue,
                    infiniteSequence: false,
                    sequence: Enumerable.Empty<RingInfo>().Concat(
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi/4f,
                            type: RingType.IceCrystals,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 3,
                            gapSize: MathHelper.TwoPi/4f,
                            type: RingType.IceCrystalsWithAsteroid,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetZigZagSequence(
                            iterations: 4,
                            gapSize: MathHelper.TwoPi*0.3f,
                            angleStep:  MathHelper.TwoPi*0.5f,
                            type: RingType.IceCrystals,
                            typeSelection: RingTypeSelection.RoundRobin),
                        ringInfoFactory.GetRandomSequence(
                            iterations: 40,
                            gapSize: Range.Create(MathHelper.TwoPi*0.18f, MathHelper.TwoPi*0.25f),
                            type: RingType.Asteroid))),
                new Level(
                    shipSpeed: MathHelper.TwoPi*1.14f,
                    rotationVelocity: MathHelper.TwoPi / 16f,
                    ringSeparation: maxRingRadius/3,
                    ringInterval: TimeSpan.FromSeconds(0.5),
                    color: Color.Red,
                    infiniteSequence: true,
                    sequence: ringInfoFactory.GetRandomSequence(
                        gapSize: Range.Create(MathHelper.TwoPi/3f, MathHelper.TwoPi/4f))),
            };
        }

        public int CurrentLevelNumber { get; private set; }

        public Level CurrentLevel { get { return GetLevel(CurrentLevelNumber); } }

        public Level NextLevel { get { return GetLevel(CurrentLevelNumber + 1); } }

        public IEnumerable<Level> Levels { get { return levels; } }

        public int NumberOfLevels { get { return levels.Count; } }

        public void SetCurrentLevel(int currentLevelNumber)
        {
            if (currentLevelNumber < 1 || currentLevelNumber >= levels.Count)
            {
                throw new ArgumentException("Invalid level number");
            }

            CurrentLevelNumber = currentLevelNumber;
        }

        public void IncrementCurrentLevel()
        {
            SetCurrentLevel(CurrentLevelNumber + 1);
        }

        public Level GetLevel(int level)
        {
            return level < levels.Count ? levels[level - 1] : levels[levels.Count - 1];
        }

        public TimeSpan GetCurrentLevelStartTime()
        {
            return GetLevelStartTime(CurrentLevelNumber);
        }

        public TimeSpan GetLevelStartTime(int levelNumber)
        {
            return levels.Take(levelNumber - 1)
                .Aggregate(
                    TimeSpan.Zero,
                    (duration, level) =>
                        duration + (level.Duration.HasValue ?
                            level.Duration.Value + RunningState.WaitBetweenLevels :
                            TimeSpan.Zero));
        }
    }
}