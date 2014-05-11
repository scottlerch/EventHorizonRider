using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Physics
{
    internal class Emitter
    {
        private readonly Random random; 
        private float nextSpawnIn;
        private float secPassed;
        private readonly LinkedList<Particle> activeParticles;

        public Vector2 RelPosition { get; set; }
        public int Budget { get; set; }
        public Texture2D ParticleSprite { get; set; }
        public Range<float> SecPerSpawn { get; set; }
        public Vector2 SpawnDirection { get; set; }
        public Range<float> SpawnNoiseAngle { get; set; }
        public Range<float> StartLife { get; set; }
        public Range<float> StartScale { get; set; }
        public Range<float> EndScale { get; set; }
        public Range<Color> StartColor { get; set; }
        public Range<Color> EndColor { get; set; }
        public Range<float> StartSpeed { get; set; }
        public Range<float> EndSpeed { get; set; }
        public bool Spawning { get; set; }
        public Vector2 GravityCenter { get; set; }
        public float GravityForce { get; set; }

        public ParticleSystem Parent { get; set; }

        public Emitter(
            Range<float> secPerSpawn, 
            Vector2 spawnDirection,
            Range<float> spawnNoiseAngle,
            Range<float> startLife,
            Range<float> startScale,
            Range<float> endScale, 
            Range<Color> startColor,
            Range<Color> endColor,
            Range<float> startSpeed,
            Range<float> endSpeed,
            int budget, 
            Vector2 relPosition, 
            Texture2D particleSprite, 
            Random random, 
            ParticleSystem parent)
        {
            SecPerSpawn = secPerSpawn;
            SpawnDirection = spawnDirection;
            SpawnNoiseAngle = spawnNoiseAngle;
            StartLife = startLife;
            StartScale = startScale;
            EndScale = endScale;
            StartColor = startColor;
            EndColor = endColor;
            StartSpeed = startSpeed;
            EndSpeed = endSpeed;
            Budget = budget;
            RelPosition = relPosition;
            ParticleSprite = particleSprite;
            Parent = parent;
            activeParticles = new LinkedList<Particle>();
            nextSpawnIn = MathUtilities.LinearInterpolate(secPerSpawn, random.NextDouble());
            secPassed = 0.0f;
            this.random = random;
        }

        public void Update(float dt)
        {
            secPassed += dt;

            while (secPassed > nextSpawnIn)
            {
                if (activeParticles.Count < Budget && Spawning)
                {
                    // Spawn a particle
                    var startDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathUtilities.LinearInterpolate(SpawnNoiseAngle, random.NextDouble())));
                    startDirection.Normalize();
                    var endDirection = startDirection * MathUtilities.LinearInterpolate(EndSpeed, random.NextDouble());

                    startDirection *= MathUtilities.LinearInterpolate(StartSpeed, random.NextDouble());

                    activeParticles.AddLast(
                        new Particle(
                            RelPosition + MathUtilities.LinearInterpolate(Parent.LastPos, Parent.Position, secPassed / dt),
                            startDirection,
                            endDirection,
                            MathUtilities.LinearInterpolate(StartLife, random.NextDouble()),
                            MathUtilities.LinearInterpolate(StartScale, random.NextDouble()),
                            MathUtilities.LinearInterpolate(EndScale, random.NextDouble()),
                            MathUtilities.LinearInterpolate(StartColor, random.NextDouble()),
                            MathUtilities.LinearInterpolate(EndColor, random.NextDouble()),
                            this)
                    );
                    activeParticles.Last.Value.Update(secPassed);
                }
                secPassed -= nextSpawnIn;
                nextSpawnIn = MathUtilities.LinearInterpolate(SecPerSpawn, random.NextDouble());
            }

            var node = activeParticles.First;

            while (node != null)
            {
                var isAlive = node.Value.Update(dt);
                node = node.Next;

                if (isAlive) continue;

                if (node == null)
                {
                    activeParticles.RemoveLast();
                }
                else
                {
                    activeParticles.Remove(node.Previous);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int scale, Vector2 offset, float depth)
        {
            LinkedListNode<Particle> node = activeParticles.First;
            while (node != null)
            {
                node.Value.Draw(spriteBatch, scale, offset, depth);
                node = node.Next;
            }
        }

        public void Clear()
        {
            activeParticles.Clear();
        }
    }
}
