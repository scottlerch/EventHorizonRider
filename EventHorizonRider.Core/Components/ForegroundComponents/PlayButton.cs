using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class PlayButton : ComponentBase
    {
        private float fadeSpeed = 1.5f;

        private SpriteFont buttonFont;

        private float colorAlphaPercent = 1f;

        private bool isRestart;
        private bool isVisible = true;
        private Vector2 restartTextSize;
        private Vector2 screenCenter;
        private Vector2 startTextSize;

        public float Scale { get; set; }

        public Button Button { get; private set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\button_font");

            restartTextSize = buttonFont.MeasureString("RESET");
            startTextSize = buttonFont.MeasureString("START");

            screenCenter = new Vector2(
                graphics.Viewport.Width / 2f,
                graphics.Viewport.Height / 2f);

            const float buttonPadding = 100f;

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int)(screenCenter.X - ((restartTextSize.X + buttonPadding) / 2f)),
                    (int)(screenCenter.Y - ((restartTextSize.Y + buttonPadding) / 2f)),
                    (int)(restartTextSize.X + buttonPadding),
                    (int)(restartTextSize.Y + buttonPadding)),
               key: Keys.Space);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(inputState, isVisible);

            if (isVisible && colorAlphaPercent < 1f)
            {
                colorAlphaPercent += fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (!isVisible && colorAlphaPercent > 0f)
            {
                colorAlphaPercent -= fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            colorAlphaPercent = MathHelper.Clamp(colorAlphaPercent, 0, 1);
        }

        public void Show(bool restart, bool fade = false, float newFadeSpeed = 1.5f)
        {
            isVisible = true;
            isRestart = restart;

            if (!fade)
            {
                colorAlphaPercent = 1f;
            }

            fadeSpeed = newFadeSpeed;
        }

        public void Hide(bool fade = true, float newFadeSpeed = 1.5f)
        {
            if (!fade)
            {
                colorAlphaPercent = 0f;    
            }

            isVisible = false;
            fadeSpeed = newFadeSpeed;
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (colorAlphaPercent >= 0f)
            {
                if (!isRestart)
                {
                    spriteBatch.DrawString(
                        buttonFont, 
                        "START", 
                        screenCenter,
                        (Button.Hover ? Color.Yellow : Color.White) * colorAlphaPercent, 
                        0, 
                        new Vector2(startTextSize.X / 2f, startTextSize.Y / 2f),
                        Scale, 
                        SpriteEffects.None, Depth);
                }
                else
                {
                    spriteBatch.DrawString(
                        buttonFont, 
                        "RESET", 
                        screenCenter,
                        (Button.Hover? Color.Yellow : Color.White) * colorAlphaPercent,
                        0,
                        new Vector2(restartTextSize.X / 2f, restartTextSize.Y / 2f), 
                        Scale,
                        SpriteEffects.None, Depth);
                }
            }
        }
    }
}