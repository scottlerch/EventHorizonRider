﻿using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components.ForegroundComponents
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
            buttonFont = content.Load<SpriteFont>(@"Fonts\button_font");

            restartTextSize = buttonFont.MeasureString("RESET");
            startTextSize = buttonFont.MeasureString("START");

            screenCenter = new Vector2(
                graphics.Viewport.Width / 2f,
                graphics.Viewport.Height / 2f);

            const float buttonPadding = 100f;

            buttonBounds = new Rectangle(
                (int)(screenCenter.X - ((restartTextSize.X + buttonPadding) / 2f)),
                (int)(screenCenter.Y - ((restartTextSize.Y + buttonPadding) / 2f)),
                (int)(restartTextSize.X + buttonPadding),
                (int)(restartTextSize.Y + buttonPadding));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Pressed = false;

            if (isVisible && IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState))
            {
                Pressed = true;
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

        public bool Pressed { get; private set; }

        public void Show(bool restart)
        {
            isVisible = true;
            isRestart = restart;
            colorAlphaPercent = 1f;
        }

        public void Hide(bool fade = true)
        {
            if (!fade)
            {
                colorAlphaPercent = 0f;    
            }

            isVisible = false;
        }


        private bool IsPressed(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
        {
            return 
                keyboardState.GetPressedKeys().Contains(Keys.Space) || 
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
                        getScale(), SpriteEffects.None, Depth);
                }
                else
                {
                    spriteBatch.DrawString(buttonFont, "RESET", screenCenter, Color.White * colorAlphaPercent, 0,
                        new Vector2(restartTextSize.X / 2f, restartTextSize.Y / 2f), getScale(), SpriteEffects.None, Depth);
                }
            }
        }
    }
}