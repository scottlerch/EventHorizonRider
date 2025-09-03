using System;

namespace EventHorizonRider.Core.Physics;

/// <summary>
/// Calculate 1-dimensional spring physics.
/// </summary>
internal class Spring
{
    /*
     * WallX               SpringLength         BlockX
     *      |====================|--------------[    ]
     */

    private float _x;
    private float _pullVelocity;

    public Spring()
    {
        BlockMass = 0.5f;
        BlockVelocity = 0f;
        BlockX = 1f;

        Friction = -0.5f;
        SpringLength = 1f;
        Stiffness = -20f;
        WallVelocity = 0f;
        WallX = 0f;
    }

    public float BlockMass { get; set; }

    public float BlockVelocity { get; set; }

    public float BlockX { get; set; }

    public float Friction { get; set; }

    public float SpringLength { get; set; }

    public float Stiffness { get; set; }

    public float WallVelocity { get; set; }

    public float WallX { get; set; }

    public void PullBlock(float newX, float newPullVelocity)
    {
        _x = newX;
        _pullVelocity = newPullVelocity;

        if ((_x < BlockX && newPullVelocity > 0) ||
            (_x > BlockX && newPullVelocity < 0))
        {
            _pullVelocity *= -1f;
        }
    }

    public void Update(TimeSpan timeElapsed)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_pullVelocity != 0)
        {
            BlockX += _pullVelocity * (float)timeElapsed.TotalSeconds;

            if ((_pullVelocity < 0 && BlockX <= _x) ||
                (_pullVelocity > 0 && BlockX >= _x))
            {
                // pull is over
                _pullVelocity = 0;
                BlockX = _x;
            }
        }
        else
        {
            var springForce = Stiffness * ((BlockX - WallX) - SpringLength);
            var damperForce = Friction * (BlockVelocity - WallVelocity);

            var acceleration = (springForce + damperForce) / BlockMass;

            BlockVelocity += acceleration * (float)timeElapsed.TotalSeconds;
            BlockX += BlockVelocity * (float)timeElapsed.TotalSeconds;
        }
    }
}
