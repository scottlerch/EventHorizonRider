using System;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents;

internal class FpsCounter : ComponentBase
{
    private const string TextFormat = "FPS: {0} Memory: {1:0.0} MB";

    private TimeSpan elapsedTime = TimeSpan.Zero;
    private int frameCounter;
    private int frameRate;
    private Vector2 position;
    private SpriteFont spriteFont;
    private Vector2 textSize;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        spriteFont = content.Load<SpriteFont>(@"Fonts\fps_font");
        textSize = spriteFont.MeasureString(TextFormat);

        const float padding = 1;

        position = new Vector2(DeviceInfo.LogicalWidth - (textSize.X + padding),
            DeviceInfo.LogicalHeight - (textSize.Y + padding));
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        elapsedTime += gameTime.ElapsedGameTime;

        if (elapsedTime > TimeSpan.FromSeconds(1))
        {
            elapsedTime -= TimeSpan.FromSeconds(1);
            frameRate = frameCounter;
            frameCounter = 0;
        }
    }

    protected override void DrawCore(SpriteBatch spriteBatch)
    {
#if DEBUG
        frameCounter++;

        var fps = string.Format(TextFormat, frameRate, GC.GetTotalMemory(false)/1024f/1024f);

        spriteBatch.DrawString(spriteFont, fps, position, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, Depth);
#endif
    }
}