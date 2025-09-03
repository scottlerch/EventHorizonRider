using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class Blackhole : ComponentBase
{
    private float _newExtraBlackholeScale;
    private float _extraBlackholeScaleSpeed;

    private Texture2D _texture;

    private bool _isStopped = true;

    private float _currentRotation;

    private SoundEffect _scaleSound;

    public Blackhole()
    {
        Spring = new Spring
        {
            Friction = -0.9f,
            Stiffness = -100f,
            BlockMass = 0.1f,
        };
    }

    public Spring Spring { get; private set; }

    public Vector2 Position { get; private set; }

    public float Height => _texture.Height * Spring.BlockX;

    public float RotationalVelocity { get; set; }

    public float ExtraScale { get; private set; }

    public Vector2 Scale => new(Spring.BlockX + ExtraScale, Spring.BlockX + ExtraScale);

    public void Gameover() => _isStopped = true;

    public void Start() => _isStopped = false;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _texture = content.Load<Texture2D>(@"Images\blackhole");
        _scaleSound = content.Load<SoundEffect>(@"Sounds\open_menu");

        Position = new Vector2(
            DeviceInfo.LogicalWidth / 2f,
            DeviceInfo.LogicalHeight / 2f);
    }

    public void Pulse(float pullX = 1.15f, float pullVelocity = 1.5f) => Spring.PullBlock(pullX, pullVelocity);

    public void SetExtraScale(float scaleSize, bool animate = false, float speed = 1f)
    {
        _newExtraBlackholeScale = scaleSize;

        if (!animate)
        {
            ExtraScale = scaleSize;
        }
        else
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (scaleSize != ExtraScale)
            {
                _scaleSound.Play();
            }

            _extraBlackholeScaleSpeed = speed;
            _extraBlackholeScaleSpeed *= ExtraScale < _newExtraBlackholeScale ? 1f : -1f;
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (ExtraScale != _newExtraBlackholeScale)
        {
            ExtraScale += (float)gameTime.ElapsedGameTime.TotalSeconds * _extraBlackholeScaleSpeed;

            if ((_extraBlackholeScaleSpeed > 0 && ExtraScale > _newExtraBlackholeScale) ||
                (_extraBlackholeScaleSpeed < 0 && ExtraScale < _newExtraBlackholeScale))
            {
                ExtraScale = _newExtraBlackholeScale;
            }
        }

        if (!_isStopped)
        {
            Spring.Update(gameTime.ElapsedGameTime);
            _currentRotation += RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            Position,
            sourceRectangle: null,
            origin: new Vector2(_texture.Width / 2f, _texture.Height / 2f),
            rotation: _currentRotation,
            scale: new Vector2(Scale.X, Scale.Y),
            color: Color.Black,
            layerDepth: Depth,
            effects: SpriteEffects.None);
    }
}
