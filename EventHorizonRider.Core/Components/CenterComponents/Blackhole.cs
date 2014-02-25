using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.CenterComponents
{
    internal class Blackhole : ComponentBase
    {
        private readonly Spring spring = new Spring
        {
            Friction = -0.9f,
            Stiffness = -100f,
            BlockMass = 0.1f,
        };

        public Vector2 Position { get; private set; }

        private Texture2D texture;

        private bool isStopped = true;

        private float currentRotation = 0f;

        public float Height
        {
            get { return texture.Height*spring.BlockX; }
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            const int padding = 4;
            const int radius = 80;
            const int diameter = radius*2;
            const int paddedRadius = radius - padding;
            const int origin = radius;

            var data = new Color[diameter*diameter];

            for (var y = -paddedRadius; y <= paddedRadius; y++)
                for (var x = -paddedRadius; x <= paddedRadius; x++)
                    if (x*x + y*y <= paddedRadius*paddedRadius)
                        data[origin + x + ((origin + y)*diameter)] = Color.Black;

            texture = new Texture2D(graphics, diameter, diameter, false, SurfaceFormat.Color);
            texture.SetData(TextureProcessor.SoftenAlpha(data, diameter, diameter));

            Position = new Vector2(
                graphics.Viewport.Width/2f,
                graphics.Viewport.Height/2f);
        }

        public void Pulse(float pullX = 1.15f, float pullVelocity = 1.5f)
        {
            spring.PullBlock(pullX, pullVelocity);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (!isStopped)
            {
                spring.Update(gameTime.ElapsedGameTime);
                currentRotation += RingInfoFactory.DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public Vector2 Scale { get { return new Vector2(spring.BlockX, spring.BlockX); } }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position,
                origin: new Vector2(texture.Width/2f, texture.Height/2f),
                rotation: currentRotation,
                scale: Scale);
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