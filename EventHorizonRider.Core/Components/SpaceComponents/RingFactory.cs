using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingFactory
    {
        private const int AsteroidsCount = 4;

        private Texture2D[] asteroidTextures;
        private byte[][] asteroidTexturesData;

        private float startRadius;
        private Vector2 viewportCenter;

        internal void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            asteroidTextures = new Texture2D[AsteroidsCount];
            asteroidTexturesData = new byte[AsteroidsCount][];

            for (int i = 0; i < asteroidTextures.Length; i++)
            {
                asteroidTextures[i] = content.Load<Texture2D>(@"Images\asteroid_" + (i + 1));
                asteroidTexturesData[i] = TextureProcessor.GetAlphaData(asteroidTextures[i]);
            }

            // TODO: calculate from viewport
            startRadius = 700;
        }

        internal Ring Create(RingInfo info)
        {
            return new Ring(
                rotationalVelocity: info.RotationalVelocity,
                textures: asteroidTextures,
                texturesData: asteroidTexturesData,
                radius: startRadius,
                origin: viewportCenter,
                gaps: Enumerable.Range(0, info.NumberOfGaps).Select(i =>
                    new RingGap
                    {
                        GapAngle = MathHelper.WrapAngle((i*(MathHelper.TwoPi/info.NumberOfGaps)) + info.Angle),
                        GapSize = info.GapSize,
                    }).ToList());
        }
    }
}