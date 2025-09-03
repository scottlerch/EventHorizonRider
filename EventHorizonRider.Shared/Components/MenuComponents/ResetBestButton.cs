using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EventHorizonRider.Core.Components.MenuComponents;

internal class ResetButton : ComponentBase
{
    private const string Text = "RESET BEST";
    private const string WarningText = "Hold to clear all stats!";

    private SpriteFont _buttonFont;
    private SoundEffect _buttonSound;

    private Vector2 _textLocation;
    private Vector2 _textSize;

    private Vector2 _warningTextLocation;
    private Vector2 _warningTextSize;
    private float _warningAlpha;

    public Button Button { get; set; }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        _buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

        _textSize = _buttonFont.MeasureString(Text);
        _warningTextSize = _buttonFont.MeasureString(WarningText);

        _textLocation = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) - (_textSize.X / 2f),
            (DeviceInfo.LogicalHeight / 2f) + 45f);

        _warningTextLocation = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) - (_warningTextSize.X / 2f),
            _textLocation.Y - _textSize.Y - 5f);

        const float buttonPadding = 25f;

        Button = new Button(
            buttonBounds: new Rectangle(
                (int)(_textLocation.X - buttonPadding),
                (int)(_textLocation.Y - buttonPadding),
                (int)(_textSize.X + (buttonPadding * 2)),
                (int)(_textSize.Y + (buttonPadding * 2))),
            key: Keys.R,
            holdDuration: TimeSpan.FromSeconds(3));
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        Button.Update(gameTime, inputState, Visible);

        if (Button.Pressed)
        {
            _buttonSound.Play();
        }

        if (Button.Holding)
        {
            var alpha = (float)Math.Sin(Button.CurrentHoldDuration.TotalSeconds * 15);
            if (alpha < 0)
            {
                alpha *= -1f;
            }

            _warningAlpha = MathHelper.Lerp(0.5f, 1f, alpha);
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _buttonFont,
            Text,
            _textLocation,
            Button.Hover ? Color.Yellow : Color.White,
            0,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            Depth);

        if (Button.Holding && Button.HoldDurationRemaining > TimeSpan.Zero)
        {
            spriteBatch.DrawString(
                _buttonFont,
                WarningText,
                _warningTextLocation,
                Color.Red * _warningAlpha,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);

            var holdDurationText = (Button.HoldDurationRemaining.Seconds + 1).ToString();

            spriteBatch.DrawString(
                _buttonFont,
                holdDurationText,
                new Vector2(_textLocation.X + _textSize.X + 20f, _textLocation.Y),
                Color.Gray,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);

            var holdDurationTextSize = _buttonFont.MeasureString(holdDurationText);

            spriteBatch.DrawString(
                _buttonFont,
                holdDurationText,
                new Vector2(_textLocation.X - holdDurationTextSize.X - 20f, _textLocation.Y),
                Color.Gray,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }
    }
}
