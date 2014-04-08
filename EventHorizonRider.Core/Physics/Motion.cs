using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    internal struct Motion
    {
        public float Target { get; private set; }

        public float Value { get; private set; }

        public float Speed { get; private set; }

        public float Acceleration { get; private set; }

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

            if (Target < Value && Speed > 0)
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

            if (Speed > 0)
            {
                Value += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
                Value = Value > Target ? Target : Value;
            }
            else
            {
                Value += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
                Value = Value < Target ? Target : Value;
            }
        }

        public void Set(float value)
        {
            Value = value;
            Target = value;
        }
    }
}
