using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class StarFactory
{
    private Vector2 _rotationOrigin;
    private Texture2D[] _starTextures;
    private Color[] _colors;

    public void LoadContent(ContentManager content)
    {
        const int maxStars = 4;

        _rotationOrigin = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);
        _starTextures = new Texture2D[maxStars];

        for (var i = 0; i < _starTextures.Length; i++)
        {
            _starTextures[i] = content.Load<Texture2D>(@"Images\star_" + (i + 1));
        }

        _colors =
        [
            Color.White,
            Color.Lavender,
            Color.LavenderBlush,
            Color.Ivory,
            Color.OldLace,
            Color.Thistle,
            Color.Pink,
            Color.MistyRose
        ];
    }

    public Star[] GetStars(int numberOfStars)
    {
        var stars = new Star[numberOfStars];
        var maxRadius = _rotationOrigin.Length();

        for (var i = 0; i < stars.Length; i++)
        {
            var radius = MathUtilities.GetRandomBetween(0, maxRadius);

            stars[i] = new Star
            {
                Texture = _starTextures[MathUtilities.GetRandomBetween(0, _starTextures.Length - 1)],
                Angle = MathUtilities.GetRandomBetween(0, MathHelper.TwoPi),
                Radius = radius,
                Origin = new Vector2(_starTextures[0].Width / 2f, _starTextures[0].Height / 2f),
                Scale = MathUtilities.GetRandomBetween(0.4f, 1f),
                RotationOrigin = _rotationOrigin,
                Transparency = MathUtilities.GetRandomBetween(0.5f, 1f) * (radius / maxRadius),
                RotationSpeed = MathUtilities.GetRandomBetween(MathHelper.TwoPi / -180, 0),
                Color = _colors[MathUtilities.GetRandomBetween(0, _colors.Length - 1)],
            };
        }

        return stars;
    }
}
