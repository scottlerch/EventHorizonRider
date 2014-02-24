using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingGap
    {
        public float GapAngle;
        public float GapSize;

        public float GapStart
        {
            get { return MathHelper.WrapAngle(GapAngle - (GapSize/2)); }
        }

        public float GapEnd
        {
            get { return MathHelper.WrapAngle(GapAngle + (GapSize/2)); }
        }

        public bool IsInsideGap(float angle)
        {
            if (GapStart < GapEnd && (angle > GapStart && angle < GapEnd))
                return true;

            if (GapStart > GapEnd && (angle > GapStart || angle < GapEnd))
                return true;

            return false;
        }
    }
}