using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Extensions
{
    internal static class ColorExtensions
    {
        public static Color AdjustLight(this Color color, float percent)
        {
            return new Color((int)(color.R * percent), (int)(color.G * percent), (int)(color.B * percent), color.A);
        }

        public static Color SetColors(this Color color, float redPercent, float greenPercent, float bluePercent)
        {
            return new Color((int)(255 * redPercent), (int)(255 * greenPercent), (int)(255 * bluePercent), color.A);
        }

        public static Color SetRed(this Color color, float percent)
        {
            return new Color((int)(255 * percent), (int)(color.G), (int)(color.B), color.A);
        }

        public static Color SetGreen(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(255 * percent), (int)(color.B), color.A);
        }

        public static Color SetBlue(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(color.G), (int)(255 * percent), color.A);
        }

        public static Color SetAlpha(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(color.G), (int)(color.B), (int)(255 * percent));
        }

        public static Color AdjustRed(this Color color, float percent)
        {
            return new Color((int)(color.R * percent), (int)(color.G), (int)(color.B ), color.A);
        }

        public static Color AdjustGreen(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(color.G * percent), (int)(color.B), color.A);
        }

        public static Color AdjustBlue(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(color.G), (int)(color.B * percent), color.A);
        }

        public static Color AdjustAlpha(this Color color, float percent)
        {
            return new Color((int)(color.R), (int)(color.G), (int)(color.B), (int)( color.A * percent));
        }
    }
}
