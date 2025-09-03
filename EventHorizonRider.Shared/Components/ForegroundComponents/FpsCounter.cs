using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class FpsCounter : ComponentBase
{
    private const string TextFormat = "FPS: {0} Memory: {1:0.0} MB";

    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private int _frameCounter;
    private int _frameRate;
    private Vector2 _position;
    private SpriteFont _spriteFont;
    private Vector2 _textSize;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        _spriteFont = content.Load<SpriteFont>(@"Fonts\fps_font");
        _textSize = _spriteFont.MeasureString(TextFormat);

        const float padding = 1;

        _position = new Vector2(DeviceInfo.LogicalWidth - (_textSize.X + padding),
            DeviceInfo.LogicalHeight - (_textSize.Y + padding));
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        _elapsedTime += gameTime.ElapsedGameTime;

        if (_elapsedTime > TimeSpan.FromSeconds(1))
        {
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
#if DEBUG
        _frameCounter++;

        var fps = string.Format(TextFormat, _frameRate, GC.GetTotalMemory(false) / 1024f / 1024f);

        spriteBatch.DrawString(_spriteFont, fps, _position, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, Depth);
#endif
    }
}
