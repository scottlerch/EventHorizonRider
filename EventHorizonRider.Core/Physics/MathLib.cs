using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    internal class MathLib
    {
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
        public static Vector4 LinearInterpolate(Vector4 a, Vector4 b, double t)
        {
            return new Vector4(LinearInterpolate(a.X, b.X, t), LinearInterpolate(a.Y, b.Y, t), LinearInterpolate(a.Z, b.Z, t), LinearInterpolate(a.W, b.W, t));
        }
        public static Color LinearInterpolate(Color a, Color b, double t)
        {
            return new Color(LinearInterpolate(a.R, b.R, t), LinearInterpolate(a.G, b.G, t), LinearInterpolate(a.B, b.B, t), LinearInterpolate(a.A, b.A, t));
        }
    }
}
