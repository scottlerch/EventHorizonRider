using EventHorizonRider.Core.Audio;
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

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class Ship : ComponentBase, ISpriteInfo
{
    private const float YTouchThreshold = 90f;

    private readonly Blackhole _blackhole;

    private SoundEffect _thrustSound;
    private SoundComponent _thrustSoundInstanceLeft;
    private SoundComponent _thrustSoundInstanceRight;

    private SoundEffect _crashSound;
    private bool _stopped = true;
    private bool _visible = true;
    private bool _rotationEnabled = false;

    private Texture2D _particleBase;
    private ParticleSystem _particleSystem;
    private Emitter _sideThrustEmitter;
    private Emitter _mainThrustEmitter;

    public Ship(Blackhole blackhole) : base(new ShipShield())
    {
        _blackhole = blackhole;
        Shield = Children.First() as ShipShield;
    }

    public Vector2 Position { get; set; }

    public float Rotation { get; set; }

    public Texture2D Texture { get; set; }

    public ShipShield Shield { get; private set; }

    public float Radius { get; private set; }

    public float Speed { get; set; }

    public Vector2 Origin { get; set; }

    public Vector2 Scale => Vector2.One;

    public CollisionInfo CollisionInfo { get; private set; }

    public Color Color { get; set; }

    public void Initialize()
    {
        Rotation = 0;

        Position = new Vector2(
            _blackhole.Position.X,
            _blackhole.Position.Y - (_blackhole.Height / 2f) - (Texture.Height / 2f));

        _mainThrustEmitter.Clear();
        _sideThrustEmitter.Clear();

        _stopped = false;
        _visible = true;
        _rotationEnabled = false;
    }

    public void Start()
    {
        _stopped = false;
        _visible = true;
        _rotationEnabled = true;
    }

    public void Gameover()
    {
        _crashSound.Play();

        _stopped = true;
        _rotationEnabled = false;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _thrustSound = content.Load<SoundEffect>(@"Sounds\thrust");
        _thrustSoundInstanceLeft = new SoundComponent(_thrustSound) { FadeSpeed = 3f };
        _thrustSoundInstanceRight = new SoundComponent(_thrustSound) { FadeSpeed = 3f };

        Texture = content.Load<Texture2D>(@"Images\ship");
        CollisionInfo = CollisionDetection.GetCollisionInfo(
            Texture,
            resolution: DeviceInfo.Platform.CollisionDetectionDetail == CollisionDetectionDetail.Full ? 1f : 0.75f);

        _crashSound = content.Load<SoundEffect>(@"Sounds\crash_sound");

        _particleBase = content.Load<Texture2D>(@"Images\particle_base");
        _particleSystem = new ParticleSystem(new Vector2(10000, 10000));
        _sideThrustEmitter = _particleSystem.AddEmitter(
            secPerSpawn: Range.Create(0.001f, 0.0015f),
            spawnDirection: new Vector2(0f, -1f),
            spawnNoiseAngle: Range.Create(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
            startLife: Range.Create(0.5f, 0.75f),
            startScale: Range.Create(22f, 22f),
            endScale: Range.Create(8f, 8f),
            startColor: Range.Create(Color.Orange, Color.Crimson),
            endColor: Range.Create(Color.Orange.AdjustAlpha(0), Color.Orange.AdjustAlpha(0)),
            startSpeed: Range.Create(400f, 500f),
            endSpeed: Range.Create(100f, 120f),
            budget: DeviceInfo.Platform.ParticleEffectsDetails == ParticleEffectsDetails.Full ? 500 : 0,
            relPosition: Vector2.Zero,
            particleSprite: _particleBase);

        _sideThrustEmitter.GravityCenter = DeviceInfo.LogicalCenter;
        _sideThrustEmitter.GravityForce = 1.3f;

        _mainThrustEmitter = _particleSystem.AddEmitter(
            secPerSpawn: Range.Create(0.001f, 0.0015f),
            spawnDirection: new Vector2(0f, -1f),
            spawnNoiseAngle: Range.Create(0.3f * MathHelper.Pi, 0.3f * -MathHelper.Pi),
            startLife: Range.Create(0.1f, 0.5f),
            startScale: Range.Create(22f, 22f),
            endScale: Range.Create(8f, 8f),
            startColor: _mainThrustStartColorRange,
            endColor: _mainThrustEndcolorRange,
            startSpeed: Range.Create(400f, 500f),
            endSpeed: Range.Create(100f, 120f),
            budget: 50,
            relPosition: Vector2.Zero,
            particleSprite: _particleBase);

        _mainThrustEmitter.GravityCenter = DeviceInfo.LogicalCenter;
        _mainThrustEmitter.GravityForce = 1.3f;
        _mainThrustEmitter.Spawning = true;

        Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
    }

    private readonly Range<Color> _mainThrustStartColorRange = Range.Create(Color.SkyBlue, Color.Blue);
    private readonly Range<Color> _mainThrustEndcolorRange = Range.Create(Color.SkyBlue.AdjustAlpha(0), Color.SkyBlue.AdjustAlpha(0));

    private readonly Range<Color> _sideThrustStartColor = Range.Create(Color.Orange, Color.Crimson);
    private readonly Range<Color> _sideThrustEndColor = Range.Create(Color.Orange.AdjustAlpha(0), Color.Orange.AdjustAlpha(0));

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (!_visible)
        {
            return;
        }

        _particleSystem.Draw(spriteBatch, Depth - 0.0001f);

        spriteBatch.Draw(
            Texture,
            Position,
            sourceRectangle: null,
            origin: Origin,
            rotation: Rotation,
            layerDepth: Depth,
            color: Color.Lerp(Color.White, Color, 0.3f),
            effects: SpriteEffects.None,
            scale: Vector2.One);
    }

    protected override void OnUpdatingChanged()
    {
        if (!Updating)
        {
            _thrustSoundInstanceLeft.Pause();
            _thrustSoundInstanceRight.Pause();
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        _mainThrustEmitter.StartColor = Range.Create(Color.Lerp(_mainThrustStartColorRange.Low, Color, 0.3f), Color.Lerp(_mainThrustStartColorRange.High, Color, 0.3f));
        _mainThrustEmitter.EndColor = Range.Create(Color.Lerp(_mainThrustEndcolorRange.Low, Color, 0.3f), Color.Lerp(_mainThrustEndcolorRange.High, Color, 0.3f));

        _sideThrustEmitter.StartColor = Range.Create(Color.Lerp(_sideThrustStartColor.Low, Color, 0.3f), Color.Lerp(_sideThrustStartColor.High, Color, 0.3f));
        _sideThrustEmitter.EndColor = Range.Create(Color.Lerp(_sideThrustEndColor.Low, Color, 0.3f), Color.Lerp(_sideThrustEndColor.High, Color, 0.3f));

        if (_stopped)
        {
            _thrustSoundInstanceLeft.PlayMin(immediate: true);
            _thrustSoundInstanceRight.PlayMin(immediate: true);
            return;
        }

        if (_rotationEnabled)
        {
            Rotation += _blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _sideThrustEmitter.Spawning = false;

        var left = false;

        if (Left(inputState.KeyState, inputState.TouchState, inputState.MouseState))
        {
            Rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
            _sideThrustEmitter.Spawning = true;
            left = true;

            _thrustSoundInstanceLeft.PlayMax();
        }
        else
        {
            _thrustSoundInstanceLeft.PlayMin();
        }

        if (Right(inputState.KeyState, inputState.TouchState, inputState.MouseState))
        {
            Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
            _sideThrustEmitter.Spawning = true;

            _thrustSoundInstanceRight.PlayMax();
        }
        else
        {
            _thrustSoundInstanceRight.PlayMin();
        }

        Rotation = MathHelper.WrapAngle(Rotation);

        const float radiusPadding = 20;

        Radius = (_blackhole.Height / 2f) + (Texture.Height / 2f) + radiusPadding;

        Position = new Vector2(
            _blackhole.Position.X + ((float)Math.Sin(Rotation) * Radius),
            _blackhole.Position.Y - ((float)Math.Cos(Rotation) * Radius));

        _particleSystem.Position = Position;

        _sideThrustEmitter.SpawnDirection = (DeviceInfo.LogicalCenter - Position);
        _sideThrustEmitter.SpawnDirection = new Vector2(-_sideThrustEmitter.SpawnDirection.Y, _sideThrustEmitter.SpawnDirection.X);

        if (left)
        {
            _sideThrustEmitter.SpawnDirection = new Vector2(-_sideThrustEmitter.SpawnDirection.X, -_sideThrustEmitter.SpawnDirection.Y);
        }

        _sideThrustEmitter.SpawnDirection.Normalize();

        _mainThrustEmitter.SpawnDirection = (DeviceInfo.LogicalCenter - Position);
        _mainThrustEmitter.SpawnDirection.Normalize();

        _particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        _thrustSoundInstanceLeft.Update(gameTime);
        _thrustSoundInstanceRight.Update(gameTime);
    }

    private bool Left(KeyboardState keyState, TouchCollection touchState, MouseState mouseState)
    {
        var threshold = (DeviceInfo.LogicalCenter.X - (_blackhole.Height / 2f));
        return
            (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right)) ||
            (mouseState.LeftButton == ButtonState.Pressed && mouseState.Position.X < threshold && mouseState.Y > YTouchThreshold) ||
            (touchState.Count > 0 &&
             touchState.All(
                 t =>
                     (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                     t.Position.X < threshold && t.Position.Y > YTouchThreshold));
    }

    private bool Right(KeyboardState keyState, TouchCollection touchState, MouseState mouseState)
    {
        var threshold = (DeviceInfo.LogicalCenter.X + (_blackhole.Height / 2f));
        return
            (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
            (mouseState.LeftButton == ButtonState.Pressed && mouseState.Position.X > threshold && mouseState.Y > YTouchThreshold) ||
            (touchState.Count > 0 &&
             touchState.All(
                 t =>
                     (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                     t.Position.X > threshold && t.Position.Y > YTouchThreshold));
    }
}
