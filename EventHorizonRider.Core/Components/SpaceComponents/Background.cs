using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Background : ComponentBase
    {
        private Texture2D background;
        private Texture2D stars;

        private Color backgroundColor = Color.Black;
        private Vector2 center;
        private float currentRotation;

        public float Scale { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            Scale = 1f;

            background = content.Load<Texture2D>(@"Images\background");
            stars = content.Load<Texture2D>(@"Images\stars");

            center = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (backgroundColor != Color.Red)
            {
                spriteBatch.Draw(background,
                    position: center,
                    origin: new Vector2(background.Width / 2f, background.Height / 2f),
                    depth:Depth,
                    color:Color.White * 0.5f,
                    scale:(new Vector2(2f, 2f) * Scale));

                spriteBatch.Draw(stars,
                    position: center,
                    origin: new Vector2(stars.Width / 2f, stars.Height / 2f),
                    rotation: currentRotation,
                    color: Color.White * 0.8f,
                    scale: new Vector2(1.3f, 1.3f) * Scale,
                    depth: Depth + 0.001f);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            currentRotation += (RingInfoFactory.DefaultRotationVelocity / 4f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public Color BackgroundColor { get { return backgroundColor; } }

        public void Start()
        {
            backgroundColor = Color.Black;
        }

        public void Gameover()
        {
            backgroundColor = Color.Red.AdjustLight(0.8f);
        }
    }
}
