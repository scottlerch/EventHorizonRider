using System;

namespace EventHorizonRider.Core
{
    [Flags]
    public enum DetailLevel
    {
        None = 0,
        PixelShaderEffectsNone = 1,
        PixelShaderEffectsHalf = 2,
        PixelShaderEffectsFull = 4,
        CollisionDetectionHalf = 8,
        CollisionDetectionFull = 16,
        Full = 0x7fffffff,
    }
}
