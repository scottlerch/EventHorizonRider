using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class ControlsHelp : ComponentBase
{
    private const float MaxAlpha = 0.3f;

    private Vector2 _helpLeftPosition;
    private Texture2D _helpLeft;

    private Vector2 _helpRightPosition;
    private Texture2D _helpRight;

    private Vector2 _helpStartPosition;
    private Texture2D _helpStart;

    private Motion _startMotion;

    private bool _fading;
    private float _directionAlpha = MaxAlpha;
    private float _directionFadeSpeed;

    private float _startAlpha = MaxAlpha;
    private float _startFadeSpeed;

    private bool _touchEnabled;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        Visible = true;

        _touchEnabled = DeviceInfo.Platform.TouchEnabled;

        if (_touchEnabled)
        {
            _helpLeft = content.Load<Texture2D>(@"Images\help_left");
            _helpRight = content.Load<Texture2D>(@"Images\help_right");
        }
        else
        {
            _helpLeft = content.Load<Texture2D>(@"Images\help_key_left");
            _helpRight = content.Load<Texture2D>(@"Images\help_key_right");
        }

        _helpStart = content.Load<Texture2D>(@"Images\help_start");

        _helpLeftPosition = new Vector2(0, (DeviceInfo.LogicalHeight / 2) - (_helpLeft.Height / 2));

        _helpRightPosition = new Vector2(
            DeviceInfo.LogicalWidth - _helpRight.Width,
            (DeviceInfo.LogicalHeight / 2) - (_helpLeft.Height / 2));

        _helpStartPosition = new Vector2(
            (DeviceInfo.LogicalWidth / 2) - (_helpStart.Width / 2),
            (DeviceInfo.LogicalHeight / 2) + 125);

        _startMotion = new Motion(value: 0, target: 20, speed: 80);
    }

    public void Hide(float speed)
    {
        if (_fading && _directionFadeSpeed < 0f)
        {
            return;
        }

        _fading = true;
        _directionFadeSpeed = speed > 0f ? speed * -1f : speed;
        _startFadeSpeed = _directionFadeSpeed * 8f;
    }

    public void Show(float speed)
    {
        if (_fading && _directionFadeSpeed > 0f)
        {
            return;
        }

        Visible = true;
        _fading = true;
        _directionFadeSpeed = speed < 0f ? speed * -1f : speed;
        _startFadeSpeed = _directionFadeSpeed;
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        _startMotion.Update(gameTime);

        if (_startMotion.IsDone)
        {
            _startMotion.UpdateTarget(_startMotion.Value > 0 ? 0 : 20);
        }

        if (_fading)
        {
            _directionAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * _directionFadeSpeed;
            _startAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * _startFadeSpeed;

            _startAlpha = MathHelper.Clamp(_startAlpha, 0f, MaxAlpha);

            if (_directionAlpha < 0f)
            {
                _directionAlpha = 0f;
                _fading = false;
                Visible = false;
            }
            else if (_directionAlpha > MaxAlpha)
            {
                _directionAlpha = MaxAlpha;
                _fading = false;
            }
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _helpLeft,
            color: Color.White * _directionAlpha,
            position: _helpLeftPosition,
            layerDepth: Depth,
            origin: Vector2.Zero,
            scale: Vector2.One,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None);

        spriteBatch.Draw(
            _helpRight,
            color: Color.White * _directionAlpha,
            position: _helpRightPosition,
            layerDepth: Depth,
            origin: Vector2.Zero,
            scale: Vector2.One,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None);

        spriteBatch.Draw(
            _helpStart,
            color: Color.White * _startAlpha,
            position: new Vector2(_helpStartPosition.X, _helpStartPosition.Y - _startMotion.Value),
            layerDepth: Depth,
            origin: Vector2.Zero,
            scale: Vector2.One,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None);
    }
}
