using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class MenuButton : ComponentBase
    {
        private Rectangle buttonBounds;
        private SpriteFont buttonFont;

        private bool isVisible = true;
        private bool isBack = false;

        private Vector2 menuTextLocation;
        private Vector2 menuTextSize;

        private Vector2 backTextLocation;
        private Vector2 backTextSize;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");

            menuTextSize = buttonFont.MeasureString("MENU");
            backTextSize = buttonFont.MeasureString("BACK");

            const float textPadding = 10;

            menuTextLocation = new Vector2(graphics.Viewport.Width - (menuTextSize.X) - textPadding, textPadding);
            backTextLocation = new Vector2(graphics.Viewport.Width - (backTextSize.X) - textPadding, textPadding);

            const float buttonPadding = 50f;

            buttonBounds = new Rectangle(
                (int)(menuTextLocation.X - buttonPadding),
                (int)(menuTextLocation.Y - buttonPadding),
                (int)(menuTextSize.X + (buttonPadding * 2)),
                (int)(menuTextSize.Y + (buttonPadding * 2)));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Pressed = false;

            if (isVisible && IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState))
            {
                Pressed = true;
            }
        }

        public bool Pressed { get; private set; }

        public void Show(bool back = false)
        {
            isVisible = true;
            isBack = back;
        }

        public void Hide()
        {
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
            if (isVisible)
            {
                if (isBack)
                {
                    spriteBatch.DrawString(
                        buttonFont,
                        "BACK",
                        backTextLocation,
                        Color.LightGray.AdjustLight(0.9f),
                        0,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        Depth);
                }
                else
                {
                    spriteBatch.DrawString(
                        buttonFont,
                        "MENU",
                        menuTextLocation,
                        Color.LightGray.AdjustLight(0.9f),
                        0,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        Depth);
                }
            }
        }
    }
}