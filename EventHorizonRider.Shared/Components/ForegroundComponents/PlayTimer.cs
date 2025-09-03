using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class PlayTimer : ComponentBase
{
    const float TextPadding = 10;
    const float TextVerticalSpacing = 15;

    private readonly LevelCollection _levelsCollection;
    private bool _updatingTime;

    private Vector2 _bestTextSize;

    private int _currentLevelNumber;

    private string _levelNumberText;
    private string _bestNumberText;
    private string _timeNumberText;

    private float _levelTextSize;

    private readonly Color _scoreColor = Color.Yellow;

    private SpriteFont _labelFont;
    private SpriteFont _timeFont;

    private SoundEffect _newBestSound;
    private SoundEffect _newLevelSound;

    private Vector2 _viewSize;

    private List<float> _textOffset;

    private const string BestText = "Best: ";
    private const string LevelText = "Level: ";

    private bool _isLevelAndScoreVisible;

    private readonly Motion _levelNumberScaling;

    private bool _newBest;

    private bool _animatingNewLevel;

    private TimeSpan _newBestDuration;
    private readonly TimeSpan _newBestDurationMax = TimeSpan.FromSeconds(2);
    private float _newBestAlpha;

    public PlayTimer(LevelCollection levelsCollection)
    {
        _levelsCollection = levelsCollection;
        _levelNumberScaling = new Motion();

        ProgressBar = new ProgressBar();
        AddChild(ProgressBar, Depth);
    }

    public TimeSpan Elapsed { get; set; }

    public TimeSpan Best { get; private set; }

    public ProgressBar ProgressBar { get; private set; }

    public void ShowLevelAndScore()
    {
        _isLevelAndScoreVisible = true;
        ProgressBar.Visible = true;
    }

    public void HideLevelAndScore()
    {
        _isLevelAndScoreVisible = false;
        _newBest = false;
        ProgressBar.Visible = false;
    }

    public void Restart(TimeSpan initialElapsedTime)
    {
        _updatingTime = true;
        Elapsed = initialElapsedTime;
    }

    public void Stop(bool newNewBest = false)
    {
        _newBest = newNewBest;
        _updatingTime = false;
    }

    public void SetLevel(int newCurrentLevelNumber, bool animate = false)
    {
        _currentLevelNumber = newCurrentLevelNumber;

        if (animate)
        {
            _levelNumberScaling.Initialize(1f, 0f, 0.5f);
            _animatingNewLevel = true;
        }
    }

    public void UpdateBest(TimeSpan best, bool isNew = false)
    {
        if (isNew && Best > TimeSpan.Zero)
        {
            _newBestSound.Play();
            _newBestAlpha = 0f;
            _newBestDuration = _newBestDurationMax;
        }

        Best = best;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _viewSize = new Vector2(DeviceInfo.LogicalWidth, DeviceInfo.LogicalHeight);

        _labelFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        _timeFont = content.Load<SpriteFont>(@"Fonts\time_font");

        _newLevelSound = content.Load<SoundEffect>(@"Sounds\newlevel_sound");
        _newBestSound = content.Load<SoundEffect>(@"Sounds\new_best");

        _bestTextSize = _labelFont.MeasureString(BestText);
        _labelFont.MeasureString(LevelText);

        _textOffset =
        [
            _timeFont.MeasureString("0.00").X,
            _timeFont.MeasureString("00.00").X,
            _timeFont.MeasureString("000.00").X,
            _timeFont.MeasureString("0000.00").X,
            _timeFont.MeasureString("00000.00").X
        ];

        _levelTextSize = _labelFont.MeasureString(LevelText).X;

        var levelNumberTextSize = _labelFont.MeasureString("5").X;

        ProgressBar.Initialize(
            new Vector2(_viewSize.X - (levelNumberTextSize + _levelTextSize) - TextPadding, _bestTextSize.Y + 8),
            new Vector2(levelNumberTextSize + _levelTextSize - 2, 6));
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (_newBest)
        {
            _newBestDuration = TimeSpan.Zero;
        }

        if (_newBestDuration > TimeSpan.Zero)
        {
            _newBestDuration -= gameTime.ElapsedGameTime;
        }

        var alpha = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 15);
        if (alpha < 0)
        {
            alpha *= -1f;
        }

        _newBestAlpha = MathHelper.Lerp(0.5f, 1f, alpha);

        _levelNumberScaling.Update(gameTime);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_animatingNewLevel && _levelNumberScaling.Value == 0f)
        {
            _animatingNewLevel = false;
            _newLevelSound.Play(0.5f, 0f, 0f);
        }

        if (_updatingTime)
        {
            Elapsed += gameTime.ElapsedGameTime;
        }

        _bestNumberText = FormatTime(Best);
        _levelNumberText = _currentLevelNumber.ToString();
        _timeNumberText = FormatTime(Elapsed);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        DrawBestTime(spriteBatch);

        if (_isLevelAndScoreVisible)
        {
            DrawCurrentTime(spriteBatch);
            DrawLevelNumber(spriteBatch);
        }
    }

    private void DrawBestTime(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _labelFont,
            BestText,
            new Vector2(TextPadding, TextPadding),
            Color.LightGray.AdjustLight(0.9f),
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            Depth);

        spriteBatch.DrawString(
            _labelFont,
            _bestNumberText,
            new Vector2(TextPadding + _bestTextSize.X, TextPadding),
            Color.White,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            Depth);

        if (_newBest || _newBestDuration > TimeSpan.Zero)
        {
            spriteBatch.DrawString(
                _labelFont,
                "NEW!",
                new Vector2(TextPadding, (TextPadding + 5) + _bestTextSize.Y),
                Color.Yellow * _newBestAlpha,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: Depth);
        }
    }

    private void DrawCurrentTime(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
          _timeFont,
          _timeNumberText,
          new Vector2(
              _viewSize.X - _textOffset[_timeNumberText.Length - 4] - TextPadding,
              TextPadding + _bestTextSize.Y + TextVerticalSpacing),
          _scoreColor,
          rotation: 0,
          origin: Vector2.Zero,
          scale: 1f,
          effects: SpriteEffects.None,
          layerDepth: Depth);
    }

    private void DrawLevelNumber(SpriteBatch spriteBatch)
    {
        const string infiniteText = "Infinite";

        var isInifiniteLevel = _levelsCollection.GetLevel(_currentLevelNumber).IsInfiniteSequence;
        var text = isInifiniteLevel ? infiniteText : _levelNumberText;
        var levelNumberTextSize = _labelFont.MeasureString(text).X;

        if (!isInifiniteLevel)
        {
            spriteBatch.DrawString(
                _labelFont,
                LevelText,
                new Vector2(
                    _viewSize.X - (levelNumberTextSize + _levelTextSize) - TextPadding,
                    TextPadding),
                Color.LightGray.AdjustLight(0.9f),
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: Depth);
        }

        var scaleFactor = 10f * _levelNumberScaling.Value;
        var textScale = Vector2.One + (Vector2.One * scaleFactor);

        spriteBatch.DrawString(
            _labelFont,
            text,
            new Vector2(
                (_viewSize.X - (levelNumberTextSize * textScale.X) - TextPadding),
                TextPadding),
            Color.White * (1f - _levelNumberScaling.Value),
            rotation: 0,
            origin: Vector2.Zero,
            scale: textScale,
            effects: SpriteEffects.None,
            layerDepth: Depth + 0.00003f);
    }

    private static string FormatTime(TimeSpan time) => string.Format("{0:0.00}", time.TotalSeconds);
}
