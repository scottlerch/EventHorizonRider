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

    private readonly Vector2 _startDirection = startDirection;
    private readonly Vector2 _endDirection = endDirection;
    private readonly float _startingLife = 1f / startingLife;
    private readonly float _scaleBegin = scaleBegin;
    private readonly float _scaleEnd = scaleEnd;
    private readonly Color _startColor = startColor;
    private readonly Color _endColor = endColor;
    private readonly Vector2 _gravityCenter = emitter.GravityCenter;
    private readonly float _gravityForce = emitter.GravityForce;
    private readonly Texture2D _texture = emitter.ParticleSprite;

    private Vector2 _position = position;
    private float _lifePhase = 1f;
    private float _lifeLeft = startingLife;
    private Color _currColor = Color.White;
    private Rectangle _drawRectangle = new();

    public bool Update(float dt)
    {
        _lifeLeft -= dt;

        if (_lifeLeft <= 0)
        {
            IsAlive = false;
            return false;
        }

        _lifePhase = _lifeLeft * _startingLife;      // 1 means newly created 0 means dead.

        _position +=
            (
                Vector2.Lerp(_endDirection, _startDirection, _lifePhase) +
                ((_gravityCenter - _position) * _gravityForce)
            ) * dt;

        _currColor = Color.Lerp(_endColor, _startColor, _lifePhase);
        var currScale = MathHelper.Lerp(_scaleEnd, _scaleBegin, _lifePhase);

        _drawRectangle = new Rectangle(
            (int)((_position.X - 0.5f * currScale)),
            (int)((_position.Y - 0.5f * currScale)),
            (int)(currScale),
            (int)(currScale));

        return true;
    }

    public readonly void Draw(SpriteBatch spriteBatch, float depth)
    {
        if (IsAlive)
        {
            spriteBatch.Draw(
                _texture,
                destinationRectangle: _drawRectangle,
                color: _currColor,
                layerDepth: depth,
                sourceRectangle: null,
                rotation: 0f,
                effects: SpriteEffects.None,
                origin: Vector2.Zero);
        }
    }
}
