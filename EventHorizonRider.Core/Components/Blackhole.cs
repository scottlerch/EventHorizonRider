using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Blackhole
    {
        private Texture2D Texture;
        public Vector2 Position;

        internal void LoadContent(ContentManager Content, GraphicsDevice graphics)
        {
            var padding = 4;
            var radius = 80;
            var diameter = radius * 2;
            var paddedRadius = radius - padding;
            var origin = radius;

            var data = new Color[diameter * diameter];

            for (int y = -paddedRadius; y <= paddedRadius; y++)
                for (int x = -paddedRadius; x <= paddedRadius; x++)
                    if (x * x + y * y <= paddedRadius * paddedRadius)
                        data[origin + x + ((origin + y) * diameter)] = Color.Black;

            Texture = new Texture2D(graphics, diameter, diameter, false, SurfaceFormat.Color);
            Texture.SetData(TextureProcessor.SoftenAlpha(data, diameter, diameter));

            Position = new Vector2(
                graphics.Viewport.Width / 2,
                graphics.Viewport.Height / 2);
        }

        public float Height { get { return Texture.Height * spring.BlockX; } }

        private bool isStopped = false;

        private Spring spring = new Spring
        {
            Friction = -0.9f,
            Stiffness = -100f,
            BlockMass = 0.1f,
        };

        public void Pulse(float pullX = 1.15f, float pullVelocity = 1.5f)
        {
            spring.PullBlock(pullX, pullVelocity);
        }

        internal void Update(GameTime gameTime)
        {
            if (!isStopped)
            {
                spring.Update(gameTime.ElapsedGameTime);
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                position: Position,
                origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                scale: new Vector2(spring.BlockX, spring.BlockX));
        }

        internal void Stop()
        {
            isStopped = true;
        }

        internal void Start()
        {
            isStopped = false;
        }
    }
}
