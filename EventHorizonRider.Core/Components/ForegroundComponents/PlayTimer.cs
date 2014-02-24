using System;
using System.Collections.Generic;
using System.Diagnostics;
using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class PlayTimer : ComponentBase
    {
        private readonly Stopwatch gameTimeElapsed = new Stopwatch();
        private readonly PlayerData playerData;

        private Vector2 bestTextSize;
        private Vector2 levelTextSize;

        private int currentLevelNumber;

        private string levelNumberText;
        private string bestNumberText;
        private string timeNumberText;

        private Color scoreColor;

        private SpriteFont labelFont;
        private SpriteFont timeFont;

        private Vector2 viewSize;

        private List<float> textOffset;

        private const string bestText = "Best: ";
        private const string levelText = "Level: ";

        //private Texture2D foreground;

        public PlayTimer(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        public TimeSpan Elapsed
        {
            get { return gameTimeElapsed.Elapsed; }
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            viewSize = new Vector2(graphics.Viewport.Width, graphics.Viewport.Height);

            //foreground = content.Load<Texture2D>("foreground");

            labelFont = content.Load<SpriteFont>("highscore_font");
            timeFont = content.Load<SpriteFont>("time_font");

            bestTextSize = labelFont.MeasureString(bestText);
            levelTextSize = labelFont.MeasureString(levelText);

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

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            scoreColor = Color.White;

            if (gameTimeElapsed.Elapsed >= playerData.Highscore)
            {
                scoreColor = Color.Yellow;
            }
            else
            {
                var percentComplete = 1f -
                                      (float) (gameTimeElapsed.Elapsed.TotalSeconds/playerData.Highscore.TotalSeconds);

                scoreColor = Color.White.SetColors(percentComplete, 1f, percentComplete);
            }

            bestNumberText = FormatTime(playerData.Highscore);
            levelNumberText = currentLevelNumber.ToString();
            timeNumberText = FormatTime(gameTimeElapsed.Elapsed);
        }

        public void Restart()
        {
            gameTimeElapsed.Restart();
        }

        public void Stop()
        {
            gameTimeElapsed.Stop();
        }

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0:0.00}", time.TotalSeconds);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            const float textPadding = 10;
            const float textVerticalSpacing = 5;

            //spriteBatch.Draw(foreground, Vector2.Zero);

            // Draw time text
            spriteBatch.DrawString(
                timeFont,
                timeNumberText,
                new Vector2(viewSize.X - textOffset[timeNumberText.Length - 4] - textPadding, textPadding + bestTextSize.Y + textVerticalSpacing),
                scoreColor,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.1f);

            // Draw best time text
            spriteBatch.DrawString(
                labelFont,
                bestText,
                new Vector2(textPadding, textPadding),
                Color.LightGray.AdjustLight(0.9f),
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.1f);

            spriteBatch.DrawString(
                labelFont,
                bestNumberText,
                new Vector2(textPadding + bestTextSize.X, textPadding),
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.1f);

            // Draw level number
            var levelNumberTextSize = labelFont.MeasureString(levelNumberText).X;
            var levelTextSize = labelFont.MeasureString(levelText).X;

            spriteBatch.DrawString(
                labelFont,
                levelText,
                new Vector2(viewSize.X - (levelNumberTextSize + levelTextSize) - textPadding, textPadding),
                Color.LightGray.AdjustLight(0.9f),
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None, 0.1f);

            spriteBatch.DrawString(
                labelFont,
                levelNumberText,
                new Vector2(viewSize.X - levelNumberTextSize - textPadding, textPadding),
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None, 0.1f);
        }
    }
}