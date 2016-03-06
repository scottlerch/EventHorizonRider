namespace EventHorizonRider.Core.Engine
{
    internal class RingInfo
    {
        public RingType Type { get; set; }

        public float Angle { get; set; }

        public float GapSize { get; set; }

        public int NumberOfGaps { get; set; }

        public float RotationalVelocity { get; set; }

        public float SpiralRadius { get; set; }
    }
}