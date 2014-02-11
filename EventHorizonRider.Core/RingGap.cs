using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class RingGap
    {
        public float GapAngle;
        public float GapSize;
        public float GapStart { get { return MathHelper.WrapAngle(GapAngle - (GapSize / 2)); } }
        public float GapEnd { get { return MathHelper.WrapAngle(GapAngle + (GapSize / 2)); } }

        public bool IsInsideGap(float angle)
        {
            if (GapStart < GapEnd && (angle > GapStart && angle < GapEnd))
                return true;

            if (GapStart > GapEnd && (angle > GapStart || angle < GapEnd))
                return true;

            return false;
        }
    }
}
