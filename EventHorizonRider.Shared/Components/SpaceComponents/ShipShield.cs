using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class ShipShield : ComponentBase
{
    private const float BaseShieldAlpha = 0.8f;

    private readonly Motion _shieldPulseMotion;

    private Texture2D _shieldPusleTexture;
    private Vector2 _shieldPulseLocation;
    private Vector2 _shieldPulseOrigin;
    private float _shieldPulseAlpha;
    private float _shieldPulseScale;

    private int _shieldTextureIndex;
    private Texture2D[] _shieldTextures;
    private Vector2[] _shieldTexturesOrigins;

    private Ship _ship;

    public ShipShield()
    {
        _shieldPulseMotion = new Motion();
    }

    public Color Color { get; set; }

    public void Pulse()
    {
        _shieldPulseLocation = _ship.Position;
        _shieldPulseMotion.Initialize(0, 1, 1f);
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        const int NumberOfShieldPulseTextures = 3;

        _ship = Parent as Ship;

        _shieldPusleTexture = content.Load<Texture2D>(@"Images\shield_pulse");

        _shieldTextures = new Texture2D[NumberOfShieldPulseTextures];
        _shieldTexturesOrigins = new Vector2[NumberOfShieldPulseTextures];

        for (var i = 0; i < _shieldTextures.Length; i++)
        {
            _shieldTextures[i] = content.Load<Texture2D>(@"Images\shield_" + (i + 1));
            _shieldTexturesOrigins[i] = new Vector2(_shieldTextures[i].Width / 2f, _shieldTextures[i].Height / 2f);
        }

        _shieldPulseMotion.Initialize(0, 0, 0);

        _shieldTextureIndex = 0;

        _shieldPulseOrigin = new Vector2(_shieldPusleTexture.Width / 2f, _shieldPusleTexture.Height / 2f);
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        const int frameInterval = 100;
        _shieldTextureIndex = (int)((((int)gameTime.TotalGameTime.TotalMilliseconds % frameInterval) /
            (float)frameInterval) * _shieldTextures.Length);

        _shieldPulseMotion.Update(gameTime);

        _shieldPulseAlpha = MathHelper.Lerp(BaseShieldAlpha, 0, _shieldPulseMotion.Value);
        _shieldPulseScale = (10f * _shieldPulseMotion.Value) + 1f;
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (_shieldPulseMotion.Value > 0f)
        {
            spriteBatch.Draw(
                _shieldPusleTexture,
                _shieldPulseLocation,
                sourceRectangle: null,
                origin: _shieldPulseOrigin,
                color: Color.Lerp(Color, Color.White, 0.3f) * _shieldPulseAlpha,
                scale: Vector2.One * _shieldPulseScale,
                rotation: _ship.Rotation,
                layerDepth: Depth - 0.0003f,
                effects: SpriteEffects.None);
        }

        spriteBatch.Draw(
            _shieldTextures[_shieldTextureIndex],
            _ship.Position,
            sourceRectangle: null,
            origin: _shieldTexturesOrigins[_shieldTextureIndex],
            color: Color.Lerp(Color, Color.White, 0.3f) * BaseShieldAlpha,
            scale: Vector2.One,
            rotation: _ship.Rotation,
            layerDepth: Depth - 0.0002f,
            effects: SpriteEffects.None);
    }
}
