using System;
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
        private string bestText;

        private Vector2 bestTextSize;
        private int currentLevelNumber;

        private string levelText;
        private Color scoreColor;
        private SpriteFont timeFont;

        private Vector2 viewSize;

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

            timeFont = content.Load<SpriteFont>("highscore_font");
            bestTextSize = timeFont.MeasureString("Best: 00:00:00.00");
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

            bestText = "Best: " + FormatTime(playerData.Highscore);
            levelText = "Level: " + currentLevelNumber;
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
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds,
                (time.Milliseconds/1000f)*100);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            const float textPadding = 10;

            spriteBatch.DrawString(
                timeFont,
                FormatTime(gameTimeElapsed.Elapsed),
                new Vector2(textPadding, textPadding),
                scoreColor,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.1f);

            spriteBatch.DrawString(
                timeFont,
                bestText,
                new Vector2(viewSize.X - bestTextSize.X - textPadding, textPadding),
                Color.White, //Color.LightGray.AdjustLight(0.9f), 
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.1f);

            spriteBatch.DrawString(
                timeFont,
                levelText,
                new Vector2(textPadding, viewSize.Y - (textPadding + bestTextSize.Y)),
                Color.LightGray.AdjustLight(0.9f),
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None, 0.1f);
        }
    }
}