﻿using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Ship : ComponentBase
    {
        private readonly Blackhole blackhole;
        private const float DefaultRotationVelocity = MathHelper.TwoPi / 32;

        public Vector2 Position;
        public float Rotation = 0;
        public Texture2D Texture;
        private Texture2D shieldTexture;

        private SoundEffect crashSound;
        private bool stopped = true;

        private Vector2 viewportCenter;

        private Texture2D particleBase;
        private ParticleSystem particleSystem;
        private Emitter emitter;

        public Ship(Blackhole blackhole)
        {
            this.blackhole = blackhole;
        }

        internal float Radius { get; private set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            var shipColor = Color.DarkGray.AdjustLight(0.9f);

            const int padding = 2;
            const int height = 15 + (padding * 2);
            const int width = 15 + (padding * 2);

            const float center = width / 2f;
            const float slope = center / ((float)height - (padding * 2));

            var data = new Color[width * height];

            for (var y = padding; y < height - padding; y++)
            {
                var left = (int)Math.Round(center - (slope * y));
                var right = (int)Math.Round(center + (slope * y));

                for (var x = left; x <= right; x++)
                {
                    data[x + (y * width)] = shipColor;
                }
            }

            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            Texture.SetData(TextureProcessor.SoftenAlpha(data, width, height));

            Texture = content.Load<Texture2D>(@"Images\ship");
            shieldTexture = content.Load<Texture2D>(@"Images\shield");

            crashSound = content.Load<SoundEffect>(@"Sounds\crash_sound");

            particleBase = content.Load<Texture2D>(@"Images\particle_base");
            particleSystem = new ParticleSystem(new Vector2(4000, 3000));
            emitter = particleSystem.AddEmitter(
                secPerSpawn:new Vector2(0.001f, 0.0015f),
                spawnDirection:new Vector2(0f, -1f), 
                spawnNoiseAngle:new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                startLife:new Vector2(0.5f, 0.75f),
                startScale:new Vector2(16, 16),
                endScale:new Vector2(4, 4),
                startColor1:Color.Orange, 
                startColor2:Color.Crimson, 
                endColor1:new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, 0), 
                endColor2:new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, 0),
                startSpeed:new Vector2(400, 500), 
                endSpeed:new Vector2(100, 120), 
                budget:500, 
                relPosition:Vector2.Zero, 
                particleSprite:particleBase);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shieldTexture, Position,
                origin: new Vector2(shieldTexture.Width / 2f, shieldTexture.Height / 2f),
                rotation: Rotation,
                depth: Depth - 0.0002f);

            particleSystem.Draw(spriteBatch, 1, Vector2.Zero, Depth - 0.0001f);

            spriteBatch.Draw(Texture, Position,
                origin: new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                rotation: Rotation,
                depth: Depth);
        }

        private bool Left(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right)) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X < viewportCenter.X));
        }

        private bool Right(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X > viewportCenter.Y));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (stopped)
            {
                return;
            }

            Rotation += DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            const float moveSpeed = 1.1f;

            emitter.Spawning = false;

            var left = false;

            if (Left(inputState.KeyState, inputState.TouchState))
            {
                Rotation -= (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                emitter.Spawning = true;
                left = true;
            }

            if (Right(inputState.KeyState, inputState.TouchState))
            {
                Rotation += (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                emitter.Spawning = true;
            }

            Rotation = MathHelper.WrapAngle(Rotation);

            const float radiusPadding = 5;

            Radius = (blackhole.Height / 2f) + (Texture.Height / 2f) + radiusPadding;

            Position.Y = blackhole.Position.Y - ((float)Math.Cos(Rotation) * Radius);
            Position.X = blackhole.Position.X + ((float)Math.Sin(Rotation) * Radius);

            particleSystem.Position = Position;
            emitter.SpawnDirection = (viewportCenter - Position);
            emitter.SpawnDirection = new Vector2(-emitter.SpawnDirection.Y, emitter.SpawnDirection.X);

            if (left)
            {
                emitter.SpawnDirection = new Vector2(-emitter.SpawnDirection.X, -emitter.SpawnDirection.Y);
            }
            
            emitter.SpawnDirection.Normalize();
            

            particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        internal void Initialize()
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f);
        }

        internal void Start()
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f);

            stopped = false;
        }

        internal void Stop()
        {
            crashSound.Play();

            stopped = true;
        }
    }
}