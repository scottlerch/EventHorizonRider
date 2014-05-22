using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    internal static class MathUtilities
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Get random value between two numbers inclusive.
        /// </summary>
        public static int GetRandomBetween(int low, int high)
        {
            if (low > high)
            {
                return Random.Next(high, low + 1);
            }

            return Random.Next(low, high + 1);
        }

        /// <summary>
        /// Get random value between two numbers inclusive.
        /// </summary>
        public static float GetRandomBetween(float low, float high)
        {
            return low + ((high - low)*(float)Random.NextDouble());
        }

        public static byte Lerp(byte a, byte b, float percent)
        {
            return (byte)(a * (1 - percent) + b * percent);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float percent)
        {
            return new Vector2(
                MathHelper.Lerp(a.X, b.X, percent),
                MathHelper.Lerp(a.Y, b.Y, percent));
        }

        public static float Lerp(Range<float> range, float percent)
        {
            return MathHelper.Lerp(range.Low, range.High, percent);
        }

        public static byte Lerp(Range<byte> range, float percent)
        {
            return Lerp(range.Low, range.High, percent);
        }

        public static Color Lerp(Range<Color> range, float percent)
        {
            return Color.Lerp(range.Low, range.High, percent);
        }

        public static Vector2 Lerp(Range<Vector2> range, float percent)
        {
            return Vector2.Lerp(range.Low, range.High, percent);
        }
    }
}
