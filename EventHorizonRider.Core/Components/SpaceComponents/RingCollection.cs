using EventHorizonRider.Core.Components.CenterComponents;
using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class RingCollection : ComponentBase
    {
        private readonly Blackhole blackhole;
        private readonly RingFactory ringFactory = new RingFactory();
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

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
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

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (stopped)
            {
                return;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var ring = (Ring)Children[i];

                ring.Radius -= (float) gameTime.ElapsedGameTime.TotalSeconds*level.RingSpeed;

                if (!ring.ConsumedByBlackhole && ring.Radius <= blackhole.Height*0.5f)
                {
                    blackhole.Pulse();
                    ring.ConsumedByBlackhole = true;
                }

                if (ring.Radius <= 0f)
                {
                    RemoveChild(ring);
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
                    AddChild(ringFactory.Create(ringInfo));
                }
            }
        }

        public bool Intersects(Ship ship)
        {
            return Children.Cast<Ring>().Any(ring => ring.Intersects(ship));
        }

        public void Start()
        {
            ClearChildren();
            stopped = false;
        }

        public void Remove(Ring ring)
        {
            RemoveChild(ring);
        }

        internal void Stop()
        {
            stopped = true;

            foreach (var ring in Children.Cast<Ring>())
            {
                ring.Stop();
            }
        }
    }
}