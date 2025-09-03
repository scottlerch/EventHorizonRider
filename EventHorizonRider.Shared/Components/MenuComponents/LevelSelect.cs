using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace EventHorizonRider.Core.Components.MenuComponents;

internal class LevelSelect(LevelCollection levelsCollection) : ComponentBase
{
    private class LevelButton
    {
        public Vector2 Position;
        public string Text;
        public int LevelNumber;
        public Button Button;
    }

    private readonly LevelCollection _levelsCollection = levelsCollection;

    private SpriteFont _buttonFont;
    private SoundEffect _buttonSound;

    private const string StartLevelText = "START LEVEL";
    private Vector2 _startLevelTextLocation;
    private Vector2 _startLevelTextSize;

    private const string UnlockLevelText = "PLAY TO UNLOCK!";
    private Vector2 _unlockLevelTextLocation;
    private Vector2 _unlockLevelTextSize;

    private LevelButton[] _levelButtons;

    public int StartLevel { get; set; }

    public int MaximumStartLevel { get; set; }

    public int? Pressed { get; private set; }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        StartLevel = 1;
        MaximumStartLevel = 1;

        _buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        _buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

        _startLevelTextSize = _buttonFont.MeasureString(StartLevelText);
        _unlockLevelTextSize = _buttonFont.MeasureString(UnlockLevelText);

        const float buttonPadding = 50f;
        const float startTextOffset = 200f;

        _startLevelTextLocation = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) - (_startLevelTextSize.X / 2f),
            (DeviceInfo.LogicalHeight / 2f) - startTextOffset);

        _unlockLevelTextLocation = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) - (_unlockLevelTextSize.X / 2f),
            (DeviceInfo.LogicalHeight / 2f) - startTextOffset);

        _levelButtons = new LevelButton[_levelsCollection.NumberOfLevels];

        // We know last level is infinite, so will treat differently
        var numberOfFiniteLevels = _levelButtons.Length - 1;

        const string infiniteLevelText = "INFINITE";
        var infiniteButtonSize = _buttonFont.MeasureString(infiniteLevelText);

        const float levelsButtonTotalWidth = 500f;
        var levelButtonY = _startLevelTextLocation.Y + 75f;
        var levelButtonsWidth = levelsButtonTotalWidth - infiniteButtonSize.X;
        var levelButtonSpacing = levelButtonsWidth / (numberOfFiniteLevels);
        var levelButtonXBase = (DeviceInfo.LogicalWidth / 2f) - (levelsButtonTotalWidth / 2f);

        for (var i = 0; i < numberOfFiniteLevels; i++)
        {
            var position = new Vector2(
                levelButtonXBase + (i * levelButtonSpacing),
                levelButtonY);

            var size = _buttonFont.MeasureString(i.ToString());

            _levelButtons[i] = new LevelButton
            {
                LevelNumber = i + 1,
                Text = (i + 1).ToString(),
                Position = position,
                Button = new Button(
                    buttonBounds: new Rectangle(
                        (int)(position.X),
                        (int)(position.Y),
                        (int)(size.X + buttonPadding),
                        (int)(size.Y + buttonPadding)),
                    key: Keys.D1 + i),
            };
        }

        // Infinite level button
        var inifiniteButtonPosition = new Vector2(
            (DeviceInfo.LogicalWidth / 2f) + (levelsButtonTotalWidth / 2) - infiniteButtonSize.X,
            levelButtonY);

        _levelButtons[^1] = new LevelButton
        {
            LevelNumber = _levelButtons.Length,
            Text = infiniteLevelText,
            Position = inifiniteButtonPosition,
            Button = new Button(
                buttonBounds: new Rectangle(
                    (int)(inifiniteButtonPosition.X),
                    (int)(inifiniteButtonPosition.Y),
                    (int)(infiniteButtonSize.X + buttonPadding),
                    (int)(infiniteButtonSize.Y + buttonPadding)),
                key: Keys.I),
        };
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        Pressed = null;

        if (Visible)
        {
            Pressed = IsPressed(gameTime, inputState);

            if (Pressed.HasValue)
            {
                _buttonSound.Play();
            }
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (MaximumStartLevel > 1)
        {
            spriteBatch.DrawString(
                _buttonFont,
                StartLevelText,
                _startLevelTextLocation,
                Color.LightGray.AdjustLight(0.7f),
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }
        else
        {
            spriteBatch.DrawString(
                _buttonFont,
                UnlockLevelText,
                _unlockLevelTextLocation,
                Color.LightGray.AdjustLight(0.7f),
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }

        foreach (var levelButton in _levelButtons)
        {
            var color = Color.LightGray.AdjustLight(0.4f);

            if (levelButton.Button.Hover && levelButton.LevelNumber <= MaximumStartLevel)
            {
                color = Color.Yellow;
            }
            else if (levelButton.LevelNumber == StartLevel)
            {
                color = Color.Yellow;
            }
            else if (levelButton.LevelNumber <= MaximumStartLevel)
            {
                color = Color.White;
            }

            spriteBatch.DrawString(
                _buttonFont,
                levelButton.Text,
                levelButton.Position,
                color,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }
    }

    private int? IsPressed(GameTime gameTime, InputState inputState)
    {
        foreach (var levelButton in _levelButtons)
        {
            levelButton.Button.Update(gameTime, inputState, Visible);
        }

        var pressedLevelButton = _levelButtons.FirstOrDefault(levelButton => levelButton.Button.Pressed);

        return pressedLevelButton != null && pressedLevelButton.LevelNumber <= MaximumStartLevel ? pressedLevelButton.LevelNumber : null;
    }
}
