using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Physics;

internal class ParticleSystem
{
    private readonly Random _random;
    private Vector2 _position;

    public ParticleSystem(Vector2 position = default)
    {
        Position = position;
        LastPos = position;
        _random = new Random();
        EmitterList = [];
    }

    public List<Emitter> EmitterList { get; set; }

    public Vector2 Position
    {
        get => _position;
        set { LastPos = _position; _position = value; }
    }

    public Vector2 LastPos { get; set; }

    public void Update(float dt)
    {
        foreach (var emitter in EmitterList)
        {
            if (emitter.Budget > 0)
            {
                emitter.Update(dt);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, float depth)
    {
        foreach (var emitter in EmitterList)
        {
            if (emitter.Budget > 0)
            {
                emitter.Draw(spriteBatch, depth);
            }
        }
    }

    public void Clear()
    {
        foreach (var emitter in EmitterList)
        {
            if (emitter.Budget > 0)
            {
                emitter.Clear();
            }
        }
    }

    public Emitter AddEmitter(
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
        Texture2D particleSprite)
    {
        var emitter = new Emitter(
            secPerSpawn,
            spawnDirection,
            spawnNoiseAngle,
            startLife,
            startScale,
            endScale,
            startColor,
            endColor,
            startSpeed,
            endSpeed,
            budget,
            relPosition,
            particleSprite,
            _random,
            this);

        EmitterList.Add(emitter);

        return emitter;
    }
}
