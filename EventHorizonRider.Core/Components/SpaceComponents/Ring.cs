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
        private readonly List<RingObject> ringObjects;
        private readonly float rotationalVelocity;

        private float innerRadius;
        private float maxRadius;
        private bool isStopped;
        private float ringCollapseSpeed;

        public float InnerRadius
        {
            get { return innerRadius; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (innerRadius == 0)
                {
                    maxRadius = value;
                }

                innerRadius = value;
            }
        }

        public float OutterRadius { get { return InnerRadius + Width; } }

        public float Width { get; private set; }

        public bool ConsumedByBlackhole { get; set; }

        public Ring(
            float ringCollapseSpeed,
            float rotationalVelocity,
            RingTexturesInfoGroup texturesInfoGroup,
            float innerRadius,
            Vector2 origin,
            float spiralRadius,
            float spiralSpeed,
            List<RingGap> gaps)
        {
            InnerRadius = innerRadius;
            Width = spiralRadius;

            this.ringCollapseSpeed = ringCollapseSpeed;
            this.rotationalVelocity = rotationalVelocity;
            this.origin = origin;
            
            var newRingObjects = new List<RingObject>();
            var random = new Random();
            var currentDepthOffset = 0f;
            int index = 0;
            var spiralRotations = (spiralSpeed*(spiralRadius/ringCollapseSpeed)) / MathHelper.TwoPi;
            var isSpiral = spiralRadius > 0;

            foreach (var texturesInfo in texturesInfoGroup.TextureInfos)
            {
                var maximumAsteroidsPerRing = texturesInfo.DensityRange.GetRandom();

                var minimumAngleSpacing = MathHelper.TwoPi/texturesInfo.DensityRange.High; 
                var angleSpacing = MathHelper.TwoPi/maximumAsteroidsPerRing;
                var count = 0;
                var radiusOffset = 0f;

                var spiralRadiusOffset = spiralRadius / (maximumAsteroidsPerRing * spiralRotations);
                var maximumAngle = isSpiral? MathHelper.TwoPi*spiralRotations : MathHelper.TwoPi;

                for (var angle = 0f; angle < maximumAngle; angle += angleSpacing)
                {
                    if (isSpiral)
                    {
                        radiusOffset += spiralRadiusOffset;
                    }

                    if (gaps.Any(gap => gap.IsInsideGap(angle)))
                        continue;

                    // When object is closer to gap edge make sure it's scaled smaller with less random jitter
                    // so the gaps stay closer to a constant size
                    const int gapScaleFadeSize = 2;
                    float gapScaleFade = 1f;
                    bool isGapEdge = false;

                    for (int i = 1; i <= gapScaleFadeSize; i++)
                    {
                        if (gaps.Any(gap => gap.IsInsideGap(angle + (minimumAngleSpacing * i))) ||
                            gaps.Any(gap => gap.IsInsideGap(angle - (minimumAngleSpacing * i))))
                        {
                            gapScaleFade = 0.5f + (((i - 1) / (float)gapScaleFadeSize) * 0.5f);
                            isGapEdge = i == 1;
                            break;
                        }

                        if (isSpiral)
                        {
                            if (angle < (minimumAngleSpacing * i) || angle > (maximumAngle - (minimumAngleSpacing * i)))
                            {
                                gapScaleFade = 0.5f + (((i - 1) / (float)gapScaleFadeSize) * 0.5f);
                                isGapEdge = i == 1;
                                break;
                            }
                        }
                    }

                    var textureIndex = random.Next(0, texturesInfo.Textures.Length);

                    var ringObject = new RingObject
                    {
                        Texture = texturesInfo.Textures[textureIndex],
                        TextureAlphaData = texturesInfo.TexturesAlphaData[textureIndex],
                        Rotation = MathHelper.WrapAngle((float) random.NextDouble()*MathHelper.TwoPi),
                        RotationRate = (float) random.NextDouble()*MathHelper.TwoPi/4f*(random.Next(2) == 0 ? -1f : 1f),
                        Scale = Vector2.One * texturesInfo.ScaleRange.ScaleHigh(gapScaleFade).GetRandom(),
                        Origin =
                            new Vector2(texturesInfo.Textures[textureIndex].Width/2f,
                                texturesInfo.Textures[textureIndex].Height/2f),
                        RadiusOffset = ((float) random.NextDouble()*texturesInfo.RadiusOffsetJitter) + radiusOffset,
                        Color = texturesInfo.TextureColors[random.Next(0, texturesInfo.TextureColors.Length)],
                        Angle = angle + ((isGapEdge? 0f : gapScaleFade) * ((float) random.NextDouble()*(texturesInfo.AngleJitter*angleSpacing))),
                    };

                    ringObject.UpdatePosition(origin, InnerRadius);

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

            // Reverse so removal of objects is optimized
            newRingObjects.Reverse();

            ringObjects = newRingObjects;
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            foreach (var ringObject in ringObjects)
            {
                spriteBatch.Draw(
                    ringObject.Texture,
                    ringObject.Position,
                    origin: ringObject.Origin,
                    color: ringObject.Color.AdjustLight(ringObject.ColorLightness),
                    rotation: ringObject.Rotation,
                    depth: ringObject.RelativeDepth + Depth,
                    scale: ringObject.Scale);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (!isStopped)
            {
                InnerRadius -= (float)gameTime.ElapsedGameTime.TotalSeconds * ringCollapseSpeed;

                const float low = 0.1f;
                const float high = 0.6f;
                const float diff = high - low;

                var rotationalOffset = rotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = ringObjects.Count - 1; i >= 0; i--)
                {
                    var scale = 1f - ((InnerRadius + ringObjects[i].RadiusOffset) / maxRadius);

                    ringObjects[i].ColorLightness = (scale*diff) + low;
                    ringObjects[i].Angle += rotationalOffset;
                    ringObjects[i].Rotation += ringObjects[i].RotationRate*(float)gameTime.ElapsedGameTime.TotalSeconds;
                    ringObjects[i].UpdatePosition(origin, InnerRadius);

                    if ((ringObjects[i].Position - origin).LengthSquared() <= 50f) // TODO: where to get this fudge factor?
                    {
                        ringObjects.RemoveAt(i);
                    }
                }
            }
        }

        internal bool Intersects(Ship ship)
        {
            // TODO: add heuristics to optimize collision detection

            for (var i = 0; i < ringObjects.Count; i++)
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