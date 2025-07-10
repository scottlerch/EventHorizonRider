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

    private float baseTransparency;
    private float transparency;

    public float Transparency
    {
        get { return transparency; }
        set
        {
            baseTransparency = value;
            transparency = value;
        }
    }

    public float RotationSpeed { get; set; }

    private bool twinkling;
    private float twinkleSpeed;
    private float twinkleMax;

    public void Twinkle()
    {
        if (twinkling) return;

        var maxRadius = RotationOrigin.Length();
        var twinkleStrenth = MathHelper.Lerp(0.5f, 1f, Radius / maxRadius);

        twinkling = true;
        twinkleSpeed = MathUtilities.GetRandomBetween(0.4f, 1.5f);
        twinkleMax = MathUtilities.GetRandomBetween(twinkleStrenth - 0.1f, twinkleStrenth);
    }

    public void Update(GameTime gameTime, float scale)
    {
        if (twinkling)
        {
            transparency += (float)gameTime.ElapsedGameTime.TotalSeconds * twinkleSpeed;

            if (transparency >= twinkleMax)
            {
                twinkleSpeed *= -1f;
            }

            if (transparency <= baseTransparency)
            {
                transparency = baseTransparency;
                twinkling = false;
            }
        }

        Position = new Vector2(
            RotationOrigin.X + ((float)Math.Sin(Angle) * (Radius * scale)),
            RotationOrigin.Y - ((float)Math.Cos(Angle) * (Radius * scale)));
    }
}
