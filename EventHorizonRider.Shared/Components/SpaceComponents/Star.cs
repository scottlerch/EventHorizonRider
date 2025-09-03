using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class Star
{
    public Texture2D Texture { get; set; }

    public float Scale { get; set; }

    public Vector2 Position { get; private set; }

    public Vector2 Origin { get; set; }

    public float Radius { get; set; }

    public float Angle { get; set; }

    public Vector2 RotationOrigin { get; set; }

    public Color Color { get; set; }

    private float _baseTransparency;
    private float _transparency;

    public float Transparency
    {
        get => _transparency;
        set
        {
            _baseTransparency = value;
            _transparency = value;
        }
    }

    public float RotationSpeed { get; set; }

    private bool _twinkling;
    private float _twinkleSpeed;
    private float _twinkleMax;

    public void Twinkle()
    {
        if (_twinkling)
        {
            return;
        }

        var maxRadius = RotationOrigin.Length();
        var twinkleStrenth = MathHelper.Lerp(0.5f, 1f, Radius / maxRadius);

        _twinkling = true;
        _twinkleSpeed = MathUtilities.GetRandomBetween(0.4f, 1.5f);
        _twinkleMax = MathUtilities.GetRandomBetween(twinkleStrenth - 0.1f, twinkleStrenth);
    }

    public void Update(GameTime gameTime, float scale)
    {
        if (_twinkling)
        {
            _transparency += (float)gameTime.ElapsedGameTime.TotalSeconds * _twinkleSpeed;

            if (_transparency >= _twinkleMax)
            {
                _twinkleSpeed *= -1f;
            }

            if (_transparency <= _baseTransparency)
            {
                _transparency = _baseTransparency;
                _twinkling = false;
            }
        }

        Position = new Vector2(
            RotationOrigin.X + ((float)Math.Sin(Angle) * (Radius * scale)),
            RotationOrigin.Y - ((float)Math.Cos(Angle) * (Radius * scale)));
    }
}
