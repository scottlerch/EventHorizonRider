﻿using Microsoft.Xna.Framework;
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

        private bool stopped = false;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ringFactory.LoadContent(graphicsDevice);
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
            rings.Add(ringFactory.Create(gaps: 1, gapSize: MathHelper.Pi / 3));
        }

        public void Initialize()
        {
            rings.Clear();
            stopped = false;
        }

        public IEnumerable<Ring> AllRings
        {
            get { return rings; }
        }

        public void Remove(Ring ring)
        {
            rings.Remove(ring);
        }

        internal void Stop()
        {
            stopped = true;
        }
    }
}
