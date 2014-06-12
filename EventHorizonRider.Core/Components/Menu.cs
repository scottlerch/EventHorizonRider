using EventHorizonRider.Core.Components.MenuComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Menu : ComponentBase
    {
        public LevelSelect LevelSelect { get; private set; }

        public ResetButton ResetButton { get; private set; }

        public CreditsButton CreditsButton { get; private set; }

        public Credits Credits { get; private set; }

        public Menu(LevelSelect levelSelect, ResetButton resetButton, CreditsButton creditsButton, Credits credits) 
            : base(levelSelect, resetButton, creditsButton, credits)
        {
            LevelSelect = levelSelect;
            ResetButton = resetButton;
            CreditsButton = creditsButton;
            Credits = credits;
        }

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
}
