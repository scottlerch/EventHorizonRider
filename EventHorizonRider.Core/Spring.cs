using System;

namespace EventHorizonRider.Core
{
    internal class Spring
    {
        /*
         * WallX               SpringLength         BlockX
         *      |====================|--------------[    ]
         */

        public float WallX = 0f;
        public float WallVelocity = 0f;

        public float BlockX = 1f;
        public float BlockMass = 0.5f;
        public float BlockVelocity = 0f;
        public float SpringLength = 1f;

        public float Friction = -0.5f;
        public float Stiffness = -20f;

        private float newX;
        private float pullVelocity;

        public void PullBlock(float newX, float pullVelocity)
        {
            this.newX = newX;
            this.pullVelocity = pullVelocity;

            if ((this.newX < BlockX && pullVelocity < 0) ||
                (this.newX > BlockX && pullVelocity > 0))
            {
                pullVelocity *= -1f;
            }
        }

        public void Update(TimeSpan timeElapsed)
        {
            if (this.pullVelocity != 0)
            {
                BlockX += pullVelocity * (float)timeElapsed.TotalSeconds;

                if ((pullVelocity < 0 && BlockX <= newX) ||
                    (pullVelocity > 0 && BlockX >= newX))
                {
                    // pull is over
                    this.pullVelocity = 0;
                    BlockX = newX;
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
}
