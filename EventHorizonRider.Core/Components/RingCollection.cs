using System;
using System.Collections.Generic;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class RingCollection : ComponentBase
    {
        private readonly Blackhole blackhole;
        private readonly RingFactory ringFactory = new RingFactory();
        private readonly List<Ring> rings = new List<Ring>();
        private IEnumerator<RingInfo> currentSequence;

        private DateTime lastRingAdd = DateTime.UtcNow;

        private Level level;

        private SoundEffect newLevelSound;

        private bool stopped = true;

        public RingCollection(Blackhole blackhole)
        {
            this.blackhole = blackhole;
        }

        public bool HasMoreRings { get; private set; }

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            ringFactory.LoadContent(graphics);

            newLevelSound = content.Load<SoundEffect>("newlevel_sound");
        }

        public void SetLevel(Level newLevel)
        {
            level = newLevel;

            currentSequence = level.Sequence.GetEnumerator();
            HasMoreRings = true;

            newLevelSound.Play();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var ring in rings)
            {
                ring.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            if (stopped)
            {
                return;
            }

            foreach (var ring in rings.ToList())
            {
                ring.Update(gameTime, inputState);

                ring.Radius -= (float) gameTime.ElapsedGameTime.TotalSeconds*level.RingSpeed;

                if (!ring.ConsumedByBlackhole && ring.Radius <= blackhole.Height*0.5f)
                {
                    blackhole.Pulse();
                    ring.ConsumedByBlackhole = true;
                }

                if (ring.Radius <= 0f)
                {
                    rings.Remove(ring);
                }
            }

            if (DateTime.UtcNow - lastRingAdd > level.RingInterval)
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

        public bool Intersects(Ship ship)
        {
            return rings.Any(ring => ring.Intersects(ship));
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
    }
}