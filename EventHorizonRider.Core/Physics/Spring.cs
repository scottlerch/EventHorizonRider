using System;

namespace EventHorizonRider.Core.Physics
{
    internal class Spring
    {
        /*
         * WallX               SpringLength         BlockX
         *      |====================|--------------[    ]
         */

        public float BlockMass = 0.5f;
        public float BlockVelocity = 0f;
        public float BlockX = 1f;

        public float Friction = -0.5f;
        public float SpringLength = 1f;
        public float Stiffness = -20f;
        public float WallVelocity = 0f;
        public float WallX = 0f;

        private float x;
        private float pullVelocity;

        public void PullBlock(float newX, float newPullVelocity)
        {
            x = newX;
            pullVelocity = newPullVelocity;

            if ((x < BlockX && newPullVelocity < 0) ||
                (x > BlockX && newPullVelocity > 0))
            {
                pullVelocity *= -1f;
            }
        }

        public void Update(TimeSpan timeElapsed)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (pullVelocity != 0)
            {
                BlockX += pullVelocity*(float) timeElapsed.TotalSeconds;

                if ((pullVelocity < 0 && BlockX <= x) ||
                    (pullVelocity > 0 && BlockX >= x))
                {
                    // pull is over
                    pullVelocity = 0;
                    BlockX = x;
                }
            }
            else
            {
                var springForce = Stiffness*((BlockX - WallX) - SpringLength);
                var damperForce = Friction*(BlockVelocity - WallVelocity);

                var acceleration = (springForce + damperForce)/BlockMass;

                BlockVelocity += acceleration*(float) timeElapsed.TotalSeconds;
                BlockX += BlockVelocity*(float) timeElapsed.TotalSeconds;
            }
        }
    }
}