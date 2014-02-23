using System;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class FpsCounter : ComponentBase
    {
        private const string TextFormat = "FPS: {0} Memory: {1:0.0} MB";

        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int frameCounter;
        private int frameRate;
        private Vector2 position;
        private SpriteFont spriteFont;
        private Vector2 textSize;

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            spriteFont = content.Load<SpriteFont>("fps_font");
            textSize = spriteFont.MeasureString(TextFormat);

            const float padding = 1;

            position = new Vector2(graphics.Viewport.Width - (textSize.X + padding),
                graphics.Viewport.Height - (textSize.Y + padding));
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
#if DEBUG
            frameCounter++;

            var fps = string.Format(TextFormat, frameRate, GC.GetTotalMemory(false)/1024f/1024f);

            spriteBatch.DrawString(spriteFont, fps, position, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
#endif
        }
    }
}