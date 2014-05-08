using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
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

        private Color currentColor;
        private Color executeColor;

        public Shockwave(Blackhole newBlackhole)
        {
            blackhole = newBlackhole;
            Visible = false;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            currentColor = Color.White;

            texture = content.Load<Texture2D>(@"Images\shockwave");
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, blackhole.Position,
                    origin: new Vector2(texture.Width / 2f, texture.Height / 2f),
                    rotation: currentRotation,
                    scale: new Vector2(currentScale, currentScale),
                    color: executeColor * 0.3f,
                    depth: Depth);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            currentRotation += blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentScale += (float)gameTime.ElapsedGameTime.TotalSeconds*1.2f;

            if (currentScale > 4f)
            {
                Visible = false;
            }
        }

        public void Execute()
        {
            Visible = true;
            currentScale = 0.4f;
            executeColor = currentColor;
        }

        public void SetColor(Color color)
        {
            currentColor = color;
        }
    }
}
