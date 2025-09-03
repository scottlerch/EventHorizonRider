using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class PlayButton : ComponentBase
{
    private class Info(string text, Vector2 size, Color color, Color hoverColor)
    {
        public readonly string Text = text;
        public readonly Vector2 Size = size;
        public readonly Color Color = color;
        public readonly Color HoverColor = hoverColor;
    }

    private float _fadeSpeed = 1.5f;

    private SpriteFont _buttonFont;

    private float _colorAlphaPercent = 1f;

    private PlayButtonState _playButtonState;
    private bool _isVisible = true;

    private Dictionary<PlayButtonState, Info> _textInfo;

    public float Scale { get; set; }

    public Button Button { get; private set; }

    public void Show(PlayButtonState state, bool fade = false, float newFadeSpeed = 1.5f)
    {
        _isVisible = true;
        _playButtonState = state;

        if (!fade)
        {
            _colorAlphaPercent = 1f;
        }

        _fadeSpeed = newFadeSpeed;
    }

    public void Hide(bool fade = true, float newFadeSpeed = 1.5f)
    {
        if (!fade)
        {
            _colorAlphaPercent = 0f;
        }

        _isVisible = false;
        _fadeSpeed = newFadeSpeed;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _buttonFont = content.Load<SpriteFont>(@"Fonts\button_font");

        _textInfo = new Dictionary<PlayButtonState, Info>
        {
            {
                PlayButtonState.Start,
                new Info("START", _buttonFont.MeasureString("START"), Color.White, Color.Yellow)
            },
            {
                PlayButtonState.Restart,
                new Info("RESET", _buttonFont.MeasureString("RESET"), Color.White, Color.Yellow)
                },
            {
                PlayButtonState.Resume,
                new Info("RESUME", _buttonFont.MeasureString("RESUME"), Color.White, Color.Yellow)
            },
            {
                PlayButtonState.Pause,
                new Info("PAUSE", _buttonFont.MeasureString("PAUSE"), Color.DarkGray.AdjustLight(0.2f), Color.Gray.AdjustLight(0.3f))
            },
        };

        const float buttonPadding = 100f;

        var textSize = _textInfo[PlayButtonState.Restart].Size;

        Button = new Button(
            buttonBounds: new Rectangle(
                (int)(DeviceInfo.LogicalCenter.X - ((textSize.X + buttonPadding) / 2f)),
                (int)(DeviceInfo.LogicalCenter.Y - ((textSize.Y + buttonPadding) / 2f)),
                (int)(textSize.X + buttonPadding),
                (int)(textSize.Y + buttonPadding)),
           key: Keys.Space);
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        Button.Update(gameTime, inputState, _isVisible);

        if (_isVisible && _colorAlphaPercent < 1f)
        {
            _colorAlphaPercent += _fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else if (!_isVisible && _colorAlphaPercent > 0f)
        {
            _colorAlphaPercent -= _fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _colorAlphaPercent = MathHelper.Clamp(_colorAlphaPercent, 0, 1);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (_colorAlphaPercent >= 0f)
        {
            var info = _textInfo[_playButtonState];

            spriteBatch.DrawString(
                _buttonFont,
                info.Text,
                DeviceInfo.LogicalCenter,
                (Button.Hover ? info.HoverColor : info.Color) * _colorAlphaPercent,
                rotation: 0f,
                origin: new Vector2(info.Size.X / 2f, info.Size.Y / 2f),
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: Depth);
        }
    }
}
