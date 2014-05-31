using System;
using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings
{
    internal class RingFactory
    {
        public RingTexturesInfo Asteroids { get; private set; }

        public RingTexturesInfo SparseAsteroids { get; private set; }

        public RingTexturesInfo Dust { get; private set; }

        public RingTexturesInfo Crystals { get; private set; }

        // TODO: refactor this and calculate from viewport size and maximum ring width
        public static float StartRadius = 700;

        internal void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            Asteroids = LoadData(content, "asteroid", 3, hasShadow:true);
            Asteroids.ShadowOffset = new Vector2(30, 30);
            Asteroids.DensityRange = Range.Create(15, 35);
            Asteroids.ScaleRange = Range.Create(0.2f, 0.8f);
            Asteroids.RadiusOffsetJitter = 10f;
            Asteroids.AngleJitter = 0.8f;
            Asteroids.TaperAmount = 1;
            Asteroids.TaperScale = 1f;
            Asteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige,
                Color.Lerp(Color.Tan.AdjustLight(0.8f), Color.DarkGray, 0.5f)
            };

            SparseAsteroids = new RingTexturesInfo();
            SparseAsteroids.ShadowOffset = new Vector2(30, 30);
            SparseAsteroids.ShadowTextures = Asteroids.ShadowTextures;
            SparseAsteroids.Textures = Asteroids.Textures;
            SparseAsteroids.CollisionInfos = Asteroids.CollisionInfos;
            SparseAsteroids.DensityRange = Range.Create(5, 10);
            SparseAsteroids.ScaleRange = Range.Create(0.1f, 0.6f);
            SparseAsteroids.RadiusOffsetJitter = 10f;
            SparseAsteroids.AngleJitter = 0.8f;
            SparseAsteroids.TaperAmount = 2;
            SparseAsteroids.TaperScale = 0.8f;
            SparseAsteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige
            };

            Dust = LoadData(content, "dust", 1, hasShadow:true);
            Dust.MergeShadows = true;
            Dust.ShadowOffset = new Vector2(4, 4);
            Dust.DensityRange = Range.Create(85, 95);
            Dust.ScaleRange = Range.Create(0.3f * 4f, 1.2f * 4f);
            Dust.RadiusOffsetJitter = 10f;
            Dust.AngleJitter = 0.8f;
            Dust.TaperAmount = 6;
            Dust.TaperScale = 0.3f;
            Dust.TextureColors = new []
            {
                Color.Tan, 
                Color.Tan.AdjustLight(0.8f),
                Color.Lerp(Color.Tan, Color.Beige, 0.5f),
                Color.Beige,
                Color.Beige.AdjustLight(0.9f),
                Color.Lerp(Color.Tan.AdjustLight(0.8f), Color.Beige.AdjustLight(0.9f), 0.5f)
            };

            Crystals = LoadData(content, "crystals", 2, hasShadow: false);
            Crystals.DensityRange = Range.Create(65, 75);
            Crystals.ScaleRange = Range.Create(0.2f, 0.8f);
            Crystals.RadiusOffsetJitter = 10f;
            Crystals.AngleJitter = 0.8f;
            Crystals.TaperAmount = 6;
            Crystals.TaperScale = 0.4f;
            Crystals.TextureColors = new[]
            {
                Color.White * 0.8f,
                Color.White,
                Color.Thistle * 0.9f
            };
        }

        private RingTexturesInfo LoadData(ContentManager content, string imageBaseName, int count, bool hasShadow)
        {
            // TODO: figure out count from image names
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
                texturesInfo.CollisionInfos[i] = CollisionDetection.GetCollisionInfo(
                    texturesInfo.Textures[i], 
                    resolution: DeviceInfo.Platform.CollisionDetectionDetail == CollisionDetectionDetail.Full ? 1f : 0.75f);
            }

            return texturesInfo;
        }

        private RingTexturesInfoGroup GetTexturesInfo(RingType type)
        {
            switch (type)
            {
                case RingType.Asteroid:
                    return new RingTexturesInfoGroup(new[] { Asteroids }, RingTexturesInfoGroupMode.Sequential);
                case RingType.Dust:
                    return new RingTexturesInfoGroup(new[] { Dust }, RingTexturesInfoGroupMode.Sequential);
                case RingType.DustWithAsteroid:
                    return new RingTexturesInfoGroup(new[] {Dust, SparseAsteroids  }, RingTexturesInfoGroupMode.Sequential);
                case RingType.IceCrystals:
                    return new RingTexturesInfoGroup(new[] { Crystals }, RingTexturesInfoGroupMode.Sequential);
                case RingType.IceCrystalsWithAsteroid:
                    return new RingTexturesInfoGroup(new[] { Crystals, SparseAsteroids }, RingTexturesInfoGroupMode.Sequential);
                default:
                    throw new Exception("invalid ring type");
            }
        }

        internal Ring Create(RingInfo info, Level level)
        {
            return new Ring(
                ringCollapseSpeed: level.RingSpeed,
                rotationalVelocity: info.RotationalVelocity + level.RotationalVelocity,
                texturesInfoGroup: GetTexturesInfo(info.Type),
                innerRadius: StartRadius,
                origin: DeviceInfo.LogicalCenter,
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