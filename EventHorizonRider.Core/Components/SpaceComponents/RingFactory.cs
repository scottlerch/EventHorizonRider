using System;
using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingFactory
    {
        private RingTexturesInfo asteroids;
        private RingTexturesInfo sparseAsteroids;
        private RingTexturesInfo dust;

        private float startRadius;
        private Vector2 viewportCenter;

        internal void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);

            asteroids = LoadData(content, "asteroid", 4, hasShadow:true);
            asteroids.DensityRange = Range.Create(15, 35);
            asteroids.ScaleRange = Range.Create(0.2f, 0.8f);
            asteroids.RadiusOffsetJitter = 10f;
            asteroids.AngleJitter = 0.8f;
            asteroids.TaperAmount = 1;
            asteroids.TaperScale = 1f;
            asteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige,
            };

            sparseAsteroids = new RingTexturesInfo();
            sparseAsteroids.ShadowTextures = asteroids.ShadowTextures;
            sparseAsteroids.Textures = asteroids.Textures;
            sparseAsteroids.CollisionInfos = asteroids.CollisionInfos;
            sparseAsteroids.DensityRange = Range.Create(5, 10);
            sparseAsteroids.ScaleRange = Range.Create(0.1f, 0.6f);
            sparseAsteroids.RadiusOffsetJitter = 10f;
            sparseAsteroids.AngleJitter = 0.8f;
            sparseAsteroids.TaperAmount = 1;
            sparseAsteroids.TaperScale = 1f;
            sparseAsteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige,
            };

            dust = LoadData(content, "dust", 1, hasShadow:false);
            dust.DensityRange = Range.Create(85, 95);
            dust.ScaleRange = Range.Create(0.3f, 1.2f);
            dust.RadiusOffsetJitter = 10f;
            dust.AngleJitter = 0.8f;
            dust.TaperAmount = 4;
            dust.TaperScale = 0.4f;
            dust.TextureColors = new []
            {
                Color.Tan, 
                Color.Tan.AdjustLight(0.8f),
                Color.Beige,
                Color.Beige.AdjustLight(0.9f),
            };

            // TODO: calculate from viewport
            startRadius = 700;
        }

        private RingTexturesInfo LoadData(ContentManager content, string imageBaseName, int count, bool hasShadow)
        {
            var texturesInfo = new RingTexturesInfo
            {
                Textures = new Texture2D[count],
                ShadowTextures = hasShadow? new Texture2D[count] : null,
                CollisionInfos = new CollisionInfo[count],
            };

            for (var i = 0; i < count; i++)
            {
                texturesInfo.Textures[i] = content.Load<Texture2D>(@"Images\" + imageBaseName + "_" + (i + 1));
                if (hasShadow) texturesInfo.ShadowTextures[i] = content.Load<Texture2D>(@"Images\" + imageBaseName + "_shadow_" + (i + 1));
                texturesInfo.CollisionInfos[i] = CollisionDetection.GetCollisionInfo(texturesInfo.Textures[i], resolution:DeviceInfo.DetailLevel.HasFlag(DetailLevel.CollisionDetectionFull)? 1f : 0.75f);
            }

            return texturesInfo;
        }

        private RingTexturesInfoGroup GetTexturesInfo(RingType type)
        {
            switch (type)
            {
                case RingType.Asteroid:
                    return new RingTexturesInfoGroup(new[] { asteroids }, RingTexturesInfoGroupMode.Sequential);
                case RingType.Dust:
                    return new RingTexturesInfoGroup(new[] { dust }, RingTexturesInfoGroupMode.Sequential);
                case RingType.DustWithAsteroid:
                    return new RingTexturesInfoGroup(new[] {dust, sparseAsteroids  }, RingTexturesInfoGroupMode.Sequential);
                default:
                    throw new Exception("invalid ring type");
            }
        }

        internal Ring Create(RingInfo info, Level level)
        {
            return new Ring(
                ringCollapseSpeed: level.RingSpeed,
                rotationalVelocity: info.RotationalVelocity,
                texturesInfoGroup: GetTexturesInfo(info.Type),
                innerRadius: startRadius,
                origin: viewportCenter,
                spiralRadius: info.SpiralRadius,
                spiralSpeed: level.ShipSpeed,
                gaps: Enumerable.Range(0, info.NumberOfGaps).Select(i =>
                    new RingGap
                    {
                        GapAngle = MathHelper.WrapAngle((i*(MathHelper.TwoPi/info.NumberOfGaps)) + info.Angle),
                        GapSize = info.GapSize,
                    }).ToList());
        }
    }
}