using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class ProgressBar : ComponentBase
{
    private float _progress;
    private Texture2D _progressBar;
    private Vector2 _position;
    private Vector2 _scale;

    public void Initialize(Vector2 initPosition, Vector2 size)
    {
        _position = initPosition;
        _scale = size;
    }

    public void SetProgress(float newProgress) => _progress = newProgress;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _progressBar = new Texture2D(graphics, 1, 1);
        _progressBar.SetData([Color.White]);
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _progressBar,
            position: new Vector2(_position.X + 2, _position.Y + 2),
            color: Color.Black,
            scale: _scale,
            layerDepth: Depth,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None,
            origin: Vector2.Zero);

        spriteBatch.Draw(
            _progressBar,
            position: _position,
            color: Color.DarkGray.AdjustLight(0.5f),
            scale: _scale,
            layerDepth: Depth + 0.00001f,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None,
            origin: Vector2.Zero);

        spriteBatch.Draw(
            _progressBar,
            position: _position,
            color: Color.Green,
            scale: new Vector2(MathHelper.Lerp(0f, _scale.X, _progress), _scale.Y),
            layerDepth: Depth + 0.00002f,
            sourceRectangle: null,
            rotation: 0f,
            effects: SpriteEffects.None,
            origin: Vector2.Zero);
    }
}
