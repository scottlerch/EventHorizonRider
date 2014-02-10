using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    class Ring
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

            for (float i = -MathHelper.Pi; i < MathHelper.Pi; i += 0.01f)
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
    }
}
