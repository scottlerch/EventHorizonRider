using EventHorizonRider.Core.Components.SpaceComponents.Rings;
using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class RingCollection : ComponentBase
{
    private readonly Blackhole blackhole;
    private readonly Shockwave shockwave;

    private IEnumerator<RingInfo> currentSequence;

    private TimeSpan? lastRingAddTime;
    private TimeSpan lastRingDuration;
    private TimeSpan totalElapsedGameTime;

    private Level level;

    private bool stopped;

    public RingCollection(Blackhole blackhole, Shockwave shockwave, RingFactory ringFactory)
    {
        stopped = true;

        this.blackhole = blackhole;
        this.shockwave = shockwave;

        RingFactory = ringFactory;
    }

    public Color Color { get; set; }

    public RingFactory RingFactory { get; private set; }

    public bool HasMoreRings { get; private set; }

    public bool CollisionDetectionDisabled { get; set; }

    public float ShadowDepth { get; set; }

    public void SetLevel(Level newLevel)
    {
        level = newLevel;

        currentSequence = level.Sequence.GetEnumerator();
        HasMoreRings = true;
    }

    public bool Intersects(Ship ship)
    {
        if (CollisionDetectionDisabled) return false;

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

    public void Gameover()
    {
        stopped = true;

        ForEach<Ring>(ring => ring.Stop());
    }

    public void Clear()
    {
        ClearChildren();
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        RingFactory.LoadContent(content, graphics);
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (stopped) return;

        RemoveConsumedRings();

        // Track relative total elapsed game time since if we use gameTime.TotalGameTime it won't work when paused
        totalElapsedGameTime += gameTime.ElapsedGameTime;
        lastRingAddTime = lastRingAddTime ?? totalElapsedGameTime;

        if ((totalElapsedGameTime - lastRingAddTime) >= (level.RingInterval + lastRingDuration))
        {
            var ringInfo = currentSequence.Next();

            if (ringInfo == null)
            {
                HasMoreRings = false;
            }
            else
            {
                var ring = RingFactory.Create(ringInfo, level);

                AddChild(ring, Depth);

                lastRingAddTime = totalElapsedGameTime;
                lastRingDuration = TimeSpan.FromSeconds(Math.Abs(ringInfo.SpiralRadius) / level.RingSpeed);
            }
        }

        foreach (var ring in Children)
        {
            (ring as Ring).Color = Color;
        }
    }

    private void RemoveConsumedRings()
    {
        ForEachReverse<Ring>(ring =>
        {
            if (ring.OutterRadius <= (blackhole.Height * 0.3f))
            {
                if (!ring.ConsumedByBlackhole)
                {
                    // TODO: pulse needs to handle spirals better, maybe slowly grow as spiral consumed?
                    blackhole.Pulse(1.2f, level.RingSpeed / 200f);
                    ring.ConsumedByBlackhole = true;

                    if (ChildrenCount == 1)
                    {
                        shockwave.Execute();
                    }
                }
            }
            
            if (ring.OutterRadius <= (blackhole.Height * 0.01f))
            {
                RemoveChild(ring);
            }
        });
    }
}