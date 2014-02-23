using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class Levels
    {
        private RingInfoFactory ringInfoFactory = new RingInfoFactory();

        public Level GetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return GetLevelOne();
                    //return GetTestLevel();
                case 2:
                    return GetLevelTwo();
                case 3:
                    return GetLevelThree();
                case 4:
                    return GetLevelFour();
                default:
                    return GetInfiniteLevel();
            }
        }

        public Level GetTestLevel()
        {
            return new Level
            {
                RingSpeed = 250,
                RingInterval = TimeSpan.FromSeconds(4),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(numberOfSteps: 10, gapSize: MathHelper.TwoPi / 4f),
                    ringInfoFactory.GetZigZagSequence(iterations: 10, gapSize: MathHelper.TwoPi / 4f)),
            };
        }

        public Level GetLevelOne()
        {
            return new Level
            {
                RingSpeed = 200,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(numberOfSteps:10, gapSize:MathHelper.TwoPi / 4f),
                    ringInfoFactory.GetZigZagSequence(iterations: 10, gapSize: MathHelper.TwoPi / 4f)),
            };
        }

        public Level GetLevelTwo()
        {
            return new Level
            {
                RingSpeed = 250,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(numberOfSteps: 5, gapSize: MathHelper.TwoPi / 5f),
                    ringInfoFactory.GetZigZagSequence(iterations: 10, gapSize: MathHelper.TwoPi / 5f)),
            };
        }

        public Level GetLevelThree()
        {
            return new Level
            {
                RingSpeed = 275,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(numberOfSteps: 3, gapSize: MathHelper.TwoPi / 5f),
                    ringInfoFactory.GetZigZagSequence(iterations: 10, gapSize: MathHelper.TwoPi / 5f)),
            };
        }

        public Level GetLevelFour()
        {
            return new Level
            {
                RingSpeed = 300,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = ringInfoFactory.GetRandomSequence(iterations: 10, gapSize: MathHelper.TwoPi / 4),
            };
        }

        public Level GetInfiniteLevel()
        {
            return new Level
            {
                RingSpeed = 325,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = ringInfoFactory.GetRandomSequence(),
            };
        }
    }
}
