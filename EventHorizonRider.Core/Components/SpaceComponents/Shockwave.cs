using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Shockwave : ComponentBase
    {
        private Texture2D texture;

        private float currentRotation;

        private readonly Blackhole blackhole;

        private float currentScale;

        private bool visible;

        public Shockwave(Blackhole newBlackhole)
        {
            blackhole = newBlackhole;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\shockwave");
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, blackhole.Position,
                    origin: new Vector2(texture.Width / 2f, texture.Height / 2f),
                    rotation: currentRotation,
                    scale: new Vector2(currentScale, currentScale),
                    depth: Depth);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            currentRotation += RingInfoFactory.DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentScale += (float)gameTime.ElapsedGameTime.TotalSeconds*1.2f;

            if (currentScale > 4f)
            {
                visible = false;
            }
        }

        public void Execute()
        {
            visible = true;
            currentScale = 0.4f;
        }
    }
}
