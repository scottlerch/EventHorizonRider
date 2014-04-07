using EventHorizonRider.Core.Engine;
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
    internal class LevelSelect : ComponentBase
    {
        private class LevelButton
        {
            public Vector2 Position;
            public Vector2 Size;
            public string Text;
            public Rectangle Bounds;
            public int LevelNumber;
        }

        private SpriteFont buttonFont;

        private Vector2 startLevelTextLocation;
        private Vector2 startLevelTextSize;

        private LevelButton[] levelButtons;

        public int StartLevel { get; set; }

        public int MaximumStartLevel { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            StartLevel = 1;
            MaximumStartLevel = 1;

            buttonFont = content.Load<SpriteFont>(@"Fonts\highscore_font");

            startLevelTextSize = buttonFont.MeasureString("START LEVEL");

            const float buttonPadding = 50f;

            startLevelTextLocation = new Vector2(
                (graphics.Viewport.Width / 2f) - (startLevelTextSize.X / 2f),
                (graphics.Viewport.Height / 2f) - 175f);

            levelButtons = new LevelButton[Levels.NumberOfLevels];

            var levelButtonY = startLevelTextLocation.Y + 75f;
            var levelButtonsWidth = 500f;
            var levelButtonSpacing = levelButtonsWidth/(Levels.NumberOfLevels - 1);
            var levelButtonXBase = (graphics.Viewport.Width / 2f) - (levelButtonsWidth / 2f);

            for (int i = 0; i < Levels.NumberOfLevels; i++)
            {
                var position = new Vector2(
                    levelButtonXBase + (i * levelButtonSpacing), 
                    levelButtonY);

                var size = buttonFont.MeasureString(i.ToString());

                levelButtons[i] = new LevelButton
                {
                    LevelNumber = i + 1,
                    Text = (i + 1).ToString(),
                    Position = position,
                    Size = size,
                    Bounds = new Rectangle(
                        (int)(position.X),
                        (int)(position.Y),
                        (int)(size.X + buttonPadding),
                        (int)(size.Y + buttonPadding))
                };
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            Pressed = null;

            if (Visible)
            {
                Pressed = IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState);
            }
        }

        public int? Pressed { get; private set; }

        private int? IsPressed(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
        {
            var key = keyboardState.GetPressedKeys()
                .FirstOrDefault(k => k >= Keys.D1 && k <= (Keys) ((int) Keys.D1 + Levels.NumberOfLevels));

            if (key != default(Keys))
            {
                return key - Keys.D1;
            }

            var levelButton =
                levelButtons.Where((button, i) =>
                    touchState.Any(t => t.State == TouchLocationState.Pressed && button.Bounds.Contains(t.Position)) ||
                    (mouseState.LeftButton == ButtonState.Pressed && button.Bounds.Contains(mouseState.Position)))
                    .FirstOrDefault();

            if (levelButton != null && levelButton.LevelNumber <= MaximumStartLevel)
            {
                return levelButton.LevelNumber;
            }

            return null;
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                buttonFont,
                "START LEVEL",
                startLevelTextLocation,
                Color.LightGray.AdjustLight(0.7f),
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Depth);

            foreach (var levelButton in levelButtons)
            {
                var color = Color.LightGray.AdjustLight(0.4f);

                if (levelButton.LevelNumber == StartLevel)
                {
                    color = Color.Yellow;
                }
                else if (levelButton.LevelNumber <= MaximumStartLevel)
                {
                    color = Color.White;
                }

                spriteBatch.DrawString(
                    buttonFont,
                    levelButton.Text,
                    levelButton.Position,
                    color,
                    0,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    Depth);
            }
        }
    }
}