using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Halo : ComponentBase
    {
        private Texture2D texture;

        private float currentRotation;

        private readonly Blackhole blackhole;

        public Halo(Blackhole newBlackhole)
        {
            blackhole = newBlackhole;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\halo");
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, blackhole.Position,
                    origin: new Vector2(texture.Width/2f, texture.Height/2f),
                    rotation: currentRotation,
                    scale: blackhole.Scale,
                    color: Color.White * 0.4f,
                    depth: Depth);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            currentRotation += RingInfoFactory.DefaultRotationVelocity*(float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
