﻿using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.MenuComponents
{
    internal class CreditsButton : ComponentBase
    {
        private SpriteFont buttonFont;

        private Vector2 textLocation;
        private Vector2 textSize;

        public Button Button { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");

            textSize = buttonFont.MeasureString("CREDITS");

            const float buttonPadding = 50f;

            textLocation = new Vector2(
                (DeviceInfo.LogicalWidth / 2f) - (textSize.X / 2f),
                (DeviceInfo.LogicalHeight / 2f) + 150f);

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int)(textLocation.X - buttonPadding),
                    (int)(textLocation.Y - buttonPadding),
                    (int)(textSize.X + (buttonPadding * 2)),
                    (int)(textSize.Y + (buttonPadding * 2))),
                key: Keys.C);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(inputState, Visible);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                buttonFont,
                "CREDITS",
                textLocation,
                Button.Hover? Color.Yellow : Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }
    }
}