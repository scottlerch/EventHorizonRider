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

        private TimeSpan? lastRingAdd;
        private TimeSpan lastRingDuration = TimeSpan.Zero;

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

            ForEachReverse<Ring>(ring =>
            {
                if (ring.OutterRadius <= (blackhole.Height * 0.2f))
                {
                    if (!ring.ConsumedByBlackhole)
                    {
                        // TODO: pulse needs to handle spirals better, maybe slowly grow as spiral consumed?
                        blackhole.Pulse(1.3f, level.RingSpeed / 200f);
                        ring.ConsumedByBlackhole = true;

                        if (ChildrenCount == 1)
                        {
                            shockwave.Execute();
                            newLevelSound.Play();
                        }
                    }

                    RemoveChild(ring);
                }
            });

            lastRingAdd = lastRingAdd ?? gameTime.TotalGameTime;

            if (gameTime.TotalGameTime - lastRingAdd > (level.RingInterval + lastRingDuration))
            {
                var ringInfo = currentSequence.Next();

                if (ringInfo == null)
                {
                    HasMoreRings = false;
                }
                else
                {
                    AddChild(ringFactory.Create(ringInfo, level), Depth);

                    lastRingAdd = gameTime.TotalGameTime;
                    lastRingDuration = TimeSpan.FromSeconds(ringInfo.SpiralRadius / level.RingSpeed);
                }
            }
        }

        public bool Intersects(Ship ship)
        {
            // TODO: optimize collision detection, this is the biggest bottleneck right now
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

            ForEach<Ring>(ring => ring.Stop());
        }

        internal void Clear()
        {
            ClearChildren();
        }
    }
}