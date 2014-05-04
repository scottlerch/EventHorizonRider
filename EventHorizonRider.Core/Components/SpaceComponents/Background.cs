using System;
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

        private readonly Color gameOverColor = Color.Red.AdjustLight(0.8f);
        private readonly float gameOverAlpha = 0.5f;
        private readonly Color defaultColor = Color.Black;
        private readonly float defaultAlpha = 0.8f;

        private Texture2D background;
        private Texture2D starsBackground;

        private Color backgroundColor = Color.Black;
        private float backgroundAlpha = 1f;
        private Vector2 center;
        private float currentRotation;
        private readonly StarFactory starFactory;
        private Star[] stars;

        public Background(StarFactory newStarFactory)
        {
            starFactory = newStarFactory;
        }

        private bool UseStaticStars { get; set; }

        public float Scale { get; set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            StarBackgroundColor = Color.White;

            background = content.Load<Texture2D>(@"Images\background");
            starsBackground = content.Load<Texture2D>(@"Images\stars");

            center = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);

            starFactory.LoadContent(content, graphics);
            stars = starFactory.GetStars(NumberOfStars);

            UseStaticStars = DeviceInfo.DetailLevel.HasFlag(DetailLevel.StaticStars);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            //if (backgroundColor != gameOverColor)
            //{
                spriteBatch.Draw(background,
                    position: center,
                    origin: new Vector2(background.Width / 2f, background.Height / 2f),
                    depth:Depth,
                    color: StarBackgroundColor * backgroundAlpha,
                    scale: (new Vector2(2f, 2f) * Scale));


                if (UseStaticStars)
                {
                    spriteBatch.Draw(starsBackground,
                        position: center,
                        origin: new Vector2(starsBackground.Width/2f, starsBackground.Height/2f),
                        rotation: currentRotation,
                        color: Color.White * backgroundAlpha,
                        scale: new Vector2(1.3f, 1.3f) * Scale,
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
            //}
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

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        public Color StarBackgroundColor { get; set; }

        public void Start()
        {
            backgroundColor = defaultColor;
            backgroundAlpha = defaultAlpha;
        }

        public void Gameover()
        {
            backgroundColor = gameOverColor;
            backgroundAlpha = gameOverAlpha;
        }
    }
}
