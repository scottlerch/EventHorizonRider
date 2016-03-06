using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    internal struct Particle
    {
        public bool IsAlive;
        
        private readonly Vector2 startDirection;
        private readonly Vector2 endDirection;
        private readonly float startingLife;
        private readonly float scaleBegin;
        private readonly float scaleEnd;
        private readonly Color startColor;
        private readonly Color endColor;
        private readonly Vector2 gravityCenter;
        private readonly float gravityForce;
        private readonly Texture2D texture;

        private Vector2 position;
        private float lifePhase;
        private float lifeLeft;
        private Color currColor;
        private Rectangle drawRectangle;

        public Particle(
            Vector2 position, 
            Vector2 startDirection,
            Vector2 endDirection, 
            float startingLife, 
            float scaleBegin, 
            float scaleEnd,
            Color startColor, 
            Color endColor, 
            Emitter emitter)
        {
            this.position = position;
            this.startDirection = startDirection;
            this.endDirection = endDirection;
            this.startingLife = 1f / startingLife;
            this.scaleBegin = scaleBegin;
            this.scaleEnd = scaleEnd;
            this.startColor = startColor;
            this.endColor = endColor;
            gravityCenter = emitter.GravityCenter;
            gravityForce = emitter.GravityForce;
            texture = emitter.ParticleSprite;
            IsAlive = true;
            lifePhase = 1f;
            lifeLeft = startingLife;
            currColor = Color.White;
            drawRectangle = new Rectangle();
        }

        public bool Update(float dt)
        {
            lifeLeft -= dt;

            if (lifeLeft <= 0)
            {
                IsAlive = false;
                return false;
            }

            lifePhase = lifeLeft * startingLife;      // 1 means newly created 0 means dead.

            position += 
                (
                    Vector2.Lerp(endDirection, startDirection, lifePhase) + 
                    ((gravityCenter - position) * gravityForce)
                ) * dt;

            currColor = Color.Lerp(endColor, startColor, lifePhase);
            var currScale = MathHelper.Lerp(scaleEnd, scaleBegin, lifePhase);

            drawRectangle = new Rectangle(
                (int) ((position.X - 0.5f*currScale)),
                (int) ((position.Y - 0.5f*currScale)),
                (int) (currScale),
                (int) (currScale));

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(
                    texture,
                    destinationRectangle: drawRectangle,
                    color: currColor,
                    layerDepth: depth);
            }
        }
    }
}
