using System;

namespace EventHorizonRider.Core.Engine
{
    [Flags]
    internal enum RingType
    {
        Asteroid = 1,
        Dust = 2,
        All = Asteroid | Dust,
    }
}
