using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    internal class Blackhole
    {
        public Texture2D Texture;
        public Vector2 Position;

        internal void LoadContent(ContentManager Content, GraphicsDevice graphics)
        {
            var padding = 4;
            var radius = 50;
            var diameter = radius * 2;
            var paddedRadius = radius - padding;
            var origin = radius;

            uint[] data = new uint[diameter * diameter];

            for (int y = -paddedRadius; y <= paddedRadius; y++)
                for (int x = -paddedRadius; x <= paddedRadius; x++)
                    if (x * x + y * y <= paddedRadius * paddedRadius)
                        data[origin + x + ((origin + y) * diameter)] = Color.Black.PackedValue;

            Texture = new Texture2D(graphics, diameter, diameter, false, SurfaceFormat.Color);
            Texture.SetData(data);

            // Texture = Content.Load<Texture2D>("blackhole");

            Position = new Vector2(
                graphics.Viewport.Width / 2,
                graphics.Viewport.Height / 2);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                position: Position,
                origin: new Vector2(Texture.Width / 2, Texture.Height / 2));
        }
    }
}
