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
        private static readonly Color[] SegmentColors =
        {
            Color.DarkGray,
            Color.LightGray,
            Color.DarkGray.AdjustLight(0.8f)
        };

        private readonly Vector2 origin;
        private readonly Asteriod[] asteroids;
        private readonly float rotationalVelocity;

        private float radius;
        private float maxRadius;
        private bool isStopped;

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
            Texture2D[] textures,
            Color[][] texturesData,
            float radius,
            Vector2 origin,
            List<RingGap> gaps)
        {
            Radius = radius;

            this.rotationalVelocity = rotationalVelocity;
            this.origin = origin;
            
            var newAsteroids = new List<Asteriod>();
            var random = new Random();

            var maximumAsteroidsPerRing = (int)MathLib.GetRandomBetween(30, 50);

            var depthOffset = new float[maximumAsteroidsPerRing];
            for (int i = 0; i < depthOffset.Length; i++)
            {
                depthOffset[i] = i*0.0001f;
            }
            depthOffset = depthOffset.OrderBy(x => random.Next()).ToArray();

            var angleSpacing = MathHelper.TwoPi/maximumAsteroidsPerRing;
            var index = 0;

            for (var angle = -MathHelper.Pi; angle < MathHelper.Pi; angle += angleSpacing)
            {
                if (gaps.Any(gap => gap.IsInsideGap(angle)))
                    continue;

                var textureIndex = random.Next(0, textures.Length);

                var asteroid = new Asteriod
                {
                    RelativeDepth = depthOffset[index],
                    Texture = textures[textureIndex],
                    TextureData = texturesData[textureIndex],
                    Rotation = MathHelper.WrapAngle((float) random.NextDouble()*MathHelper.TwoPi),
                    RotationRate = (float)random.NextDouble() * MathHelper.TwoPi / 4f * (random.Next(2) == 0 ? -1f : 1f),
                    Scale = Vector2.One* MathLib.GetRandomBetween(0.2f, 0.8f),
                    Origin = new Vector2(textures[textureIndex].Width/2f, textures[textureIndex].Height/2f),
                    RadiusOffset = (float) random.NextDouble()*10f,
                    Color = SegmentColors[random.Next(0, SegmentColors.Length)],
                    Angle = angle + ((float) random.NextDouble()*(0.5f*angleSpacing)),
                };

                asteroid.UpdatePosition(origin, Radius);

                newAsteroids.Add(asteroid);

                index++;
            }

            asteroids = newAsteroids.ToArray();
        }

        private float asteroidColorOffset;

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            foreach (var asteroid in asteroids)
            {
                spriteBatch.Draw(
                    asteroid.Texture,
                    asteroid.Position,
                    origin: asteroid.Origin,
                    color: asteroid.Color.AdjustLight(asteroidColorOffset),
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

                const float low = 0.3f;
                const float high = 0.6f;
                const float diff = high - low;

                var lightness = (scale * diff) + low;

                asteroidColorOffset = lightness;

                var rotationalOffset = rotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = 0 ; i < asteroids.Length; i++)
                {
                    asteroids[i].Angle += rotationalOffset;
                    asteroids[i].Rotation += asteroids[i].RotationRate*(float)gameTime.ElapsedGameTime.TotalSeconds;
                    asteroids[i].UpdatePosition(origin, Radius);
                }
            }
        }

        internal bool Intersects(Ship ship)
        {
            // TODO: add heuristics to optimize collision detection

            //if (IsInsideRing(ship))
            //{

            for (var i = 0; i < asteroids.Length; i++)
            {
                if (CollisionDetection.Collides(ship, asteroids[i]))
                {
                    return true;
                }
            }

            //}

            return false;
        }

        /*
        private bool IsInsideRing(Ship ship)
        {
            var ringWidth = textures[0].Height;

            var startRingEdge = Radius - (ringWidth / 2f);
            var endRingEdge = Radius + (ringWidth / 2f);

            var shipFrontEdge = (ringOrigin - ship.Position).Length() + (ship.Texture.Height / 2f);

            return shipFrontEdge.IsBetween(startRingEdge, endRingEdge);
        }

        private bool IsInsideGap(Ship ship, RingGap ringGap)
        {
            var gapEdges = GetGapEdges(ship, ringGap);

            return ship.Rotation.IsBetweenAngles(gapEdges.Start, gapEdges.End);
        }

        private Range<float> GetGapEdges(Ship ship, RingGap ringGap)
        {
            var textureOffset = (float)Math.Asin((textures[0].Width / 2f) / ship.Radius);

            var halfGap = (ringGap.GapSize / 2f) - textureOffset;

            var gapStartAngle = (ringGap.GapAngle + rotationalOffset) - halfGap;
            var gapEndAngle = (ringGap.GapAngle + rotationalOffset) + halfGap;

            return new Range<float>
            {
                Start = MathHelper.WrapAngle(gapStartAngle),
                End = MathHelper.WrapAngle(gapEndAngle)
            };
        }
         */

        internal void Stop()
        {
            isStopped = true;
        }
    }
}