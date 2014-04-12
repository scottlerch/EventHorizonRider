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
        public static int GetRandomBetween(int a, int b)
        {
            return Random.Next(a, b + 1);
        }

        /// <summary>
        /// Get random value between two numbers inclusive.
        /// </summary>
        public static float GetRandomBetween(float a, float b)
        {
            return a + ((b - a)*(float)Random.NextDouble());
        }

        public static byte LinearInterpolate(byte a, byte b, double t)
        {
            return (byte)(a * (1 - t) + b * t);
        }

        public static float LinearInterpolate(float a, float b, double t)
        {
            return (float)(a * (1 - t) + b * t);
        }

        public static Vector2 LinearInterpolate(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t));
        }

        public static Color LinearInterpolate(Color a, Color b, double t)
        {
            return new Color(LinearInterpolate(a.R, b.R, t), LinearInterpolate(a.G, b.G, t), LinearInterpolate(a.B, b.B, t), LinearInterpolate(a.A, b.A, t));
        }

        public static float LinearInterpolate(Range<float> range, double t)
        {
            return LinearInterpolate(range.Low, range.High, t);
        }

        public static byte LinearInterpolate(Range<byte> range, double t)
        {
            return LinearInterpolate(range.Low, range.High, t);
        }

        public static Color LinearInterpolate(Range<Color> range, double t)
        {
            return LinearInterpolate(range.Low, range.High, t);
        }

        public static Vector2 LinearInterpolate(Range<Vector2> range, double t)
        {
            return LinearInterpolate(range.Low, range.High, t);
        }
    }
}
