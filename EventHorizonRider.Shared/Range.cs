using EventHorizonRider.Core.Physics;

namespace EventHorizonRider.Core
{
    internal class Range<T>
    {
        public T Low { get; set; }

        public T High { get; set; }
    }

    internal static class Range
    {
        public static Range<T> Create<T>(T low, T high)
        {
            return new Range<T> { High = high, Low = low };
        }

        public static Range<T> Create<T>(T value)
        {
            return new Range<T> { High = value, Low = value };
        }

        public static float GetRandom(this Range<float> range)
        {
            return MathUtilities.GetRandomBetween(range.Low, range.High);
        }

        public static int GetRandom(this Range<int> range)
        {
            return MathUtilities.GetRandomBetween(range.Low, range.High);
        }

        public static float GetDifference(this Range<float> range)
        {
            return range.High - range.Low;
        }

        public static float LinearInterpolate(this Range<float> range, float scale)
        {
            return MathUtilities.Lerp(range, scale);
        }

        public static Range<float> ScaleHigh(this Range<float> range, float scale)
        {
            return Create(range.Low, range.Low + (range.GetDifference() * scale));
        }
    }
}