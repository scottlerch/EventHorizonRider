using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class RingCollection
    {
        private List<Ring> rings = new List<Ring>();

        private GraphicsDevice graphicsDevice;
        private DateTime lastRingAdd = DateTime.UtcNow;

        private RingFactory ringFactory = new RingFactory();
        private Level level;

        private IEnumerator<RingInfo> currentSequence;

        private bool stopped = false;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ringFactory.LoadContent(graphicsDevice);
        }

        public void SetLevel(Level level)
        {
            this.level = level;

            currentSequence = level.Sequence.GetEnumerator();
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
            if (stopped)
            {
                return;
            }

            foreach (var ring in rings.ToList())
            {
                ring.Radius -= (float)gameTime.ElapsedGameTime.TotalSeconds * this.level.RingSpeed;

                if (ring.Radius <= 0)
                {
                    rings.Remove(ring);
                }
            }

            if (DateTime.UtcNow - lastRingAdd > this.level.RingInterval)
            {
                lastRingAdd = DateTime.UtcNow;
                rings.Add(ringFactory.Create(currentSequence.Next()));
            }
        }

        public bool Intersects(Ship ship)
        {
            foreach (var ring in rings)
            {
                if (ring.Intersects(ship))
                {
                    return true;
                }
            }

            return false;
        }

        public void Initialize()
        {
            rings.Clear();
            stopped = false;
        }

        public void Remove(Ring ring)
        {
            rings.Remove(ring);
        }

        internal void Stop()
        {
            stopped = true;
        }

        internal void ClampToNearestGapEdge(Ship ship)
        {
            foreach (var ring in rings)
            {
                ring.ClampToNearestGapEdge(ship);
            }
        }
    }
}
