﻿using System;

namespace EventHorizonRider.Core.Engine
{
    [Flags]
    internal enum RingType
    {
        Asteroid = 1,
        Dust = 2,
        DustWithAsteroid = 4,
        IceCrystals = 8,
        All = Asteroid | Dust | DustWithAsteroid | IceCrystals,
    }
}
