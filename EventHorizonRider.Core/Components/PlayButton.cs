using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components
{
    internal class PlayButton
    {
        private Vector2 restartTextSize;
        private Vector2 startTextSize;

        private SpriteFont buttonFont;
        private Vector2 screenCenter;
        private Rectangle buttonBounds;

        private float colorAlphaPercent = 1f;
        private bool isVisible = true;

        public float FadeSpeed = 1.5f;

        private bool isRestart;

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>("button_font");

            restartTextSize = buttonFont.MeasureString("RESTART");
            startTextSize = buttonFont.MeasureString("START");

            screenCenter = new Vector2(
                graphics.Viewport.Width / 2f,
                graphics.Viewport.Height / 2f);

            const float buttonPadding = 10f;

            buttonBounds = new Rectangle(
                (int)(screenCenter.X - (restartTextSize.X / 2f) - buttonPadding),
                (int)(screenCenter.Y - (restartTextSize.Y / 2f) - buttonPadding),
                (int)(restartTextSize.X + buttonPadding),
                (int)(restartTextSize.Y + buttonPadding));
        }

        public void Update(GameTime gameTime, MouseState mouseState, TouchCollection touchState)
        {
            if (isVisible && IsPressed(mouseState, touchState))
            {
                Pressed(this, EventArgs.Empty);
            }

            if (isVisible && colorAlphaPercent < 1f)
            {
                colorAlphaPercent += FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (!isVisible && colorAlphaPercent > 0f)
            {
                colorAlphaPercent -= FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            colorAlphaPercent = MathHelper.Clamp(colorAlphaPercent, 0, 1);
        }

        public event EventHandler Pressed = delegate { };

        public void Show(bool isRestart)
        {
            isVisible = true;
            this.isRestart = isRestart;
            colorAlphaPercent = 1f;
        }

        public void Hide()
        {
            isVisible = false;
        }


        private bool IsPressed(MouseState mouseState, TouchCollection touchState)
        {
            return
                touchState.Any(t => t.State == TouchLocationState.Pressed && buttonBounds.Contains(t.Position)) ||
                (mouseState.LeftButton == ButtonState.Pressed && buttonBounds.Contains(mouseState.Position));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (colorAlphaPercent >= 0f)
            {
                if (!isRestart)
                {
                    var position = new Vector2(
                        screenCenter.X - (startTextSize.X / 2f),
                        screenCenter.Y - (startTextSize.Y / 2f));

                    spriteBatch.DrawString(buttonFont, "START", position, Color.White * colorAlphaPercent, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
                }
                else
                {
                    var position = new Vector2(
                        screenCenter.X - (restartTextSize.X / 2f),
                        screenCenter.Y - (restartTextSize.Y / 2f));

                    spriteBatch.DrawString(buttonFont, "RESTART", position, Color.White * colorAlphaPercent, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
                }
            }
        }
    }
}
