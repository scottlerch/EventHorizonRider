using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics;

internal struct Particle(
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
    public bool IsAlive = true;
    
    private readonly Vector2 startDirection = startDirection;
    private readonly Vector2 endDirection = endDirection;
    private readonly float startingLife = 1f / startingLife;
    private readonly float scaleBegin = scaleBegin;
    private readonly float scaleEnd = scaleEnd;
    private readonly Color startColor = startColor;
    private readonly Color endColor = endColor;
    private readonly Vector2 gravityCenter = emitter.GravityCenter;
    private readonly float gravityForce = emitter.GravityForce;
    private readonly Texture2D texture = emitter.ParticleSprite;

    private Vector2 position = position;
    private float lifePhase = 1f;
    private float lifeLeft = startingLife;
    private Color currColor = Color.White;
    private Rectangle drawRectangle = new();

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

    public readonly void Draw(SpriteBatch spriteBatch, float depth)
    {
        if (IsAlive)
        {
            spriteBatch.Draw(
                texture,
                destinationRectangle: drawRectangle,
                color: currColor,
                layerDepth: depth,
                sourceRectangle: null,
                rotation: 0f,
                effects: SpriteEffects.None,
                origin: Vector2.Zero);
        }
    }
}
