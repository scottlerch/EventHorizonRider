using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Physics
{
    internal class ParticleSystem
    {
        private readonly Random random;
        private Vector2 position;

        public List<Emitter> EmitterList { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { LastPos = position; position = value; }
        }

        public Vector2 LastPos { get; set; }

        public ParticleSystem(Vector2 position = default(Vector2))
        {
            Position = position;
            LastPos = position;
            random = new Random();
            EmitterList = new List<Emitter>();
        }

        public void Update(float dt)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Update(dt);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int scale, Vector2 offset, float depth)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Draw(spriteBatch, scale, offset, depth);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Clear();
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
                random,
                this);

            EmitterList.Add(emitter);

            return emitter;
        }
    }
}
