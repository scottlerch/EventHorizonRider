using System;
using System.Linq;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EventHorizonRider.Core.Components.CenterComponents
{
    internal class PlayButton : ComponentBase
    {
        public float FadeSpeed = 1.5f;
        private Rectangle buttonBounds;
        private SpriteFont buttonFont;

        private float colorAlphaPercent = 1f;

        private bool isRestart;
        private bool isVisible = true;
        private Vector2 restartTextSize;
        private Vector2 screenCenter;
        private Vector2 startTextSize;

        private readonly Func<Vector2> getScale;

        public PlayButton(Blackhole blackhole)
        {
            getScale = () => blackhole.Scale;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>("button_font");

            restartTextSize = buttonFont.MeasureString("RESTART");
            startTextSize = buttonFont.MeasureString("START");

            screenCenter = new Vector2(
                graphics.Viewport.Width/2f,
                graphics.Viewport.Height/2f);

            const float buttonPadding = 10f;

            buttonBounds = new Rectangle(
                (int) (screenCenter.X - (restartTextSize.X/2f) - buttonPadding),
                (int) (screenCenter.Y - (restartTextSize.Y/2f) - buttonPadding),
                (int) (restartTextSize.X + buttonPadding),
                (int) (restartTextSize.Y + buttonPadding));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Pressed = false;

            if (isVisible && IsPressed(inputState.MouseState, inputState.TouchState))
            {
                Pressed = true;
            }

            if (isVisible && colorAlphaPercent < 1f)
            {
                colorAlphaPercent += FadeSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (!isVisible && colorAlphaPercent > 0f)
            {
                colorAlphaPercent -= FadeSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
            }

            colorAlphaPercent = MathHelper.Clamp(colorAlphaPercent, 0, 1);
        }

        public bool Pressed { get; private set; }

        public void Show(bool restart)
        {
            isVisible = true;
            isRestart = restart;
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

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (colorAlphaPercent >= 0f)
            {
                if (!isRestart)
                {
                    spriteBatch.DrawString(buttonFont, "START", screenCenter, Color.White * colorAlphaPercent, 0, new Vector2(startTextSize.X / 2f, startTextSize.Y / 2f), 
                        getScale(), SpriteEffects.None, 0.1f);
                }
                else
                {
                    spriteBatch.DrawString(buttonFont, "RESTART", screenCenter, Color.White * colorAlphaPercent, 0,
                        new Vector2(restartTextSize.X / 2f, restartTextSize.Y / 2f), getScale(), SpriteEffects.None, 0.1f);
                }
            }
        }
    }
}