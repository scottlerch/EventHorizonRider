using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core.Components
{
    public class FpsCounter
    {
        private SpriteFont spriteFont;

        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private Vector2 textSize;
        private readonly string textFormat = "FPS: {0} Memory: {1:0.0} MB";
        private Vector2 position;

        internal void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            spriteFont = content.Load<SpriteFont>("fps_font");
            textSize = spriteFont.MeasureString(textFormat);

            const float padding = 1;

            position = new Vector2(graphicsDevice.Viewport.Width - (textSize.X + padding), graphicsDevice.Viewport.Height - (textSize.Y + padding));
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;

            string fps = string.Format(textFormat, frameRate, GC.GetTotalMemory(false) / 1024f / 1024f);

            spriteBatch.DrawString(spriteFont, fps, position, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
    }
}
