using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class BlackholeHalo : ComponentBase
    {
        private readonly Blackhole blackhole;

        private Texture2D texture;
        private Vector2 textureOrigin;
        private float currentRotation;

        public BlackholeHalo(Blackhole newBlackhole)
        {
            blackhole = newBlackhole;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\halo");
            textureOrigin = new Vector2(texture.Width/2f, texture.Height/2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, blackhole.Position,
                    origin: textureOrigin,
                    rotation: currentRotation,
                    scale: blackhole.Scale,
                    color: Color.White * 0.4f,
                    layerDepth: Depth);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            currentRotation += blackhole.RotationalVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
