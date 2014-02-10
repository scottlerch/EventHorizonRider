using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core
{
    internal class Ring
    {
        public float GapAngle;
        public float Radius;
        public Texture2D Texture;
        public Vector2 Origin;
        public float GapSize;

        public void Draw(SpriteBatch spriteBatch)
        {
            var gapStart = MathHelper.WrapAngle(GapAngle - (GapSize / 2));
            var gapEnd = MathHelper.WrapAngle(GapAngle + (GapSize / 2));

            for (float i = -MathHelper.Pi; i < MathHelper.Pi; i += 0.02f)
            {
                if (gapStart < gapEnd && (i > gapStart && i < gapEnd))
                    continue;

                if (gapStart > gapEnd && (i > gapStart || i < gapEnd))
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
            var halfGap = GapSize / 2f;
            var ringWidth = Texture.Width;

            var startRingEdge = Radius - (ringWidth / 2f);
            // For don't collide while inside ring for the most part, this is better for gameplay
            var endRingEdge = startRingEdge + 5; // Radius + (ringWidth / 2f);

            var shipFrontEdge = (Origin - ship.Position).Length() + (ship.Texture.Height / 2) + 5; // TODO: figure out why this fudge factor is needed

            return
                !(
                    (ship.Rotation < GapAngle + halfGap) &&
                    (ship.Rotation > GapAngle - halfGap)
                ) &&
                (shipFrontEdge > startRingEdge && shipFrontEdge < endRingEdge);
        }
    }
}
