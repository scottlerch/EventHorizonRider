using EventHorizonRider.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components
{
    internal class RingCollection
    {
        private List<Ring> rings = new List<Ring>();

        private GraphicsDevice graphicsDevice;
        private DateTime lastRingAdd = DateTime.UtcNow;

        private RingFactory ringFactory = new RingFactory();
        private Level level;

        private IEnumerator<RingInfo> currentSequence;

        private SoundEffect newLevelSound;

        private bool stopped = true;

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ringFactory.LoadContent(graphicsDevice);

            newLevelSound = content.Load<SoundEffect>("newlevel_sound");
        }

        public void SetLevel(Level level)
        {
            this.level = level;

            currentSequence = level.Sequence.GetEnumerator();
            HasMoreRings = true;

            newLevelSound.Play();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var ring in rings)
            {
                ring.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime, Blackhole blackhole)
        {
            if (stopped)
            {
                return;
            }

            foreach (var ring in rings.ToList())
            {
                ring.Update(gameTime);

                ring.Radius -= (float)gameTime.ElapsedGameTime.TotalSeconds * this.level.RingSpeed;

                if (!ring.ConsumedByBlackhole && ring.Radius <= blackhole.Height * 0.5f)
                {
                    blackhole.Pulse();
                    ring.ConsumedByBlackhole = true;
                }

                if (ring.Radius <= 0f)
                {
                    rings.Remove(ring);
                }
            }

            if (DateTime.UtcNow - lastRingAdd > this.level.RingInterval)
            {
                lastRingAdd = DateTime.UtcNow;

                var ringInfo = currentSequence.Next();

                if (ringInfo == null)
                {
                    HasMoreRings = false;
                }
                else
                {
                    rings.Add(ringFactory.Create(ringInfo));
                }
            }
        }

        public bool HasMoreRings { get; private set; }

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

        public void Start()
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
