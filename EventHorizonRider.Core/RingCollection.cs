using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class RingCollection
    {
        private Texture2D ringEdgeTexture;
        private List<Ring> rings = new List<Ring>();
        private Random rand = new Random();
        private GraphicsDevice graphicsDevice;
        private DateTime lastRingAdd = DateTime.UtcNow;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ringEdgeTexture = new Texture2D(graphicsDevice, 16, 25, false, SurfaceFormat.Color);
            ringEdgeTexture.SetData(Enumerable.Range(0, ringEdgeTexture.Width * ringEdgeTexture.Height).Select(i => Color.DarkGray.PackedValue).ToArray());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var ring in rings)
            {
                ring.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var ring in rings.ToList())
            {
                ring.Radius -= (float)gameTime.ElapsedGameTime.TotalSeconds * 150f;

                if (ring.Radius <= 0)
                {
                    rings.Remove(ring);
                }
            }

            if (DateTime.UtcNow - lastRingAdd > TimeSpan.FromSeconds(0.9))
            {
                lastRingAdd = DateTime.UtcNow;
                AddRing();
            }
        }

        public void AddRing()
        {
            rings.Add(new Ring
            {
                Texture = ringEdgeTexture,
                GapAngle = MathHelper.WrapAngle((float)rand.NextDouble() * MathHelper.Pi * 2f),
                Radius = 1024f,
                Origin = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2),
                GapSize = MathHelper.Pi / 6,
            });
        }

        public void Clear()
        {
            rings.Clear();
        }

        public IEnumerable<Ring> AllRings
        {
            get { return rings; }
        }

        public void Remove(Ring ring)
        {
            rings.Remove(ring);
        }
    }
}
