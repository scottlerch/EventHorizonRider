using EventHorizonRider.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core
{
    internal class Blackhole
    {
        private Texture2D Texture;
        public Vector2 Position;

        internal void LoadContent(ContentManager Content, GraphicsDevice graphics)
        {
            var padding = 4;
            var radius = 60;
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

        public float Height { get { return Texture.Height * scale;  } }

        private bool startPulse = false;
        private bool isPulsing = false;
        private TimeSpan pulseDuration = TimeSpan.FromSeconds(0.5);
        private TimeSpan pulseEndTime;
        private float scale = 1f;

        public void Pulse()
        {
            if (!isPulsing)
            {
                startPulse = true;
            }
        }

        internal void Update(GameTime gameTime)
        {
            // TODO: add bounce back, each call to pulse should add more engery into pulsing
            if (startPulse)
            {
                pulseEndTime = gameTime.TotalGameTime + pulseDuration;
                isPulsing = true;
                startPulse = false;
            }
            else if (isPulsing)
            {
                if (pulseEndTime < gameTime.TotalGameTime)
                {
                    scale = 1f;
                    isPulsing = false;
                }
                else
                {
                    var x = ((4f * (float)(pulseDuration.TotalSeconds - (pulseEndTime.TotalSeconds - gameTime.TotalGameTime.TotalSeconds))) - 1f);
                    scale = -(x * x) + 2f;
                    scale = ((scale - 1f) * 0.2f) + 1f;
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                position: Position,
                origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                scale: new Vector2(scale, scale));
        }
    }
}
