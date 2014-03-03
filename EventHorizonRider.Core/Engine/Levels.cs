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
                    return GetLevelOne();
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

        public Level GetLevelOne()
        {
            return new Level
            {
                RingSpeed = 150,
                RingInterval = TimeSpan.FromSeconds(2),
                Sequence = ringInfoFactory.GetRandomSequence(10, gapSize: MathHelper.TwoPi / 4),
            };
        }

        public Level GetLevelTwo()
        {
            return new Level
            {
                RingSpeed = 200,
                RingInterval = TimeSpan.FromSeconds(1.5),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetStepSequence(5, MathHelper.TwoPi / 5f),
                    ringInfoFactory.GetStepSequence(5, MathHelper.TwoPi / 5f)),
            };
        }

        public Level GetLevelThree()
        {
            return new Level
            {
                RingSpeed = 250,
                RingInterval = TimeSpan.FromSeconds(1),
                Sequence = Enumerable.Empty<RingInfo>().Concat(
                    ringInfoFactory.GetZigZagSequence(20, MathHelper.TwoPi / 5f)),
            };
        }

        public Level GetLevelFour()
        {
            return new Level
            {
                RingSpeed = 300,
                RingInterval = TimeSpan.FromSeconds(0.75),
                Sequence = ringInfoFactory.GetRandomSequence(10, gapSize: MathHelper.TwoPi / 4),
            };
        }

        public Level GetInfiniteLevel()
        {
            return new Level
            {
                RingSpeed = 350,
                RingInterval = TimeSpan.FromSeconds(0.5),
                Sequence = ringInfoFactory.GetRandomSequence(),
            };
        }
    }
}