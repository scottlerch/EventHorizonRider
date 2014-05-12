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

        private readonly Motion shieldPulseMotion;

        private Texture2D shieldPusleTexture;
        private Vector2 shieldPulseLocation;
        private Vector2 shieldPulseOrigin;
        private float shieldPulseAlpha;
        private float shieldPulseScale;

        private int shieldTextureIndex;
        private Texture2D[] shieldTextures;
        private Vector2[] shieldTexturesOrigins;

        private Ship ship;

        public ShipShield()
        {
            shieldPulseMotion = new Motion();
        }

        public void Pulse()
        {
            shieldPulseLocation = ship.Position;
            shieldPulseMotion.Initialize(0, 1, 1f);
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            const int NumberOfShieldPulseTextures = 3;

            ship = Parent as Ship;

            shieldPusleTexture = content.Load<Texture2D>(@"Images\shield_pulse");

            shieldTextures = new Texture2D[NumberOfShieldPulseTextures];
            shieldTexturesOrigins = new Vector2[NumberOfShieldPulseTextures];

            for (int i = 0; i < shieldTextures.Length; i++)
            {
                shieldTextures[i] = content.Load<Texture2D>(@"Images\shield_" + (i + 1));
                shieldTexturesOrigins[i] = new Vector2(shieldTextures[i].Width / 2f, shieldTextures[i].Height / 2f);
            }

            shieldPulseMotion.Initialize(0, 0, 0);

            shieldTextureIndex = 0;

            shieldPulseOrigin = new Vector2(shieldPusleTexture.Width/2f, shieldPusleTexture.Height/2f);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            const int frameInterval = 100;
            shieldTextureIndex = (int)((((int)gameTime.TotalGameTime.TotalMilliseconds % frameInterval) / (float)frameInterval) * shieldTextures.Length);

            shieldPulseMotion.Update(gameTime);

            shieldPulseAlpha = MathUtilities.LinearInterpolate(BaseShieldAlpha, 0, shieldPulseMotion.Value);
            shieldPulseScale = (10f * shieldPulseMotion.Value) + 1f;
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (shieldPulseMotion.Value > 0f)
            {
                spriteBatch.Draw(
                    shieldPusleTexture,
                    shieldPulseLocation,
                    origin: shieldPulseOrigin,
                    color: Color.White * shieldPulseAlpha,
                    scale: Vector2.One * shieldPulseScale,
                    rotation: ship.Rotation,
                    depth: Depth - 0.0003f);
            }

            spriteBatch.Draw(
                shieldTextures[shieldTextureIndex],
                ship.Position,
                origin: shieldTexturesOrigins[shieldTextureIndex],
                color: Color.White * BaseShieldAlpha,
                scale: Vector2.One,
                rotation: ship.Rotation,
                depth: Depth - 0.0002f);
        }
    }
}
