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
        private readonly Shockwave shockwave;
        private readonly RingFactory ringFactory = new RingFactory();
        private IEnumerator<RingInfo> currentSequence;

        private DateTime lastRingAdd = DateTime.UtcNow;

        private Level level;

        private SoundEffect newLevelSound;

        private bool stopped = true;

        public RingCollection(Blackhole blackhole, Shockwave shockwave)
        {
            this.blackhole = blackhole;
            this.shockwave = shockwave;
        }

        public bool HasMoreRings { get; private set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            ringFactory.LoadContent(content, graphics);

            newLevelSound = content.Load<SoundEffect>(@"Sounds\newlevel_sound");
        }

        public void SetLevel(Level newLevel)
        {
            level = newLevel;

            currentSequence = level.Sequence.GetEnumerator();
            HasMoreRings = true;
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
                    blackhole.Pulse(1.3f, level.RingSpeed / 200f);
                    ring.ConsumedByBlackhole = true;

                    if (Children.Count == 1)
                    {
                        shockwave.Execute();
                        newLevelSound.Play();
                    }
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
                    AddChild(ringFactory.Create(ringInfo), Depth);
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