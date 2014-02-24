using System;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class Levels
    {
        private readonly RingInfoFactory ringInfoFactory = new RingInfoFactory();

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
                    ringInfoFactory.GetStepSequence(10, MathHelper.TwoPi/4f),
                    ringInfoFactory.GetZigZagSequence(10, MathHelper.TwoPi/4f)),
            };
        }

        public Level GetLevelOne()
        {
            return new Level
            {
                RingSpeed = 200,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(10, MathHelper.TwoPi/4f),
                    ringInfoFactory.GetZigZagSequence(10, MathHelper.TwoPi/4f)),
            };
        }

        public Level GetLevelTwo()
        {
            return new Level
            {
                RingSpeed = 250,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(5, MathHelper.TwoPi/5f),
                    ringInfoFactory.GetZigZagSequence(10, MathHelper.TwoPi/5f)),
            };
        }

        public Level GetLevelThree()
        {
            return new Level
            {
                RingSpeed = 275,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(3, MathHelper.TwoPi/5f),
                    ringInfoFactory.GetZigZagSequence(10, MathHelper.TwoPi/5f)),
            };
        }

        public Level GetLevelFour()
        {
            return new Level
            {
                RingSpeed = 300,
                RingInterval = TimeSpan.FromSeconds(0.9),
                Sequence = ringInfoFactory.GetRandomSequence(10, gapSize: MathHelper.TwoPi/4),
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