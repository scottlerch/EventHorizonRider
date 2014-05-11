using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    internal class Particle
    {
        public Vector2 Position { get; set; }
        
        private readonly Vector2 startDirection;
        private readonly Vector2 endDirection;
        private readonly float startingLife;
        private readonly float scaleBegin;
        private readonly float scaleEnd;
        private readonly Color startColor;
        private readonly Color endColor;
        private readonly Emitter parent;

        private float lifePhase;
        private float lifeLeft;

        public Particle(Vector2 position, Vector2 startDirection, Vector2 endDirection, float startingLife, float scaleBegin, float scaleEnd, Color startColor, Color endColor, Emitter yourself)
        {
            Position = position;
            this.startDirection = startDirection;
            this.endDirection = endDirection;
            this.startingLife = startingLife;
            lifeLeft = startingLife;
            this.scaleBegin = scaleBegin;
            this.scaleEnd = scaleEnd;
            this.startColor = startColor;
            this.endColor = endColor;
            parent = yourself;
        }

        public bool Update(float dt)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0)
                return false;
            lifePhase = lifeLeft / startingLife;      // 1 means newly created 0 means dead.
            Position += MathUtilities.LinearInterpolate(endDirection, startDirection, lifePhase) * dt;
            Position += (parent.GravityCenter - Position)*parent.GravityForce*dt;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, int scale, Vector2 offset, float depth)
        {
            float currScale = MathUtilities.LinearInterpolate(scaleEnd, scaleBegin, lifePhase);
            Color currCol = MathUtilities.LinearInterpolate(endColor, startColor, lifePhase);
            spriteBatch.Draw(parent.ParticleSprite, new Rectangle((int)((Position.X - 0.5f * currScale) * scale + offset.X), (int)((Position.Y - 0.5f * currScale) * scale + offset.Y), (int)(currScale * scale), (int)(currScale * scale)), null, currCol, 0, Vector2.Zero, SpriteEffects.None, depth);
        }
    }
}
