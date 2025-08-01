﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine;

internal class RingInfoFactory
{
    private class RingTypeSelectionHelper(RingType type, RingTypeSelection typeSelection)
    {
        private int index;
        private readonly RingType[] ringTypes = [.. AllRingTypes.Where(t => type.HasFlag(t))];
        private readonly RingTypeSelection ringTypeSelection = typeSelection;

        public RingType GetNext()
        {
            var nextType = ringTypeSelection == RingTypeSelection.UniformRandom
                ? ringTypes[Rand.Next(ringTypes.Length)]
                : ringTypes[index];

            index++;
            index %= ringTypes.Length;

            return nextType;
        }
    }

    private static readonly Random Rand = new();
    private static readonly RingType[] AllRingTypes =
        [.. Enum.GetValues<RingType>()
            .Cast<RingType>()
            .Where(x => ((int) x != 0) && (((int) x & ((int) x - 1)) == 0))];

    public static IEnumerable<RingInfo> GetSpirals(
        int? iterations = null,
        float spiralRadius = 700f,
        RingType type = RingType.All,
        RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
    {
        var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);

        for (var i = 0; i < iterations || iterations == null; i++)
        {
            yield return new RingInfo
            {
                Type = ringTypeSelector.GetNext(),
                GapSize = 0,
                NumberOfGaps = 0,
                Angle = (float)Rand.NextDouble() * MathHelper.TwoPi,
                RotationalVelocity = 0f,
                SpiralRadius = spiralRadius,
            };
        }
    }

    public static IEnumerable<RingInfo> GetRandomSequence(
        int? iterations = null, 
        Range<int> numberOfGaps = null, 
        Range<float> gapSize = null,
        Range<float> rotation = null,
        RingType type = RingType.All,
        RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
    {
        var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);

        gapSize ??= Range.Create(MathHelper.TwoPi/8, MathHelper.TwoPi/4);
        numberOfGaps ??= Range.Create(1, 4);
        rotation ??= Range.Create(0f);

        for (var i = 0; i < iterations || iterations == null; i++)
        {
            var currentGapSize = gapSize.GetRandom();
            var currentNumberOfGaps = numberOfGaps.GetRandom();

            var maxNumberOfGaps = (int)(MathHelper.TwoPi/currentGapSize) - 1;

            currentNumberOfGaps = Math.Min(currentNumberOfGaps, maxNumberOfGaps);

            yield return new RingInfo
            {
                Type = ringTypeSelector.GetNext(),
                GapSize = currentGapSize,
                NumberOfGaps = currentNumberOfGaps,
                Angle = (float) Rand.NextDouble()*MathHelper.TwoPi,
                RotationalVelocity = rotation.GetRandom(),
            };
        }
    }

    public static IEnumerable<RingInfo> GetStepSequence(
        int numberOfSteps, 
        float gapSize,
        Range<float> rotation = null,
        RingType type = RingType.All, 
        RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
    {
        var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);
        var angleStep = MathHelper.TwoPi/numberOfSteps;

        rotation ??= Range.Create(0f);

        for (var i = 0; i < numberOfSteps; i++)
        {
            yield return new RingInfo
            {
                Type = ringTypeSelector.GetNext(),
                GapSize = gapSize,
                NumberOfGaps = 1,
                Angle = angleStep*i,
                RotationalVelocity = rotation.GetRandom(),
            };
        }
    }

    public static IEnumerable<RingInfo> GetZigZagSequence(
        int iterations, 
        float gapSize,
        float? angleStep = null,
        Range<float> rotation = null,
        RingType type = RingType.All, 
        RingTypeSelection typeSelection = RingTypeSelection.UniformRandom)
    {
        var ringTypeSelector = new RingTypeSelectionHelper(type, typeSelection);
        var baseAngle = (float) Rand.NextDouble()*MathHelper.TwoPi;
        
        angleStep ??= MathHelper.TwoPi/4;
        rotation ??= Range.Create(0f);

        for (var i = 0; i < iterations; i++)
        {
            yield return new RingInfo
            {
                Type = ringTypeSelector.GetNext(),
                GapSize = gapSize,
                NumberOfGaps = 1,
                Angle = baseAngle + ((i%2)*angleStep.Value),
                RotationalVelocity = rotation.GetRandom(),
            };
        }
    }
}