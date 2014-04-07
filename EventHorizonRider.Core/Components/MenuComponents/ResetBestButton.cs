using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace EventHorizonRider.Core.Components.MenuComponents
{
    internal class ResetButton : ComponentBase
    {
        private Rectangle buttonBounds;
        private SpriteFont buttonFont;

        private Vector2 textLocation;
        private Vector2 textSize;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");

            textSize = buttonFont.MeasureString("RESET BEST");

            textLocation = new Vector2(
                (graphics.Viewport.Width / 2f) - (textSize.X / 2f),
                graphics.Viewport.Height / 2f);

            const float buttonPadding = 50f;

            buttonBounds = new Rectangle(
                (int)(textLocation.X),
                (int)(textLocation.Y),
                (int)(textSize.X + buttonPadding),
                (int)(textSize.Y + buttonPadding));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Pressed = false;

            if (Visible && IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState))
            {
                Pressed = true;
            }
        }

        public bool Pressed { get; private set; }

        private bool IsPressed(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
        {
            return
                keyboardState.GetPressedKeys().Contains(Keys.R) ||
                touchState.Any(t => t.State == TouchLocationState.Pressed && buttonBounds.Contains(t.Position)) ||
                (mouseState.LeftButton == ButtonState.Pressed && buttonBounds.Contains(mouseState.Position));
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                buttonFont,
                "RESET BEST",
                textLocation,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);
        }
    }
}