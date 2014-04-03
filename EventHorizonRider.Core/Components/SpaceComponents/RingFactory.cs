﻿using System;
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
        private RingTexturesInfo asteroids;
        private RingTexturesInfo sparseAsteroids;
        private RingTexturesInfo dust;

        private float startRadius;
        private Vector2 viewportCenter;

        internal void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            asteroids = LoadData(content, "asteroid", 4);
            asteroids.DensityRange = Range.Create(15, 35);
            asteroids.ScaleRange = Range.Create(0.2f, 0.8f);
            asteroids.RadiusOffsetJitter = 10f;
            asteroids.AngleJitter = 0.8f;
            asteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige,
            };

            sparseAsteroids = new RingTexturesInfo();
            sparseAsteroids.Textures = asteroids.Textures;
            sparseAsteroids.TexturesAlphaData = asteroids.TexturesAlphaData;
            sparseAsteroids.DensityRange = Range.Create(5, 15);
            sparseAsteroids.ScaleRange = Range.Create(0.1f, 0.6f);
            sparseAsteroids.RadiusOffsetJitter = 10f;
            sparseAsteroids.AngleJitter = 0.8f;
            sparseAsteroids.TextureColors = new[]
            {
                Color.DarkGray.AdjustLight(1.2f),
                Color.LightGray,
                Color.DarkGray,
                Color.Beige,
            };

            dust = LoadData(content, "dust", 1);
            dust.DensityRange = Range.Create(100, 110);
            dust.ScaleRange = Range.Create(0.3f, 1f);
            dust.RadiusOffsetJitter = 10f;
            dust.AngleJitter = 0.8f;
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

        private RingTexturesInfo LoadData(ContentManager content, string imageBaseName, int count)
        {
            var texturesInfo = new RingTexturesInfo
            {
                Textures = new Texture2D[count],
                TexturesAlphaData = new byte[count][]
            };

            for (var i = 0; i < count; i++)
            {
                texturesInfo.Textures[i] = content.Load<Texture2D>(@"Images\" + imageBaseName + "_" + (i + 1));
                texturesInfo.TexturesAlphaData[i] = TextureProcessor.GetAlphaData(texturesInfo.Textures[i]);
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

        internal Ring Create(RingInfo info)
        {
            return new Ring(
                rotationalVelocity: info.RotationalVelocity,
                texturesInfoGroup: GetTexturesInfo(info.Type),
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