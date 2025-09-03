using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class RingGap
{
    public float GapAngle { get; set; }

    public float GapSize { get; set; }


    public float GapStart => GapAngle - (GapSize / 2f);

    public float GapEnd => GapAngle + (GapSize / 2f);

    public bool IsInsideGap(float angle) => angle.IsBetweenAngles(GapStart, GapEnd);
}
