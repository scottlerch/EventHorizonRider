using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
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
        const float TextVerticalSpacing = 5;

        private TimeSpan gameTimeElapsed;
        private TimeSpan? gameStartTime;
        private bool updatingTime;
        private readonly PlayerData playerData;

        private Vector2 bestTextSize;

        private int currentLevelNumber;

        private string levelNumberText;
        private string bestNumberText;
        private string timeNumberText;

        private Color scoreColor;

        private SpriteFont labelFont;
        private SpriteFont timeFont;

        private Vector2 viewSize;

        private List<float> textOffset;

        private const string BestText = "Best: ";
        private const string LevelText = "Level: ";

        private bool isLevelAndScoreVisible;

        //private Texture2D foreground;

        public PlayTimer(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        public TimeSpan Elapsed
        {
            get { return gameTimeElapsed; }
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
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
        }

        public void SetLevel(int newCurrentLevelNumber)
        {
            currentLevelNumber = newCurrentLevelNumber;
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
                gameStartTime = gameStartTime ?? gameTime.TotalGameTime;
                gameTimeElapsed = gameTime.TotalGameTime - gameStartTime.Value;
            }

            scoreColor = Color.White;

            if (gameTimeElapsed >= playerData.BestTime)
            {
                scoreColor = Color.Yellow;
            }
            else
            {
                var percentComplete = 1f - (float) (gameTimeElapsed.TotalSeconds/playerData.BestTime.TotalSeconds);

                scoreColor = Color.White.SetColors(percentComplete, 1f, percentComplete);
            }

            bestNumberText = FormatTime(playerData.BestTime);
            levelNumberText = currentLevelNumber.ToString();
            timeNumberText = FormatTime(gameTimeElapsed);
        }

        public void Restart()
        {
            updatingTime = true;
            gameStartTime = null;
            gameTimeElapsed = TimeSpan.Zero;
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
            var levelTextSize = labelFont.MeasureString(LevelText).X;

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
    }
}