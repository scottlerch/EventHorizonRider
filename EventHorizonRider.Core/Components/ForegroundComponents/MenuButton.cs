using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class MenuButton : ComponentBase
    {
        public Button Button { get; private set; }

        private SpriteFont buttonFont;

        private bool isVisible = true;
        private bool isBack;

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

            menuTextLocation = new Vector2(ScreenInfo.LogicalWidth - (menuTextSize.X) - textPadding, textPadding);
            backTextLocation = new Vector2(ScreenInfo.LogicalWidth - (backTextSize.X) - textPadding, textPadding);

            const float buttonPadding = 50f;

            Button = new Button(
                buttonBounds: new Rectangle(
                    (int) (menuTextLocation.X - buttonPadding),
                    (int) (menuTextLocation.Y - buttonPadding),
                    (int) (menuTextSize.X + (buttonPadding*2)),
                    (int) (menuTextSize.Y + (buttonPadding*2))),
                key: Keys.M);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Button.Update(inputState, Visible);
        }

        public void Show(bool back = false)
        {
            isVisible = true;
            isBack = back;
        }

        public void Hide()
        {
            isVisible = false;
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
                        Button.Hover? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
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
                        Button.Hover ? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
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