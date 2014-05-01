﻿using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    internal struct Motion
    {
        public float Target { get; private set; }

        public float Value { get; private set; }

        public float Speed { get; private set; }

        public float Acceleration { get; private set; }

        public bool IsDone { get; private set; }

        public void Initialize(float value, float target, float speed, float acceleration = 0f)
        {
            Value = value;
            Acceleration = 0f;

            UpdateTarget(target, speed);
        }

        public void UpdateTarget(float target, float speed)
        {
            Speed = speed;
            
            UpdateTarget(target);
        }

        public void UpdateTarget(float target)
        {
            Target = target;

            if ((Target < Value && Speed > 0) || (Target > Value && Speed < 0))
            {
                Speed *= -1f;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Acceleration > 0)
            {
                Speed *= Acceleration*(float) gameTime.ElapsedGameTime.TotalSeconds;
            }

            Value += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;

            if (Speed > 0)
            {
                if (Value > Target)
                {
                    IsDone = true;
                    Value = Target;
                }
                else
                {
                    IsDone = false;
                }
            }
            else
            {
                if (Value < Target)
                {
                    IsDone = true;
                    Value = Target;
                }
                else
                {
                    IsDone = false;
                }
            }
        }

        public void Set(float value)
        {
            Value = value;
            Target = value;
        }
    }
}
