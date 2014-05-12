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
            return Random.Next(low, high + 1);
        }

        /// <summary>
        /// Get random value between two numbers inclusive.
        /// </summary>
        public static float GetRandomBetween(float low, float high)
        {
            return low + ((high - low)*(float)Random.NextDouble());
        }

        public static byte LinearInterpolate(byte a, byte b, float percent)
        {
            return (byte)(a * (1 - percent) + b * percent);
        }

        public static float LinearInterpolate(float a, float b, float percent)
        {
            return (float)(a * (1 - percent) + b * percent);
        }

        public static Vector2 LinearInterpolate(Vector2 a, Vector2 b, float percent)
        {
            return new Vector2(
                LinearInterpolate(a.X, b.X, percent), 
                LinearInterpolate(a.Y, b.Y, percent));
        }

        public static Color LinearInterpolate(Color a, Color b, float percent)
        {
            return new Color(
                LinearInterpolate(a.R, b.R, percent), 
                LinearInterpolate(a.G, b.G, percent), 
                LinearInterpolate(a.B, b.B, percent),
                LinearInterpolate(a.A, b.A, percent));
        }

        public static float LinearInterpolate(Range<float> range, float percent)
        {
            return LinearInterpolate(range.Low, range.High, percent);
        }

        public static byte LinearInterpolate(Range<byte> range, float percent)
        {
            return LinearInterpolate(range.Low, range.High, percent);
        }

        public static Color LinearInterpolate(Range<Color> range, float percent)
        {
            return LinearInterpolate(range.Low, range.High, percent);
        }

        public static Vector2 LinearInterpolate(Range<Vector2> range, float percent)
        {
            return LinearInterpolate(range.Low, range.High, percent);
        }
    }
}
