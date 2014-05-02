using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Background : ComponentBase
    {
        private const int NumberOfStars = 500;

        private Texture2D background;
        private Texture2D starsBackground;

        private Color backgroundColor = Color.Black;
        private Vector2 center;
        private float currentRotation;
        private readonly StarFactory starFactory;
        private Star[] stars;

        public Background(StarFactory newStarFactory)
        {
            starFactory = newStarFactory;
        }

        public float Scale { get; set; }

        private bool UseStaticStars { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            Scale = 1f;

            background = content.Load<Texture2D>(@"Images\background");
            starsBackground = content.Load<Texture2D>(@"Images\stars");

            center = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);

            starFactory.LoadContent(content, graphics);
            stars = starFactory.GetStars(NumberOfStars);

            UseStaticStars = DeviceInfo.DetailLevel.HasFlag(DetailLevel.StaticStars);
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


                if (UseStaticStars)
                {
                    spriteBatch.Draw(starsBackground,
                        position: center,
                        origin: new Vector2(starsBackground.Width/2f, starsBackground.Height/2f),
                        rotation: currentRotation,
                        color: Color.White*0.8f,
                        scale: new Vector2(1.3f, 1.3f)*Scale,
                        depth: Depth + 0.001f);
                }
                else
                {
                    const float depthOffset = 0.0001f;

                    for (var i = 0; i < stars.Length; i++)
                    {
                        var star = stars[i];
                        spriteBatch.Draw(star.Texture,
                            position: star.Position,
                            origin: star.Origin,
                            depth: Depth + (depthOffset * i),
                            color: star.Color * star.Transparency,
                            scale: new Vector2(star.Scale * Scale),
                            rotation: star.Angle * 20f);
                    }
                }
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            var changeInRotation = (RingInfoFactory.DefaultRotationVelocity/4f)*
                                   (float) gameTime.ElapsedGameTime.TotalSeconds;

            currentRotation += changeInRotation;

            if (!UseStaticStars)
            {
                if (MathUtilities.GetRandomBetween(0, 5) == 1)
                {
                    stars[MathUtilities.GetRandomBetween(0, stars.Length - 1)].Twinkle();
                }

                var timeElapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

                foreach (var star in stars)
                {
                    star.Angle += changeInRotation + (star.RotationSpeed*timeElapsed);
                    star.Update(gameTime, Scale);
                }
            }
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
