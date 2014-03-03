using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Physics
{
    internal class Emitter
    {
        private readonly Random random;                   // Pointer to a random object passed trough constructor.
        private float nextSpawnIn;                      // This is a random number generated using the SecPerSpawn.
        private float secPassed;                        // Time pased since last spawn.
        private readonly LinkedList<Particle> activeParticles;   // A list of all the active particles.

        public Vector2 RelPosition;             // Position relative to collection.
        public int Budget;                      // Max number of alive particles.
        public Texture2D ParticleSprite;        // This is what the particle looks like.
        public Vector2 SecPerSpawn;
        public Vector2 SpawnDirection;
        public Vector2 SpawnNoiseAngle;
        public Vector2 StartLife;
        public Vector2 StartScale;
        public Vector2 EndScale;
        public Color StartColor1;
        public Color StartColor2;
        public Color EndColor1;
        public Color EndColor2;
        public Vector2 StartSpeed;
        public Vector2 EndSpeed;
        public bool Spawning;

        public ParticleSystem Parent;

        public Emitter(
            Vector2 secPerSpawn, 
            Vector2 spawnDirection,
            Vector2 spawnNoiseAngle, 
            Vector2 startLife,
            Vector2 startScale,
            Vector2 endScale, 
            Color startColor1, 
            Color startColor2, 
            Color endColor1, 
            Color endColor2,
            Vector2 startSpeed,
            Vector2 endSpeed,
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
            StartColor1 = startColor1;
            StartColor2 = startColor2;
            EndColor1 = endColor1;
            EndColor2 = endColor2;
            StartSpeed = startSpeed;
            EndSpeed = endSpeed;
            Budget = budget;
            RelPosition = relPosition;
            ParticleSprite = particleSprite;
            Parent = parent;
            activeParticles = new LinkedList<Particle>();
            nextSpawnIn = MathLib.LinearInterpolate(secPerSpawn.X, secPerSpawn.Y, random.NextDouble());
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
                    var startDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                    startDirection.Normalize();
                    var endDirection = startDirection * MathLib.LinearInterpolate(EndSpeed.X, EndSpeed.Y, random.NextDouble());

                    startDirection *= MathLib.LinearInterpolate(StartSpeed.X, StartSpeed.Y, random.NextDouble());

                    activeParticles.AddLast(
                        new Particle(
                            RelPosition + MathLib.LinearInterpolate(Parent.LastPos, Parent.Position, secPassed / dt),
                            startDirection,
                            endDirection,
                            MathLib.LinearInterpolate(StartLife.X, StartLife.Y, random.NextDouble()),
                            MathLib.LinearInterpolate(StartScale.X, StartScale.Y, random.NextDouble()),
                            MathLib.LinearInterpolate(EndScale.X, EndScale.Y, random.NextDouble()),
                            MathLib.LinearInterpolate(StartColor1, StartColor2, random.NextDouble()),
                            MathLib.LinearInterpolate(EndColor1, EndColor2, random.NextDouble()),
                            this)
                    );
                    activeParticles.Last.Value.Update(secPassed);
                }
                secPassed -= nextSpawnIn;
                nextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
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
