using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Physics;

internal static class MathUtilities
{
    private static readonly Random _random = new();

    /// <summary>
    /// Get random value between two numbers inclusive.
    /// </summary>
    public static int GetRandomBetween(int low, int high) => low > high ? _random.Next(high, low + 1) : _random.Next(low, high + 1);

    /// <summary>
    /// Get random value between two numbers inclusive.
    /// </summary>
    public static float GetRandomBetween(float low, float high) => low + ((high - low) * (float)_random.NextDouble());

    public static byte Lerp(byte a, byte b, float percent) => (byte)(a * (1 - percent) + b * percent);

    public static Vector2 Lerp(Vector2 a, Vector2 b, float percent)
    {
        return new Vector2(
            MathHelper.Lerp(a.X, b.X, percent),
            MathHelper.Lerp(a.Y, b.Y, percent));
    }

    public static float Lerp(Range<float> range, float percent) => MathHelper.Lerp(range.Low, range.High, percent);

    public static byte Lerp(Range<byte> range, float percent) => Lerp(range.Low, range.High, percent);

    public static Color Lerp(Range<Color> range, float percent) => Color.Lerp(range.Low, range.High, percent);

    public static Vector2 Lerp(Range<Vector2> range, float percent) => Vector2.Lerp(range.Low, range.High, percent);
}
