using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class Title : ComponentBase
{
    private Texture2D _texture;
    private Vector2 _position;
    private Vector2 _origin;

    private float _alpha = 1f;

    public Title()
    {
        FadeSpeed = 1f;
    }

    public float FadeSpeed { get; set; }

    public bool FadingOut { get; private set; }

    public new bool Visible => _alpha > 0f;

    public void Show() => _alpha = 1f;

    public void Hide() => FadingOut = true;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _texture = content.Load<Texture2D>(@"Images\title");

        _position = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);
        _origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        if (Visible && FadingOut)
        {
            _alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 1f;
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (Visible)
        {
            spriteBatch.Draw(
                _texture,
                _position,
                origin: _origin,
                color: Color.White * _alpha,
                layerDepth: Depth,
                scale: new Vector2(1.01f),
                sourceRectangle: null,
                rotation: 0f,
                effects: SpriteEffects.None);
        }
    }
}
