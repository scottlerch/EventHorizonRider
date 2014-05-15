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
        private const float yTouchThreshold = 90f;

        private readonly Blackhole blackhole;

        private SoundEffect thrustSound;
        private SoundEffectInstance thrustSoundInstance;

        private SoundEffect crashSound;
        private bool stopped = true;
        private bool visible = true;
        private bool rotationEnabled = false;

        private Texture2D particleBase;
        private ParticleSystem particleSystem;
        private Emitter sideThrustEmitter;
        private Emitter mainThrustEmitter;

        public Ship(Blackhole blackhole) : base(new ShipShield())
        {
            this.blackhole = blackhole;
            Shield = Children.First() as ShipShield;
        }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Texture2D Texture { get; set; }

        public ShipShield Shield { get; private set; }

        public float Radius { get; private set; }

        public float Speed { get; set; }

        public Vector2 Origin{ get; set; }

        public Vector2 Scale{ get { return Vector2.One; } }

        public CollisionInfo CollisionInfo { get; private set; }

        public void Initialize()
        {
            Rotation = 0;

            Position = new Vector2(
                blackhole.Position.X,
                blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f));

            mainThrustEmitter.Clear();
            sideThrustEmitter.Clear();

            stopped = false;
            visible = true;
            rotationEnabled = false;
        }

        public void Start()
        {
            stopped = false;
            visible = true;
            rotationEnabled = true;
        }

        public void Stop()
        {
            crashSound.Play();

            stopped = true;
            rotationEnabled = false;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            thrustSound = content.Load<SoundEffect>(@"Sounds\thrust");
            thrustSoundInstance = thrustSound.CreateInstance();
            thrustSoundInstance.IsLooped = true;

            Texture = content.Load<Texture2D>(@"Images\ship");
            CollisionInfo = CollisionDetection.GetCollisionInfo(
                Texture, 
                resolution: DeviceInfo.Platform.CollisionDetectionDetail == CollisionDetectionDetail.Full ? 1f : 0.75f);

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

            sideThrustEmitter.GravityCenter = DeviceInfo.LogicalCenter;
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

            mainThrustEmitter.GravityCenter = DeviceInfo.LogicalCenter;
            mainThrustEmitter.GravityForce = 1.3f;
            mainThrustEmitter.Spawning = true;

            Origin = new Vector2(Texture.Width/2f, Texture.Height/2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (!visible)
            {
                return;
            }

            particleSystem.Draw(spriteBatch, 1, Vector2.Zero, Depth - 0.0001f);

            spriteBatch.Draw(
                Texture, 
                Position,
                origin: Origin,
                rotation: Rotation,
                depth: Depth);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (stopped)
            {
                if (thrustSoundInstance.State == SoundState.Playing)
                {
                    thrustSoundInstance.Stop(true);
                }
                return;
            }

            if (rotationEnabled)
            {
                Rotation += blackhole.RotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;
            }

            sideThrustEmitter.Spawning = false;

            var left = false;
            var isMoving = false;

            if (Left(inputState.KeyState, inputState.TouchState, inputState.MouseState))
            {
                Rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
                sideThrustEmitter.Spawning = true;
                left = true;
                isMoving = true;
            }

            if (Right(inputState.KeyState, inputState.TouchState, inputState.MouseState))
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

            sideThrustEmitter.SpawnDirection = (DeviceInfo.LogicalCenter - Position);
            sideThrustEmitter.SpawnDirection = new Vector2(-sideThrustEmitter.SpawnDirection.Y, sideThrustEmitter.SpawnDirection.X);

            if (left)
            {
                sideThrustEmitter.SpawnDirection = new Vector2(-sideThrustEmitter.SpawnDirection.X, -sideThrustEmitter.SpawnDirection.Y);
            }
            
            sideThrustEmitter.SpawnDirection.Normalize();

            mainThrustEmitter.SpawnDirection = (DeviceInfo.LogicalCenter - Position);
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
        }

        private bool Left(KeyboardState keyState, TouchCollection touchState, MouseState mouseState)
        {
            var threshold = (DeviceInfo.LogicalCenter.X - (blackhole.Height/2f));
            return
                (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right)) ||
                (mouseState.LeftButton == ButtonState.Pressed && mouseState.Position.X < threshold && mouseState.Y > yTouchThreshold) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X < threshold && t.Position.Y > yTouchThreshold));
        }

        private bool Right(KeyboardState keyState, TouchCollection touchState, MouseState mouseState)
        {
            var threshold = (DeviceInfo.LogicalCenter.X + (blackhole.Height/2f));
            return
                (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
                (mouseState.LeftButton == ButtonState.Pressed && mouseState.Position.X > threshold && mouseState.Y > yTouchThreshold) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X > threshold && t.Position.Y > yTouchThreshold));
        }
    }
}