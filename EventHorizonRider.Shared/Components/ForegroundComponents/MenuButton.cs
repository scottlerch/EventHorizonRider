using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class MenuButton : ComponentBase
{
    private const string MenuText = "LEVEL SELECT";
    private const string BackText = "BACK";

    private SpriteFont _buttonFont;
    private SoundEffect _buttonSound;

    private bool _isVisible = true;
    private bool _isBack;

    private Vector2 _menuTextLocation;
    private Vector2 _menuTextSize;

    private Vector2 _backTextLocation;
    private Vector2 _backTextSize;

    // Safe-area insets captured each frame, plus the pre-inset hit area, so the top-right
    // "LEVEL SELECT"/"BACK" label and its touch target clear the notch / rounded corner.
    private Vector4 _safeAreaInsets;
    private Rectangle _baseButtonBounds;

    public Button Button { get; private set; }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        _buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

        _menuTextSize = _buttonFont.MeasureString(MenuText);
        _backTextSize = _buttonFont.MeasureString(BackText);

        const float textPadding = 10;

        _menuTextLocation = new Vector2(DeviceInfo.LogicalWidth - (_menuTextSize.X) - textPadding, textPadding);
        _backTextLocation = new Vector2(DeviceInfo.LogicalWidth - (_backTextSize.X) - textPadding, textPadding);

        const float buttonPadding = 50f;

        Button = new Button(
            buttonBounds: new Rectangle(
                (int)(_menuTextLocation.X - buttonPadding),
                (int)(_menuTextLocation.Y - buttonPadding),
                (int)(_menuTextSize.X + (buttonPadding * 2)),
                (int)(_menuTextSize.Y + (buttonPadding * 2))),
            key: Keys.M);

        _baseButtonBounds = Button.ButtonBounds;
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        // Inset the label/hit area from the top-right: shift left by the right inset, down by the top.
        _safeAreaInsets = DeviceInfo.SafeAreaInsets;
        Button.SetBounds(new Rectangle(
            _baseButtonBounds.X - (int)_safeAreaInsets.Z,
            _baseButtonBounds.Y + (int)_safeAreaInsets.Y,
            _baseButtonBounds.Width,
            _baseButtonBounds.Height));

        Button.Update(gameTime, inputState, Visible);

        if (Button.Pressed)
        {
            _buttonSound.Play();
        }
    }

    public void Show(bool back = false)
    {
        _isVisible = true;
        _isBack = back;
    }

    public void Hide() => _isVisible = false;

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (_isVisible)
        {
            var insetOffset = new Vector2(-_safeAreaInsets.Z, _safeAreaInsets.Y);

            if (_isBack)
            {
                spriteBatch.DrawString(
                    _buttonFont,
                    BackText,
                    _backTextLocation + insetOffset,
                    Button.Hover ? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: Depth);
            }
            else
            {
                spriteBatch.DrawString(
                    _buttonFont,
                    MenuText,
                    _menuTextLocation + insetOffset,
                    Button.Hover ? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: Depth);
            }
        }
    }
}
