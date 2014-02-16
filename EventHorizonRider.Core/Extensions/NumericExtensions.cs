using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core.Extensions
{
    internal static class NumericExtensions
    {
        public static bool IsBetweenAngles(this float angle, float startAngle, float endAngle)
        {
            angle = MathHelper.WrapAngle(angle);
            startAngle = MathHelper.WrapAngle(startAngle);
            endAngle = MathHelper.WrapAngle(endAngle);

            if (startAngle < endAngle)
            {
                return angle > startAngle && angle < endAngle;
            }

            return angle > startAngle || angle < endAngle; 
        }

        public static bool IsBetween(this int edge, int startEdge, int endEdge)
        {
            return edge > startEdge && edge < endEdge;
        }

        public static bool IsBetween(this float edge, float startEdge, float endEdge)
        {
            return edge > startEdge && edge < endEdge;
        }
    }
}
