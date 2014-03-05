using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingGap
    {
        public float GapAngle;
        public float GapSize;

        public float GapStart
        {
            get { return GapAngle - (GapSize/2f); }
        }

        public float GapEnd
        {
            get { return GapAngle + (GapSize/2f); }
        }

        public bool IsInsideGap(float angle)
        {
            return angle.IsBetweenAngles(GapStart, GapEnd);
        }
    }
}