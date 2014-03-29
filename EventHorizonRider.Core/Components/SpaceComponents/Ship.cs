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
    internal class Ship : ComponentBase, ISpriteInfo
    {
        private readonly Blackhole blackhole;
        private const float DefaultRotationVelocity = MathHelper.TwoPi / 32;

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Texture2D Texture { get; set; }
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

            Texture = content.Load<Texture2D>(@"Images\ship");
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);

            shieldTexture = content.Load<Texture2D>(@"Images\shield");

            crashSound = content.Load<SoundEffect>(@"Sounds\crash_sound");

            particleBase = content.Load<Texture2D>(@"Images\particle_base");
            particleSystem = new ParticleSystem(new Vector2(4000, 3000));
            emitter = particleSystem.AddEmitter(
                secPerSpawn:new Vector2(0.001f, 0.0015f),
                spawnDirection:new Vector2(0f, -1f), 
                spawnNoiseAngle:new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                startLife:new Vector2(0.5f, 0.75f),
                startScale:new Vector2(20, 20),
                endScale:new Vector2(8, 8),
                startColor1:Color.Orange, 
                startColor2:Color.Crimson, 
                endColor1:new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, 0), 
                endColor2:new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, 0),
                startSpeed:new Vector2(400, 500), 
                endSpeed:new Vector2(100, 120), 
                budget:500, 
                relPosition:Vector2.Zero, 
                particleSprite:particleBase);

            emitter.GravityCenter = viewportCenter;
            emitter.GravityForce = 1.3f;

            Origin = new Vector2(Texture.Width/2f, Texture.Height/2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shieldTexture, Position,
                origin: new Vector2(shieldTexture.Width / 2f, shieldTexture.Height / 2f),
                rotation: Rotation,
                depth: Depth - 0.0002f);

            particleSystem.Draw(spriteBatch, 1, Vector2.Zero, Depth - 0.0001f);

            spriteBatch.Draw(
                Texture, 
                Position,
                origin: Origin,
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

            const float radiusPadding = 20;

            Radius = (blackhole.Height / 2f) + (Texture.Height / 2f) + radiusPadding;

            Position = new Vector2(
                blackhole.Position.X + ((float)Math.Sin(Rotation) * Radius),
                blackhole.Position.Y - ((float)Math.Cos(Rotation) * Radius));

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

            Position = new Vector2(
                blackhole.Position.X,
                blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f));
        }

        internal void Start()
        {
            Rotation = 0;

            Position = new Vector2(
                blackhole.Position.X,
                blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f));

            stopped = false;
        }

        internal void Stop()
        {
            crashSound.Play();

            stopped = true;
        }

        public Vector2 Origin
        {
            get; set;
        }

        public Vector2 Scale
        {
            get { return Vector2.One; }
        }

        public Color[] TextureData{get; private set; }
    }
}