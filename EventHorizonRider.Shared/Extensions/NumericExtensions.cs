using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Extensions;

internal static class NumericExtensions
{
    public static bool IsBetweenAngles(this float angle, float startAngle, float endAngle)
    {
        angle = MathHelper.WrapAngle(angle);
        startAngle = MathHelper.WrapAngle(startAngle);
        endAngle = MathHelper.WrapAngle(endAngle);

        return startAngle < endAngle ? angle > startAngle && angle < endAngle : angle > startAngle || angle < endAngle;
    }

    public static bool IsBetween(this int edge, int startEdge, int endEdge) => edge > startEdge && edge < endEdge;

    public static bool IsBetween(this float edge, float startEdge, float endEdge) => edge > startEdge && edge < endEdge;
}
