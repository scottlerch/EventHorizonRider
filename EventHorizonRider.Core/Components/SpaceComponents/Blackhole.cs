using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Blackhole : ComponentBase
    {
        private float extraBlackholeScale;
        private float newExtraBlackholeScale;
        private float extraBlackholeScaleSpeed;

        private Texture2D texture;

        private bool isStopped = true;

        private float currentRotation;

        private SoundEffect scaleSound;

        public Blackhole()
        {
            Spring = new Spring
            {
                Friction = -0.9f,
                Stiffness = -100f,
                BlockMass = 0.1f,
            };
        }

        public Spring Spring { get; private set; }

        public Vector2 Position { get; private set; }

        public float Height
        {
            get { return texture.Height*Spring.BlockX; }
        }

        public float RotationalVelocity { get; set; }

        public float ExtraScale { get { return extraBlackholeScale; } }

        public Vector2 Scale
        {
            get
            {
                return new Vector2(Spring.BlockX + extraBlackholeScale, Spring.BlockX + extraBlackholeScale);
            }
        }

        public void Gameover()
        {
            isStopped = true;
        }

        public void Start()
        {
            isStopped = false;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\blackhole");
            scaleSound = content.Load<SoundEffect>(@"Sounds\open_menu");

            Position = new Vector2(
                DeviceInfo.LogicalWidth/2f,
                DeviceInfo.LogicalHeight/2f);
        }

        public void Pulse(float pullX = 1.15f, float pullVelocity = 1.5f)
        {
            Spring.PullBlock(pullX, pullVelocity);
        }

        public void SetExtraScale(float scaleSize, bool animate = false, float speed = 1f)
        {
            newExtraBlackholeScale = scaleSize;

            if (!animate)
            {
                extraBlackholeScale = scaleSize;
            }
            else
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (scaleSize != extraBlackholeScale)
                {
                    scaleSound.Play();
                }

                extraBlackholeScaleSpeed = speed;
                extraBlackholeScaleSpeed *= extraBlackholeScale < newExtraBlackholeScale ? 1f : -1f;
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (extraBlackholeScale != newExtraBlackholeScale)
            {
                extraBlackholeScale += (float)gameTime.ElapsedGameTime.TotalSeconds * extraBlackholeScaleSpeed;

                if ((extraBlackholeScaleSpeed > 0 && extraBlackholeScale > newExtraBlackholeScale) ||
                    (extraBlackholeScaleSpeed < 0 && extraBlackholeScale < newExtraBlackholeScale))
                {
                    extraBlackholeScale = newExtraBlackholeScale;
                }
            }

            if (!isStopped)
            {
                Spring.Update(gameTime.ElapsedGameTime);
                currentRotation += RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position,
                origin: new Vector2(texture.Width/2f, texture.Height/2f),
                rotation: currentRotation,
                scale: new Vector2(Scale.X, Scale.Y),
                depth: Depth);
        }
    }
}