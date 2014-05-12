using System;

namespace EventHorizonRider.Core.Physics
{
    internal class Spring
    {
        /*
         * WallX               SpringLength         BlockX
         *      |====================|--------------[    ]
         */

        public float BlockMass { get; set; }

        public float BlockVelocity { get; set; }

        public float BlockX { get; set; }

        public float Friction { get; set; }

        public float SpringLength { get; set; }

        public float Stiffness { get; set; }

        public float WallVelocity { get; set; }

        public float WallX { get; set; }

        private float x;
        private float pullVelocity;

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

        public void PullBlock(float newX, float newPullVelocity)
        {
            x = newX;
            pullVelocity = newPullVelocity;

            if ((x < BlockX && newPullVelocity > 0) ||
                (x > BlockX && newPullVelocity < 0))
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