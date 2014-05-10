using System;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.MenuComponents
{
    internal class ResetButton : ComponentBase
    {
        private const string Text = "RESET BEST";
        private const string WarningText = "Hold to clear all stats!";

        private SpriteFont buttonFont;
        private SoundEffect buttonSound;

        private Vector2 textLocation;
        private Vector2 textSize;

        private Vector2 warningTextLocation;
        private Vector2 warningTextSize;
        private float warningAlpha;

        public Button Button { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
            buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

            textSize = buttonFont.MeasureString(Text);
            warningTextSize = buttonFont.MeasureString(WarningText);

            textLocation = new Vector2(
                (DeviceInfo.LogicalWidth / 2f) - (textSize.X / 2f),
                (DeviceInfo.LogicalHeight / 2f) + 35f);

            warningTextLocation = new Vector2(
                (DeviceInfo.LogicalWidth / 2f) - (warningTextSize.X / 2f),
                textLocation.Y - textSize.Y - 5f);

            const float buttonPadding = 25f;

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int) (textLocation.X - buttonPadding),
                    (int) (textLocation.Y - buttonPadding),
                    (int) (textSize.X + (buttonPadding*2)),
                    (int) (textSize.Y + (buttonPadding*2))),
                key: Keys.R,
                holdDuration:TimeSpan.FromSeconds(3));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(gameTime, inputState, Visible);

            if (Button.Pressed)
            {
                buttonSound.Play();
            }

            if (Button.Holding)
            {
                var alpha = (float) Math.Sin(Button.CurrentHoldDuration.TotalSeconds*15);
                if (alpha < 0)
                {
                    alpha *= -1f;
                }

                warningAlpha = MathUtilities.LinearInterpolate(0.5f, 1f, alpha);
            }
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                buttonFont,
                Text,
                textLocation,
                Button.Hover? Color.Yellow : Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);

            if (Button.Holding && Button.HoldDurationRemaining > TimeSpan.Zero)
            {
                spriteBatch.DrawString(
                    buttonFont,
                    WarningText,
                    warningTextLocation,
                    Color.Red * warningAlpha,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    Depth);

                var holdDurationText = (Button.HoldDurationRemaining.Seconds + 1).ToString();

                spriteBatch.DrawString(
                    buttonFont,
                    holdDurationText,
                    new Vector2(textLocation.X + textSize.X + 20f, textLocation.Y), 
                    Color.White,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    Depth);

                var holdDurationTextSize = buttonFont.MeasureString(holdDurationText);

                spriteBatch.DrawString(
                    buttonFont,
                    holdDurationText,
                    new Vector2(textLocation.X - holdDurationTextSize.X - 20f, textLocation.Y),
                    Color.White,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    Depth);
            }
        }
    }
}