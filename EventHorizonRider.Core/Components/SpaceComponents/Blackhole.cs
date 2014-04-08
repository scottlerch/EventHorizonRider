using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Blackhole : ComponentBase
    {
        private readonly Spring spring = new Spring
        {
            Friction = -0.9f,
            Stiffness = -100f,
            BlockMass = 0.1f,
        };

        public Vector2 Position { get; private set; }

        private Texture2D texture;

        private bool isStopped = true;

        private float currentRotation;

        public float Height
        {
            get { return texture.Height*spring.BlockX; }
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\blackhole");

            Position = new Vector2(
                graphics.Viewport.Width/2f,
                graphics.Viewport.Height/2f);
        }

        public void Pulse(float pullX = 1.15f, float pullVelocity = 1.5f)
        {
            spring.PullBlock(pullX, pullVelocity);
        }

        private float extraBlackholdScale;
        private float newExtraBlackholdScale;
        private float extraBlackholeScaleSpeed;

        public float ExtraScale { get { return extraBlackholdScale; } }

        public void SetExtraScale(float scaleSize, bool animate = false, float speed = 1f)
        {
            newExtraBlackholdScale = scaleSize;

            if (!animate)
            {
                extraBlackholdScale = scaleSize;
            }
            else
            {
                extraBlackholeScaleSpeed = speed;
                extraBlackholeScaleSpeed *= extraBlackholdScale < newExtraBlackholdScale ? 1f : -1f;
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (extraBlackholdScale != newExtraBlackholdScale)
            {
                extraBlackholdScale += (float)gameTime.ElapsedGameTime.TotalSeconds * extraBlackholeScaleSpeed;

                if ((extraBlackholeScaleSpeed > 0 && extraBlackholdScale > newExtraBlackholdScale) ||
                    (extraBlackholeScaleSpeed < 0 && extraBlackholdScale < newExtraBlackholdScale))
                {
                    extraBlackholdScale = newExtraBlackholdScale;
                }
            }

            if (!isStopped)
            {
                spring.Update(gameTime.ElapsedGameTime);
                currentRotation += RingInfoFactory.DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public Vector2 Scale { get { return new Vector2(spring.BlockX + extraBlackholdScale, spring.BlockX + extraBlackholdScale); } }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position,
                origin: new Vector2(texture.Width/2f, texture.Height/2f),
                rotation: currentRotation,
                scale: new Vector2(Scale.X, Scale.Y),
                depth: Depth);
        }

        internal void Stop()
        {
            isStopped = true;
        }

        internal void Start()
        {
            isStopped = false;
        }
    }
}