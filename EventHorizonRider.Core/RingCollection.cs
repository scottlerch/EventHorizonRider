using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    class RingCollection
    {
        Texture2D ringEdgeTexture;
        List<Ring> rings = new List<Ring>();
        Random rand = new Random();
        GraphicsDevice graphicsDevice;
        DateTime lastRingAdd = DateTime.UtcNow;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ringEdgeTexture = new Texture2D(graphicsDevice, 8, 25, false, SurfaceFormat.Color);
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

        public IEnumerable<Ring> GetRings()
        {
            return rings.ToList();
        }

        public void Remove(Ring ring)
        {
            rings.Remove(ring);
        }
    }
}
