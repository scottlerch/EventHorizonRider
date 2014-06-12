using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class MenuButton : ComponentBase
    {
        private const string MenuText = "LEVEL SELECT";
        private const string BackText = "BACK";

        private SpriteFont buttonFont;
        private SoundEffect buttonSound;

        private bool isVisible = true;
        private bool isBack;

        private Vector2 menuTextLocation;
        private Vector2 menuTextSize;

        private Vector2 backTextLocation;
        private Vector2 backTextSize;

        public Button Button { get; private set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");
            buttonSound = content.Load<SoundEffect>(@"Sounds\button_click");

            menuTextSize = buttonFont.MeasureString(MenuText);
            backTextSize = buttonFont.MeasureString(BackText);

            const float textPadding = 10;

            menuTextLocation = new Vector2(DeviceInfo.LogicalWidth - (menuTextSize.X) - textPadding, textPadding);
            backTextLocation = new Vector2(DeviceInfo.LogicalWidth - (backTextSize.X) - textPadding, textPadding);

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
            Button.Update(gameTime, inputState, Visible);

            if (Button.Pressed)
            {
                buttonSound.Play();
            }
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
                        BackText,
                        backTextLocation,
                        Button.Hover? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        depth: Depth);
                }
                else
                {
                    spriteBatch.DrawString(
                        buttonFont,
                        MenuText,
                        menuTextLocation,
                        Button.Hover ? Color.Yellow : Color.LightGray.AdjustLight(0.9f),
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        depth: Depth);
                }
            }
        }
    }
}