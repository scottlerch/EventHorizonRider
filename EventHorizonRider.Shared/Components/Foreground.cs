using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components;

internal class Foreground(PlayButton playButton, MenuButton menuButton, PlayTimer playTime, ControlsHelp controlsHelp, Title title, FpsCounter fpsCounter) : ComponentBase(playButton, menuButton, playTime, controlsHelp, title, fpsCounter)
{
    public FpsCounter FpsCounter { get; private set; } = fpsCounter;

    public PlayButton PlayButton { get; private set; } = playButton;

    public MenuButton MenuButton { get; private set; } = menuButton;

    public PlayTimer PlayTimer { get; private set; } = playTime;

    public Title Title { get; private set; } = title;

    public ControlsHelp ControlsHelp { get; private set; } = controlsHelp;

    protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics) => spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, DeviceInfo.OutputScaleMatrix);

    protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics) => spriteBatch.End();
}
