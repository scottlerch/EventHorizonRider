﻿using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class PlayTimer : ComponentBase
    {
        const float TextPadding = 10;
        const float TextVerticalSpacing = 15;

        private TimeSpan gameTimeElapsed;
        private bool updatingTime;
        private readonly PlayerData playerData;

        private Vector2 bestTextSize;

        private int currentLevelNumber;

        private string levelNumberText;
        private string bestNumberText;
        private string timeNumberText;

        private float levelTextSize;

        private readonly Color scoreColor = Color.Yellow;

        private SpriteFont labelFont;
        private SpriteFont timeFont;

        private Vector2 viewSize;

        private List<float> textOffset;

        private const string BestText = "Best: ";
        private const string LevelText = "Level: ";

        private bool isLevelAndScoreVisible;

        private float progress;

        private Texture2D progressBar;

        public PlayTimer(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        public TimeSpan Elapsed
        {
            get { return gameTimeElapsed; }
            set { gameTimeElapsed = value; }
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            progressBar = new Texture2D(graphics, 1, 1);
            progressBar.SetData(new[] { Color.White });

            viewSize = new Vector2(DeviceInfo.LogicalWidth, DeviceInfo.LogicalHeight);

            labelFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
            timeFont = content.Load<SpriteFont>(@"Fonts\time_font");

            bestTextSize = labelFont.MeasureString(BestText);
            labelFont.MeasureString(LevelText);

            textOffset = new List<float>
            {
                timeFont.MeasureString("0.00").X,
                timeFont.MeasureString("00.00").X,
                timeFont.MeasureString("000.00").X,
                timeFont.MeasureString("0000.00").X,
                timeFont.MeasureString("00000.00").X
            };

            levelTextSize = labelFont.MeasureString(LevelText).X;
        }

        public void SetLevel(int newCurrentLevelNumber)
        {
            currentLevelNumber = newCurrentLevelNumber;
        }

        public void SetProgress(float progress)
        {
            this.progress = progress;
        }

        public void ShowLevelAndScore()
        {
            isLevelAndScoreVisible = true;
        }

        public void HideLevelAndScore()
        {
            isLevelAndScoreVisible = false;
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (updatingTime)
            {
                gameTimeElapsed += gameTime.ElapsedGameTime;
            }

            bestNumberText = FormatTime(playerData.BestTime);
            levelNumberText = currentLevelNumber.ToString();
            timeNumberText = FormatTime(gameTimeElapsed);
        }

        public void Restart(TimeSpan initialElapsedTime)
        {
            updatingTime = true;
            gameTimeElapsed = initialElapsedTime;
        }

        public void Stop()
        {
            updatingTime = false;
        }

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0:0.00}", time.TotalSeconds);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            DrawBestTime(spriteBatch);

            if (isLevelAndScoreVisible)
            {
                DrawCurrentTime(spriteBatch);
                DrawLevelNumber(spriteBatch);
                DrawProgressBar(spriteBatch);
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
        }

        private void DrawCurrentTime(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
              timeFont,
              timeNumberText,
              new Vector2(viewSize.X - textOffset[timeNumberText.Length - 4] - TextPadding, TextPadding + bestTextSize.Y + TextVerticalSpacing),
              scoreColor,
              0,
              Vector2.Zero,
              1,
              SpriteEffects.None,
              Depth);
        }

        private void DrawLevelNumber(SpriteBatch spriteBatch)
        {
            var levelNumberTextSize = labelFont.MeasureString(levelNumberText).X;

            spriteBatch.DrawString(
                labelFont,
                LevelText,
                new Vector2(viewSize.X - (levelNumberTextSize + levelTextSize) - TextPadding, TextPadding),
                Color.LightGray.AdjustLight(0.9f),
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                Depth);

            spriteBatch.DrawString(
                labelFont,
                levelNumberText,
                new Vector2(viewSize.X - levelNumberTextSize - TextPadding, TextPadding),
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                Depth);
        }

        private void DrawProgressBar(SpriteBatch spriteBatch)
        {
            var levelNumberTextSize = labelFont.MeasureString("1").X;

            var position = new Vector2(viewSize.X - (levelNumberTextSize + levelTextSize) - TextPadding, bestTextSize.Y + 8);
            var scale = new Vector2(levelNumberTextSize + levelTextSize - 2, 6);

            spriteBatch.Draw(
                progressBar,
                position: new Vector2(position.X + 2, position.Y + 2),
                color: Color.Black,
                scale: scale,
                depth: Depth);

            spriteBatch.Draw(
                progressBar,
                position: position,
                color: Color.DarkGray.AdjustLight(0.5f),
                scale: scale,
                depth: Depth + 0.00001f);

            spriteBatch.Draw(
                progressBar,
                position: position,
                color: Color.Green,
                scale: new Vector2(MathUtilities.LinearInterpolate(0f, scale.X, progress), scale.Y),
                depth: Depth + 0.00002f);
        }
    }
}