﻿using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class ControlsHelp : ComponentBase
    {
        private const float MaxAlpha = 0.3f;

        private Vector2 helpLeftPosition;
        private Texture2D helpLeft;

        private Vector2 helpRightPosition;
        private Texture2D helpRight;

        private Vector2 helpStartPosition;
        private Texture2D helpStart;

        private bool fading;
        private float directionAlpha = MaxAlpha;
        private float directionFadeSpeed;

        private float startAlpha = MaxAlpha;
        private float startFadeSpeed;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            Visible = true;

            helpLeft = content.Load<Texture2D>(@"Images\help_left");
            helpRight = content.Load<Texture2D>(@"Images\help_right");
            helpStart = content.Load<Texture2D>(@"Images\help_start");

            helpLeftPosition = new Vector2(0, (DeviceInfo.LogicalHeight / 2) - (helpLeft.Height / 2));
            helpRightPosition = new Vector2(DeviceInfo.LogicalWidth - helpRight.Width, (DeviceInfo.LogicalHeight / 2) - (helpLeft.Height / 2));
            helpStartPosition = new Vector2((DeviceInfo.LogicalWidth / 2) - (helpStart.Width / 2), (DeviceInfo.LogicalHeight / 2) + 125);
        }

        public void Hide(float speed)
        {
            if (fading && directionFadeSpeed < 0f) return;

            fading = true;
            directionFadeSpeed = speed > 0f ? speed * -1f : speed;
            startFadeSpeed = directionFadeSpeed*8f;
        }

        public void Show(float speed)
        {
            if (fading && directionFadeSpeed > 0f) return;

            Visible = true;
            fading = true;
            directionFadeSpeed = speed < 0f ? speed * -1f : speed;
            startFadeSpeed = directionFadeSpeed;
        }
            
        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (fading)
            {
                directionAlpha += (float) gameTime.ElapsedGameTime.TotalSeconds*directionFadeSpeed;
                startAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * startFadeSpeed;

                startAlpha = MathHelper.Clamp(startAlpha, 0f, MaxAlpha);

                if (directionAlpha < 0f)
                {
                    directionAlpha = 0f;
                    fading = false;
                    Visible = false;
                }
                else if (directionAlpha > MaxAlpha)
                {
                    directionAlpha = MaxAlpha;
                    fading = false;
                }
            }
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(helpLeft, color: Color.White * directionAlpha, position: helpLeftPosition, depth: Depth);
            spriteBatch.Draw(helpRight, color: Color.White * directionAlpha, position: helpRightPosition, depth: Depth);
            spriteBatch.Draw(helpStart, color: Color.White * startAlpha, position: helpStartPosition, depth: Depth);
        }
    }
}