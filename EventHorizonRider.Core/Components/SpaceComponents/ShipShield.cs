using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class ShipShield : ComponentBase
    {
        private const float BaseShieldAlpha = 0.8f;

        private Texture2D shieldPusleTexture;
        private Texture2D[] shieldTextures;

        private int shieldTextureIndex;

        private Motion shieldPulse = new Motion();
        private float shieldPulseAlpha;
        private float shieldPulseScale;
        private Vector2 shieldPulseLocation;

        private Ship ship;

        public void Pulse()
        {
            shieldPulseLocation = ship.Position;
            shieldPulse.Initialize(0, 1, 1f);
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            ship = Parent as Ship;

            shieldPusleTexture = content.Load<Texture2D>(@"Images\shield_pulse");

            shieldTextures = new Texture2D[3];
            for (int i = 0; i < shieldTextures.Length; i++)
            {
                shieldTextures[i] = content.Load<Texture2D>(@"Images\shield_" + (i + 1).ToString());
            }

            shieldPulse.Initialize(0, 0, 0);

            shieldTextureIndex = 0;
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            const int frameInterval = 100;
            shieldTextureIndex = (int)((((int)gameTime.TotalGameTime.TotalMilliseconds % frameInterval) / (float)frameInterval) * shieldTextures.Length);

            shieldPulse.Update(gameTime);

            shieldPulseAlpha = MathUtilities.LinearInterpolate(BaseShieldAlpha, 0, shieldPulse.Value);
            shieldPulseScale = (10f * shieldPulse.Value) + 1f;
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (shieldPulse.Value > 0f)
            {
                spriteBatch.Draw(shieldPusleTexture,
                    shieldPulseLocation,
                    origin: new Vector2(shieldPusleTexture.Width / 2f, shieldPusleTexture.Height / 2f),
                    color: Color.White * shieldPulseAlpha,
                    scale: Vector2.One * shieldPulseScale,
                    rotation: ship.Rotation,
                    depth: Depth - 0.0003f);
            }

            spriteBatch.Draw(shieldTextures[shieldTextureIndex],
                ship.Position,
                origin: new Vector2(shieldTextures[shieldTextureIndex].Width / 2f, shieldTextures[shieldTextureIndex].Height / 2f),
                color: Color.White * BaseShieldAlpha,
                scale: Vector2.One * 1f,
                rotation: ship.Rotation,
                depth: Depth - 0.0002f);
        }
    }
}
