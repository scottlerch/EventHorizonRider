using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.MenuComponents;

internal class CreditsButton : ComponentBase
{
    private const string ButtonText = "CREDITS";

    private SpriteFont _buttonFont;
    private SoundEffect _buttonSound;

    private Vector2 _textLocation;
    private Vector2 _textSize;

    public Button Button { get; set; }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        _buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

        _textSize = _buttonFont.MeasureString(ButtonText);

        const float buttonPadding = 25f;

        _textLocation = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) - (_textSize.X / 2f),
            (DeviceInfo.LogicalHeight / 2f) + 150f);

        Button = new Button(
            buttonBounds: new Rectangle(
                (int)(_textLocation.X - buttonPadding),
                (int)(_textLocation.Y - buttonPadding),
                (int)(_textSize.X + (buttonPadding * 2)),
                (int)(_textSize.Y + (buttonPadding * 2))),
            key: Keys.C);
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        Button.Update(gameTime, inputState, Visible);

        if (Button.Pressed)
        {
            _buttonSound.Play();
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _buttonFont,
            ButtonText,
            _textLocation,
            Button.Hover ? Color.Yellow : Color.White,
            0,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            Depth);
    }
}
