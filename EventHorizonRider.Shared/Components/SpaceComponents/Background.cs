﻿using EventHorizonRider.Core.Engine;
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
        // Number of stars if using dynamically generated stars.
        private const int NumberOfStars = 500;

        private readonly Color gameOverColor = Color.Red.AdjustLight(0.6f);
        private const float gameOverAlpha = 0.5f;

        private readonly Color defaultColor = Color.Black;
        private const float defaultAlpha = 1f;

        private Texture2D radialGradient;
        private Texture2D background;
        private Texture2D starsBackground;

        private Vector2 starsBackgroundOrigin;
        private Vector2 radialGradientOrigin;
        private Vector2 backgroundOrigin;

        private Color backgroundColor = Color.Black;
        private float backgroundAlpha = 1f;

        private float currentRotation;
        private float currentBackgroundRotation;

        private readonly StarFactory starFactory;
        private Star[] stars;

        private Vector2 starsBackgroundScale;
        private Vector2 backgroundScale;
        private Vector2 radialGradientScale;

        public Background(StarFactory newStarFactory)
        {
            starFactory = newStarFactory;
        }

        private bool UseStaticStars { get; set; }

        public float Scale { get; set; }

        public float RotationalVelocity { get; set; }

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

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            StarBackgroundColor = Color.White;

            background = content.Load<Texture2D>(@"Images\background");
            starsBackground = content.Load<Texture2D>(@"Images\stars");
            radialGradient = content.Load<Texture2D>(@"Images\radial_gradient");

            starFactory.LoadContent(content, graphics);
            stars = starFactory.GetStars(NumberOfStars);

            UseStaticStars = !DeviceInfo.Platform.UseDynamicStars;
            RotationalVelocity = Level.DefaultRotationalVelocity;

            // HACK: This fudge factor helps hide intermitent red border caused by some issue in gaussian blur code
            const float fudgeFactor = 1.1f;
            radialGradientScale = new Vector2(
                (float)DeviceInfo.LogicalWidth / radialGradient.Width,
                (float)DeviceInfo.LogicalWidth / radialGradient.Width) * fudgeFactor;

            starsBackgroundOrigin = new Vector2(starsBackground.Width/2f, starsBackground.Height/2f);
            radialGradientOrigin = new Vector2(radialGradient.Width/2f, radialGradient.Height/2f);
            backgroundOrigin = new Vector2(background.Width/2f, background.Height/2f);

            backgroundScale = new Vector2(2f, 2f);
            starsBackgroundScale = new Vector2(DeviceInfo.LogicalCenter.Length() / starsBackgroundOrigin.X);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (backgroundColor != gameOverColor)
            {
                spriteBatch.Draw(
                    background,
                    position: DeviceInfo.LogicalCenter,
                    origin: backgroundOrigin,
                    rotation: currentBackgroundRotation,
                    layerDepth: Depth,
                    color: StarBackgroundColor*backgroundAlpha,
                    scale: backgroundScale*Scale);
            }
            else
            {
                spriteBatch.Draw(
                    radialGradient,
                    position: DeviceInfo.LogicalCenter,
                    origin: radialGradientOrigin,
                    layerDepth: Depth,
                    color: Color.White * 0.9f,
                    scale: radialGradientScale * Scale);
            }

            if (UseStaticStars)
            {
                spriteBatch.Draw(
                    starsBackground,
                    position: DeviceInfo.LogicalCenter,
                    origin: starsBackgroundOrigin,
                    rotation: currentRotation,
                    color: Color.White * backgroundAlpha,
                    scale: starsBackgroundScale * Scale,
                    layerDepth: Depth + 0.001f);
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
                        layerDepth: Depth + (depthOffset * i),
                        color: star.Color * star.Transparency,
                        scale: new Vector2(star.Scale * Scale),
                        rotation: star.Angle * 20f);
                }
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (backgroundColor == gameOverColor) return;

            var changeInRotation = (RotationalVelocity / 4f) *
                                   (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentRotation += changeInRotation;
            currentBackgroundRotation += changeInRotation/2f;

            if (!UseStaticStars)
            {
                if (MathUtilities.GetRandomBetween(0, 5) == 1)
                {
                    stars[MathUtilities.GetRandomBetween(0, stars.Length - 1)].Twinkle();
                }

                var timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                foreach (var star in stars)
                {
                    star.Angle += changeInRotation + (star.RotationSpeed * timeElapsed);
                    star.Update(gameTime, Scale);
                }
            }
        }
    }
}
