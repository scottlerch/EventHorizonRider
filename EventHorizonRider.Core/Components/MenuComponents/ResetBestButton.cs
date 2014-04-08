using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.MenuComponents
{
    internal class ResetButton : ComponentBase
    {
        private SpriteFont buttonFont;

        private Vector2 textLocation;
        private Vector2 textSize;

        public Button Button { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");

            textSize = buttonFont.MeasureString("RESET BEST");

            textLocation = new Vector2(
                (graphics.Viewport.Width / 2f) - (textSize.X / 2f),
                (graphics.Viewport.Height / 2f) + 25f);

            const float buttonPadding = 50f;

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int) (textLocation.X - buttonPadding),
                    (int) (textLocation.Y - buttonPadding),
                    (int) (textSize.X + (buttonPadding*2)),
                    (int) (textSize.Y + (buttonPadding*2))),
                key: Keys.R);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(inputState, Visible);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                buttonFont,
                "RESET BEST",
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