using EventHorizonRider.Core.Components.MenuComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components;

internal class Menu(LevelSelect levelSelect, ResetButton resetButton, CreditsButton creditsButton, Credits credits) : ComponentBase(levelSelect, resetButton, creditsButton, credits)
{
    public LevelSelect LevelSelect { get; private set; } = levelSelect;

    public ResetButton ResetButton { get; private set; } = resetButton;

    public CreditsButton CreditsButton { get; private set; } = creditsButton;

    public Credits Credits { get; private set; } = credits;

    protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        spriteBatch.Begin(
            SpriteSortMode.FrontToBack,
            BlendState.AlphaBlend,
            samplerState: null, 
            depthStencilState: null, 
            rasterizerState: null,
            effect: null,
            transformMatrix: DeviceInfo.OutputScaleMatrix);
    }

    protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        spriteBatch.End();
    }
}
