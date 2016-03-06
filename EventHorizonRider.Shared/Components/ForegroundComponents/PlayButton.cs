using System.Collections.Generic;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class PlayButton : ComponentBase
    {
        private class Info
        {
            public readonly string Text;
            public readonly Vector2 Size;
            public readonly Color Color;
            public readonly Color HoverColor;

            public Info(string text, Vector2 size, Color color, Color hoverColor)
            {
                Text = text;
                Size = size;
                Color = color;
                HoverColor = hoverColor;
            }
        }

        private float fadeSpeed = 1.5f;

        private SpriteFont buttonFont;

        private float colorAlphaPercent = 1f;

        private PlayButtonState playButtonState;
        private bool isVisible = true;

        private Dictionary<PlayButtonState, Info> textInfo;

        public float Scale { get; set; }

        public Button Button { get; private set; }

        public void Show(PlayButtonState state, bool fade = false, float newFadeSpeed = 1.5f)
        {
            isVisible = true;
            playButtonState = state;

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

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\button_font");

            textInfo = new Dictionary<PlayButtonState, Info>
            {
                {
                    PlayButtonState.Start, 
                    new Info("START", buttonFont.MeasureString("START"), Color.White, Color.Yellow)
                },
                { 
                    PlayButtonState.Restart, 
                    new Info("RESET", buttonFont.MeasureString("RESET"), Color.White, Color.Yellow) 
                    },
                {
                    PlayButtonState.Resume, 
                    new Info("RESUME", buttonFont.MeasureString("RESUME"), Color.White, Color.Yellow)
                },
                {
                    PlayButtonState.Pause, 
                    new Info("PAUSE", buttonFont.MeasureString("PAUSE"), Color.DarkGray.AdjustLight(0.2f), Color.Gray.AdjustLight(0.3f))
                },
            };

            const float buttonPadding = 100f;

            var textSize = textInfo[PlayButtonState.Restart].Size;

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int)(DeviceInfo.LogicalCenter.X - ((textSize.X + buttonPadding) / 2f)),
                    (int)(DeviceInfo.LogicalCenter.Y - ((textSize.Y + buttonPadding) / 2f)),
                    (int)(textSize.X + buttonPadding),
                    (int)(textSize.Y + buttonPadding)),
               key: Keys.Space);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(gameTime, inputState, isVisible);

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

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (colorAlphaPercent >= 0f)
            {
                var info = textInfo[playButtonState];

                spriteBatch.DrawString(
                    buttonFont,
                    info.Text,
                    DeviceInfo.LogicalCenter,
                    (Button.Hover ? info.HoverColor : info.Color) * colorAlphaPercent,
                    rotation: 0f,
                    origin: new Vector2(info.Size.X / 2f, info.Size.Y / 2f),
                    scale: Scale,
                    effects: SpriteEffects.None,
                    layerDepth: Depth);
            }
        }
    }
}