using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class RingGap
{
    public float GapAngle { get; set; }

    public float GapSize { get; set; }


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