using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class BlackholeHalo(Blackhole newBlackhole) : ComponentBase
{
    private readonly Blackhole _blackhole = newBlackhole;

    private Texture2D _texture;
    private Vector2 _textureOrigin;
    private float _currentRotation;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _texture = content.Load<Texture2D>(@"Images\halo");
        _textureOrigin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (Visible)
        {
            spriteBatch.Draw(
                _texture,
                _blackhole.Position,
                sourceRectangle: null,
                origin: _textureOrigin,
                rotation: _currentRotation,
                scale: _blackhole.Scale,
                color: Color.White * 0.4f,
                layerDepth: Depth,
                effects: SpriteEffects.None);
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState) => _currentRotation += _blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
}
