using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private readonly Random _random = random;
    private float _nextSpawnIn = MathUtilities.Lerp(secPerSpawn, (float)random.NextDouble());
    private float _secPassed = 0.0f;
    private Particle[] _particles = new Particle[budget];
    private Queue<int> _activeParticleIndices = new(Enumerable.Range(0, budget));

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
        _secPassed += dt;

        while (_secPassed > _nextSpawnIn)
        {
            if (_activeParticleIndices.Count > 0 && Spawning)
            {
                // Spawn a particle
                var startDirection = Vector2.Transform(
                    SpawnDirection,
                    Matrix.CreateRotationZ(MathUtilities.Lerp(SpawnNoiseAngle, (float)_random.NextDouble())));

                startDirection.Normalize();

                var endDirection = startDirection * MathUtilities.Lerp(EndSpeed, (float)_random.NextDouble());

                startDirection *= MathUtilities.Lerp(StartSpeed, (float)_random.NextDouble());

                var nextParticleIndex = _activeParticleIndices.Dequeue();

                _particles[nextParticleIndex] =
                    new Particle(
                        RelPosition + Vector2.Lerp(Parent.LastPos, Parent.Position, _secPassed / dt),
                        startDirection,
                        endDirection,
                        MathUtilities.Lerp(StartLife, (float)_random.NextDouble()),
                        MathUtilities.Lerp(StartScale, (float)_random.NextDouble()),
                        MathUtilities.Lerp(EndScale, (float)_random.NextDouble()),
                        MathUtilities.Lerp(StartColor, (float)_random.NextDouble()),
                        MathUtilities.Lerp(EndColor, (float)_random.NextDouble()),
                        this);

                _particles[nextParticleIndex].Update(_secPassed);
            }

            _secPassed -= _nextSpawnIn;
            _nextSpawnIn = MathUtilities.Lerp(SecPerSpawn, (float)_random.NextDouble());
        }

        var count = _particles.Length;
        for (var i = 0; i < count; i++)
        {
            if (_particles[i].IsAlive)
            {
                var isAlive = _particles[i].Update(dt);
                if (!isAlive)
                {
                    _activeParticleIndices.Enqueue(i);
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, float depth)
    {
        var count = _particles.Length;
        for (var i = 0; i < count; i++)
        {
            _particles[i].Draw(spriteBatch, depth);
        }
    }

    public void Clear()
    {
        _particles = new Particle[_particles.Length];
        _activeParticleIndices = new Queue<int>(Enumerable.Range(0, _particles.Length));
    }
}
