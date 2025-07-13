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

    private readonly LevelCollection levelsCollection;

    private TimeSpan gameTimeElapsed;
    private bool updatingTime;

    private Vector2 bestTextSize;

    private int currentLevelNumber;

    private string levelNumberText;
    private string bestNumberText;
    private string timeNumberText;

    private float levelTextSize;

    private readonly Color scoreColor = Color.Yellow;

    private SpriteFont labelFont;
    private SpriteFont timeFont;

    private SoundEffect newBestSound;
    private SoundEffect newLevelSound;

    private Vector2 viewSize;

    private List<float> textOffset;

    private const string BestText = "Best: ";
    private const string LevelText = "Level: ";

    private bool isLevelAndScoreVisible;

    private readonly Motion levelNumberScaling;

    private bool newBest;

    private bool animatingNewLevel;

    private TimeSpan newBestDuration;
    private readonly TimeSpan newBestDurationMax = TimeSpan.FromSeconds(2);
    private float newBestAlpha;

    public PlayTimer(LevelCollection levelsCollection)
    {
        this.levelsCollection = levelsCollection;
        levelNumberScaling = new Motion();

        ProgressBar = new ProgressBar();
        AddChild(ProgressBar, Depth);
    }

    public TimeSpan Elapsed
    {
        get { return gameTimeElapsed; }
        set { gameTimeElapsed = value; }
    }

    public TimeSpan Best { get; private set; }

    public ProgressBar ProgressBar { get; private set; }

    public void ShowLevelAndScore()
    {
        isLevelAndScoreVisible = true;
        ProgressBar.Visible = true;
    }

    public void HideLevelAndScore()
    {
        isLevelAndScoreVisible = false;
        newBest = false;
        ProgressBar.Visible = false;
    }

    public void Restart(TimeSpan initialElapsedTime)
    {
        updatingTime = true;
        gameTimeElapsed = initialElapsedTime;
    }

    public void Stop(bool newNewBest = false)
    {
        newBest = newNewBest;
        updatingTime = false;
    }

    public void SetLevel(int newCurrentLevelNumber, bool animate = false)
    {
        currentLevelNumber = newCurrentLevelNumber;

        if (animate)
        {
            levelNumberScaling.Initialize(1f, 0f, 0.5f);
            animatingNewLevel = true;
        }
    }

    public void UpdateBest(TimeSpan best, bool isNew = false)
    {
        if (isNew && Best > TimeSpan.Zero)
        {
            newBestSound.Play();
            newBestAlpha = 0f;
            newBestDuration = newBestDurationMax;
        }

        Best = best;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        viewSize = new Vector2(DeviceInfo.LogicalWidth, DeviceInfo.LogicalHeight);

        labelFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
        timeFont = content.Load<SpriteFont>(@"Fonts\time_font");

        newLevelSound = content.Load<SoundEffect>(@"Sounds\newlevel_sound");
        newBestSound = content.Load<SoundEffect>(@"Sounds\new_best");

        bestTextSize = labelFont.MeasureString(BestText);
        labelFont.MeasureString(LevelText);

        textOffset =
        [
            timeFont.MeasureString("0.00").X,
            timeFont.MeasureString("00.00").X,
            timeFont.MeasureString("000.00").X,
            timeFont.MeasureString("0000.00").X,
            timeFont.MeasureString("00000.00").X
        ];

        levelTextSize = labelFont.MeasureString(LevelText).X;

        var levelNumberTextSize = labelFont.MeasureString("5").X;

        ProgressBar.Initialize(
            new Vector2(viewSize.X - (levelNumberTextSize + levelTextSize) - TextPadding, bestTextSize.Y + 8),
            new Vector2(levelNumberTextSize + levelTextSize - 2, 6));
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (newBest)
        {
            newBestDuration = TimeSpan.Zero;
        }

        if (newBestDuration > TimeSpan.Zero)
        {
            newBestDuration -= gameTime.ElapsedGameTime;
        }

        var alpha = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 15);
        if (alpha < 0)
        {
            alpha *= -1f;
        }

        newBestAlpha = MathHelper.Lerp(0.5f, 1f, alpha);

        levelNumberScaling.Update(gameTime);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (animatingNewLevel && levelNumberScaling.Value == 0f)
        {
            animatingNewLevel = false;
            newLevelSound.Play(0.5f, 0f, 0f);
        }

        if (updatingTime)
        {
            gameTimeElapsed += gameTime.ElapsedGameTime;
        }

        bestNumberText = FormatTime(Best);
        levelNumberText = currentLevelNumber.ToString();
        timeNumberText = FormatTime(gameTimeElapsed);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        DrawBestTime(spriteBatch);

        if (isLevelAndScoreVisible)
        {
            DrawCurrentTime(spriteBatch);
            DrawLevelNumber(spriteBatch);
        }
    }

    private void DrawBestTime(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            labelFont,
            BestText,
            new Vector2(TextPadding, TextPadding),
            Color.LightGray.AdjustLight(0.9f),
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            Depth);

        spriteBatch.DrawString(
            labelFont,
            bestNumberText,
            new Vector2(TextPadding + bestTextSize.X, TextPadding),
            Color.White,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            Depth);

        if (newBest || newBestDuration > TimeSpan.Zero)
        {
            spriteBatch.DrawString(
                labelFont,
                "NEW!",
                new Vector2(TextPadding, (TextPadding + 5) + bestTextSize.Y),
                Color.Yellow * newBestAlpha,
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
          timeFont,
          timeNumberText,
          new Vector2(
              viewSize.X - textOffset[timeNumberText.Length - 4] - TextPadding, 
              TextPadding + bestTextSize.Y + TextVerticalSpacing),
          scoreColor,
          rotation: 0,
          origin: Vector2.Zero,
          scale: 1f,
          effects: SpriteEffects.None,
          layerDepth: Depth);
    }

    private void DrawLevelNumber(SpriteBatch spriteBatch)
    {
        const string infiniteText = "Infinite";

        var isInifiniteLevel = levelsCollection.GetLevel(currentLevelNumber).IsInfiniteSequence;
        var text = isInifiniteLevel ? infiniteText : levelNumberText;
        var levelNumberTextSize = labelFont.MeasureString(text).X;

        if (!isInifiniteLevel)
        {
            spriteBatch.DrawString(
                labelFont,
                LevelText,
                new Vector2(
                    viewSize.X - (levelNumberTextSize + levelTextSize) - TextPadding, 
                    TextPadding),
                Color.LightGray.AdjustLight(0.9f),
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: Depth);
        }

        var scaleFactor = 10f * levelNumberScaling.Value;
        var textScale = Vector2.One + (Vector2.One * scaleFactor);

        spriteBatch.DrawString(
            labelFont,
            text,
            new Vector2(
                (viewSize.X - (levelNumberTextSize * textScale.X) - TextPadding), 
                TextPadding),
            Color.White * (1f - levelNumberScaling.Value),
            rotation: 0,
            origin: Vector2.Zero,
            scale: textScale,
            effects: SpriteEffects.None,
            layerDepth: Depth + 0.00003f);
    }

    private static string FormatTime(TimeSpan time)
    {
        return string.Format("{0:0.00}", time.TotalSeconds);
    }
}