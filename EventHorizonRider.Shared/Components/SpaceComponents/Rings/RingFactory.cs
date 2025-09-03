using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class RingFactory
{
    public RingTexturesInfo Asteroids { get; private set; }

    public RingTexturesInfo SparseAsteroids { get; private set; }

    public RingTexturesInfo Dust { get; private set; }

    public RingTexturesInfo Crystals { get; private set; }

    // TODO: refactor this and calculate from viewport size and maximum ring width
    public static float StartRadius = 700;

    internal void LoadContent(ContentManager content)
    {
        Asteroids = LoadData(content, "asteroid", 3, hasShadow: true);
        Asteroids.ShadowOffset = new Vector2(30, 30);
        Asteroids.DensityRange = Range.Create(15, 35);
        Asteroids.ScaleRange = Range.Create(0.2f, 0.8f);
        Asteroids.RadiusOffsetJitter = 10f;
        Asteroids.AngleJitter = 0.8f;
        Asteroids.TaperAmount = 1;
        Asteroids.TaperScale = 1f;
        Asteroids.TextureColors =
        [
            Color.DarkGray.AdjustLight(1.2f),
            Color.LightGray,
            Color.DarkGray,
            Color.Beige,
            Color.Lerp(Color.Tan.AdjustLight(0.8f), Color.DarkGray, 0.5f)
        ];

        SparseAsteroids = new RingTexturesInfo
        {
            ShadowOffset = new Vector2(30, 30),
            ShadowTextures = Asteroids.ShadowTextures,
            Textures = Asteroids.Textures,
            CollisionInfos = Asteroids.CollisionInfos,
            DensityRange = Range.Create(5, 10),
            ScaleRange = Range.Create(0.1f, 0.6f),
            RadiusOffsetJitter = 10f,
            AngleJitter = 0.8f,
            TaperAmount = 2,
            TaperScale = 0.8f,
            TextureColors =
            [
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige
            ]
        };

        Dust = LoadData(content, "dust", 1, hasShadow: true);
        Dust.MergeShadows = true;
        Dust.ShadowOffset = new Vector2(4, 4);
        Dust.DensityRange = Range.Create(85, 95);
        Dust.ScaleRange = Range.Create(0.3f * 4f, 1.2f * 4f);
        Dust.RadiusOffsetJitter = 10f;
        Dust.AngleJitter = 0.8f;
        Dust.TaperAmount = 6;
        Dust.TaperScale = 0.3f;
        Dust.TextureColors =
        [
            Color.Tan,
            Color.Tan.AdjustLight(0.8f),
            Color.Lerp(Color.Tan, Color.Beige, 0.5f),
            Color.Beige,
            Color.Beige.AdjustLight(0.9f),
            Color.Lerp(Color.Tan.AdjustLight(0.8f), Color.Beige.AdjustLight(0.9f), 0.5f)
        ];

        Crystals = LoadData(content, "crystals", 2, hasShadow: false);
        Crystals.DensityRange = Range.Create(65, 75);
        Crystals.ScaleRange = Range.Create(0.2f, 0.8f);
        Crystals.RadiusOffsetJitter = 10f;
        Crystals.AngleJitter = 0.8f;
        Crystals.TaperAmount = 6;
        Crystals.TaperScale = 0.4f;
        Crystals.TextureColors =
        [
            Color.White * 0.8f,
            Color.White,
            Color.Thistle * 0.9f
        ];
    }

    private static RingTexturesInfo LoadData(ContentManager content, string imageBaseName, int count, bool hasShadow)
    {
        // TODO: figure out count from image names
        var texturesInfo = new RingTexturesInfo
        {
            Textures = new Texture2D[count],
            ShadowTextures = hasShadow ? new Texture2D[count] : null,
            CollisionInfos = new CollisionInfo[count],
        };

        for (var i = 0; i < count; i++)
        {
            texturesInfo.Textures[i] = content.Load<Texture2D>(@"Images\" + imageBaseName + "_" + (i + 1));

            if (hasShadow)
            {
                texturesInfo.ShadowTextures[i] = content.Load<Texture2D>(@"Images\" + imageBaseName + "_shadow_" + (i + 1));
            }

            texturesInfo.CollisionInfos[i] = CollisionDetection.GetCollisionInfo(
                texturesInfo.Textures[i],
                resolution: DeviceInfo.Platform.CollisionDetectionDetail == CollisionDetectionDetail.Full ? 1f : 0.75f);
        }

        return texturesInfo;
    }

    private RingTexturesInfoGroup GetTexturesInfo(RingType type)
    {
        return type switch
        {
            RingType.Asteroid => new RingTexturesInfoGroup([Asteroids], RingTexturesInfoGroupMode.Sequential),
            RingType.Dust => new RingTexturesInfoGroup([Dust], RingTexturesInfoGroupMode.Sequential),
            RingType.DustWithAsteroid => new RingTexturesInfoGroup([Dust, SparseAsteroids], RingTexturesInfoGroupMode.Sequential),
            RingType.IceCrystals => new RingTexturesInfoGroup([Crystals], RingTexturesInfoGroupMode.Sequential),
            RingType.IceCrystalsWithAsteroid => new RingTexturesInfoGroup([Crystals, SparseAsteroids], RingTexturesInfoGroupMode.Sequential),
            _ => throw new Exception("invalid ring type"),
        };
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
            gaps: [.. Enumerable.Range(0, info.NumberOfGaps).Select(i =>
                new RingGap
                {
                    GapAngle = MathHelper.WrapAngle((i*(MathHelper.TwoPi/info.NumberOfGaps)) + info.Angle),
                    GapSize = info.GapSize,
                })]);
    }
}
