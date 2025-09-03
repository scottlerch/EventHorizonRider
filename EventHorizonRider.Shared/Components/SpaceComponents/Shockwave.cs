using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents;

internal class Shockwave : ComponentBase
{
    private readonly Blackhole _blackhole;

    private Texture2D _texture;
    private Vector2 _textureOrigin;
    private SoundEffect _sound;

    private float _rotation;
    private float _scale;

    private Color _currentColor;
    private Color _executeColor;

    public Shockwave(Blackhole newBlackhole)
    {
        _blackhole = newBlackhole;
        Visible = false;
    }

    public void Execute()
    {
        Visible = true;
        _scale = 0.4f;
        _executeColor = _currentColor;
        _sound.Play();
    }

    public void SetColor(Color color) => _currentColor = color;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _currentColor = Color.White;

        _texture = content.Load<Texture2D>(@"Images\shockwave");
        _sound = content.Load<SoundEffect>(@"Sounds\shockwave");

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
                rotation: _rotation,
                scale: new Vector2(_scale, _scale),
                color: _executeColor * 0.3f,
                layerDepth: Depth,
                effects: SpriteEffects.None);
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        _rotation += _blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        _scale += (float)gameTime.ElapsedGameTime.TotalSeconds * 1.2f;

        if (_scale > 4f)
        {
            Visible = false;
        }
    }
}
