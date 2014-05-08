using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
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

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Texture2D Texture { get; set; }

        private Texture2D shieldPusleTexture;
        private Texture2D[] shieldTextures;
        private SoundEffect thrustSound;
        private SoundEffectInstance thrustSoundInstance;

        private SoundEffect crashSound;
        private bool stopped = true;
        private bool visible = true;

        private Vector2 viewportCenter;

        private Texture2D particleBase;
        private ParticleSystem particleSystem;
        private Emitter sideThrustEmitter;
        private Emitter mainThrustEmitter;

        public Ship(Blackhole blackhole)
        {
            this.blackhole = blackhole;
        }

        internal float Radius { get; private set; }

        public float Speed { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);

            thrustSound = content.Load<SoundEffect>(@"Sounds\thrust");
            thrustSoundInstance = thrustSound.CreateInstance();
            thrustSoundInstance.IsLooped = true;

            Texture = content.Load<Texture2D>(@"Images\ship");
            CollisionInfo = CollisionDetection.GetCollisionInfo(Texture, resolution: DeviceInfo.DetailLevel.HasFlag(DetailLevel.CollisionDetectionFull) ? 1f : 0.75f);

            shieldPusleTexture = content.Load<Texture2D>(@"Images\shield_pulse");

            shieldTextures = new Texture2D[3];
            for (int i = 0; i < shieldTextures.Length; i++)
            {
                shieldTextures[i] = content.Load<Texture2D>(@"Images\shield_" + (i+1).ToString());
            }

            crashSound = content.Load<SoundEffect>(@"Sounds\crash_sound");

            particleBase = content.Load<Texture2D>(@"Images\particle_base");
            particleSystem = new ParticleSystem(new Vector2(10000, 10000));
            sideThrustEmitter = particleSystem.AddEmitter(
                secPerSpawn:Range.Create(0.001f, 0.0015f),
                spawnDirection:new Vector2(0f, -1f),
                spawnNoiseAngle: Range.Create(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                startLife: Range.Create(0.5f, 0.75f),
                startScale: Range.Create(20f, 20f),
                endScale: Range.Create(8f, 8f),
                startColor:Range.Create(Color.Orange, Color.Crimson),
                endColor: Range.Create(Color.Orange.AdjustAlpha(0), Color.Orange.AdjustAlpha(0)),
                startSpeed: Range.Create(400f, 500f),
                endSpeed: Range.Create(100f, 120f), 
                budget:500, 
                relPosition:Vector2.Zero, 
                particleSprite:particleBase);

            sideThrustEmitter.GravityCenter = viewportCenter;
            sideThrustEmitter.GravityForce = 1.3f;

            mainThrustEmitter = particleSystem.AddEmitter(
                secPerSpawn: Range.Create(0.001f, 0.0015f),
                spawnDirection: new Vector2(0f, -1f),
                spawnNoiseAngle: Range.Create(0.3f * MathHelper.Pi, 0.3f * -MathHelper.Pi),
                startLife: Range.Create(0.1f, 0.5f),
                startScale: Range.Create(20f, 20f),
                endScale: Range.Create(8f, 8f),
                startColor: Range.Create(Color.SkyBlue, Color.Blue),
                endColor: Range.Create(Color.SkyBlue.AdjustAlpha(0), Color.SkyBlue.AdjustAlpha(0)),
                startSpeed: Range.Create(400f, 500f),
                endSpeed: Range.Create(100f, 120f),
                budget: 50,
                relPosition: Vector2.Zero,
                particleSprite: particleBase);

            mainThrustEmitter.GravityCenter = viewportCenter;
            mainThrustEmitter.GravityForce = 1.3f;
            mainThrustEmitter.Spawning = true;

            Origin = new Vector2(Texture.Width/2f, Texture.Height/2f);

            shieldPulse.Initialize(0, 0, 0);

            shieldTextureIndex = 0;
        }

        private int shieldTextureIndex;

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (!visible)
            {
                return;
            }

            spriteBatch.Draw(shieldTextures[shieldTextureIndex], 
                Position,
                origin: new Vector2(shieldTextures[shieldTextureIndex].Width / 2f, shieldTextures[shieldTextureIndex].Height / 2f),
                color: Color.White * baseShieldAlpha,
                scale: Vector2.One * 1f,
                rotation: Rotation,
                depth: Depth - 0.0002f);

            if (shieldPulse.Value > 0f)
            {
                spriteBatch.Draw(shieldPusleTexture, 
                    shieldPulseLocation,
                    origin: new Vector2(shieldPusleTexture.Width / 2f, shieldPusleTexture.Height / 2f),
                    color: Color.White*shieldPulseAlpha,
                    scale: Vector2.One*shieldPulseScale,
                    rotation: Rotation,
                    depth: Depth - 0.0003f);
            }

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
                         t.Position.X < (viewportCenter.X - (blackhole.Height / 2f))));
        }

        private bool Right(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X > (viewportCenter.X  + (blackhole.Height / 2f))));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            const int frameInterval = 100;
            shieldTextureIndex = (int)((((int)gameTime.TotalGameTime.TotalMilliseconds % frameInterval) / (float)frameInterval) * shieldTextures.Length);

            if (stopped)
            {
                if (thrustSoundInstance.State == SoundState.Playing)
                {
                    thrustSoundInstance.Stop(true);
                }
                return;
            }

            Rotation += blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            sideThrustEmitter.Spawning = false;

            var left = false;
            var isMoving = false;

            if (Left(inputState.KeyState, inputState.TouchState))
            {
                Rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
                sideThrustEmitter.Spawning = true;
                left = true;
                isMoving = true;
            }

            if (Right(inputState.KeyState, inputState.TouchState))
            {
                Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
                sideThrustEmitter.Spawning = true;
                isMoving = true;
            }

            Rotation = MathHelper.WrapAngle(Rotation);

            const float radiusPadding = 20;

            Radius = (blackhole.Height / 2f) + (Texture.Height / 2f) + radiusPadding;

            Position = new Vector2(
                blackhole.Position.X + ((float)Math.Sin(Rotation) * Radius),
                blackhole.Position.Y - ((float)Math.Cos(Rotation) * Radius));

            particleSystem.Position = Position;

            sideThrustEmitter.SpawnDirection = (viewportCenter - Position);
            sideThrustEmitter.SpawnDirection = new Vector2(-sideThrustEmitter.SpawnDirection.Y, sideThrustEmitter.SpawnDirection.X);

            if (left)
            {
                sideThrustEmitter.SpawnDirection = new Vector2(-sideThrustEmitter.SpawnDirection.X, -sideThrustEmitter.SpawnDirection.Y);
            }
            
            sideThrustEmitter.SpawnDirection.Normalize();

            mainThrustEmitter.SpawnDirection = (viewportCenter - Position);
            mainThrustEmitter.SpawnDirection.Normalize();

            particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (isMoving && thrustSoundInstance.State == SoundState.Stopped)
            {
                thrustSoundInstance.Play();
            }
            else if (!isMoving && thrustSoundInstance.State == SoundState.Playing)
            {
                thrustSoundInstance.Stop();
            }

            shieldPulse.Update(gameTime);

            shieldPulseAlpha = MathUtilities.LinearInterpolate(baseShieldAlpha, 0, shieldPulse.Value);
            shieldPulseScale = (10f*shieldPulse.Value) + 1f;
        }

        private Motion shieldPulse;
        private float shieldPulseAlpha;
        private float shieldPulseScale;
        private Vector2 shieldPulseLocation;
        private const float baseShieldAlpha = 0.8f;

        public void PulseShield()
        {
            shieldPulseLocation = Position;
            shieldPulse.Initialize(0, 1, 1f);
        }

        internal void Initialize()
        {
            Rotation = 0;

            Position = new Vector2(
                blackhole.Position.X,
                blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f));

            mainThrustEmitter.Clear();
            sideThrustEmitter.Clear();
        }

        internal void Start()
        {
            Rotation = 0;

            Position = new Vector2(
                blackhole.Position.X,
                blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f));

            stopped = false;
            visible = true;
        }

        internal void Stop()
        {
            crashSound.Play();

            stopped = true;
            //visible = false;
        }

        public Vector2 Origin
        {
            get; set;
        }

        public Vector2 Scale
        {
            get { return Vector2.One; }
        }

        public CollisionInfo CollisionInfo {get; private set; }
    }
}