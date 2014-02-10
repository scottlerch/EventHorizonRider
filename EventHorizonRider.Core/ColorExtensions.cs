using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core
{
    internal static class ColorExtensions
    {
        public static Color AdjustLight(this Color color, float percent)
        {
            return new Color((int)(color.R * percent), (int)(color.G * percent), (int)(color.B * percent), color.A);
        }
    }
}
