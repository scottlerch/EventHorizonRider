using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class Ring : ComponentBase
{
    private struct EdgeModifier
    {
        public bool IsEdge;
        public float EdgeScale;
    }

    private class RingLayer
    {
        public Vector2 ShadowOffset { get; set; }

        public float ShadowDepth { get; set; }
    }

    private const float DepthStep = 0.0001f;

    private readonly Random random = new Random();
    private readonly Vector2 origin;
    private readonly List<RingObject> ringObjects;
    private readonly List<RingLayer> ringLayers; 
    private readonly float rotationalVelocity;

    private float innerRadius;
    private float maxRadius;
    private bool isStopped;
    private readonly float ringCollapseSpeed;

    public Ring(
        float ringCollapseSpeed,
        float rotationalVelocity,
        RingTexturesInfoGroup texturesInfoGroup,
        float innerRadius,
        Vector2 origin,
        float spiralRadius,
        float spiralSpeed,
        IList<RingGap> gaps)
    {
        InnerRadius = innerRadius;
        Width = Math.Abs(spiralRadius);

        this.ringCollapseSpeed = ringCollapseSpeed;
        this.rotationalVelocity = rotationalVelocity;
        this.origin = origin;

        ringLayers = new List<RingLayer>();
        ringObjects = CreateRingObjects(spiralSpeed, spiralRadius, texturesInfoGroup, gaps);
    }

    public Color Color { get; set; }

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

    public bool Intersects(Ship ship)
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

    public void Stop()
    {
        isStopped = true;
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        foreach (var ringObject in ringObjects)
        {
            if (ringObject.ShadowTexture != null)
            {
                spriteBatch.Draw(
                    ringObject.ShadowTexture,
                    ringObject.Position + ringLayers[ringObject.LayerIndex].ShadowOffset * ringObject.Scale,
                    sourceRectangle: null,
                    origin: ringObject.Origin,
                    color: Color.Lerp(Color.White, Color, 0.3f) * 0.8f,
                    rotation: ringObject.Rotation,
                    layerDepth: ringObject.RelativeDepth + (Depth - 0.00001f) - ringLayers[ringObject.LayerIndex].ShadowDepth,
                    scale: ringObject.Scale,
                    effects: SpriteEffects.None);
            }

            spriteBatch.Draw(
                ringObject.Texture,
                ringObject.Position,
                sourceRectangle: null,
                origin: ringObject.Origin,
                color: Color.Lerp(ringObject.Color, Color, 0.3f).AdjustLight(ringObject.ColorLightness),
                rotation: ringObject.Rotation,
                layerDepth: ringObject.RelativeDepth + Depth,
                scale: ringObject.Scale,
                effects: SpriteEffects.None);
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

    private List<RingObject> CreateRingObjects(
        float spiralSpeed,
        float spiralRadius,
        RingTexturesInfoGroup texturesInfoGroup,
        IList<RingGap> gaps)
    {
        // TODO: the code below is a mess, needs to be cleaned up...

        var newRingObjects = new List<RingObject>();
        var currentDepthOffset = 0f;
        int index = 0;
        var spiralRotations = (spiralSpeed * (Math.Abs(spiralRadius) / ringCollapseSpeed)) / MathHelper.TwoPi;
        var isSpiral = spiralRadius > 0 || spiralRadius < 0;
        var maximumAngle = isSpiral ? MathHelper.TwoPi * spiralRotations : MathHelper.TwoPi;

        foreach (var texturesInfo in texturesInfoGroup.TextureInfos)
        {
            var count = 0;

            var maximumAsteroidsPerRing = texturesInfo.DensityRange.GetRandom();

            var minimumAngleSpacing = MathHelper.TwoPi / texturesInfo.DensityRange.High;
            var angleSpacing = MathHelper.TwoPi / maximumAsteroidsPerRing;

            var radiusOffset = 0f;
            var spiralRadiusOffset = Math.Abs(spiralRadius) / (maximumAsteroidsPerRing * spiralRotations);

            Action<float, float, Action<float>> angleIterator = spiralRadius >= 0 ? 
                (Action<float, float, Action<float>>)ClockwiseAngles : 
                CounterClockwiseAngles;

            angleIterator(maximumAngle, angleSpacing, angle =>
            {
                if (isSpiral)
                {
                    radiusOffset += spiralRadiusOffset;
                }

                if (gaps.Any(gap => gap.IsInsideGap(angle)))
                    return;

                var edgeModifier = CalculateEdgeModifer(
                    gaps, 
                    angle,
                    minimumAngleSpacing,
                    maximumAngle, 
                    isSpiral, 
                    texturesInfo.TaperAmount, 
                    texturesInfo.TaperScale);

                var textureIndex = random.Next(0, texturesInfo.Textures.Length);

                var ringObject = new RingObject
                {
                    LayerIndex = ringLayers.Count,
                    Texture = texturesInfo.Textures[textureIndex],
                    ShadowTexture = texturesInfo.ShadowTextures != null ? texturesInfo.ShadowTextures[textureIndex] : null,
                    CollisionInfo = texturesInfo.CollisionInfos[textureIndex],
                    Rotation = MathHelper.WrapAngle((float)random.NextDouble() * MathHelper.TwoPi),
                    RotationRate = (float)random.NextDouble() * MathHelper.TwoPi / 4f * (random.Next(2) == 0 ? -1f : 1f),
                    Scale = Vector2.One * texturesInfo.ScaleRange.ScaleHigh(edgeModifier.EdgeScale).GetRandom(),
                    Origin = new Vector2(texturesInfo.Textures[textureIndex].Width / 2f, texturesInfo.Textures[textureIndex].Height / 2f),
                    RadiusOffset = ((float)random.NextDouble() * texturesInfo.RadiusOffsetJitter) + radiusOffset,
                    Color = texturesInfo.TextureColors[random.Next(0, texturesInfo.TextureColors.Length)],
                    Angle = angle + ((edgeModifier.IsEdge ? 0f : edgeModifier.EdgeScale) * ((float)random.NextDouble() * (texturesInfo.AngleJitter * angleSpacing))),
                };

                ringObject.UpdatePosition(origin, InnerRadius);

                newRingObjects.Add(ringObject);
                count++;
            });

            if (texturesInfoGroup.Mode == RingTexturesInfoGroupMode.Sequential)
            {
                RandomizeDepthOrder(newRingObjects, index, count, currentDepthOffset);
                index = count;
            }

            currentDepthOffset = newRingObjects.Count * DepthStep;

            ringLayers.Add(new RingLayer
            {
                ShadowOffset = texturesInfo.ShadowOffset,
                ShadowDepth = texturesInfo.MergeShadows ? DepthStep * count : DepthStep / 10,
            });
        }

        if (texturesInfoGroup.Mode == RingTexturesInfoGroupMode.Interleave)
        {
            RandomizeDepthOrder(newRingObjects, 0, newRingObjects.Count);
        }

        // Reverse so removal of objects is optimized
        newRingObjects.Reverse();

        return newRingObjects;
    }

    private void ClockwiseAngles(float maximumAngle, float angleSpacing, Action<float> action)
    {
        for (var angle = 0f; angle < maximumAngle; angle += angleSpacing)
        {
            action(angle);
        }
    }

    private void CounterClockwiseAngles(float maximumAngle, float angleSpacing, Action<float> action)
    {
        for (var angle = maximumAngle; angle > 0f; angle -= angleSpacing)
        {
            action(angle);
        }
    }

    /// <summary>
    /// This will calculate if objects near the edge of gap within a ring
    /// need to be modified, like tapering off in size by adjusting the scale.
    /// </summary>
    private EdgeModifier CalculateEdgeModifer(
        IList<RingGap> gaps,
        float angle,
        float minimumAngleSpacing,
        float maximumAngle,
        bool isSpiral,
        int taperAmount,
        float taperScale)
    {
        // When object is closer to gap edge make sure it's scaled smaller with less random jitter
        // so the gaps stay closer to a constant size
        int gapScaleFadeSize = taperAmount;

        var edgeModifier = new EdgeModifier { EdgeScale = 1f, IsEdge = false };

        for (int i = 1; i <= gapScaleFadeSize; i++)
        {
            if (gaps.Any(gap => gap.IsInsideGap(angle + (minimumAngleSpacing * i))) ||
                gaps.Any(gap => gap.IsInsideGap(angle - (minimumAngleSpacing * i))))
            {
                edgeModifier.EdgeScale = taperScale + (((i - 1) / (float)gapScaleFadeSize) * taperScale);
                edgeModifier.IsEdge = i == 1;
                break;
            }

            if (isSpiral)
            {
                if (angle < (minimumAngleSpacing * i) || angle > (maximumAngle - (minimumAngleSpacing * i)))
                {
                    edgeModifier.EdgeScale = taperScale + (((i - 1) / (float)gapScaleFadeSize) * taperScale);
                    edgeModifier.IsEdge = i == 1;
                    break;
                }
            }
        }

        return edgeModifier;
    }

    private void RandomizeDepthOrder(List<RingObject> objects, int startIndex, int count, float baseDepth = 0f)
    {
        var index = startIndex;
        foreach (var depthOffset in Enumerable
            .Range(0, count)
            .Select(i => i * DepthStep)
            .OrderBy(x => random.Next()))
        {
            objects[index++].RelativeDepth = baseDepth + depthOffset;
        }
    }
}