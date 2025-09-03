using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class Background(StarFactory newStarFactory) : ComponentBase
{
    // Number of stars if using dynamically generated stars.
    private const int NumberOfStars = 500;

    private readonly Color _gameOverColor = Color.Red.AdjustLight(0.6f);
    private const float GameOverAlpha = 0.5f;

    private readonly Color _defaultColor = Color.Black;
    private const float DefaultAlpha = 1f;

    private Texture2D _radialGradient;
    private Texture2D _background;
    private Texture2D _starsBackground;

    private Vector2 _starsBackgroundOrigin;
    private Vector2 _radialGradientOrigin;
    private Vector2 _backgroundOrigin;

    private Color _backgroundColor = Color.Black;
    private float _backgroundAlpha = 1f;

    private float _currentRotation;
    private float _currentBackgroundRotation;

    private readonly StarFactory _starFactory = newStarFactory;
    private Star[] _stars;

    private Vector2 _starsBackgroundScale;
    private Vector2 _backgroundScale;
    private Vector2 _radialGradientScale;

    private bool UseStaticStars { get; set; }

    public float Scale { get; set; }

    public float RotationalVelocity { get; set; }

    public Color BackgroundColor
    {
        get => _backgroundColor; set => _backgroundColor = value;
    }

    public Color StarBackgroundColor { get; set; }

    public void Start()
    {
        _backgroundColor = _defaultColor;
        _backgroundAlpha = DefaultAlpha;
    }

    public void Gameover()
    {
        _backgroundColor = _gameOverColor;
        _backgroundAlpha = GameOverAlpha;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        StarBackgroundColor = Color.White;

        _background = content.Load<Texture2D>(@"Images\background");
        _starsBackground = content.Load<Texture2D>(@"Images\stars");
        _radialGradient = content.Load<Texture2D>(@"Images\radial_gradient");

        _starFactory.LoadContent(content);
        _stars = _starFactory.GetStars(NumberOfStars);

        UseStaticStars = !DeviceInfo.Platform.UseDynamicStars;
        RotationalVelocity = Level.DefaultRotationalVelocity;

        // HACK: This fudge factor helps hide intermitent red border caused by some issue in gaussian blur code
        const float fudgeFactor = 1.1f;
        _radialGradientScale = new Vector2(
            (float)DeviceInfo.LogicalWidth / _radialGradient.Width,
            (float)DeviceInfo.LogicalWidth / _radialGradient.Width) * fudgeFactor;

        _starsBackgroundOrigin = new Vector2(_starsBackground.Width / 2f, _starsBackground.Height / 2f);
        _radialGradientOrigin = new Vector2(_radialGradient.Width / 2f, _radialGradient.Height / 2f);
        _backgroundOrigin = new Vector2(_background.Width / 2f, _background.Height / 2f);

        _backgroundScale = new Vector2(2f, 2f);
        _starsBackgroundScale = new Vector2(DeviceInfo.LogicalCenter.Length() / _starsBackgroundOrigin.X);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (_backgroundColor != _gameOverColor)
        {
            spriteBatch.Draw(
                _background,
                position: DeviceInfo.LogicalCenter,
                sourceRectangle: null,
                origin: _backgroundOrigin,
                rotation: _currentBackgroundRotation,
                layerDepth: Depth,
                color: StarBackgroundColor * _backgroundAlpha,
                scale: _backgroundScale * Scale,
                effects: SpriteEffects.None);
        }
        else
        {
            spriteBatch.Draw(
                _radialGradient,
                position: DeviceInfo.LogicalCenter,
                sourceRectangle: null,
                origin: _radialGradientOrigin,
                rotation: 0f,
                layerDepth: Depth,
                color: Color.White * 0.9f,
                scale: _radialGradientScale * Scale,
                effects: SpriteEffects.None);
        }

        if (UseStaticStars)
        {
            spriteBatch.Draw(
                _starsBackground,
                position: DeviceInfo.LogicalCenter,
                origin: _starsBackgroundOrigin,
                rotation: _currentRotation,
                color: Color.White * _backgroundAlpha,
                scale: _starsBackgroundScale * Scale,
                layerDepth: Depth + 0.001f,
                sourceRectangle: null,
                effects: SpriteEffects.None);
        }
        else
        {
            const float depthOffset = 0.0001f;

            for (var i = 0; i < _stars.Length; i++)
            {
                var star = _stars[i];
                spriteBatch.Draw(
                    star.Texture,
                    position: star.Position,
                    sourceRectangle: null,
                    origin: star.Origin,
                    layerDepth: Depth + (depthOffset * i),
                    color: star.Color * star.Transparency,
                    scale: new Vector2(star.Scale * Scale),
                    rotation: star.Angle * 20f,
                    effects: SpriteEffects.None);
            }
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (_backgroundColor == _gameOverColor)
        {
            return;
        }

        var changeInRotation = (RotationalVelocity / 4f) *
                               (float)gameTime.ElapsedGameTime.TotalSeconds;

        _currentRotation += changeInRotation;
        _currentBackgroundRotation += changeInRotation / 2f;

        if (!UseStaticStars)
        {
            if (MathUtilities.GetRandomBetween(0, 5) == 1)
            {
                _stars[MathUtilities.GetRandomBetween(0, _stars.Length - 1)].Twinkle();
            }

            var timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var star in _stars)
            {
                star.Angle += changeInRotation + (star.RotationSpeed * timeElapsed);
                star.Update(gameTime, Scale);
            }
        }
    }
}
