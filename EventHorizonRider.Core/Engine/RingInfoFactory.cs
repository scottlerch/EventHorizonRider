using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class RingInfoFactory
    {
        private class RingTypeSelectionHelper
        {
            private int index;
            private readonly RingType[] ringTypes;
            private readonly RingTypeSelection ringTypeSelection;

            public RingTypeSelectionHelper(RingType type, RingTypeSelection typeSelection)
            {
                ringTypes = AllRingTypes.Where(t => type.HasFlag(t)).ToArray();
                ringTypeSelection = typeSelection;
            }

            public RingType GetNext()
            {
                var nextType = ringTypeSelection == RingTypeSelection.UniformRandom
                    ? ringTypes[Rand.Next(ringTypes.Length)]
                    : ringTypes[index];

                index++;
                index = index % ringTypes.Length;

                return nextType;
            }
        }

        public const float DefaultRotationVelocity = MathHelper.TwoPi / 32;
        private static readonly Random Rand = new Random();
        private static readonly RingType[] AllRingTypes =
            Enum.GetValues(typeof (RingType))
                .Cast<RingType>()
                .Where(x => ((int) x != 0) && (((int) x & ((int) x - 1)) == 0))
                .ToArray();

        public IEnumerable<RingInfo> GetRandomSequence(
            int? iterations = null, 
            Range<int>? numberOfGaps = null, 
            Range<float>? gapSize = null, 
            RingType type = RingType.All,
            RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
        {
            var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);

            gapSize = gapSize ?? Range.Create(MathHelper.TwoPi/8, MathHelper.TwoPi/4);
            numberOfGaps = numberOfGaps ?? Range.Create(1, 4);

            for (var i = 0; i < iterations || iterations == null; i++)
            {
                var currentGapSize = gapSize.Value.GetRandom();
                var currentNumberOfGaps = numberOfGaps.Value.GetRandom();

                var maxNumberOfGaps = (int)(MathHelper.TwoPi/currentGapSize) - 1;

                currentNumberOfGaps = Math.Min(currentNumberOfGaps, maxNumberOfGaps);

                yield return new RingInfo
                {
                    Type = ringTypeSelector.GetNext(),
                    GapSize = currentGapSize,
                    NumberOfGaps = currentNumberOfGaps,
                    Angle = (float) Rand.NextDouble()*MathHelper.TwoPi,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetStepSequence(
            int numberOfSteps, 
            float gapSize,
            RingType type = RingType.All, 
            RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
        {
            var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);
            var angleStep = MathHelper.TwoPi/numberOfSteps;

            for (var i = 0; i < numberOfSteps; i++)
            {
                yield return new RingInfo
                {
                    Type = ringTypeSelector.GetNext(),
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = angleStep*i,
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }

        public IEnumerable<RingInfo> GetZigZagSequence(
            int iterations, 
            float gapSize, 
            RingType type = RingType.All, 
            RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
        {
            var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);
            var baseAngle = (float) Rand.NextDouble()*MathHelper.TwoPi;
            const float angleStep = MathHelper.TwoPi/4;

            for (var i = 0; i < iterations; i++)
            {
                yield return new RingInfo
                {
                    Type = ringTypeSelector.GetNext(),
                    GapSize = gapSize,
                    NumberOfGaps = 1,
                    Angle = baseAngle + ((i%2)*angleStep),
                    RotationalVelocity = DefaultRotationVelocity,
                };
            }
        }
    }
}