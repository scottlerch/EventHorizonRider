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

internal class RingCollection(Blackhole blackhole, Shockwave shockwave, RingFactory ringFactory) : ComponentBase
{
    private readonly Blackhole _blackhole = blackhole;
    private readonly Shockwave _shockwave = shockwave;

    private IEnumerator<RingInfo> _currentSequence;

    private TimeSpan? _lastRingAddTime;
    private TimeSpan _lastRingDuration;
    private TimeSpan _totalElapsedGameTime;

    private Level _level;

    private bool _stopped = true;

    public Color Color { get; set; }

    public RingFactory RingFactory { get; private set; } = ringFactory;

    public bool HasMoreRings { get; private set; }

    public bool CollisionDetectionDisabled { get; set; }

    public float ShadowDepth { get; set; }

    public void SetLevel(Level newLevel)
    {
        _level = newLevel;

        _currentSequence = _level.Sequence.GetEnumerator();
        HasMoreRings = true;
    }

    public bool Intersects(Ship ship)
    {
        if (CollisionDetectionDisabled)
        {
            return false;
        }

        // TODO: optimize collision detection, this is the biggest bottleneck right now
        return Children.Cast<Ring>().Any(ring => ring.Intersects(ship));
    }

    public void Start()
    {
        ClearChildren();
        _stopped = false;
    }

    public void Remove(Ring ring) => RemoveChild(ring);

    public void Gameover()
    {
        _stopped = true;

        ForEach<Ring>(ring => ring.Stop());
    }

    public void Clear() => ClearChildren();

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics) => RingFactory.LoadContent(content);

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (_stopped)
        {
            return;
        }

        RemoveConsumedRings();

        // Track relative total elapsed game time since if we use gameTime.TotalGameTime it won't work when paused
        _totalElapsedGameTime += gameTime.ElapsedGameTime;
        _lastRingAddTime ??= _totalElapsedGameTime;

        if ((_totalElapsedGameTime - _lastRingAddTime) >= (_level.RingInterval + _lastRingDuration))
        {
            var ringInfo = _currentSequence.Next();

            if (ringInfo == null)
            {
                HasMoreRings = false;
            }
            else
            {
                var ring = RingFactory.Create(ringInfo, _level);

                AddChild(ring, Depth);

                _lastRingAddTime = _totalElapsedGameTime;
                _lastRingDuration = TimeSpan.FromSeconds(Math.Abs(ringInfo.SpiralRadius) / _level.RingSpeed);
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
            if (ring.OutterRadius <= (_blackhole.Height * 0.3f))
            {
                if (!ring.ConsumedByBlackhole)
                {
                    // TODO: pulse needs to handle spirals better, maybe slowly grow as spiral consumed?
                    _blackhole.Pulse(1.2f, _level.RingSpeed / 200f);
                    ring.ConsumedByBlackhole = true;

                    if (ChildrenCount == 1)
                    {
                        _shockwave.Execute();
                    }
                }
            }

            if (ring.OutterRadius <= (_blackhole.Height * 0.01f))
            {
                RemoveChild(ring);
            }
        });
    }
}
