using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHorizonRider.Core.Extensions;

namespace EventHorizonRider.Core
{
    internal class PlayTimer
    {
        private GameState gameState;
        private Stopwatch gameTimeElapsed = new Stopwatch();

        private Vector2 bestTextSize;

        private GraphicsDevice graphics;
        private SpriteFont timeFont;
        private Color scoreColor;
        private string bestText;
        private string levelText;

        public TimeSpan Elapsed { get { return gameTimeElapsed.Elapsed; } }

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            this.graphics = graphics;

            timeFont = content.Load<SpriteFont>("highscore_font");
            bestTextSize = timeFont.MeasureString("Best: 00:00:00.00");
        }

        public void Update(GameTime gameTime, GameState gameState, PlayerData playerData, int currentLevelNumber)
        {
            this.gameState = gameState;

            scoreColor = Color.White;

            if (gameTimeElapsed.Elapsed >= playerData.Highscore)
            {
                scoreColor = Color.Yellow;
            }
            else
            {
                var percentComplete = 1f - (float)(gameTimeElapsed.Elapsed.TotalSeconds / playerData.Highscore.TotalSeconds);

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
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, (time.Milliseconds / 1000f) * 100);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            const float textPadding = 10;
            const float textNewLinePadding = 5;

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
                new Vector2(graphics.Viewport.Width - bestTextSize.X - textPadding, textPadding),
                Color.White, //Color.LightGray.AdjustLight(0.9f), 
                0, 
                Vector2.Zero, 
                1, 
                SpriteEffects.None, 
                0.1f);

            spriteBatch.DrawString(
                timeFont, 
                levelText,
                new Vector2(textPadding, graphics.Viewport.Height - (textPadding + bestTextSize.Y)), 
                Color.LightGray.AdjustLight(0.9f), 
                0, 
                Vector2.Zero,
                1, 
                SpriteEffects.None, 0.1f);
        }
    }
}
