using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    public class StarFactory
    {
        private Vector2 rotationOrigin;
        private Texture2D[] starTextures;
        private Color[] colors;

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            const int maxStars = 4;

            rotationOrigin = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);
            starTextures = new Texture2D[maxStars];

            for (int i = 0; i < starTextures.Length; i++)
            {
                starTextures[i] = content.Load<Texture2D>(@"Images\star_" + (i + 1));
            }

            colors = new[]
            {
                Color.White,
                Color.Lavender,
                Color.LavenderBlush,
                Color.Ivory,
                Color.OldLace,
                Color.Thistle,
                Color.Pink,
                Color.MistyRose
            };
        }

        public Star[] GetStars(int numberOfStars)
        {
            var stars = new Star[numberOfStars];
            var maxRadius = rotationOrigin.Length();

            for (var i = 0; i < stars.Length; i++)
            {
                var radius = MathUtilities.GetRandomBetween(0, maxRadius);
                stars[i] = new Star
                {
                    Texture = starTextures[MathUtilities.GetRandomBetween(0, starTextures.Length - 1)],
                    Angle = MathUtilities.GetRandomBetween(0, MathHelper.TwoPi),
                    Radius = radius,
                    Origin = new Vector2(starTextures[0].Width / 2f, starTextures[0].Height / 2f),
                    Scale = MathUtilities.GetRandomBetween(0.4f, 1f),
                    RotationOrigin = rotationOrigin,
                    Transparency = MathUtilities.GetRandomBetween(0.5f, 1f) * (radius / maxRadius),
                    RotationSpeed = MathUtilities.GetRandomBetween(MathHelper.TwoPi / -180, 0),
                    Color = colors[MathUtilities.GetRandomBetween(0, colors.Length - 1)],
                };
            }

            return stars;
        }
    }
}
