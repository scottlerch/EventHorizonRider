using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.MenuComponents;

internal class Credits : ComponentBase
{
    private Texture2D _texture;
    private Vector2 _textureOrigin;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        Visible = false;

        _texture = content.Load<Texture2D>(@"Images\credits");
        _textureOrigin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            sourceRectangle: null,
            origin: _textureOrigin,
            position: DeviceInfo.LogicalCenter,
            rotation: 0f,
            scale: Vector2.One,
            color: Color.White,
            layerDepth: Depth,
            effects: SpriteEffects.None);
    }
}
