using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Physics;

internal class Emitter(
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
    private readonly Random random = random; 
    private float nextSpawnIn = MathUtilities.Lerp(secPerSpawn, (float)random.NextDouble());
    private float secPassed = 0.0f;
    private Particle[] particles = new Particle[budget];
    private Queue<int> activeParticleIndices = new(Enumerable.Range(0, budget));

    public Vector2 RelPosition { get; set; } = relPosition;

    public int Budget { get; set; } = budget;

    public Texture2D ParticleSprite { get; set; } = particleSprite;

    public Range<float> SecPerSpawn { get; set; } = secPerSpawn;

    public Vector2 SpawnDirection { get; set; } = spawnDirection;

    public Range<float> SpawnNoiseAngle { get; set; } = spawnNoiseAngle;

    public Range<float> StartLife { get; set; } = startLife;

    public Range<float> StartScale { get; set; } = startScale;

    public Range<float> EndScale { get; set; } = endScale;

    public Range<Color> StartColor { get; set; } = startColor;

    public Range<Color> EndColor { get; set; } = endColor;

    public Range<float> StartSpeed { get; set; } = startSpeed;

    public Range<float> EndSpeed { get; set; } = endSpeed;

    public bool Spawning { get; set; }

    public Vector2 GravityCenter { get; set; }

    public float GravityForce { get; set; }

    public ParticleSystem Parent { get; set; } = parent;

    public void Update(float dt)
    {
        secPassed += dt;

        while (secPassed > nextSpawnIn)
        {
            if (activeParticleIndices.Count > 0 && Spawning)
            {
                // Spawn a particle
                var startDirection = Vector2.Transform(
                    SpawnDirection,
                    Matrix.CreateRotationZ(MathUtilities.Lerp(SpawnNoiseAngle, (float)random.NextDouble())));

                startDirection.Normalize();

                var endDirection = startDirection * MathUtilities.Lerp(EndSpeed, (float)random.NextDouble());

                startDirection *= MathUtilities.Lerp(StartSpeed, (float)random.NextDouble());

                int nextParticleIndex = activeParticleIndices.Dequeue();

                particles[nextParticleIndex] = 
                    new Particle(
                        RelPosition + Vector2.Lerp(Parent.LastPos, Parent.Position, secPassed / dt),
                        startDirection,
                        endDirection,
                        MathUtilities.Lerp(StartLife, (float)random.NextDouble()),
                        MathUtilities.Lerp(StartScale, (float)random.NextDouble()),
                        MathUtilities.Lerp(EndScale, (float)random.NextDouble()),
                        MathUtilities.Lerp(StartColor, (float)random.NextDouble()),
                        MathUtilities.Lerp(EndColor, (float)random.NextDouble()),
                        this);

                particles[nextParticleIndex].Update(secPassed);
            }

            secPassed -= nextSpawnIn;
            nextSpawnIn = MathUtilities.Lerp(SecPerSpawn, (float)random.NextDouble());
        }

        int count = particles.Length;
        for (int i = 0; i < count; i++)
        {
            if (particles[i].IsAlive)
            {
                var isAlive = particles[i].Update(dt);
                if (!isAlive) activeParticleIndices.Enqueue(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, float depth)
    {
        int count = particles.Length;
        for (int i = 0; i < count; i++)
        {
            particles[i].Draw(spriteBatch, depth);
        }
    }

    public void Clear()
    {
        particles = new Particle[particles.Length];
        activeParticleIndices = new Queue<int>(Enumerable.Range(0, particles.Length));
    }
}
