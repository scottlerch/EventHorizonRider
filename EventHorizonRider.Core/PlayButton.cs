using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    internal class PlayButton
    {
        private Vector2 restartTextSize;
        private Vector2 startTextSize;

        private SpriteFont buttonFont;
        private Vector2 screenCenter;
        private Rectangle buttonBounds;

        private GameState gameState;

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            buttonFont = content.Load<SpriteFont>("button_font");

            restartTextSize = buttonFont.MeasureString("RESTART");
            startTextSize = buttonFont.MeasureString("START");

            screenCenter = new Vector2(
                graphics.Viewport.Width / 2f,
                graphics.Viewport.Height / 2f);

            const float buttonPadding = 10f;

            buttonBounds = new Rectangle(
                (int)(screenCenter.X - (restartTextSize.X / 2f) - buttonPadding),
                (int)(screenCenter.Y - (restartTextSize.Y / 2f) - buttonPadding),
                (int)(restartTextSize.X + buttonPadding),
                (int)(restartTextSize.Y + buttonPadding));
        }

        public void Update(GameTime gameTime, MouseState mouseState, TouchCollection touchState, GameState gameState)
        {
            this.gameState = gameState;

            if ((gameState == GameState.Init || gameState == GameState.Paused) &&
                IsPressed(mouseState, touchState))
            {
                Pressed = true;
            }
            else
            {
                Pressed = false;
            }
        }

        private bool IsPressed(MouseState mouseState, TouchCollection touchState)
        {
            return
                touchState.Any(t => t.State == TouchLocationState.Pressed && buttonBounds.Contains(t.Position)) ||
                (mouseState.LeftButton == ButtonState.Pressed && buttonBounds.Contains(mouseState.Position));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (gameState == GameState.Init)
            {
                var position = new Vector2(
                    screenCenter.X - (startTextSize.X / 2f),
                    screenCenter.Y - (startTextSize.Y / 2f));

                spriteBatch.DrawString(buttonFont, "START", position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            }
            else if (gameState == GameState.Paused)
            {
                var position = new Vector2(
                    screenCenter.X - (restartTextSize.X / 2f),
                    screenCenter.Y - (restartTextSize.Y / 2f));

                spriteBatch.DrawString(buttonFont, "RESTART", position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            }
        }

        public bool Pressed { get; private set; }
    }
}
