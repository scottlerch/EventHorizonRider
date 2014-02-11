using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class Ring
    {
        public List<RingGap> Gaps;

        public float Radius;
        public Texture2D Texture;
        public Vector2 Origin;

        public void Draw(SpriteBatch spriteBatch)
        {
            for (float i = -MathHelper.Pi; i < MathHelper.Pi; i += 0.02f)
            {
                if (Gaps.Any(gap => gap.IsInsideGap(i)))
                    continue;

                spriteBatch.Draw(Texture,
                    position: new Vector2(
                        Origin.X + ((float)Math.Sin(i) * Radius),
                        Origin.Y - ((float)Math.Cos(i) * Radius)),
                    origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                    rotation: i);
            }
        }

        internal bool Intersects(Ship ship)
        {
            return !Gaps.Any(gap => IsInsideGap(ship, gap));
        }

        private bool IsInsideGap(Ship ship, RingGap ringGap)
        {
            var halfGap = ringGap.GapSize / 2f;
            var ringWidth = Texture.Width;

            var startRingEdge = Radius - (ringWidth / 2f);
            // For don't collide while inside ring for the most part, this is better for gameplay
            var endRingEdge = startRingEdge + 5; // Radius + (ringWidth / 2f);

            var shipFrontEdge = (Origin - ship.Position).Length() + (ship.Texture.Height / 2) + 5; // TODO: figure out why this fudge factor is needed

            return !(
                !(
                    (ship.Rotation < ringGap.GapAngle + halfGap) &&
                    (ship.Rotation > ringGap.GapAngle - halfGap)
                ) &&
                (shipFrontEdge > startRingEdge && shipFrontEdge < endRingEdge));
        }
    }
}
