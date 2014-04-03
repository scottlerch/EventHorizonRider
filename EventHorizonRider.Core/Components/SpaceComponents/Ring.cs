using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Ring : ComponentBase
    {
        private readonly Vector2 origin;
        private readonly RingObject[] ringObjects;
        private readonly float rotationalVelocity;

        private float radius;
        private float maxRadius;
        private bool isStopped;
        private float ringObjectsColorOffset;

        public float Radius
        {
            get { return radius; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (radius == 0)
                {
                    maxRadius = value;
                }

                radius = value;
            }
        }

        public bool ConsumedByBlackhole { get; set; }

        public Ring(
            float rotationalVelocity,
            RingTexturesInfoGroup texturesInfoGroup,
            float radius,
            Vector2 origin,
            List<RingGap> gaps)
        {
            Radius = radius;

            this.rotationalVelocity = rotationalVelocity;
            this.origin = origin;
            
            var newRingObjects = new List<RingObject>();
            var random = new Random();
            var currentDepthOffset = 0f;
            int index = 0;

            foreach (var texturesInfo in texturesInfoGroup.TextureInfos)
            {
                var maximumAsteroidsPerRing = texturesInfo.DensityRange.GetRandom();

                var angleSpacing = MathHelper.TwoPi/maximumAsteroidsPerRing;
                var count = 0;

                for (var angle = -MathHelper.Pi; angle < MathHelper.Pi; angle += angleSpacing)
                {
                    if (gaps.Any(gap => gap.IsInsideGap(angle)))
                        continue;

                    var textureIndex = random.Next(0, texturesInfo.Textures.Length);

                    var ringObject = new RingObject
                    {
                        Texture = texturesInfo.Textures[textureIndex],
                        TextureAlphaData = texturesInfo.TexturesAlphaData[textureIndex],
                        Rotation = MathHelper.WrapAngle((float) random.NextDouble()*MathHelper.TwoPi),
                        RotationRate = (float) random.NextDouble()*MathHelper.TwoPi/4f*(random.Next(2) == 0 ? -1f : 1f),
                        Scale = Vector2.One*texturesInfo.ScaleRange.GetRandom(),
                        Origin =
                            new Vector2(texturesInfo.Textures[textureIndex].Width/2f,
                                texturesInfo.Textures[textureIndex].Height/2f),
                        RadiusOffset = (float) random.NextDouble()*texturesInfo.RadiusOffsetJitter,
                        Color = texturesInfo.TextureColors[random.Next(0, texturesInfo.TextureColors.Length)],
                        Angle = angle + ((float) random.NextDouble()*(texturesInfo.AngleJitter*angleSpacing)),
                    };

                    ringObject.UpdatePosition(origin, Radius);

                    newRingObjects.Add(ringObject);
                    count++;
                }

                if (texturesInfoGroup.Mode == RingTexturesInfoGroupMode.Sequential)
                {
                    foreach (var depthOffset in Enumerable
                        .Range(0, count)
                        .Select(i => i * 0.0001f)
                        .OrderBy(x => random.Next()))
                    {
                        newRingObjects[index++].RelativeDepth = currentDepthOffset + depthOffset;
                    }
                }

                currentDepthOffset = newRingObjects.Count*0.0001f;
            }

            if (texturesInfoGroup.Mode == RingTexturesInfoGroupMode.Interleave)
            {
                index = 0;
                foreach (var depthOffset in Enumerable
                    .Range(0, newRingObjects.Count)
                    .Select(i => i*0.0001f)
                    .OrderBy(x => random.Next()))
                {
                    newRingObjects[index++].RelativeDepth = depthOffset;
                }
            }

            ringObjects = newRingObjects.ToArray();
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            foreach (var asteroid in ringObjects)
            {
                spriteBatch.Draw(
                    asteroid.Texture,
                    asteroid.Position,
                    origin: asteroid.Origin,
                    color: asteroid.Color.AdjustLight(ringObjectsColorOffset),
                    rotation: asteroid.Rotation,
                    depth: asteroid.RelativeDepth + Depth,
                    scale: asteroid.Scale);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (!isStopped)
            {
                var scale = 1f - (Radius / maxRadius);

                const float low = 0.1f;
                const float high = 0.6f;
                const float diff = high - low;

                var lightness = (scale * diff) + low;

                ringObjectsColorOffset = lightness;

                var rotationalOffset = rotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = 0 ; i < ringObjects.Length; i++)
                {
                    ringObjects[i].Angle += rotationalOffset;
                    ringObjects[i].Rotation += ringObjects[i].RotationRate*(float)gameTime.ElapsedGameTime.TotalSeconds;
                    ringObjects[i].UpdatePosition(origin, Radius);
                }
            }
        }

        internal bool Intersects(Ship ship)
        {
            // TODO: add heuristics to optimize collision detection

            for (var i = 0; i < ringObjects.Length; i++)
            {
                if (CollisionDetection.Collides(ship, ringObjects[i]))
                {
                    return true;
                }
            }

            return false;
        }

        internal void Stop()
        {
            isStopped = true;
        }
    }
}